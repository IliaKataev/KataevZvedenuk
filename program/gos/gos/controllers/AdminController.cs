using gos.models;
using gos.models.DTO;
using gos.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
