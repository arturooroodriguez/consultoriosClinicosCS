use BdClinicaWeb
go

create or alter procedure pro_Crear_Paciente
    @pacienteCodigo nchar(10),
    @pacienteDNI nchar(8),
    @pacienteNombreCompleto nvarchar(100),
    @pacienteFechaNacimiento date,
    @pacienteDireccion nvarchar(255) = null,
    @pacienteTelefono nvarchar(15) = null,
    @pacienteCorreoElectronico nvarchar(100) = null,
	@historialClinicoCodigo nchar(10) null
	as
	set nocount on;

	insert into Salud.Pacientes 
	(
		pacienteCodigo,
		pacienteDNI,
		pacienteNombreCompleto,
		pacienteFechaNacimiento,
		pacienteDireccion,
		pacienteTelefono,
		pacienteCorreoElectronico,
		historialClinicoCodigo
	)
	values 
	(
		@pacienteCodigo,
		@pacienteDNI,
		@pacienteNombreCompleto,
		@pacienteFechaNacimiento,
		@pacienteDireccion,
		@pacienteTelefono,
		@pacienteCorreoElectronico,
		@historialClinicoCodigo
	);
go

create or alter procedure pro_Actualizar_Paciente
    @pacienteCodigo nchar(10),
    @pacienteNombreCompleto nvarchar(100) = null,
    @pacienteDireccion nvarchar(255) = null,
    @pacienteTelefono nvarchar(15) = null,
    @pacienteCorreoElectronico nvarchar(100) = null
	as
	begin
		set nocount on;

		update Salud.Pacientes
		set 
			pacienteNombreCompleto = coalesce(@pacienteNombreCompleto, pacienteNombreCompleto),
			pacienteDireccion = coalesce(@pacienteDireccion, pacienteDireccion),
			pacienteTelefono = coalesce(@pacienteTelefono, pacienteTelefono),
			pacienteCorreoElectronico = coalesce(@pacienteCorreoElectronico, pacienteCorreoElectronico)
		where pacienteCodigo = @pacienteCodigo;
		set nocount off;
	end;
go

create or alter procedure pro_Eliminar_Paciente
		@pacienteCodigo nchar(10)
	as
	set nocount on;

	update Salud.Pacientes
	set pacienteEstado = 'I'
	where pacienteCodigo = @pacienteCodigo;
go


create or alter procedure pro_Recuperar_Paciente
		@pacienteCodigo nchar(10)
	as
	set nocount on;

	update Salud.Pacientes
	set pacienteEstado = 'A'
	where pacienteCodigo = @pacienteCodigo;
go

create or alter procedure pro_Mostrar_Paciente_por_codigo
		@pacienteCodigo nchar(10)
	as
	set nocount on;

	select pacienteCodigo,
		   pacienteDNI,
		   pacienteNombreCompleto,
		   pacienteFechaNacimiento,
		   pacienteDireccion,
		   pacienteTelefono,
		   pacienteCorreoElectronico,
		   pacienteEstado
	from Salud.Pacientes
	where pacienteCodigo = @pacienteCodigo;
go

create or alter procedure pro_listar_pacientes
	as
begin
    set nocount on;
    select 
        P.pacienteCodigo,
        P.pacienteDNI,
        P.pacienteNombreCompleto,
        P.pacienteFechaNacimiento,
        P.pacienteDireccion,
        P.pacienteTelefono,
        P.pacienteCorreoElectronico,
        P.pacienteEstado,
		p.historialClinicoCodigo
    from 
        Salud.Pacientes as P
    set nocount off;
end
go

CREATE OR ALTER PROCEDURE pro_Mostrar_HistoriaClinica
    @pacienteCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.pacienteCodigo,               
        hc.historialClinicoCodigo,      
        hc.HistoriaClinicafechaCreacion,
        c.consultaCodigo,
        c.consultacitaCodigo
    FROM Salud.Pacientes p
    INNER JOIN Salud.HistoriaClinica hc
        ON p.historialClinicoCodigo = hc.historialClinicoCodigo
    LEFT JOIN Gestion.Consulta c
        ON p.pacienteCodigo = c.pacienteCodigo
    WHERE p.pacienteCodigo = @pacienteCodigo;
