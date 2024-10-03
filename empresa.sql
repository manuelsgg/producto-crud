create database empresa;
go

use empresa
CREATE TABLE Usuarios (
    UsuarioId INT PRIMARY KEY IDENTITY,
    Email NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL 
);

CREATE TABLE Categorias (
    CategoriaId INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100) NOT NULL
);

CREATE TABLE Productos (
    ProductoId INT PRIMARY KEY IDENTITY,
    CategoriaId INT FOREIGN KEY REFERENCES Categorias(CategoriaId),
    NombreProducto NVARCHAR(100) NOT NULL,
    Precio FLOAT NOT NULL,
    UsuarioCreacion INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

SET IDENTITY_INSERT dbo.Usuarios ON 

INSERT dbo.Usuarios (UsuarioId, Email, PasswordHash, Role) VALUES (1, N'vedendor1@gmail.com', N'$2a$12$9Q0hXZb/NV4SGxTR9WrvBeIMWmy/jA1Wk/mH.MUxdYwAQw7q7oFlG', N'vendedor')
INSERT dbo.Usuarios (UsuarioId, Email, PasswordHash, Role) VALUES (2, N'cliente1@gmail.com', N'$2a$12$YfuoIKbF.zmMHBBZmjAXn.e.YSBNqMlzxHguYP98IUzpp1irVD9Da', N'cliente')
SET IDENTITY_INSERT dbo.Usuarios OFF
GO

SET IDENTITY_INSERT dbo.Categorias ON 

INSERT dbo.Categorias (CategoriaId, Nombre) VALUES (1, N'Vegetales')
INSERT dbo.Categorias (CategoriaId, Nombre) VALUES (2, N'Abarrotes')
INSERT dbo.Categorias (CategoriaId, Nombre) VALUES (3, N'Bebidas')
INSERT dbo.Categorias (CategoriaId, Nombre) VALUES (4, N'Artefacto Electrico')
SET IDENTITY_INSERT dbo.Categorias OFF
GO

SET IDENTITY_INSERT dbo.Productos ON 

INSERT dbo.Productos (ProductoId, CategoriaId, NombreProducto, Precio, UsuarioCreacion, FechaCreacion) VALUES (1, 1, N'Manzana', 16, 1, CAST(N'2024-09-02T06:50:30.577' AS DateTime))
INSERT dbo.Productos (ProductoId, CategoriaId, NombreProducto, Precio, UsuarioCreacion, FechaCreacion) VALUES (2, 2, N'Frejoles', 14.5, 1, CAST(N'2024-09-02T07:01:12.490' AS DateTime))
SET IDENTITY_INSERT dbo.Productos OFF
GO


CREATE TABLE Documento (
    DocumentoId INT PRIMARY KEY IDENTITY(1,1), 
    NombreDocumento NVARCHAR(255) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL, 
    Ruta NVARCHAR(500) NOT NULL, 
    FechaCreacion DATETIME NOT NULL,     
    FechaModificacion DATETIME NOT NULL 
);
