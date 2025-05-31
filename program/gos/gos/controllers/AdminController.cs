using gos.models;
using gos.models.DTO;
using gos.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace gos.controllers
{
    public class AdminController
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;

        public AdminController(IAdminService adminService, IAuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }


        public async Task CreateNewUser(string fullName, string login, string password, UserRole role)
        {
            var user = _authService.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            var _userDTO = new UserDTO
            {
                FullName = fullName,
                Login = login,
                Password = password,
                Role = role
            };

            await _adminService.CreateUser(_userDTO);
        }

        public async Task CreateParameterType(string type, string name)
        {
            var user = _authService.GetCurrentUser();
            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            var _parType = new ParameterTypeDTO
            {
                Type = type,
                Name = name
            };

            await _adminService.CreateParameterType(_parType);
        }

        public async Task<List<ParameterTypeDTO>> GetParameterTypes()
        {
            
            return await _adminService.GetParameterTypes();
        }

        public async Task ReplaceAllParameterTypes(List<(string Name, string Type)> parameters)
        {
            var existingDtos = await _adminService.GetParameterTypes();

            var updatedDtos = parameters.Select((p, i) =>
            {
                var existing = existingDtos.ElementAtOrDefault(i);
                return new ParameterTypeDTO
                {
                    Id = existing?.Id ?? 0,
                    Name = p.Name,
                    Type = p.Type
                };
            }).ToList();

            await _adminService.ReplaceAllParameterTypes(updatedDtos);
        }

        public async Task ReplaceAllServices(List<ServiceDTO> servicesDTO)
        {
            var existingDtos = await _adminService.GetAllServices();

            var updatedDtos = servicesDTO.Select((p, i) =>
            {
                var existing = existingDtos.ElementAtOrDefault(i);
                return new ServiceDTO
                {
                    Id = existing?.Id ?? 0,
                    Name = p.Name,
                    Description = p.Description,
                    ActivationDate = p.ActivationDate,
                    DeactivationDate = p.DeactivationDate
                };
            }).ToList();

            await _adminService.ReplaceAllServices(updatedDtos);
        }

        public async Task DeleteParameterType(int id)
        {
            await _adminService.DeleteParameterType(id);
        }

        public async Task<List<ServiceDTO>> GetAllServices()
        {
            return await _adminService.GetAllServices();
        }

        public async Task<List<RuleDTO>> GetRulesForService(int serviceId)
        {
            return await _adminService.GetRulesForService(serviceId);
        }

        public async Task SaveService(ServiceDTO serviceDto)
        {
            await _adminService.CreateService(serviceDto);
        }

        public async Task AddRule(RuleDTO ruleDTO)
        {
            await _adminService.AddRule(ruleDTO);
        }

        public async Task DeleteRule(int ruleId)
        {
            await _adminService.DeleteRule(ruleId);
        }

        public async Task UpdateRule(RuleDTO ruleDTO)
        {
            await _adminService.UpdateRule(ruleDTO);
        }
    }
}
