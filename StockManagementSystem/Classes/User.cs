using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagementSystem.Classes
{
    public enum UserRole { Admin, Employee }
    internal class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
   

        //public bool Authenticate(string password ) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);

        public bool HasPermission(string action) => Role == UserRole.Admin || action == "View";
    }
}
