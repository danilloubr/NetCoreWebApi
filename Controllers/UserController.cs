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

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<User>> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "funcionário".ToUpper();
                }

                var newUser = new User{
                    UserName = user.UserName,
                    Password = user.Password,
                    Role = user.Role.ToUpper()
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usuário criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar este usuário, {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authentication([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userTemp = await _context.Users.AsNoTracking().Where(userDb => userDb.UserName == user.UserName && userDb.Password == user.Password).FirstOrDefaultAsync();

            if (userTemp == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

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

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ActionResult<List<User>>> EditUser(int id, [FromBody] User model)
        {
            // Verifica se o ID ifnromado é o mesmo do modelo
            if (model.Id != id)
                return NotFound(new { menssage = "Usuário não encontrado" });

            // Verifica se os dados não são inválidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<User>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar este usuário" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "gerente")]
        public async Task<ActionResult<List<User>>> DeleteUser(int id)
        {
            // Verifica se os dados não são inválidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(product => product.Id == id);
            if (user == null)
                return NotFound(new { message = "Não encontramos esse usuário." });

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido com sucesso!" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover este usuário." });
            }
        }


    }
}