END;
GO

exec pro_Mostrar_HistoriaClinica @pacienteCodigo = PAC0000001
GO

create or alter procedure pro_Mostrar_ContactosEmergencia
    @pacienteCodigo nchar(10)
as
begin
    set nocount on;

    select 
        ce.contactoEmergenciaCodigo,
        ce.contactoEmergenciaNombre,
        ce.contactoEmergenciaRelacion,
        ce.contactoEmergenciaTelefono
    from Salud.ContactosEmergencia ce
    where ce.pacienteCodigo = @pacienteCodigo;

    set nocount off;
end;
go

create or alter procedure pro_Agregar_ContactoEmergencia 
    @contactoEmergenciaCodigo nchar(10),
    @contactoEmergenciaNombre nvarchar(100),
    @contactoEmergenciaRelacion nvarchar(50),
    @contactoEmergenciaTelefono nvarchar(15),
	@pacienteCodigo nchar(10)
as
begin
    set nocount on;

    insert into Salud.ContactosEmergencia (
        contactoEmergenciaCodigo,
        contactoEmergenciaNombre,
        contactoEmergenciaRelacion,
        contactoEmergenciaTelefono,
		pacienteCodigo
    )
    values (
        @contactoEmergenciaCodigo,
        @contactoEmergenciaNombre,
        @contactoEmergenciaRelacion,
        @contactoEmergenciaTelefono,
		@pacienteCodigo
    );

    set nocount off;
end;
go



create or alter procedure pro_Actualizar_ContactoEmergencia
    @contactoEmergenciaCodigo nchar(10),
    @contactoEmergenciaNombre nvarchar(100),
    @contactoEmergenciaRelacion nvarchar(50),
    @contactoEmergenciaTelefono nvarchar(15)
as
begin
    set nocount on;

    update Salud.ContactosEmergencia
    set
        contactoEmergenciaNombre = @contactoEmergenciaNombre,
        contactoEmergenciaRelacion = @contactoEmergenciaRelacion,
        contactoEmergenciaTelefono = @contactoEmergenciaTelefono
    where
        contactoEmergenciaCodigo = @contactoEmergenciaCodigo;
end;
go

create or alter procedure pro_Mostrar_MedicosConEspecialidad
	as
	set nocount on;

	select m.medicoCodigo,
		   m.medicoNombre,
		   m.medicoApellido,
		   e.especialidadNombre
	from Administracion.Medico m
	inner join Administracion.Especialidad e on m.especialidadCodigo = e.especialidadCodigo;
go

create or alter procedure Pro_Listar_TipoConsulta
    as
    begin
        set nocount on
        select 
            tipoConsultaCodigo,  
            tipoConsultaDescripcion 
        from 
            Gestion.tipoConsulta
        order by 
            tipoConsultaDescripcion
end
go


create or alter procedure pro_Mostrar_Medico_por_codigo
    @medicoCodigo nchar(10)
as
begin
    set nocount on;

    select 
        medicoCodigo,
        medicoApellido,
        medicoNombre,
        medicoCorreo,
        medicoDNI,
        medicoTelefono,
        medicoEstado,
        especialidadCodigo
    from Administracion.Medico
    where medicoCodigo = @medicoCodigo;
end;
go

create or alter procedure Pro_Listar_Especialidad
     as
    begin
        set nocount on
        select 
            especialidadCodigo,  
            especialidadNombre,  
            especialidadDescripcion
        from 
            Administracion.Especialidad
        order by 
            especialidadNombre
end
go

CREATE or alter PROCEDURE pro_Insertar_Cita
    @citaCodigo nchar(10),
    @citaEstado nchar(1) = 'P',
    @citaFechaHora datetime
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Gestion.cita 
    (
        citaCodigo,
        citaEstado,
        citaFechaHora
    )
    VALUES 
    (
        @citaCodigo,
        @citaEstado,
        @citaFechaHora
    );
