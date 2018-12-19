using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTasks.Data
{
    public class UserRepository
    {
        private string _connectionstring;
        public UserRepository(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);
            user.PasswordSalt = salt;
            user.PasswordHash = passwordHash;
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }

            //using (var connection = new SqlConnection(_connectionstring))
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = "INSERT INTO [User](FirstName, LastName, EmailAddress, PasswordHash, PasswordSalt)" +
            //        " VALUES (@firstname, @lastname, @emailAddress, @passwordHash, @passwordSalt)";
            //    command.Parameters.AddWithValue("@firstname", user.FirstName);
            //    command.Parameters.AddWithValue("@lastname", user.LastName);
            //    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
            //    command.Parameters.AddWithValue("@passwordHash", passwordHash);
            //    command.Parameters.AddWithValue("@passwordSalt", salt);

            //    connection.Open();
            //    command.ExecuteNonQuery();
            //}
        }

        public User LogIn(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using (var context = new SharedTasksDataContext(_connectionstring))
            {
                return context.Users.FirstOrDefault(u => u.EmailAddress == email);
            }
        }
    }
}
