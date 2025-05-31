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
        private readonly IApplicationService _applicationService;

        public CivilServantController(ICivilServantService civilServantService, IApplicationService applicationService)
        {
            _civilServantService = civilServantService;
            _applicationService = applicationService;
        }

        public async Task<List<ApplicationDTO>> LoadApplications()
        {
            return await _civilServantService.GetMyApplications();
        }

        public async Task<List<ServiceDTO>> LoadAvailableServices()
        {
            return await _civilServantService.GetAvailableServices();
        }

        public async Task UpdateApplicationResult(ApplicationDTO application)
        {
            await _civilServantService.ChangeStatus(application);
        }

        public async Task<ApplicationDTO> ProcessApplication(ApplicationDTO applicationDTO)
        {
            return await _applicationService.ProcessApplication(applicationDTO.ApplicationId, applicationDTO.ServiceId);
        }
    }
}
