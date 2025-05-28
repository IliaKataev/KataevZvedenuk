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
        Task<models.Application> CreateNewApplicationAsync(int userId, ApplicationDTO applicationDTO);
        Task CancelApplicationAsync(models.Application application);
        Task<DateTime> ProcessApplicationAsync(int applicationId, int serviceId);
    }

    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRuleRepository _ruleRepository;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IRuleRepository ruleRepository)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _ruleRepository = ruleRepository;
        }

        public async Task<models.Application> CreateNewApplicationAsync(int userId, ApplicationDTO applicationDTO)
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

            await _applicationRepository.AddAsync(application);
            return application;
        }

        public async Task CancelApplicationAsync(models.Application application)
        {
            if (application.ClosureDate != null)
                throw new InvalidOperationException("Нельзя отменить уже закрытую заявку.");

            await _applicationRepository.DeleteAsync(application.Id);
        }

        public async Task<DateTime> ProcessApplicationAsync(int applicationId, int serviceId)
        {
            var app = await _applicationRepository.GetByIdAsync(applicationId)
                ?? throw new ArgumentException("Заявление не найдено");

            var user = await _userRepository.GetByIdAsync(app.UserId)
                ?? throw new ArgumentException("Пользователь не найден");

            var userParams = await _userRepository.GetParametersAsync(user.Id);
            var rules = await _ruleRepository.GetByServiceIdAsync(serviceId);

            foreach (var rule in rules)
            {
                var userParam = userParams.FirstOrDefault(p => p.TypeId == rule.NeededTypeId);
                if (userParam == null)
                {
                    app.Status = ApplicationStatus.REJECTED;
                    app.Result = "Не хватает параметров.";
                    app.ClosureDate = DateTime.Now;
                    await _applicationRepository.UpdateAsync(app);
                    return app.ClosureDate.Value;
                }

                if (!Compare(userParam.Value, rule.ComparisonOperator, rule.Value))
                {
                    app.Status = ApplicationStatus.REJECTED;
                    app.Result = $"Параметр '{rule.NeededTypeId}' не соответствует условию.";
                    app.ClosureDate = DateTime.Now;
                    await _applicationRepository.UpdateAsync(app);
                    return app.ClosureDate.Value;
                }
            }

            app.Status = ApplicationStatus.COMPLETED;
            app.Result = "Заявление успешно обработано.";
            app.ClosureDate = DateTime.Now;
            await _applicationRepository.UpdateAsync(app);
            return app.ClosureDate.Value;
        }

        private bool Compare(string userValue, string op, string ruleValue)
        {
            if (double.TryParse(userValue, out var userVal) && double.TryParse(ruleValue, out var ruleVal))
            {
                return op switch
                {
                    "==" => userVal == ruleVal,
                    "!=" => userVal != ruleVal,
                    ">" => userVal > ruleVal,
                    "<" => userVal < ruleVal,
                    ">=" => userVal >= ruleVal,
                    "<=" => userVal <= ruleVal,
                    _ => false
                };
            }

            // Для строкового сравнения
            return op switch
            {
                "==" => userValue == ruleValue,
                "!=" => userValue != ruleValue,
                _ => false
            };
        }
    }
}