END
GO

CREATE or alter PROCEDURE pro_Buscar_Paciente
    @pacienteDNI nchar(8) = NULL,
    @pacienteNombreCompleto nvarchar(200) = NULL,
    @pacienteTelefono nvarchar(25) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        pacienteCodigo,
        pacienteNombreCompleto,
        pacienteCorreoElectronico,
        pacienteDireccion,
        pacienteTelefono,
        pacienteFechaNacimiento,
        pacienteEstado
    FROM 
        Salud.pacientes
    WHERE 
        (@pacienteDNI IS NOT NULL AND pacienteDNI = @pacienteDNI) OR
        (@pacienteNombreCompleto IS NOT NULL AND pacienteNombreCompleto LIKE '%' + @pacienteNombreCompleto + '%') OR
        (@pacienteTelefono IS NOT NULL AND pacienteTelefono = @pacienteTelefono);
END
GO


CREATE OR ALTER PROCEDURE pro_Buscar_Paciente_dni
    @pacienteDNI nchar(8) 
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        pacienteCodigo,
		pacienteDNI,
        pacienteNombreCompleto,
        pacienteCorreoElectronico,
        pacienteDireccion,
        pacienteTelefono,
        pacienteFechaNacimiento,
        pacienteEstado
    FROM 
        Salud.pacientes
    WHERE 
        pacienteDNI = @pacienteDNI;
END
GO
	
CREATE or alter PROCEDURE pro_CambiarEstadoPaciente
    @pacienteCodigo nchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Salud.paciente
    SET pacienteEstado = 'I'
    WHERE pacienteCodigo = @pacienteCodigo;
END
GO

CREATE OR ALTER PROCEDURE pro_VisualizarCitasPaciente
    @pacienteCodigo nchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.citaCodigo,
        c.citaEstado,
        c.citaFechaHora,
        tc.tipoConsultaDescripcion,
        m.medicoNombre,
        m.medicoApellido,
        con.consultaFechaHoraFinal
    FROM 
        Gestion.cita c
    INNER JOIN 
        Gestion.Consulta con ON c.citaCodigo = con.consultacitaCodigo
    INNER JOIN 
        Gestion.tipoConsulta tc ON con.tipoConsultaCodigo = tc.tipoConsultaCodigo
    INNER JOIN 
        Administracion.Medico m ON con.medicoCodigo = m.medicoCodigo
    INNER JOIN 
        Salud.Pacientes p ON con.pacienteCodigo = p.pacienteCodigo
    INNER JOIN 
        Salud.HistoriaClinica hc ON hc.historialClinicoCodigo = p.historialClinicoCodigo
    WHERE 
        p.pacienteCodigo = @pacienteCodigo;
END;
GO


CREATE OR ALTER PROCEDURE pro_Mostrar_Citas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.citaCodigo,
        c.citaEstado,
        c.citaFechaHora,
        tc.tipoConsultaDescripcion AS citaTipoConsultaDescripcion,
        m.medicoCodigo,
        m.medicoNombre,
        m.medicoApellido,
        e.especialidadCodigo,
        e.especialidadNombre,
        p.pacienteCodigo,
        p.pacienteNombreCompleto
    FROM 
        Gestion.cita AS c
    LEFT JOIN 
        Gestion.Consulta AS con ON c.citaCodigo = con.consultacitaCodigo
    LEFT JOIN 
        Administracion.Medico AS m ON con.medicoCodigo = m.medicoCodigo
    LEFT JOIN 
        Administracion.Especialidad AS e ON m.especialidadCodigo = e.especialidadCodigo
    LEFT JOIN 
        Salud.Pacientes AS p ON con.pacienteCodigo = p.pacienteCodigo
    LEFT JOIN 
        Gestion.tipoConsulta AS tc ON con.tipoConsultaCodigo = tc.tipoConsultaCodigo;

    SET NOCOUNT OFF;
