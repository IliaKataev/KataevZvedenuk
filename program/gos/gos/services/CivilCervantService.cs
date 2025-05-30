using gos.models;
using gos.models.DTO;
using gos.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace gos.services
{
    public interface ICivilServantService
    {
        //bool IsCivilServant();
        Task<List<ApplicationDTO>> GetMyApplicationsAsync();
        Task<models.Application?> GetApplicationDetailsAsync(int applicationId);
        Task ChangeStatusAsync(ApplicationDTO application);
        Task ProcessApplicationAsync(int applicationId, string result);
        Task<List<ServiceDTO>> GetAvailableServicesAsync();
    }

    public class CivilServantService : ICivilServantService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly AuthSession _authSession;

        public CivilServantService(IApplicationRepository applicationsRepository, AuthSession authSession, IServiceRepository serviceRepository)
        {
            _applicationRepository = applicationsRepository;
            _authSession = authSession;
            _serviceRepository = serviceRepository;
        }



        public async Task<List<ApplicationDTO>> GetMyApplicationsAsync() //тут должны возвращаться DTOшки
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            var apps = await _applicationRepository.GetAllAsync();
            return apps
                .Select(a => new ApplicationDTO
            {
                ApplicationId = a.Id,
                UserId = a.UserId,
                ServiceId = a.ServiceId,
                Status = a.Status,
                CreationDate = a.CreationDate,
                Deadline = a.Deadline,
                Result = a.Result,
                ClosureDate = a.ClosureDate,
            }).ToList();
        }
        public async Task<List<ServiceDTO>> GetAvailableServicesAsync()
        {
            var allServices = await _serviceRepository.GetAllAsync();

            var availableServices = allServices
                .Where(service => service.DeactivationDate == null)
                .Select(service => new ServiceDTO
                {
                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    ActivationDate = service.ActivationDate,
                    DeactivationDate = service.DeactivationDate 
                })
                .ToList();

            return availableServices;
        }


        public async Task<models.Application?> GetApplicationDetailsAsync(int applicationId)
        {
            return await _applicationRepository.GetByIdAsync(applicationId);
        }

        public async Task ChangeStatusAsync(ApplicationDTO application)
        {
            var existingApp = await _applicationRepository.GetByIdAsync(application.ApplicationId);
            if (existingApp == null)
                throw new ArgumentException("Заявление не найдено");

            // Обновляем поля
            existingApp.Status = application.Status;
            existingApp.Result = application.Result;
            existingApp.ClosureDate = DateTime.Now;

            // Сохраняем
            await _applicationRepository.UpdateAsync(existingApp);
        }


        public async Task ProcessApplicationAsync(int applicationId, string result)
        {
            var app = await _applicationRepository.GetByIdAsync(applicationId);
            if (app == null) throw new ArgumentException("Заявление не найдено");

            app.Result = result;
            app.ClosureDate = DateTime.Now;

            await _applicationRepository.UpdateAsync(app);
        }
    }
}
