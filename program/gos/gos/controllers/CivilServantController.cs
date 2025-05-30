using gos.models.DTO;
using gos.models;
using gos.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.controllers
{
    public class CivilServantController
    {
        private readonly ICivilServantService _civilServantService;
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;

        public CivilServantController(ICivilServantService civilServantService, IUserService userService, IApplicationService applicationService)
        {
            _civilServantService = civilServantService;
            _userService = userService;
            _applicationService = applicationService;
        }

        public async Task<List<ApplicationDTO>> LoadApplications()
        {
            return await _civilServantService.GetMyApplicationsAsync();
        }

        public async Task<List<ServiceDTO>> LoadAvailableServices()
        {
            return await _civilServantService.GetAvailableServicesAsync();
        }

        public async Task UpdateApplicationResult(ApplicationDTO application)
        {
            await _civilServantService.ChangeStatusAsync(application);
        }

        public async Task<ApplicationDTO> ProcessApplication(ApplicationDTO applicationDTO)
        {
            return await _applicationService.ProcessApplicationAsync(applicationDTO.ApplicationId, applicationDTO.ServiceId);
        }
    }
}