END;
GO

create or alter procedure pro_Crear_HistoriaClinica
    @historialClinicoCodigo nchar(10)
	as
	begin
		set nocount on;

		insert into Salud.HistoriaClinica
		(
			historialClinicoCodigo
		)
		values
		(
			@historialClinicoCodigo
		);

		set nocount off;
	end
go

CREATE OR ALTER PROCEDURE pro_listar_HistoriaClinica
    @historialClinicoCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.consultaCodigo,
        c.consultacitaCodigo,
        c.consultaFechaHoraFinal,
        c.tipoConsultaCodigo,
        c.medicoCodigo,
        c.pacienteCodigo,
		ci.citaEstado
    FROM 
        Gestion.Consulta AS c
    INNER JOIN 
        Salud.Pacientes AS p ON c.pacienteCodigo = p.pacienteCodigo
	inner join
		Gestion.cita as ci on c.consultacitaCodigo = ci.citaCodigo
    WHERE 
        p.historialClinicoCodigo = @historialClinicoCodigo;

    SET NOCOUNT OFF;
END;
GO


CREATE OR ALTER PROCEDURE pro_listar_DiagnosticosPorConsulta
    @consultaCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        diagnosticoCodigo,
        diagnosticoDescripcion
    FROM 
        Salud.Diagnostico
    WHERE 
        diagnosticoconsultaCodigo = @consultaCodigo;

    SET NOCOUNT OFF;
END;
GO

CREATE OR ALTER PROCEDURE pro_listar_RecetasPorConsulta
    @consultaCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        recetaCodigo,
        recetaDescripcion,
        recetaTratamiento,
        recetaRecomendaciones
    FROM 
        Salud.RecetaMedica
    WHERE 
        recetaConsultaCodigo = @consultaCodigo;

    SET NOCOUNT OFF;
END;
GO


--exec pro_listar_RecetasPorConsulta @consultaCodigo = CON0000002
--go
CREATE or alter PROCEDURE pro_Listar_Medicos
AS
BEGIN
    SELECT 
        medicoCodigo,
        medicoApellido,
        medicoNombre,
        medicoCorreo,
        medicoDNI,
        medicoTelefono,
        medicoEstado,
        especialidadCodigo
    FROM 
        Administracion.Medico
    ORDER BY 
        medicoApellido, medicoNombre;
END;
GO

create or alter procedure pro_Crear_Consulta
    @consultaCodigo nchar(10),
    @consultaCitaCodigo nchar(10),
    @consultaFechaHoraFinal datetime = null,

	@medicoCodigo nchar(10),
    @tipoConsultaCodigo nchar(10),
	@pacienteCodigo nchar(10)
    as
    begin
        set nocount on;

    -- Inserción directa en la tabla Gestion.Consulta
    insert into Gestion.Consulta (
		[consultaCodigo], [consultacitaCodigo], [consultaFechaHoraFinal], [medicoCodigo], [tipoConsultaCodigo], [pacienteCodigo]
    )
    values (
        @consultaCodigo,
        @consultaCitaCodigo,
        @consultaFechaHoraFinal,
		@medicoCodigo,
		@tipoConsultaCodigo,
		@pacienteCodigo
    );
    set nocount off;
end;
go

CREATE OR ALTER PROCEDURE pro_Listar_Consulta
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.consultaCodigo,
		C.consultacitaCodigo,
		ci.citaFechaHora,
		ci.citaEstado,
		C.consultaFechaHoraFinal,
		c.pacienteCodigo,
		p.pacienteNombreCompleto,
		c.medicoCodigo,
		me.medicoNombre,
		me.medicoApellido,
		c.tipoConsultaCodigo,
		p.historialClinicoCodigo
		
    FROM 
        Gestion.Consulta AS c
    LEFT join 
		Gestion.cita as ci on ci.citaCodigo = c.consultacitaCodigo
	left join 
		Administracion.Medico as me on c.medicoCodigo = me.medicoCodigo
	left join
		Salud.Pacientes as p on c.pacienteCodigo = p.pacienteCodigo
    SET NOCOUNT OFF;
