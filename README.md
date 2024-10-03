Tecnologías a usar:
React como front NODE versión v20.17.0
net8 como back. 
SQL SERVER.

Instrucciones:
- Ejecutar el archivo empresa.sql en SQL SERVER

 - En la carpeta de react/prudctoapp ejecutar:npm install y luego npm run (asegurese que NODE sea versión v20.17.0 )

 - Abrir la carpeta ProductApi y ejecutar ProductApi.sln para el lado del backend

Adjunto usuarios:
usuario profesor: vendedor1@gmail.com
password: 12345678

usuario profesor: cliente1@gmail.com
password: 12345678


PARA LA PARTE 2 EJECUTAR ESTE QUERY PARA CREAR LA TABLA DOCUMENTOS:

CREATE TABLE Documento (
    DocumentoId INT PRIMARY KEY IDENTITY(1,1), 
    NombreDocumento NVARCHAR(255) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL, 
    Ruta NVARCHAR(500) NOT NULL, 
    FechaCreacion DATETIME NOT NULL,     
    FechaModificacion DATETIME NOT NULL 
);

