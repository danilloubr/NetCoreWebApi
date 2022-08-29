using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private DataContext _context;

        public UserController(DataContext context) => _context = context;

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar este usuário, {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authentication([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userTemp = await _context.Users.AsNoTracking().Where(userDb => userDb.UserName == user.UserName && userDb.Password == user.Password).FirstOrDefaultAsync();

            if (userTemp == null)
                return NotFound(new { message = "Usuário ou senha inaválidos" });

            var token = TokenService.GeneretorToken(userTemp);
            return new
            {
                user = new
                {
                    id = userTemp.Id,
                    userName = userTemp.UserName,
                    role = userTemp.Role
                },
                token = token
            };
        }


    }
}