END;
GO

create or alter procedure pro_Actualizar_Estado_CitaPendiente
    @citaCodigo nchar(10)
as
begin
    set nocount on;

    update Gestion.cita
    set citaEstado = 'P'
    where citaCodigo = @citaCodigo;

    set nocount off;
end;
go

create or alter procedure pro_Actualizar_Estado_CitaCancelado
    @citaCodigo nchar(10)
as
begin
    set nocount on;

    update Gestion.cita
    set citaEstado = 'C'
    where citaCodigo = @citaCodigo;

    set nocount off;
end;
go

create or alter procedure pro_Actualizar_Estado_CitaNoAsistio
    @citaCodigo nchar(10)
as
begin
    set nocount on;

    update Gestion.cita
    set citaEstado = 'N'
    where citaCodigo = @citaCodigo;

    set nocount off;
end;
go

create or alter procedure pro_Actualizar_Estado_CitaAtendido
    @citaCodigo nchar(10)
as
begin
    set nocount on;

    update Gestion.cita
    set citaEstado = 'A'
    where citaCodigo = @citaCodigo;

    set nocount off;
end
go




CREATE OR ALTER PROCEDURE pro_Actualizar_Estado_CitaAtendiendose
    @citaCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Gestion.cita
    SET 
        citaEstado = 'T',
        citaFechaHora = GETDATE()
    WHERE citaCodigo = @citaCodigo;
END
GO

create or alter procedure spGenerarCodigoUnico
    @prefijo nvarchar(3),         
    @tabla nvarchar(128),         
    @columnaCodigo nvarchar(128)
as
    declare @vNuevoCodigo int;
    declare @vCodigoGenerado nchar(10);
    declare @vSQL nvarchar(max);

    set nocount on;
    set @vSQL = '
        select @vNuevoCodigo = isnull(max(cast(substring(' + @columnaCodigo + ', 4, len(' + @columnaCodigo + ')) as int)), 0) + 1
        from ' + @tabla + '
        where ' + @columnaCodigo + ' like @prefijo + ''%''
    ';
    exec sp_executesql @vSQL, N'@vNuevoCodigo int output, @prefijo nvarchar(3)', @vNuevoCodigo output, @prefijo;
    set @vCodigoGenerado = @prefijo + right('0000000' + cast(@vNuevoCodigo as varchar(7)), 7);
    select @vCodigoGenerado as CodigoUnico;

set nocount off;
go

CREATE OR ALTER FUNCTION fn_Listar_HorariosEspecialidad (
    @especialidadCodigo nchar(10),
    @fecha date
)
RETURNS @IntervalosTiempo TABLE (
    MedicoCodigo nchar(10),
	MedicoNombre varchar(100),
    HoraInicio time,
    HoraFin time
)
AS
BEGIN
    INSERT INTO @IntervalosTiempo (MedicoCodigo, MedicoNombre , HoraInicio, HoraFin)
    SELECT 
        h.medicoCodigo,
		CONCAT(m.medicoNombre, ' ', m.medicoApellido) as MedicoNombre, 
        DATEADD(minute, (n.n * 60), h.horarioHoraInicio) AS HoraInicio,
        DATEADD(minute, (n.n + 1) * 60, h.horarioHoraInicio) AS HoraFin
    FROM 
        (SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11) n
    JOIN 
        Gestion.horario h ON n.n < DATEDIFF(minute, h.horarioHoraInicio, h.horarioHoraFin) / 60
    JOIN
        Administracion.Medico m ON h.medicoCodigo = m.medicoCodigo
    WHERE 
        m.especialidadCodigo = @especialidadCodigo
        AND UPPER(h.horarioDia) = UPPER(DATENAME(weekday, @fecha))
    ORDER BY 
        h.horarioHoraInicio, h.medicoCodigo;

    RETURN;
END
GO


CREATE OR ALTER PROCEDURE pro_Listar_HorariosEspecialidadConCitas
    @especialidadCodigo nchar(10),
    @fecha date
