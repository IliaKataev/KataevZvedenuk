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
        Task<User?> TryLogin(string login, string password);
        Task<User> GetCurrentUser();
        void Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthSession _authSession;

        public AuthService(IUserRepository userRepository, AuthSession authSession)
        {
            _userRepository = userRepository;
            _authSession = authSession;
        }

        public async Task<User?> TryLogin(string login, string password)
        {
            var user = await _userRepository.GetByLogin(login);

            // Если пароль совпадает — авторизация успешна
            if (user != null && user.Password == password)
            {
                _authSession.CurrentUser = user; 
                return user;
            }

            return null;
        }

        public async Task<User> GetCurrentUser()
        {
            var user = _authSession.CurrentUser;

            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не авторизован.");

            var updatedUser = await _userRepository.GetById(user.Id);
            return updatedUser ?? throw new Exception("Пользователь не найден в базе данных.");
        }

        public void Logout()
        {
            // Удаляем текущего пользователя из сессии
            _authSession.CurrentUser = null;
        }
    }



}
