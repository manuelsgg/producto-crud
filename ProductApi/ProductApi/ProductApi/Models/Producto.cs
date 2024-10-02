using System;
using System.Collections.Generic;

namespace ProductApi.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public int? CategoriaId { get; set; }

    public string NombreProducto { get; set; } = null!;

    public double Precio { get; set; }

    public int? UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual Usuario? UsuarioCreacionNavigation { get; set; }
}