AS
BEGIN

    CREATE TABLE #HorariosConCitas (
        MedicoCodigo nchar(10),
        HoraInicio time,
        HoraFin time,
        ConsultaCodigo nchar(10) NULL,
        CitaCodigo nchar(10) NULL,
        PacienteCodigo nchar(10) NULL,
        CitaEstado nchar(1) NULL,
        MedicoNombre nvarchar(100) NULL,
        PacienteNombre nvarchar(100) NULL
    );

    INSERT INTO #HorariosConCitas (MedicoCodigo,MedicoNombre, HoraInicio, HoraFin)
    SELECT 
        MedicoCodigo,
		MedicoNombre,
        HoraInicio,
        HoraFin
    FROM 
        fn_Listar_HorariosEspecialidad(@especialidadCodigo, @fecha);

    UPDATE h
    SET 
        ConsultaCodigo = c.consultaCodigo,
        CitaCodigo = ci.citaCodigo,
        PacienteCodigo = c.pacienteCodigo,
        CitaEstado = ci.citaEstado,
        PacienteNombre = p.pacienteNombreCompleto
    FROM 
        #HorariosConCitas h
    JOIN 
        Gestion.Consulta c ON h.MedicoCodigo = c.medicoCodigo
    JOIN 
        Gestion.Cita ci ON c.consultacitaCodigo = ci.citaCodigo
    JOIN
        Administracion.Medico m ON h.MedicoCodigo = m.medicoCodigo
    JOIN
        Salud.Pacientes p ON c.pacienteCodigo = p.pacienteCodigo
    WHERE 
        ci.citaEstado = 'P' AND 
        ci.citaFechaHora >= @fecha AND 
        ci.citaFechaHora < DATEADD(day, 1, @fecha) AND 
        ci.citaFechaHora >= CAST(CONVERT(datetime, CONCAT(CONVERT(date, @fecha), ' ', CAST(h.HoraInicio AS char(5)))) AS datetime) AND 
        ci.citaFechaHora < CAST(CONVERT(datetime, CONCAT(CONVERT(date, @fecha), ' ', CAST(h.HoraFin AS char(5)))) AS datetime);

    SELECT 
        MedicoCodigo,
		MedicoNombre,
        HoraInicio,
        HoraFin,
        ConsultaCodigo,
        CitaCodigo,
        PacienteCodigo,
        CitaEstado,
        PacienteNombre
    FROM 
        #HorariosConCitas
    ORDER BY 
        HoraInicio, MedicoCodigo;

    DROP TABLE #HorariosConCitas;
END
GO

CREATE or alter PROCEDURE pro_registrar_DetallesConsulta
    @DetallesConsultaCodigo NCHAR(10),
    @DetallesConsultaHistoriaEnfermedad NVARCHAR(500) = NULL,
    @DetallesConsultaRevisiones NVARCHAR(500) = NULL,
    @DetallesConsultaEvaluacionPsico NVARCHAR(500) = NULL,
    @DetallesConsultaMotivoConsulta NVARCHAR(500) = NULL,
    @ConsultaCodigo NCHAR(10)
AS
BEGIN
        INSERT INTO Gestion.DetallesConsulta (
            detallesConsultaCodigo,
            detallesConsultaHistoriaEnfermedad,
            detallesConsultaRevisiones,
            detallesConsultaEvaluacionPsico,
            detallesConsultaMotivoConsulta,
            consultaCodigo
        )
        VALUES (
            @DetallesConsultaCodigo,
            @DetallesConsultaHistoriaEnfermedad,
            @DetallesConsultaRevisiones,
            @DetallesConsultaEvaluacionPsico,
            @DetallesConsultaMotivoConsulta,
            @ConsultaCodigo
        );
END;
GO

CREATE OR ALTER PROCEDURE pro_registrar_RecetaMedica
        @RecetaCodigo NCHAR(10),
        @RecetaConsultaCodigo NCHAR(10),
        @RecetaDescripcion NVARCHAR(500),
        @RecetaTratamiento NVARCHAR(500),
        @RecetaRecomendaciones NVARCHAR(500)
