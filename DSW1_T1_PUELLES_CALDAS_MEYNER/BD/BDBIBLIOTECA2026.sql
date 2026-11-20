CREATE DATABASE BDBIBLIOTECA2026
GO

USE BDBIBLIOTECA2026
GO

CREATE TABLE dbo.CategoriasLibro
(
    CodCategoria  CHAR(5)      NOT NULL,
    NombreCategoria VARCHAR(60) NOT NULL,
    Descripcion   VARCHAR(150) NULL,
    CONSTRAINT PK_CategoriasLibro PRIMARY KEY (CodCategoria)
)
GO

CREATE TABLE dbo.Libros
(
    CodLibro       CHAR(6)       NOT NULL,
    CodCategoria   CHAR(5)       NOT NULL,
    Titulo         VARCHAR(120)  NOT NULL,
    ISBN           VARCHAR(20)   NOT NULL,
    AnioPublicacion INT          NOT NULL,
    Stock          INT           NOT NULL CONSTRAINT DF_Libros_Stock DEFAULT (0),
    Estado         TINYINT       NOT NULL CONSTRAINT DF_Libros_Estado DEFAULT (1),
    CONSTRAINT PK_Libros PRIMARY KEY (CodLibro),
    CONSTRAINT UQ_Libros_ISBN UNIQUE (ISBN),
    CONSTRAINT FK_Libros_CategoriasLibro FOREIGN KEY (CodCategoria)
        REFERENCES dbo.CategoriasLibro(CodCategoria),
    CONSTRAINT CK_Libros_Anio CHECK (AnioPublicacion BETWEEN 1450 AND 2100),
    CONSTRAINT CK_Libros_Stock CHECK (Stock >= 0),
    CONSTRAINT CK_Libros_Estado CHECK (Estado IN (0,1))
)
GO

CREATE TABLE dbo.Estudiantes
(
    CodEstudiante CHAR(6)      NOT NULL,
    Apellidos     VARCHAR(80)  NOT NULL,
    Nombres       VARCHAR(80)  NOT NULL,
    CONSTRAINT PK_Estudiantes PRIMARY KEY (CodEstudiante)
)
GO

CREATE TABLE dbo.Prestamos
(
    IdPrestamo      INT IDENTITY(1,1) NOT NULL,
    CodLibro        CHAR(6)           NOT NULL,
    CodEstudiante   CHAR(6)           NOT NULL,
    FechaPrestamo   DATE              NOT NULL,
    FechaDevolucion DATE              NULL,
    EstadoPrestamo  VARCHAR(15)       NOT NULL,
    Multa           DECIMAL(8,2)      NOT NULL CONSTRAINT DF_Prestamos_Multa DEFAULT (0),
    CONSTRAINT PK_Prestamos PRIMARY KEY (IdPrestamo),
    CONSTRAINT FK_Prestamos_Libros FOREIGN KEY (CodLibro)
        REFERENCES dbo.Libros(CodLibro),
    CONSTRAINT FK_Prestamos_Estudiantes FOREIGN KEY (CodEstudiante)
        REFERENCES dbo.Estudiantes(CodEstudiante),
    CONSTRAINT CK_Prestamos_Estado CHECK (EstadoPrestamo IN ('Pendiente','Devuelto','Retrasado')),
    CONSTRAINT CK_Prestamos_Multa CHECK (Multa >= 0)
)
GO

INSERT INTO dbo.CategoriasLibro (CodCategoria, NombreCategoria, Descripcion)
VALUES
    ('CAT01', N'Programación',                N'Lenguajes, buenas prácticas y desarrollo de software'),
    ('CAT02', N'Bases de Datos',              N'Modelado, SQL y administración de datos'),
    ('CAT03', N'Redes y Comunicaciones',      N'Protocolos, arquitectura y seguridad de redes'),
    ('CAT04', N'Ingeniería de Software',      N'Procesos, requisitos, diseño y construcción de software'),
    ('CAT05', N'Algoritmos',                  N'Estructuras de datos y análisis de algoritmos'),
    ('CAT06', N'Inteligencia Artificial',     N'Aprendizaje automático y sistemas inteligentes'),
    ('CAT07', N'Matemática',                  NULL);
GO

