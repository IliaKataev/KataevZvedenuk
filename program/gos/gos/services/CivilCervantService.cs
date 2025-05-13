using gos.models;
using gos.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.services
{
    public interface ICivilServantService
    {
        //bool IsCivilServant();
        Task<List<models.Application>> GetAllApplicationsAsync();
        Task<models.Application?> GetApplicationDetailsAsync(int applicationId);
        Task ChangeStatusAsync(int applicationId, ApplicationStatus status);
        Task ProcessApplicationAsync(int applicationId, string result);
    }

    public class CivilServantService : ICivilServantService
    {
        private readonly IApplicationRepository _applicationsRepository;
        private readonly AuthSession _authSession;

        public CivilServantService(IApplicationRepository applicationsRepository, AuthSession authSession)
        {
            _applicationsRepository = applicationsRepository;
            _authSession = authSession;
        }

        /*public bool IsCivilServant()
        {
            var user = _authSession.CurrentUser;  // Использование экземпляра _authSession
            return user != null && user.Login.StartsWith("gov_"); // Примерная проверка
        }*/

        public async Task<List<models.Application>> GetAllApplicationsAsync()
        {
            return await _applicationsRepository.GetAllAsync();
        }

        public async Task<models.Application?> GetApplicationDetailsAsync(int applicationId)
        {
            return await _applicationsRepository.GetByIdAsync(applicationId);
        }

        public async Task ChangeStatusAsync(int applicationId, ApplicationStatus status)
        {
            var app = await _applicationsRepository.GetByIdAsync(applicationId);
            if (app == null) throw new ArgumentException("Заявление не найдено");

            app.Result = status.ToString();
            app.ClosureDate = DateTime.Now;

            await _applicationsRepository.UpdateAsync(app);
        }

        public async Task ProcessApplicationAsync(int applicationId, string result)
        {
            var app = await _applicationsRepository.GetByIdAsync(applicationId);
            if (app == null) throw new ArgumentException("Заявление не найдено");

            app.Result = result;
            app.ClosureDate = DateTime.Now;

            await _applicationsRepository.UpdateAsync(app);
        }
    }
}
