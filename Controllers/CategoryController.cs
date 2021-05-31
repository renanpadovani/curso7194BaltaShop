using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Shop.Data;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    [AllowAnonymous]
    public class CategoryController : ControllerBase 
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Models.Category>>> Get([FromServices]DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return categories;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Models.Category>> GetById(
            int id,
            [FromServices]DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return category;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Models.Category>>> Post(
                [FromBody]Models.Category category,
                [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
        
            try
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch
            {
                 return BadRequest(new { message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Models.Category>>> Put(
            int id, 
            [FromBody]Models.Category category, 
            [FromServices]DataContext context)
        {
            if (category.Id != id)
                return NotFound(new { message = "Categoria não encontrada"});

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Models.Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }         
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Models.Category>>> Delete(
            int id,
            [FromServices]DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}