INSERT INTO dbo.Libros (CodLibro, CodCategoria, Titulo, ISBN, AnioPublicacion, Stock, Estado)
VALUES
    ('LIB001', 'CAT01', N'Clean Code',                                       '9780132350884', 2008, 5, 1),
    ('LIB002', 'CAT01', N'The Pragmatic Programmer',                         '9780135957059', 2019, 4, 1),
    ('LIB003', 'CAT01', N'Refactoring',                                      '9780134757599', 2018, 3, 1),
    ('LIB004', 'CAT02', N'Database System Concepts',                         '9780078022159', 2019, 6, 1),
    ('LIB005', 'CAT02', N'Fundamentals of Database Systems',                 '9780133970777', 2015, 4, 1),
    ('LIB006', 'CAT03', N'Computer Networking: A Top-Down Approach',         '9780136681557', 2020, 3, 1),
    ('LIB007', 'CAT04', N'Software Engineering',                             '9780133943030', 2015, 5, 1),
    ('LIB008', 'CAT04', N'Code Complete',                                    '9780735619678', 2004, 2, 1),
    ('LIB009', 'CAT05', N'Introduction to Algorithms',                       '9780262046305', 2022, 4, 1),
    ('LIB010', 'CAT01', N'Design Patterns',                                  '9780201633610', 1994, 3, 1),
    ('LIB011', 'CAT06', N'Artificial Intelligence: A Modern Approach',       '9780134610993', 2020, 2, 1),
    ('LIB012', 'CAT03', N'Computer Networks',                                '9780132126953', 2010, 0, 0);
GO

INSERT INTO dbo.Estudiantes (CodEstudiante, Apellidos, Nombres)
VALUES
    ('EST001', N'Quispe Mamani',     N'Luis Alberto'),
    ('EST002', N'Flores Huamán',     N'Camila Sofía'),
    ('EST003', N'Ramírez Torres',    N'Diego Andrés'),
    ('EST004', N'Castillo Rojas',    N'Valeria Nicole'),
    ('EST005', N'Mendoza Salazar',   N'Jorge Luis'),
    ('EST006', N'Paredes Vega',      N'Andrea Lucía'),
    ('EST007', N'Gutiérrez Ponce',   N'Renzo Matías'),
    ('EST008', N'Soto Chávez',       N'Daniela Fernanda');
GO

INSERT INTO dbo.Prestamos (CodLibro, CodEstudiante, FechaPrestamo, FechaDevolucion, EstadoPrestamo, Multa)
VALUES
    ('LIB001', 'EST003', '2024-03-11', '2024-03-18', 'Devuelto',   0.00),
    ('LIB002', 'EST005', '2024-06-17', '2024-06-24', 'Devuelto',   0.00),
    ('LIB004', 'EST002', '2024-10-07', '2024-10-14', 'Devuelto',   0.00),

    ('LIB001', 'EST002', '2025-02-10', '2025-02-17', 'Devuelto',   0.00),
    ('LIB004', 'EST005', '2025-03-17', '2025-03-31', 'Devuelto',   8.00),
    ('LIB002', 'EST008', '2025-04-07', '2025-04-14', 'Devuelto',   0.00),
    ('LIB007', 'EST003', '2025-05-12', '2025-05-19', 'Devuelto',   0.00),
    ('LIB005', 'EST001', '2025-06-23', '2025-06-30', 'Devuelto',   0.00),
    ('LIB009', 'EST006', '2025-08-18', '2025-08-25', 'Devuelto',   0.00),
    ('LIB010', 'EST007', '2025-09-22', '2025-10-06', 'Devuelto',  10.00),
    ('LIB003', 'EST004', '2025-11-03', '2025-11-10', 'Devuelto',   0.00),

    ('LIB001', 'EST001', '2026-01-12', '2026-01-19', 'Devuelto',   0.00),
    ('LIB002', 'EST002', '2026-01-20', '2026-01-27', 'Devuelto',   0.00),
    ('LIB003', 'EST003', '2026-02-03', '2026-02-17', 'Devuelto',   5.00),
    ('LIB005', 'EST004', '2026-02-16', '2026-02-23', 'Devuelto',   0.00),
    ('LIB001', 'EST005', '2026-03-09', '2026-03-20', 'Devuelto',   6.00),
    ('LIB007', 'EST006', '2026-03-23', '2026-03-30', 'Devuelto',   0.00),
    ('LIB004', 'EST007', '2026-04-06', '2026-04-13', 'Devuelto',   0.00),
    ('LIB009', 'EST008', '2026-04-21', '2026-05-05', 'Devuelto',  10.00),
    ('LIB002', 'EST001', '2026-05-11', '2026-05-18', 'Devuelto',   0.00),
    ('LIB010', 'EST003', '2026-06-01', '2026-06-08', 'Devuelto',   0.00),
    ('LIB006', 'EST002', '2026-08-17', '2026-08-24', 'Devuelto',   0.00),
    ('LIB008', 'EST004', '2026-09-14', NULL,         'Retrasado', 24.00),
    ('LIB003', 'EST006', '2026-10-19', NULL,         'Retrasado', 12.00),
    ('LIB005', 'EST005', '2026-11-10', NULL,         'Pendiente',  0.00),
    ('LIB001', 'EST007', '2026-11-16', NULL,         'Pendiente',  0.00);
