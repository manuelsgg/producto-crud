using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
using System.Reflection.Metadata;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {

        private readonly EmpresaContext dbContext;

        public DocumentoController(EmpresaContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "vendedor")]
        public async Task<IActionResult> GetDocumentos()
        {
            var documents = await dbContext.Documentos.ToListAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "vendedor")]
        public async Task<IActionResult> GetDocumento(int id)
        {
            var document = await dbContext.Documentos.FindAsync(id);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "vendedor")]
        public async Task<IActionResult> SubirDocumento(IFormFile file, [FromForm] string nombreDocumento)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se pudo subir documento.");

            if (string.IsNullOrEmpty(nombreDocumento))
                return BadRequest("El nombre del documento es requerido.");

            var documento = new Documento
            {
                NombreDocumento = nombreDocumento,
                Tipo = Path.GetExtension(file.FileName),
                Ruta = Path.Combine("Archivo", file.FileName),
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

          
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Archivo", file.FileName);
            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           
            dbContext.Documentos.Add(documento);
            await dbContext.SaveChangesAsync();

            return Ok(documento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "vendedor")]
        public async Task<IActionResult> ActualizarDocumento(int id, Documento updatedDocument)
        {
            var documento = await dbContext.Documentos.FindAsync(id);
            if (documento == null)
                return NotFound();

            documento.NombreDocumento = updatedDocument.NombreDocumento;
            documento.FechaModificacion = DateTime.Now;

            dbContext.Documentos.Update(documento);
            await dbContext.SaveChangesAsync();

            return Ok(documento);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "vendedor")]
        public async Task<IActionResult> EliminarDocumento(int id)
        {
            var documento = await dbContext.Documentos.FindAsync(id);
            if (documento == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), documento.Ruta);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            dbContext.Documentos.Remove(documento);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("generar-pdf")]
        [Authorize(Roles = "vendedor, cliente")]
        public async Task<IActionResult> GenerarPdf()
        {

            var productos = dbContext.Productos
                        .Select(x => new
                        {
                            x.ProductoId,
                            x.NombreProducto,
                            CNombreCategoria = x.Categoria.Nombre,
                            x.Precio,
                            x.FechaCreacion
                        })
                        .ToList();

           

            using (var stream = new MemoryStream())
            {
                var pdf = new PdfSharp.Pdf.PdfDocument();
                var page = pdf.AddPage();
                var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                var font = new PdfSharp.Drawing.XFont("Arial", 12);
                var font2 = new PdfSharp.Drawing.XFont("Arial Black", 20);
                var boldFont = new PdfSharp.Drawing.XFont("Arial", 14);


                gfx.DrawString($"PRODUCTOS:", font2, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(10, 60, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);

                int yPosition = 100;

                yPosition += 20;

                gfx.DrawString("Nombre Producto", boldFont, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);
                gfx.DrawString("Categoría", boldFont, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(200, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);
                gfx.DrawString("Precio", boldFont, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(350, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);

                yPosition += 20;

                foreach (var producto in productos)
                {
                    gfx.DrawString(producto.NombreProducto, font, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(50, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(producto.CNombreCategoria, font, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(200, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(producto.Precio.ToString("C"), font, PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XRect(350, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);

                    yPosition += 20;

                    if (yPosition > page.Height - 50)
                    {
                        page = pdf.AddPage();
                        gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
                        yPosition = 50;
                    }
                }

                pdf.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", $"productos.pdf");
            }
        }

        [HttpGet("{id}/download")]
        [Authorize(Roles = "vendedor, cliente")]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
          var document = await dbContext.Documentos.FindAsync(id);

        if (document == null)
            return NotFound();

            // Construir la ruta completa del archivo
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(rootPath, document.Ruta);

            if (!System.IO.File.Exists(filePath))
            return NotFound("El archivo no existe.");

        // Determinar el tipo de contenido basado en la extensión del archivo
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return PhysicalFile(filePath, contentType, document.NombreDocumento);
        }

    }
}
