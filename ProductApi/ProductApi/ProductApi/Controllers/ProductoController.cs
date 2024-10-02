using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTO;
using ProductApi.Models;
using System.Security.Claims;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly EmpresaContext dbContext;

        public ProductoController(EmpresaContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "vendedor, cliente")]
        public IActionResult GetProductos()
        {
            try
            {
                var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var usuarioRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (usuarioRole == "vendedor")
                {                
                    var productos = dbContext.Productos
                        .Select(x => new
                        {
                            x.ProductoId,
                            x.NombreProducto,
                            x.Precio,
                            x.FechaCreacion,
                            x.CategoriaId,
                            CNombreCategoria = x.Categoria.Nombre,
                        })
                        .ToList();
                    return Ok(productos);
                }
                else
                {
                    var productos = dbContext.Productos.Select(n => new ProductoDTO { ProductoId = n.ProductoId, NombreProducto = n.NombreProducto, Precio = n.Precio })
                        .ToList();
                    return Ok(productos);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener productos: " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "vendedor")]
        public IActionResult AgregarProducto([FromBody] Producto producto)
        {
            try
            {
                var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (usuarioId == null)
                {
                    return Unauthorized("No se encontro usuario");
                }

                producto.UsuarioCreacion = int.Parse((usuarioId));
                producto.FechaCreacion = DateTime.UtcNow;
                dbContext.Productos.Add(producto);

                dbContext.SaveChanges();
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al guardar Producto: " + ex.Message);
            }

        }


        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        public IActionResult EditarProducto(int id, [FromBody] Producto producto)
        {
            try
            {
                var existProducto = dbContext.Productos.Find(id);
                if (existProducto == null) return NotFound();

                existProducto.NombreProducto = producto.NombreProducto;
                existProducto.Precio = producto.Precio;

                dbContext.SaveChanges();
                return Ok(existProducto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar Producto: " + ex.Message);
            }
        }

 
        [HttpDelete("{id}")]
        [Authorize(Roles = "vendedor")]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                var producto = dbContext.Productos.Find(id);
                if (producto == null) return NotFound();

                dbContext.Productos.Remove(producto);
                dbContext.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar Producto: " + ex.Message);
            }
        }
    }
}
