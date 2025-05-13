using gos.models;
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

        public async Task<(bool isSuccess, UserRole? userRole)> Login(string login, string password)
        {
            // Пытаемся авторизовать пользователя через сервис
            var user = await _authService.TryLoginAsync(login, password);

            if (user != null)
            {
                _currentUser = user; // Сохраняем текущего пользователя
                return (true, user.Role); // Возвращаем успешный вход и роль
            }
            else
            {
                return (false, null); // Неверный логин или пароль
            }
        }

        // Если нужно — можно очистить _currentUser в логике Logout
        public void Logout()
        {
            _currentUser = null;
        }

        
    }


}
