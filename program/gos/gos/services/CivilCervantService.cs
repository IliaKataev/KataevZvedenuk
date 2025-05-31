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
        Task<List<ApplicationDTO>> GetMyApplications();
        //Task<models.Application?> GetApplicationDetails(int applicationId);
        Task ChangeStatus(ApplicationDTO application);
        Task ProcessApplication(int applicationId, string result);
        Task<List<ServiceDTO>> GetAvailableServices();
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



        public async Task<List<ApplicationDTO>> GetMyApplications() //тут должны возвращаться DTOшки
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            var apps = await _applicationRepository.GetAll();
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
        public async Task<List<ServiceDTO>> GetAvailableServices()
        {
            var allServices = await _serviceRepository.GetAll();

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


        /*public async Task<models.Application?> GetApplicationDetails(int applicationId)
        {
            return await _applicationRepository.GetById(applicationId);
        }*/

        public async Task ChangeStatus(ApplicationDTO application)
        {
            var existingApp = await _applicationRepository.GetById(application.ApplicationId);
            if (existingApp == null)
                throw new ArgumentException("Заявление не найдено");

            // Обновляем поля
            existingApp.Status = application.Status;
            existingApp.Result = application.Result;
            existingApp.ClosureDate = DateTime.Now;

            // Сохраняем
            await _applicationRepository.Update(existingApp);
        }


        public async Task ProcessApplication(int applicationId, string result)
        {
            var app = await _applicationRepository.GetById(applicationId);
            if (app == null) throw new ArgumentException("Заявление не найдено");

            app.Result = result;
            app.ClosureDate = DateTime.Now;

            await _applicationRepository.Update(app);
        }
    }
}