GO

CREATE PROCEDURE dbo.usp_ListarCategorias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CodCategoria, NombreCategoria, Descripcion
    FROM dbo.CategoriasLibro
    ORDER BY NombreCategoria;
END;
GO

CREATE PROCEDURE dbo.usp_ListarLibros
    @Buscar NVARCHAR(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT l.CodLibro,
           l.CodCategoria,
           c.NombreCategoria,
           l.Titulo,
           l.ISBN,
           l.AnioPublicacion,
           l.Stock,
           l.Estado
    FROM dbo.Libros AS l
    INNER JOIN dbo.CategoriasLibro AS c ON c.CodCategoria = l.CodCategoria
    WHERE @Buscar IS NULL
       OR l.CodLibro        COLLATE Latin1_General_CI_AI LIKE N'%' + @Buscar + N'%'
       OR l.Titulo          COLLATE Latin1_General_CI_AI LIKE N'%' + @Buscar + N'%'
       OR c.NombreCategoria COLLATE Latin1_General_CI_AI LIKE N'%' + @Buscar + N'%'
    ORDER BY l.CodLibro;
END;
GO

CREATE PROCEDURE dbo.usp_RegistrarLibro
    @CodLibro CHAR(6),
    @CodCategoria CHAR(5),
    @Titulo VARCHAR(120),
    @ISBN VARCHAR(20),
    @AnioPublicacion INT,
    @Stock INT,
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Libros WHERE CodLibro = @CodLibro)
        THROW 50001, N'Ya existe un libro registrado con ese código.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN = @ISBN)
        THROW 50002, N'Ya existe un libro registrado con ese ISBN.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.CategoriasLibro WHERE CodCategoria = @CodCategoria)
        THROW 50003, N'La categoría seleccionada no existe.', 1;

    INSERT INTO dbo.Libros (CodLibro, CodCategoria, Titulo, ISBN, AnioPublicacion, Stock, Estado)
    VALUES (@CodLibro, @CodCategoria, @Titulo, @ISBN, @AnioPublicacion, @Stock, @Estado);
END;
GO

CREATE PROCEDURE dbo.usp_ActualizarLibro
    @CodLibro CHAR(6),
    @CodCategoria CHAR(5),
    @Titulo VARCHAR(120),
    @ISBN VARCHAR(20),
    @AnioPublicacion INT,
    @Stock INT,
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE CodLibro = @CodLibro)
        THROW 50004, N'El libro que intenta modificar no existe.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN = @ISBN AND CodLibro <> @CodLibro)
        THROW 50002, N'Ya existe otro libro registrado con ese ISBN.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.CategoriasLibro WHERE CodCategoria = @CodCategoria)
        THROW 50003, N'La categoría seleccionada no existe.', 1;

    UPDATE dbo.Libros
    SET CodCategoria = @CodCategoria,
        Titulo = @Titulo,
        ISBN = @ISBN,
        AnioPublicacion = @AnioPublicacion,
        Stock = @Stock,
        Estado = @Estado
    WHERE CodLibro = @CodLibro;
END;
GO

CREATE PROCEDURE dbo.usp_EliminarLibro
    @CodLibro CHAR(6)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE CodLibro = @CodLibro)
        THROW 50004, N'El libro que intenta eliminar no existe.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Prestamos WHERE CodLibro = @CodLibro)
        THROW 50005, N'No se puede eliminar el libro porque tiene préstamos relacionados.', 1;

    DELETE FROM dbo.Libros
    WHERE CodLibro = @CodLibro;
END;
GO

CREATE PROCEDURE dbo.usp_PrestamosPorAnio
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.IdPrestamo,
           p.FechaPrestamo,
           p.FechaDevolucion,
           e.Apellidos + N', ' + e.Nombres AS ApellidosNombres,
           l.Titulo,
           p.EstadoPrestamo,
           p.Multa
    FROM dbo.Prestamos AS p
    INNER JOIN dbo.Estudiantes AS e ON e.CodEstudiante = p.CodEstudiante
    INNER JOIN dbo.Libros AS l ON l.CodLibro = p.CodLibro
    WHERE YEAR(p.FechaPrestamo) = @Anio
    ORDER BY p.FechaPrestamo DESC, p.IdPrestamo DESC;
END;
GO