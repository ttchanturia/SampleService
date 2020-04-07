using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSample.Models;

namespace RestSample.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DbHelper _dbHelper;

        public UsersController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserListItemDto>>> List()
            => Ok((await _dbHelper.ListUsersAsync())
                .Select(x => new UserListItemDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    Country = x.Country,
                    Email = x.Email
                }));

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateUserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _dbHelper.CreateUserAsync(new User
            {
                Username = user.Username,
                Email = user.Email,
                Country = user.Country,
                Password = user.Password
            });

            return Ok();
        }
    }
}