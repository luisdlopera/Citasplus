using CitasPlus.Models;

namespace CitasPlus.Repository.Appoiment
{
    public interface IRepositoryAppoiment
    {
        Task<dynamic> CreateAppoiment(CreateAppoimentViewModel model);
        Task<dynamic> DeleteAppoiment(DeleteAppoimentViewModel model);
        Task<dynamic> GetAppoimentByUser(GetAppoimentsByUserViewModel model);
    }
}