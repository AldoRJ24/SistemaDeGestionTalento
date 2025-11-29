-- Script para insertar 20 colaboradores con skills random
-- Password hash para '123456' (usando el mismo que usas en tu app, aquí pongo un placeholder o uno común de BCrypt)
-- Nota: Asegúrate de que el Rol 'Colaborador' exista.

BEGIN TRANSACTION;

DECLARE @RolColaboradorId INT;
SELECT @RolColaboradorId = Id FROM roles WHERE Nombre = 'Colaborador';

-- Si no existe el rol, lo creamos (opcional, mejor asegurar que existe)
IF @RolColaboradorId IS NULL
BEGIN
    INSERT INTO roles (Nombre) VALUES ('Colaborador');
    SET @RolColaboradorId = SCOPE_IDENTITY();
END

-- Tabla temporal para nombres
DECLARE @Nombres TABLE (Nombre NVARCHAR(50), Apellido NVARCHAR(50));
INSERT INTO @Nombres VALUES 
('Juan', 'Perez'), ('Maria', 'Gomez'), ('Carlos', 'Lopez'), ('Ana', 'Martinez'), ('Luis', 'Rodriguez'),
('Sofia', 'Hernandez'), ('Miguel', 'Garcia'), ('Lucia', 'Sanchez'), ('Pedro', 'Diaz'), ('Elena', 'Torres'),
('Diego', 'Ramirez'), ('Valentina', 'Flores'), ('Javier', 'Rivera'), ('Camila', 'Benitez'), ('Andres', 'Acosta'),
('Isabella', 'Medina'), ('Fernando', 'Ruiz'), ('Gabriela', 'Herrera'), ('Ricardo', 'Aguilar'), ('Daniela', 'Mendoza');

DECLARE @Counter INT = 1;
DECLARE @Max INT = 20;
DECLARE @Nombre NVARCHAR(50);
DECLARE @Apellido NVARCHAR(50);
DECLARE @Email NVARCHAR(100);
DECLARE @UserId INT;

WHILE @Counter <= @Max
BEGIN
    -- Seleccionar nombre random de la tabla temporal (o secuencial para este ejemplo simple)
    SELECT TOP 1 @Nombre = Nombre, @Apellido = Apellido FROM @Nombres ORDER BY NEWID();
    SET @Email = LOWER(@Nombre) + '.' + LOWER(@Apellido) + CAST(@Counter AS NVARCHAR) + '@test.com';

    -- Insertar Usuario
    INSERT INTO usuarios (Nombre, Apellido, Correo, contraseña_hash, rol_id, puesto_actual, Estado, Disponibilidad, open_to_work, fecha_creacion)
    VALUES (@Nombre, @Apellido, @Email, '$2a$11$Z5.1.1.1.1.1.1.1.1.1.1u1', @RolColaboradorId, 'Desarrollador', 'Activo', 1, 1, GETDATE());
    
    SET @UserId = SCOPE_IDENTITY();

    -- Insertar Skills Random (3 a 5 skills por usuario)
    DECLARE @SkillCount INT = FLOOR(RAND()*(5-3+1)+3);
    DECLARE @k INT = 1;
    
    WHILE @k <= @SkillCount
    BEGIN
        DECLARE @RandomSkillId INT;
        DECLARE @RandomNivelId INT;

        SELECT TOP 1 @RandomSkillId = Id FROM skills ORDER BY NEWID();
        SELECT TOP 1 @RandomNivelId = Id FROM niveles_skill ORDER BY NEWID();

        -- Evitar duplicados
        IF NOT EXISTS (SELECT 1 FROM colaboradores_skills WHERE usuario_id = @UserId AND skill_id = @RandomSkillId)
        BEGIN
            INSERT INTO colaboradores_skills (usuario_id, skill_id, nivel_id, Estado, fecha_evaluacion)
            VALUES (@UserId, @RandomSkillId, @RandomNivelId, 'Validado', GETDATE());
        END

        SET @k = @k + 1;
    END

    SET @Counter = @Counter + 1;
END

COMMIT TRANSACTION;

SELECT 'Se han insertado 20 colaboradores con skills aleatorios.' AS Mensaje;
