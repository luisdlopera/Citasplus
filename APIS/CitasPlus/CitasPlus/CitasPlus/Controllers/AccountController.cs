using CitasPlus.Helpers.Token;
using CitasPlus.Models;
using CitasPlus.Repository.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitasPlus.Controllers
{
    [EnableCors("PolicyCore")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenHelper _Token;
        private readonly IRepositoryAccount _repository;
        public AccountController(ITokenHelper token, IRepositoryAccount repository)
        {
            _Token = token;
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var peticion = await _repository.Login(model);
            var response = peticion[0];
            if (response.Id != null)
            {
                return Ok(new
                {
                    Name = response.StrName,
                    Cod = response.Cod,
                    Token = _Token.CreateToken(new[] { new Claim("User_Id", response.Id.ToString()) }, TimeSpan.FromMinutes(2))
                });
            }
            else
            {
                return Ok(new
                {
                    Rpta = response.Rpta,
                    Cod = response.Cod
                });
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ValidateUser()
        {
            var userIdClaim = User.FindFirstValue("User_Id");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            ValidateUserByIdViewModel model = new()
            {
                User_Id = Guid.Parse(userIdClaim)
            };
            var peticion = await _repository.ValidateUserById(model);
            var response = peticion[0];
            if (response.Id != null)
            {
                return Ok(new
                {
                    Name = response.StrName,
                    Cod = response.Cod,
                    Token = _Token.CreateToken(new[] { new Claim("User_Id", response.Id.ToString()) }, TimeSpan.FromMinutes(30))
                });
            }
            else
            {
                return Ok(new
                {
                    Rpta = response.Rpta,
                    Cod = response.Cod
                });
            }
        }
    }
}
