using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private DataContext _context;

        public ProductController(DataContext context) => _context = context;

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }


        [HttpGet]
        [Route("{id:int}")]
        // [AllowAnonymous]
        [Authorize]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().FirstOrDefaultAsync(product => product.Id == id);
            return Ok(products);
        }


        [HttpGet]
        [Route("categories/{id:int}")] //products/categories/1
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().Where(product => product.CategoryId == id).ToListAsync();
            return Ok(products);
        }


        [HttpGet]
        [Route("categories/{categoryTitle}")] //products/categories/title-category
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetByCategoryTitle(string categoryTitle)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().Where(product => product.Category.Title == categoryTitle).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> Post([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Não foi possível criar este produto, {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> Put(int id, [FromBody] Product model)
        {
            // Verifica se o ID ifnromado é o mesmo do modelo
            if (model.Id != id)
                return NotFound(new { menssage = "Produto não encontrado" });

            // Verifica se os dados não são inválidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<Product>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar este produto"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "gerente")]
        public async Task<ActionResult<List<Product>>> Delete(int id)
        {
            // Verifica se os dados não são inválidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
            if (product == null)
                return NotFound(new { message = "Não encontramos esse produto." });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso!" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover esta produto." });
            }
        }

    }
}