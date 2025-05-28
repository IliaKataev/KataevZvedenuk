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
    public class AuthController
    {
        private readonly IAuthService _authService;
        private User? _currentUser;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<UserDTO?> Login(string login, string password)
        {
            var user = await _authService.TryLoginAsync(login, password);
            if (user != null)
            {
                _currentUser = user; // Сохраняем модель User
                                     // Маппинг User в UserDTO:
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Login = user.Login,
                    Role = user.Role,
                    FullName = user.FullName,
                    Password = user.Password
                };
                return userDto;
            }
            else
            {
                return null;
            }
        }



        // Если нужно — можно очистить _currentUser в логике Logout
        public void Logout()
        {
            _currentUser = null;
        }

        
    }


}
