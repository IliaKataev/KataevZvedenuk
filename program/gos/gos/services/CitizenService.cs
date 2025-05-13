using gos.models;
using gos.models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.repositories;

namespace gos.services
{
    public interface ICitizenService
    {
        //bool IsCitizen();
        string GetPersonalData();
        Task<List<Parameter>> GetParametersAsync();
        Task<Parameter> AddParameterAsync(ParameterDTO parameterDTO);
        Task UpdateParameterAsync(int parameterId, ParameterDTO parameterDTO);
        Task DeleteParameterAsync(int parameterId);
        Task<List<models.Application>> GetMyApplicationsAsync();
        Task CancelApplicationAsync(int applicationId);
        Task<List<Service>> GetAvailableServicesAsync();
        Task<List<ParameterType>> GetServiceRequirementsAsync(int serviceId);
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

        /*public bool IsCitizen()
        {
            var user = _authSession.CurrentUser;  // Использование экземпляра _authSession
            return user != null && !user.Login.StartsWith("admin") && !user.Login.StartsWith("gov_");
        }*/

        
        public string GetPersonalData()
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            return $"Имя: {user.FullName}, Логин: {user.Login}";
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
                UserId = user.Id,
                TypeId = parameterDTO.TypeId,
                Value = parameterDTO.Value
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

        public async Task<List<models.Application>> GetMyApplicationsAsync()
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException();
            return await _applicationRepository.GetByUserIdAsync(user.Id);
        }

        public async Task CancelApplicationAsync(int applicationId)
        {
            var app = await _applicationRepository.GetByIdAsync(applicationId)
                ?? throw new ArgumentException("Заявление не найдено");

            if (app.ClosureDate != null)
                throw new InvalidOperationException("Нельзя отменить закрытое заявление");

            await _applicationRepository.DeleteAsync(applicationId);
        }

        public async Task<List<Service>> GetAvailableServicesAsync()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<List<ParameterType>> GetServiceRequirementsAsync(int serviceId)
        {
            return await _parameterTypeRepository.GetByServiceIdAsync(serviceId);
        }
    }
}
