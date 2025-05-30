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
        string GetPersonalData();
        Task<List<Parameter>> GetParametersAsync();
        Task<Parameter> AddParameterAsync(ParameterDTO parameterDTO);
        Task UpdateParameterAsync(int parameterId, ParameterDTO parameterDTO);
        Task DeleteParameterAsync(int parameterId);
        Task<List<ApplicationDTO>> GetMyApplicationsAsync();
        Task CancelApplicationAsync(int applicationId);
        Task<List<ServiceDTO>> GetAvailableServicesAsync();
        Task<List<ParameterTypeDTO>> GetServiceRequirementsAsync(int serviceId);
        Task<List<ParameterType>> GetParameterTypesAsync();


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


        
        public string GetPersonalData()
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            return $"Имя: {user.FullName}, Логин: {user.Login}";
        }

        public async Task<List<ParameterType>> GetParameterTypesAsync()
        {
            return await _parameterTypeRepository.GetAllAsync();
        }


        public async Task<List<Parameter>> GetParametersAsync()
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            return await _userRepository.GetParametersAsync(user.Id);
        }

        public async Task<Parameter> AddParameterAsync(ParameterDTO parameterDTO)
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();

            var param = new Parameter
            {
                TypeId = parameterDTO.TypeId,
                Value = parameterDTO.Value,
                UserId = user.Id,
            };

            await _parameterRepository.AddAsync(param);
            return param;
        }

        public async Task UpdateParameterAsync(int parameterId, ParameterDTO parameterDTO)
        {
            var parameter = await _parameterRepository.GetByIdAsync(parameterId)
                ?? throw new ArgumentException("Параметр не найден");

            parameter.TypeId = parameterDTO.TypeId;
            parameter.Value = parameterDTO.Value;

            await _parameterRepository.UpdateAsync(parameter);
        }

        public async Task DeleteParameterAsync(int parameterId)
        {
            await _parameterRepository.DeleteAsync(parameterId);
        }

        public async Task<List<ApplicationDTO>> GetMyApplicationsAsync() //тут должны возвращаться DTOшки
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            var apps = await _applicationRepository.GetByUserIdAsync(user.Id);
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

        public async Task CancelApplicationAsync(int applicationId)
        {
            var app = await _applicationRepository.GetByIdAsync(applicationId)
                ?? throw new ArgumentException("Заявление не найдено");

            if (app.ClosureDate != null)
                throw new InvalidOperationException("Нельзя отменить закрытое заявление");

            await _applicationRepository.DeleteAsync(applicationId);
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

        public async Task<List<ParameterTypeDTO>> GetServiceRequirementsAsync(int serviceId)
        {
            var parameterTypes = await _parameterTypeRepository.GetByServiceIdAsync(serviceId);

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
