using gos.models;
using gos.repositories;
using gos.repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.services
{
    public interface IAuthService
    {
        Task<User?> TryLoginAsync(string login, string password);
        Task<User> GetCurrentUserAsync();
        void Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthSession _authSession; // Внедрение AuthSession

        public AuthService(IUserRepository userRepository, AuthSession authSession)
        {
            _userRepository = userRepository;
            _authSession = authSession; // Инициализация через DI
        }

        public async Task<User?> TryLoginAsync(string login, string password)
        {
            // Получаем пользователя по логину
            var user = await _userRepository.GetByLoginAsync(login);

            // Если пароль совпадает — авторизация успешна
            if (user != null && user.Password == password)
            {
                _authSession.CurrentUser = user;  // Доступ через экземпляр
                return user;
            }

            return null;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var user = _authSession.CurrentUser;

            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован.");

            // Актуализация пользователя из БД (если нужно)
            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            return updatedUser ?? throw new Exception("Пользователь не найден в базе данных.");
        }

        public void Logout()
        {
            // Удаляем текущего пользователя из сессии
            _authSession.CurrentUser = null; // Доступ через экземпляр
        }
    }



}
