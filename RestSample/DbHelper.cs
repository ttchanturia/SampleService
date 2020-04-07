using RestSample.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RestSample
{
    public sealed class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<User>> ListUsersAsync()
        {
            var result = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"SELECT [Id]
                                                      ,[UserName]
                                                      ,[Password]
                                                      ,[Country]
                                                      ,[Email]
                                                FROM [dbo].[RegistrationTable]", connection);
                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        result.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = GetString(reader["UserName"]),
                            Country = GetString(reader["Country"]),
                            Password = GetString(reader["Password"]),
                            Email = GetString(reader["Email"])
                        });
                    }
                }
                finally
                {
                    connection.Close();
                }

                return result;

                string GetString(object sqlReader) => !(sqlReader is DBNull) ? sqlReader.ToString() : null;
            }
        }

        public async Task<List<User>> CreateUserAsync(User user)
        {
            var result = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO [dbo].[RegistrationTable] (UserName,Password,Country,Email) values(@UserName,@Password,@Country,@Email)", connection);
                try
                {
                    command.Parameters.AddWithValue("@UserName", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Country", user.Country);
                    command.Parameters.AddWithValue("@Email", user.Email);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    connection.Close();
                }

                return result;
            }
        }
    }
}
