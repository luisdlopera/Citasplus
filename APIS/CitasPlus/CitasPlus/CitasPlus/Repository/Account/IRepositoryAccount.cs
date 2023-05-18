using CitasPlus.Models;

namespace CitasPlus.Repository.Account
{
    public interface IRepositoryAccount
    {
        Task<dynamic> Login(LoginViewModel model);
        Task<dynamic> ValidateUserById(ValidateUserByIdViewModel model);
    }
}
