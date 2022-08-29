using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }


        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().FirstOrDefaultAsync(product => product.Id == id);            
            return Ok(products);
        }


        [HttpGet]
        [Route("categories/{id:int}")] //products/categories/1
        public async Task<ActionResult<List<Product>>> GetByCategory(int id)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().Where(product => product.CategoryId == id).ToListAsync();
            return Ok(products);
        }


        [HttpGet]
        [Route("categories/{categoryTitle}")] //products/categories/title-category
        public async Task<ActionResult<List<Product>>> GetByCategoryTitle(string categoryTitle)
        {
            var products = await _context.Products.Include(product => product.Category).AsNoTracking().Where(product => product.Category.Title == categoryTitle).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
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
                return BadRequest(new { message = $"Não foi possível criar este produto, {ex.Message}"});
            }
        }
    }
}