using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly EmpresaContext dbContext;

        public CategoriaController(EmpresaContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "vendedor")]
        public IActionResult GetCategorias()
        {
            try
            {
                var categorias = dbContext.Categorias.Select(s => new
                {
                    s.CategoriaId,
                    s.Nombre
                }).ToList();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener Categorias: " + ex.Message);
            }

        }

        [HttpPost]
        [Authorize(Roles = "vendedor")]
        public IActionResult CreateCategoria([FromBody] Categoria newCategoria)
        {
            try
            {
                dbContext.Categorias.Add(newCategoria);
                dbContext.SaveChanges();
                return Ok(newCategoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear Categoria: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        public IActionResult UpdateCategoria(int id, [FromBody] Categoria updatedCategoria)
        {
            try
            {
                var categoria = dbContext.Categorias.Find(id);
                if (categoria == null)
                    return NotFound();

                categoria.Nombre = updatedCategoria.Nombre;
                dbContext.SaveChanges();
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar Categoria: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "vendedor")]
        public IActionResult DeleteCategoria(int id)
        {
            try
            {
                var categoria = dbContext.Categorias.Find(id);
                if (categoria == null)
                    return NotFound();

                dbContext.Categorias.Remove(categoria);
                dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar Categoria: " + ex.Message);
            }
        }
    }
}
