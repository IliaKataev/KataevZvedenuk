using gos.models;
using gos.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models.DTO;

namespace gos.services
{
    public interface IApplicationService
    {
        Task<models.Application> CreateNewApplication(int userId, ApplicationDTO applicationDTO);
        Task CancelApplicationAsync(models.Application application);
        Task<ApplicationDTO> ProcessApplication(int applicationId, int serviceId);
    }

    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        //private readonly IServiceRepository _serviceRepository;
        private readonly IRuleRepository _ruleRepository;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IRuleRepository ruleRepository)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            //_serviceRepository = serviceRepository;
            _ruleRepository = ruleRepository;
        }

        public async Task<models.Application> CreateNewApplication(int userId, ApplicationDTO applicationDTO)
        {
            var application = new models.Application
            {
                UserId = userId,
                ServiceId = applicationDTO.ServiceId,
                Status = applicationDTO.Status,
                CreationDate = applicationDTO.CreationDate,
                ClosureDate = null,
                Result = null,
                Deadline = applicationDTO.Deadline,
            };

            await _applicationRepository.Add(application);
            return application;
        }

        public async Task CancelApplicationAsync(models.Application application)
        {
            if (application.ClosureDate != null)
                throw new InvalidOperationException("Нельзя отменить уже закрытую заявку.");

            await _applicationRepository.Delete(application.Id);
        }

        public async Task<ApplicationDTO> ProcessApplication(int applicationId, int serviceId)
        {
            var app = await _applicationRepository.GetById(applicationId)
                ?? throw new ArgumentException("Заявление не найдено");

            var user = await _userRepository.GetById(app.UserId)
                ?? throw new ArgumentException("Пользователь не найден");

            var userParams = await _userRepository.GetParameters(user.Id);
            var rules = await _ruleRepository.GetByServiceId(serviceId);

            var ruleGroups = rules.GroupBy(r => r.DeadlineDays ?? -1);

            var groupDescriptions = string.Join("\n\n", ruleGroups.Select(g =>
                                                $"Группа DeadlineDays = {g.Key}:\n" +
                                                string.Join("\n", g.Select(r =>
                                                    $"- Тип: {r.NeededType.Type}, Оператор: {r.ComparisonOperator}, Значение: {r.Value}"))));

            MessageBox.Show("Группы правил:\n\n" + groupDescriptions);

            List<Rule> successfulGroup = null;

            foreach (var group in ruleGroups)
            {
                bool groupPassed = true;
                foreach (var rule in group)
                {
                    var userParam = userParams.FirstOrDefault(p => p.TypeId == rule.NeededTypeId);
                    if (userParam == null || !Compare(userParam.Value, rule.ComparisonOperator, rule.Value))
                    {
                        groupPassed = false;
                        break;
                    }
                }

                if (groupPassed)
                {
                    successfulGroup = group.ToList();
                    break;
                }
            }

            app.ClosureDate = DateTime.Now;

            if (successfulGroup != null)
            {
                var deadlineDays = successfulGroup.First().DeadlineDays;
                if (deadlineDays.HasValue && deadlineDays > 0)
                {
                    app.Deadline = app.CreationDate.AddDays(deadlineDays.Value);
                }
                else
                {
                    app.Deadline = null;
                }

                // Проверяем, просрочен ли дедлайн
                if (app.Deadline.HasValue && DateTime.Now > app.Deadline.Value)
                {
                    app.Status = ApplicationStatus.REJECTED;
                    app.Result = "Заявление отклонено: просрочен дедлайн.";
                    app.ClosureDate = DateTime.Now;
                    app.Deadline = null; // можно очистить, если дедлайн уже неактуален
                }
                else
                {
                    // Дедлайн не просрочен, заявка успешна
                    app.Status = ApplicationStatus.COMPLETED;
                    app.Result = $"Пройдена группа правил с DeadlineDays = {deadlineDays ?? 0}:\n" +
                                 string.Join("\n", successfulGroup.Select(r =>
                                     $"- Тип: {r.NeededType.Type}, Условие: {r.ComparisonOperator} {r.Value}"));
                    app.ClosureDate = DateTime.Now;
                }
            }
            else
            {
                app.Status = ApplicationStatus.REJECTED;
                app.Result = "Заявление отклонено: ни одна группа правил не прошла проверку.";
                app.ClosureDate = DateTime.Now;
                app.Deadline = null;
            }


            await _applicationRepository.Update(app);
            return new ApplicationDTO
            {
                ApplicationId = app.Id,
                ServiceId = app.ServiceId,
                Deadline = app.Deadline,
                Status = app.Status,
                Result = app.Result,
                ClosureDate = app.ClosureDate
            };
        }


        private bool Compare(string userValue, string op, string ruleValue)
        {
            if (double.TryParse(userValue, out var userVal) && double.TryParse(ruleValue, out var ruleVal))
            {
                return op switch
                {
                    "=" or "==" => userVal == ruleVal,
                    "!=" => userVal != ruleVal,
                    ">" => userVal > ruleVal,
                    "<" => userVal < ruleVal,
                    ">=" => userVal >= ruleVal,
                    "<=" => userVal <= ruleVal,
                    _ => false
                };
            }

            // Тут сравниваем строки без регистра
            return op switch
            {
                "=" or "==" => string.Equals(userValue, ruleValue, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(userValue, ruleValue, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }
}
