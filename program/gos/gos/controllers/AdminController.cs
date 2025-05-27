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
            var user = _authService.GetCurrentUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            var _userDTO = new UserDTO
            {
                FullName = fullName,
                Login = login,
                Password = password,
                Role = role
            };

            await _adminService.CreateUserAsync(_userDTO);
        }

        public async Task CreateParameterType(string type, string name)
        {
            var user = _authService.GetCurrentUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            var _parType = new ParameterTypeDTO
            {
                Type = type,
                Name = name
            };

            await _adminService.CreateParameterTypeAsync(_parType);
        }

        public async Task<List<ParameterTypeDTO>> GetParameterTypesAsync()
        {
            
            return await _adminService.GetParameterTypesAsync(); // теперь всё корректно
        }

        // Пример метода в контроллере (вызов из формы):
        public async Task ReplaceAllParameterTypesAsync(List<(string Name, string Type)> parameters)
        {
            // Здесь создаем DTO с учетом существующих Id (если нужно, получаем текущие из БД)
            var existingDtos = await _adminService.GetParameterTypesAsync();

            var updatedDtos = parameters.Select((p, i) =>
            {
                var existing = existingDtos.ElementAtOrDefault(i);
                return new ParameterTypeDTO
                {
                    Id = existing?.Id ?? 0, // 0 - для новых
                    Name = p.Name,
                    Type = p.Type
                };
            }).ToList();

            await _adminService.ReplaceAllParameterTypesAsync(updatedDtos);
        }

        public async Task ReplaceAllServicesAsync(List<ServiceDTO> servicesDTO)
        {
            // Здесь создаем DTO с учетом существующих Id (если нужно, получаем текущие из БД)
            var existingDtos = await _adminService.GetAllServicesAsync();

            var updatedDtos = servicesDTO.Select((p, i) =>
            {
                var existing = existingDtos.ElementAtOrDefault(i);
                return new ServiceDTO
                {
                    Id = existing?.Id ?? 0, // 0 - для новых
                    Name = p.Name,
                    Description = p.Description,
                    ActivationDate = p.ActivationDate,
                    DeactivationDate = p.DeactivationDate
                };
            }).ToList();

            await _adminService.ReplaceAllServicesAsync(updatedDtos);
        }

        public async Task DeleteParameterTypeAsync(int id)
        {
            await _adminService.DeleteParameterTypeAsync(id);
        }

        public async Task<List<ServiceDTO>> GetAllServicesAsync()
        {
            return await _adminService.GetAllServicesAsync();
        }

        public async Task<List<RuleDTO>> GetRulesForServiceAsync(int serviceId)
        {
            return await _adminService.GetRulesForServiceAsync(serviceId);
        }

        public async Task SaveServiceAsync(ServiceDTO serviceDto)
        {
            await _adminService.CreateServiceAsync(serviceDto);
        }

        public async Task AddRuleAsync(RuleDTO ruleDTO)
        {
            await _adminService.AddRuleAsync(ruleDTO);
        }

        public async Task DeleteRuleAsync(int ruleId)
        {
            await _adminService.DeleteRuleAsync(ruleId);
        }
    }
}
