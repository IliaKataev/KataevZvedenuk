using gos.models.DTO;
using gos.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.controllers
{
    public class CitizenController
    {
        private readonly ICitizenService _citizenService;
        private readonly IApplicationService _applicationService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public CitizenController(
            ICitizenService citizenService,
            IApplicationService applicationService,
            IAuthService authService,
            IUserService userService)
        {
            _citizenService = citizenService;
            _applicationService = applicationService;
            _authService = authService;
            _userService = userService;
        }

        public async Task UpdatePersonalData(string fullName, string password)
        {
            var user = _authService.GetCurrentUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            await _userService.UpdateUserDataAsync(fullName, password);
        }

        // Заглушки
        public string LoadPersonalData() => throw new NotImplementedException();

        public async Task<List<ParameterTypeDTO>> LoadParameterTypesAsync()
        {
            var types = await _citizenService.GetParameterTypesAsync();
            return types.Select(t => new ParameterTypeDTO
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }

        public List<ParameterDTO> LoadParameters() => throw new NotImplementedException();
        public async Task AddParameterAsync(int typeId, string value)
        {
            var dto = new ParameterDTO
            {
                TypeId = typeId,
                Value = value
            };
            await _citizenService.AddParameterAsync(dto);
        }
        public async Task UpdateParameterAsync(int parameterId, int typeId, string value)
        {
            var dto = new ParameterDTO
            {
                TypeId = typeId,
                Value = value
            };
            await _citizenService.UpdateParameterAsync(parameterId, dto);
        }
        public async Task DeleteParameterAsync(int parameterId)
        {
            await _citizenService.DeleteParameterAsync(parameterId);
        }
        public List<ServiceDTO> LoadAvailableServices() => throw new NotImplementedException();
        public List<ParameterTypeDTO> LoadServiceRequirements(int serviceId) => throw new NotImplementedException();
        public ApplicationDTO CreateNewApplication(int serviceId) => throw new NotImplementedException();
        public List<ApplicationDTO> ViewMyApplications() => throw new NotImplementedException();
        public void CancelMyApplication(int applicationId) => throw new NotImplementedException();
    }
}
