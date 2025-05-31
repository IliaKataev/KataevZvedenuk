using gos.models;
using gos.models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.repositories;
using Microsoft.VisualBasic.ApplicationServices;

namespace gos.services
{
    public interface ICitizenService
    {
        Task<List<Parameter>> GetParameters();
        Task<Parameter> AddParameter(ParameterDTO parameterDTO);
        Task UpdateParameter(int parameterId, ParameterDTO parameterDTO);
        Task DeleteParameter(int parameterId);
        Task<List<ApplicationDTO>> GetMyApplications();
        Task CancelApplication(int applicationId);
        Task<List<ServiceDTO>> GetAvailableServices();
        Task<List<ParameterTypeDTO>> GetServiceRequirements(int serviceId);
        Task<List<ParameterType>> GetParameterTypes();


    }

    public class CitizenService : ICitizenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly IParameterTypeRepository _parameterTypeRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly AuthSession _authSession;

        public CitizenService(
            IUserRepository userRepository,
            IParameterRepository parameterRepository,
            IParameterTypeRepository parameterTypeRepository,
            IApplicationRepository applicationRepository,
            IServiceRepository serviceRepository,
            AuthSession authSession)
        {
            _userRepository = userRepository;
            _parameterRepository = parameterRepository;
            _parameterTypeRepository = parameterTypeRepository;
            _applicationRepository = applicationRepository;
            _serviceRepository = serviceRepository;
            _authSession = authSession;
        }

        public async Task<List<ParameterType>> GetParameterTypes()
        {
            return await _parameterTypeRepository.GetAll();
        }


        public async Task<List<Parameter>> GetParameters()
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            return await _userRepository.GetParameters(user.Id);
        }

        public async Task<Parameter> AddParameter(ParameterDTO parameterDTO)
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();

            var param = new Parameter
            {
                TypeId = parameterDTO.TypeId,
                Value = parameterDTO.Value,
                UserId = user.Id,
            };

            await _parameterRepository.Add(param);
            return param;
        }

        public async Task UpdateParameter(int parameterId, ParameterDTO parameterDTO)
        {
            var parameter = await _parameterRepository.GetById(parameterId)
                ?? throw new ArgumentException("Параметр не найден");

            parameter.TypeId = parameterDTO.TypeId;
            parameter.Value = parameterDTO.Value;

            await _parameterRepository.Update(parameter);
        }

        public async Task DeleteParameter(int parameterId)
        {
            await _parameterRepository.Delete(parameterId);
        }

        public async Task<List<ApplicationDTO>> GetMyApplications() //тут должны возвращаться DTOшки
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            var apps = await _applicationRepository.GetByUserId(user.Id);
            return apps.Select(a => new ApplicationDTO
            {
                ApplicationId = a.Id,
                UserId = a.UserId,
                ServiceId = a.ServiceId,
                Status = a.Status,
                CreationDate = a.CreationDate,
                Deadline = a.Deadline,
                Result = a.Result
            }).ToList();
        }

        public async Task CancelApplication(int applicationId)
        {
            var app = await _applicationRepository.GetById(applicationId)
                ?? throw new ArgumentException("Заявление не найдено");

            if (app.ClosureDate != null)
                throw new InvalidOperationException("Нельзя отменить закрытое заявление");

            await _applicationRepository.Delete(applicationId);
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

        public async Task<List<ParameterTypeDTO>> GetServiceRequirements(int serviceId)
        {
            var parameterTypes = await _parameterTypeRepository.GetByServiceId(serviceId);

            var dtoList = parameterTypes.Select(pt => new ParameterTypeDTO
            {
                Id = pt.Id,
                Type = pt.Type,
                Name = pt.Name
            }).ToList();

            return dtoList;
        }
    }
}
