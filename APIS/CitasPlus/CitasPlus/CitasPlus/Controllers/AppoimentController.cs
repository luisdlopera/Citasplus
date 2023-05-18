using CitasPlus.Helpers.Token;
using CitasPlus.Models;
using CitasPlus.Repository.Account;
using CitasPlus.Repository.Appoiment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitasPlus.Controllers
{
    [EnableCors("PolicyCore")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AppoimentController : ControllerBase
    {
        private readonly IRepositoryAppoiment _repository;
        public AppoimentController(IRepositoryAppoiment repository)
        {
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppoiment(CreateAppoimentViewModel model)
        {
            var userIdClaim = User.FindFirstValue("User_Id");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            model.User_Id = Guid.Parse(userIdClaim);
            var peticion = await _repository.CreateAppoiment(model);
            var response = peticion[0];
            return Ok(new
            {
                Rpta = response.Rpta,
                Cod = response.Cod
            });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAppoiment(DeleteAppoimentViewModel model)
        {
            var userIdClaim = User.FindFirstValue("User_Id");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            model.User_Id = Guid.Parse(userIdClaim);
            var peticion = await _repository.DeleteAppoiment(model);
            var response = peticion[0];
            return Ok(new
            {
                Rpta = response.Rpta,
                Cod = response.Cod
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAppoimentByUser()
        {
            var userIdClaim = User.FindFirstValue("User_Id");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            GetAppoimentsByUserViewModel model = new();
            model.User_Id = Guid.Parse(userIdClaim);
            var peticion = await _repository.GetAppoimentByUser(model);
            List<object> appoiments = new List<object>();
            if (peticion.Count == 0)
            {
                return Ok(new
                {
                    Appoiments = appoiments
                });

            }
            var response = peticion[0];
            if (response.Id != null)
            {
                foreach (var item in peticion)
                {
                    var appoiment = new
                    {
                        Id = item.Id,
                        Name = item.StrNameClient,
                        DateStart = item.DtDateStart,
                        IntEnd = item.IntEnd
                    };
                    appoiments.Add(appoiment);
                }
            }
            else
            {
                return Ok(new
                {
                    Rpta = response.Rpta,
                    Cod = response.Cod
                });
            }
            return Ok(new
            {
                Appoiments = appoiments
            });
        }
    }
}
