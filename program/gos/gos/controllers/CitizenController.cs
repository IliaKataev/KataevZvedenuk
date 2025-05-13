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
        public List<ParameterDTO> LoadParameters() => throw new NotImplementedException();
        public ParameterDTO AddParameter(int typeId, string value) => throw new NotImplementedException();
        public void UpdateParameter(int parameterId, string newValue) => throw new NotImplementedException();
        public void DeleteParameter(int parameterId) => throw new NotImplementedException();
        public List<ServiceDTO> LoadAvailableServices() => throw new NotImplementedException();
        public List<ParameterTypeDTO> LoadServiceRequirements(int serviceId) => throw new NotImplementedException();
        public ApplicationDTO CreateNewApplication(int serviceId) => throw new NotImplementedException();
        public List<ApplicationDTO> ViewMyApplications() => throw new NotImplementedException();
        public void CancelMyApplication(int applicationId) => throw new NotImplementedException();
    }
}
