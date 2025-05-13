using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }  // В реальности пароль должен быть захеширован
        public string FullName { get; set; }
        public UserRole Role { get; set; }  // Пример: "Admin", "CivilServant", "Citizen"
    }

}
