using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class UserModel : DatabaseFactory<UserModel> {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime Bdate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsAdmin { get; set; }

        public override UserModel Create(SqliteDataReader reader) {
            return new UserModel() {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Login = reader.GetString(3),
                Password = reader.GetString(4),
                Bdate = reader.GetDateTime(5),
                Address = reader.GetString(6),
                Phone = reader.GetString(7),
                Email = reader.GetString(8),
                RegistrationDate = reader.GetDateTime(9),
                IsAdmin = reader.GetBoolean(10)
            };
        }
    }
}
