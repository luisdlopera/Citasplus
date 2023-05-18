using CitasPlus.Models;
using Dapper;
using System.Data.SqlClient;

namespace CitasPlus.Repository.Account
{
    public class RepositoryAccount : IRepositoryAccount
    {
        private readonly string connectioString;
        public RepositoryAccount(IConfiguration configuration)
        {
            connectioString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<dynamic> Login(LoginViewModel model)
        {
            using (var connection = new SqlConnection(connectioString))
            {
                try
                {
                    return await connection.QueryAsync<dynamic>("exec sp_ValidateLogin @User, @Pass",
                                                                new
                                                                { User = model.User, Pass = model.Pass });
                }
                catch
                {
                    return new { Rpta = "Error en la transacción", Cod = "-1" };
                }
            }
        }
        public async Task<dynamic> ValidateUserById(ValidateUserByIdViewModel model)
        {
            using (var connection = new SqlConnection(connectioString))
            {
                try
                {
                    return await connection.QueryAsync<dynamic>("exec sp_ValidateUserById @User_Id", new { User_Id = model.User_Id });
                }
                catch
                {
                    return new { Rpta = "Error en la transacción", Cod = "-1" };
                }
            }
        }
    }
}
