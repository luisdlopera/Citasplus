using CitasPlus.Models;
using Dapper;
using System.Data.SqlClient;

namespace CitasPlus.Repository.Appoiment
{
    public class RepositoryAppoiment : IRepositoryAppoiment
    {
        private readonly string connectioString;
        public RepositoryAppoiment(IConfiguration configuration)
        {
            connectioString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<dynamic> CreateAppoiment(CreateAppoimentViewModel model)
        {
            using (var connection = new SqlConnection(connectioString))
            {
                try
                {
                    return await connection.QueryAsync<dynamic>("exec sp_CreateAppoiment @User_Id, @StrNameClient, @DtDateStart, @Dtestimated",
                                                                new
                                                                {
                                                                    User_Id = model.User_Id,
                                                                    StrNameClient = model.NameClient,
                                                                    DtDateStart = model.DateStart,
                                                                    Dtestimated = model.DateEstimated
                                                                });
                }
                catch
                {
                    return new { Rpta = "Error en la transacción", Cod = "-1" };
                }
            }
        }
        public async Task<dynamic> DeleteAppoiment(DeleteAppoimentViewModel model)
        {
            using (var connection = new SqlConnection(connectioString))
            {
                try
                {
                    return await connection.QueryAsync<dynamic>("exec sp_DeleteAppoiment @User_Id, @Appoiment_Id",
                                                                new
                                                                { User_Id = model.User_Id, Appoiment_Id = model.Appoiment_Id });
                }
                catch
                {
                    return new { Rpta = "Error en la transacción", Cod = "-1" };
                }
            }
        }
        public async Task<dynamic> GetAppoimentByUser(GetAppoimentsByUserViewModel model)
        {
            using (var connection = new SqlConnection(connectioString))
            {
                try
                {
                    return await connection.QueryAsync<dynamic>("exec sp_GetAppoimentByUser @User_Id",
                                                                new
                                                                { User_Id = model.User_Id });
                }
                catch
                {
                    return new { Rpta = "Error en la transacción", Cod = "-1" };
                }
            }
        }
    }
}
