using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Data;
using System;
using Microsoft.EntityFrameworkCore;

[Route("categories")]
public class CategoryController : ControllerBase
{
    private DataContext _context;

    public CategoryController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Get()
    {
        var categories = await _context.Categories.AsNoTracking<Category>().ToListAsync();
        return Ok(categories);
    }


    [HttpGet]
    [Route("{id:int}")]
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

    [HttpPost]
    [Route("")]
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

    [HttpPut]
    [Route("{id:int}")]
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

    [HttpDelete]
    [Route("{id:int}")]
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
            return BadRequest(new { message = "Não foi possível atualizar esta categoria"});
        }
    }

}