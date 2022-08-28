using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;

[Route("categories")]
public class CategoryController : ControllerBase 
{

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Get()
    {
        return new List<Category>();
    }


    [HttpGet]
    [Route("{id:int}")]
    public  async Task<ActionResult<Category>> GetById(int id)
    {
        return new Category();
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Post([FromBody]Category model)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok();
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model)
    {
        // Verifica se o ID ifnromado é o mesmo do modelo
       if(model.Id != id)
       return NotFound(new { menssage = "Categoria não encontrada"});

       // Verifica se os dados não são inválidos
       if(!ModelState.IsValid)
       return BadRequest(ModelState);

       return Ok(model);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Delete(int id)
    {
        return Ok();
    }
    
}