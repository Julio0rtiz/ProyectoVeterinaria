USE VeterinariaBD;
GO

SET NOCOUNT ON;

DECLARE @Vet INT = (SELECT Id FROM Usuarios WHERE Correo = 'vet@veterinaria.com');
DECLARE @Admin INT = (SELECT Id FROM Usuarios WHERE Correo = 'admin@veterinaria.com');
DECLARE @Dueno INT = (SELECT Id FROM Usuarios WHERE Correo = 'dueno@veterinaria.com');

DECLARE @Med1 INT = (SELECT TOP 1 Id FROM Catalogo WHERE Tipo = 1 ORDER BY Id);
DECLARE @Med2 INT = (SELECT TOP 1 Id FROM Catalogo WHERE Tipo = 1 AND Id <> @Med1 ORDER BY Id);
DECLARE @Trat1 INT = (SELECT TOP 1 Id FROM Catalogo WHERE Tipo = 2 ORDER BY Id);

IF @Vet IS NULL OR @Dueno IS NULL OR @Med1 IS NULL OR @Med2 IS NULL OR @Trat1 IS NULL
BEGIN
    RAISERROR('Faltan usuarios de prueba o el catálogo necesita al menos 2 medicamentos y 1 tratamiento.', 16, 1);
    RETURN;
END

IF @Admin IS NULL SET @Admin = @Vet;

DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
DECLARE @MascotaId INT;
DECLARE @Desfase INT = 0;

DECLARE mascotas CURSOR LOCAL FAST_FORWARD FOR
    SELECT Id FROM Mascotas
    WHERE DuenoId = @Dueno
      AND NOT EXISTS (SELECT 1 FROM Vacunas v WHERE v.MascotaId = Mascotas.Id)
    ORDER BY Id;

OPEN mascotas;
FETCH NEXT FROM mascotas INTO @MascotaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO Vacunas (MascotaId, Nombre, FechaAplicacion, FechaProximaDosis, AutorId, FechaRegistro) VALUES
        (@MascotaId, 'Antirrábica', DATEADD(DAY, -400, @Hoy), DATEADD(DAY, -35, @Hoy), @Vet, DATEADD(DAY, -400, GETDATE())),
        (@MascotaId, 'Polivalente (refuerzo)', DATEADD(DAY, -340, @Hoy), DATEADD(DAY, 12, @Hoy), @Admin, DATEADD(DAY, -340, GETDATE())),
        (@MascotaId, 'Desparasitación interna', DATEADD(DAY, -20, @Hoy), DATEADD(DAY, 160, @Hoy), @Vet, DATEADD(DAY, -20, GETDATE()));

    INSERT INTO Tratamientos (MascotaId, ItemCatalogoId, Indicaciones, FechaInicio, AutorId, Suspendido, FechaSuspension, AutorSuspensionId) VALUES
        (@MascotaId, @Med1, '1 tableta cada 12 horas por 10 días', DATEADD(DAY, -6, @Hoy), @Vet, 0, NULL, NULL),
        (@MascotaId, @Trat1, 'Una sesión por semana durante un mes', DATEADD(DAY, -4, @Hoy), @Admin, 0, NULL, NULL),
        (@MascotaId, @Med2, '5 ml cada 8 horas con alimento', DATEADD(DAY, -30, @Hoy), @Vet, 1, DATEADD(DAY, -22, GETDATE()), @Admin);

    INSERT INTO Notas (MascotaId, ContenidoHtml, FechaHora, AutorId) VALUES
        (@MascotaId, '<p><strong>Consulta general.</strong> Paciente activo, buen estado corporal.</p><ul><li>Temperatura normal</li><li>Mucosas rosadas</li></ul>', DATEADD(DAY, -30, GETDATE()), @Vet),
        (@MascotaId, '<p>Control posterior al tratamiento. <em>Evolución favorable.</em></p><ol><li>Se suspende el jarabe</li><li>Continuar dieta blanda 3 días</li></ol>', DATEADD(DAY, -22, GETDATE()), @Admin),
        (@MascotaId, '<p><strong>Revisión de rutina.</strong></p><ul><li>Se aplica desparasitación</li><li>Próximo control en 6 meses</li></ul>', DATEADD(DAY, -6 - @Desfase, GETDATE()), @Vet);

    UPDATE Mascotas SET UltimaAtencion = DATEADD(DAY, -4 - @Desfase, GETDATE()) WHERE Id = @MascotaId;

    SET @Desfase = @Desfase + 5;
    FETCH NEXT FROM mascotas INTO @MascotaId;
END

CLOSE mascotas;
DEALLOCATE mascotas;
GO
