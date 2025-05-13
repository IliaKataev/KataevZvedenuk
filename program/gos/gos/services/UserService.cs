using gos.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models;

namespace gos.services
{
    public interface IUserService
    {
        Task UpdateUserDataAsync(string fullName, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthSession _authSession;

        public UserService(IUserRepository userRepository, AuthSession authSession)
        {
            _userRepository = userRepository;
            _authSession = authSession;
        }

        // Метод для обновления данных пользователя
        public async Task UpdateUserDataAsync(string fullName, string password)
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException("User not authenticated.");

            MessageBox.Show(_authSession.CurrentUser.ToString());

            user.FullName = fullName;

            // Если пароль не пустой, обновляем его
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = password;
            }

            await _userRepository.UpdateAsync(user); // Асинхронное обновление данных пользователя в репозитории
        }
    }

}
