using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Data;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Route("categories")]
public class CategoryController : ControllerBase
{
    
   /* It's a private variable. */
    private DataContext _context;
    
   /* It's a constructor. */
    public CategoryController(DataContext context) => _context = context;   

    /// <summary>
    /// It's an asynchronous function that returns an ActionResult of type List<Category> and has no
    /// parameters
    /// </summary>
    [HttpGet]
    [Route("")]
    [Authorize]
    //[ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)] //Método para colocar um Cash de 30 minutos nesse endpoint
    //[ResponseCache(VaryByHeader = Duration = 30, Location = ResponseCacheLocation.None, NoStore = true )] //Método para remover o endpoint de cash caso use o services.AddResponseCoching(); no classe Startup.cs
    public async Task<ActionResult<List<Category>>> Get()
    {
        var categories = await _context.Categories.AsNoTracking<Category>().ToListAsync();
        return Ok(categories);
    }

/// <summary>
/// This function returns a category object from the database based on the id passed in
/// </summary>
/// <param name="id">The id of the category you want to get</param>
    [HttpGet]
    [Route("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync( category => category.Id == id);
        if(category == null)
        return NotFound(new { message = "Não encontramos essa categoria"});

        try
        {
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message});
        }
    }

    /// <summary>
    /// This function is an HTTP POST request that takes a Category object as a parameter and returns a
    /// list of Category objects
    /// </summary>
    /// <param name="Category">The model that we're going to be using for this API.</param>
    [HttpPost]
    [Route("")]
    [Authorize(Roles = "FUNCIONÁRIO")]
    public async Task<ActionResult<List<Category>>> Post([FromBody] Category model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _context.Categories.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível criar esta categoria"});
        }
    }

    /// <summary>
    /// It updates the category with the given id.
    /// </summary>
    /// <param name="id">The id of the category to update</param>
    /// <param name="Category">The model that will be used to update the database.</param>
    [HttpPut]
    [Route("{id:int}")]
    [Authorize]
    public async Task<ActionResult<List<Category>>> Put(int id, [FromBody] Category model)
    {
        // Verifica se o ID ifnromado é o mesmo do modelo
        if (model.Id != id)
            return NotFound(new { menssage = "Categoria não encontrada" });

        // Verifica se os dados não são inválidos
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _context.Entry<Category>(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível atualizar esta categoria"});
        }
    }

    /// <summary>
    /// It deletes a category by id.
    /// </summary>
    /// <param name="id">int - This is the route parameter. It's the id of the category we want to
    /// delete.</param>
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize]
    public async Task<ActionResult<List<Category>>> Delete(int id)
    {
        // Verifica se os dados não são inválidos
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = await _context.Categories.FirstOrDefaultAsync( category => category.Id == id);
        if(category == null)
        return NotFound(new { message = "Não encontramos essa categoria"});

        try
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso!"});
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível remover esta categoria"});
        }
    }

}