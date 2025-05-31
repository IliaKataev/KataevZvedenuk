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
        Task UpdateUserData(string fullName, string password);
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

        public async Task UpdateUserData(string fullName, string password)
        {
            var user = _authSession.CurrentUser ?? throw new UnauthorizedAccessException("User not authenticated.");

            MessageBox.Show(_authSession.CurrentUser.ToString());

            user.FullName = fullName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = password;
            }

            await _userRepository.Update(user); 
        }
    }

}