AS
BEGIN
        INSERT INTO Salud.RecetaMedica (
            recetaCodigo,
            recetaConsultaCodigo,
            recetaDescripcion,
            recetaTratamiento,
            recetaRecomendaciones
        )
        VALUES (
            @RecetaCodigo,
            @RecetaConsultaCodigo,
            @RecetaDescripcion,
            @RecetaTratamiento,
            @RecetaRecomendaciones
    );
END;
GO


CREATE OR ALTER PROCEDURE pro_registrar_Diagnostico
    @DiagnosticoCodigo NCHAR(10),
    @DiagnosticoConsultaCodigo NCHAR(10),
    @DiagnosticoDescripcion NVARCHAR(255),
    @DiagnosticoCodigoCie11  nvarchar(50) NULL
AS
BEGIN
    INSERT INTO Salud.Diagnostico (
        diagnosticoCodigo,
        diagnosticoconsultaCodigo,
        diagnosticoDescripcion,
        diagnosticosCodigoCie11
    )
    VALUES (
        @DiagnosticoCodigo,
        @DiagnosticoConsultaCodigo,
        @DiagnosticoDescripcion,
        @DiagnosticoCodigoCie11

    );
END;
GO

CREATE or alter PROCEDURE pro_actualizar_HoraFinalConsulta
    @ConsultaCodigo NCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Gestion.Consulta
    SET consultaFechaHoraFinal = GETDATE()
    WHERE consultaCodigo = @ConsultaCodigo;
END
GO


CREATE   PROCEDURE pro_Listar_ConsultaConEspecialidad1  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT   
    c.consultaCodigo,  
    c.consultacitaCodigo,  
    ci.citaFechaHora,  
    ci.citaEstado,  
    c.consultaFechaHoraFinal,  
    c.pacienteCodigo,  
    p.pacienteNombreCompleto,  
    c.medicoCodigo,  
    me.medicoNombre,  
    me.medicoApellido,
    c.tipoConsultaCodigo,  
    p.historialClinicoCodigo,
    me.especialidadCodigo,
    es.especialidadNombre
FROM   
    Gestion.Consulta AS c  
    LEFT JOIN Gestion.Cita AS ci ON ci.citaCodigo = c.consultacitaCodigo  
    LEFT JOIN Administracion.Medico AS me ON c.medicoCodigo = me.medicoCodigo  
    LEFT JOIN Administracion.Especialidad AS es ON es.especialidadCodigo = me.especialidadCodigo
    LEFT JOIN Salud.Pacientes AS p ON c.pacienteCodigo = p.pacienteCodigo ;  
END;  

EXEC sp_helptext 'pro_Listar_Consulta';
select * from [Administracion].[Medico]
 

CREATE PROCEDURE pro_Listar_ConsultaConEspecialidad
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  
        c.consultaCodigo,  
        c.consultacitaCodigo,  
        ci.citaFechaHora,  
        ci.citaEstado,  
        c.consultaFechaHoraFinal,  
        c.pacienteCodigo,  
        p.pacienteNombreCompleto,  
        c.medicoCodigo,  
        me.medicoNombre,  
        me.medicoApellido,
        me.especialidadCodigo,
        es.especialidadNombre,
        c.tipoConsultaCodigo,  
        p.historialClinicoCodigo
    FROM Gestion.Consulta AS c
    LEFT JOIN Gestion.Cita AS ci ON ci.citaCodigo = c.consultacitaCodigo  
    LEFT JOIN Administracion.Medico AS me ON c.medicoCodigo = me.medicoCodigo  
    LEFT JOIN Administracion.Especialidad AS es ON es.especialidadCodigo = me.especialidadCodigo
    LEFT JOIN Salud.Pacientes AS p ON c.pacienteCodigo = p.pacienteCodigo;

    SET NOCOUNT OFF;
END;
GO
