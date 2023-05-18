Create table tblUser(
	Id UNIQUEIDENTIFIER primary key,
	StrName varchar(100) not null,
	StrEmail varchar(100) not null,
	HsPassword varbinary(256) not null,
	BiActive bit not null default 1
);
go

Create table tblAppoiment(
	Id UNIQUEIDENTIFIER primary key,
	UserFk UNIQUEIDENTIFIER not null FOREIGN KEY REFERENCES tblUser(Id),
	StrNameClient varchar(100) not null,
	DtDateStart DateTime not null,
	IntEnd int not null
);
go

--Procedimientos Almacenados
exec sp_CreateUser 'Monica D', 'Monica@gmail.com', '123', 1
--Crear usuarios--
CREATE PROCEDURE sp_CreateUser
	@StrName VARCHAR(100),
	@StrEmail VARCHAR(100),
	@StrPassword VARCHAR(max),
	@Active BIT
AS
BEGIN
	DECLARE @Id UNIQUEIDENTIFIER;
	SET @Id = (SELECT NEWID());
	-- Validar que el usuario que se crea no exista antes
	IF exists(SELECT Id from tblUser WHERE Id = @Id or StrEmail = @StrEmail)
		BEGIN
			SELECT 'Ya existe este usuario, vulevelo a intentar' AS Rpta, '-1' AS Cod
			RETURN
		END
	--Comienzo a hacer la transacción
	BEGIN TRANSACTION 
		BEGIN TRY
			Begin
				INSERT into tblUser (Id, StrName, StrEmail, HsPassword, BiActive) VALUES(@Id, @StrName, @StrEmail, HASHBYTES('SHA2_256', CONVERT(varbinary(16), @Id) + CAST(@StrPassword AS varbinary(max))), @Active);
				COMMIT TRANSACTION;
			End	
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION
			SELECT 'Ocurrió un error al intentar insertar al usuario. Inténtelo de nuevo o contacte con CitasPlus' AS Rpta, '-1' AS Cod
			RETURN
		END CATCH
	SELECT 'Se ha creado con existo el usuario '+@StrName AS Rpta, '0' AS Cod FROM tblUser WHERE tblUser.Id= @Id;
	RETURN;
END
GO
--exec sp_ValidateLogin 'luis.lopera28@gmail.com', '123'
--Validar el login--
CREATE PROCEDURE sp_ValidateLogin
	@User VARCHAR(100),
	@Pass VARCHAR(max)
AS
BEGIN
  	-- Validar que el usuario exista
	IF EXISTS (SELECT Id FROM tblUser WHERE StrEmail = @User and HsPassword =  HASHBYTES('SHA2_256', CONVERT(varbinary(16), Id) + CAST(@Pass AS VARBINARY(max))) and BiActive = 1)
		BEGIN
			SELECT Id, StrName, '0' AS Cod FROM tblUser WHERE StrEmail = @User
			RETURN
		END
	-- Validar que el usuario este activo
	IF EXISTS (SELECT Id FROM tblUser WHERE StrEmail = @User and BiActive = 0)
		BEGIN
			SELECT 'Usuario inactivo' AS Rpta, '-1' AS Cod
			RETURN
		END
	--Validar que la contraseña y el usuario sean correctos
	IF NOT EXISTS (SELECT Id FROM tblUser WHERE StrEmail = @User and HsPassword =  HASHBYTES('SHA2_256', CONVERT(varbinary(16), Id) + CAST(@Pass AS VARBINARY(max))))
		BEGIN
			SELECT 'El nombre de usuario o contraseña son incorrectos' AS Rpta, '-1' AS Cod
			RETURN
		END
	IF(@@ERROR > 0)
	BEGIN 
		SELECT 'Ha ocurrido un error, vuelvelo a intentar o contactate con CitasPlus' as Rpta, '-1' As Cod
		RETURN
	END
END
GO
--Validar el usuario por medio del id
CREATE PROCEDURE sp_ValidateUserById
	@Id UNIQUEIDENTIFIER
AS
BEGIN
  	-- Validar que el ID del usuario exista
	IF exists (SELECT Id FROM tblUser WHERE Id = @Id and BiActive = 1)
		BEGIN
			SELECT Id, StrName,'0' AS Cod FROM tblUser WHERE Id = @Id
			RETURN
		END
	ELSE
		BEGIN
			SELECT 'Usuario no registrado o inactivo' AS Rpta, '-1' AS Cod
			RETURN
		END
	IF(@@ERROR > 0)
	BEGIN 
		SELECT 'Ha ocurrido un error, vuelvelo a intentar o contactate con tu CitasPlus' as Rpta, '010102' As Codigo
		RETURN
	END
END
GO
exec sp_CreateAppoiment 'D4BBAB3D-F15C-47C3-BFF6-4EB7BF352124', 'Luis', '2023-05-16 15:35:00.000', 1
--Crear una cita
CREATE PROCEDURE sp_CreateAppoiment
	@User_Id UNIQUEIDENTIFIER,
	@StrNameClient VARCHAR(100),
	@DtDateStart DATETIME,
	@Dtestimated INT
AS
BEGIN
	DECLARE @Id UNIQUEIDENTIFIER;
	SET @Id = (SELECT NEWID());
	--Validar que este Id no exista en la tabla de citas
	IF exists(SELECT Id from tblAppoiment WHERE Id = @Id)
		BEGIN
			SELECT 'Ya existe este id, vulevelo a intentar' AS Rpta, '-1' AS Cod
			RETURN
		END
	-- Validar que el usuario exista
	IF not exists(SELECT Id from tblUser WHERE Id = @User_Id)
		BEGIN
			SELECT 'Este usuario no existe' AS Rpta, '-1' AS Cod
			RETURN
		END
	--Validar que no haya una cita en este rango
	IF EXISTS (SELECT 1 FROM tblAppoiment WHERE UserFk = @User_Id AND DtDateStart < DATEADD(minute, @Dtestimated, @DtDateStart) AND  DATEADD(minute, IntEnd, DtDateStart) > @DtDateStart)
	BEGIN
		SELECT 'En esta fecha no hay disponibilidad de citas' AS Rpta, '-1' AS Cod
		RETURN
	END
	--Comienzo a hacer la transacción
	BEGIN TRANSACTION 
		BEGIN TRY
			Begin
				INSERT into tblAppoiment (Id, UserFk, StrNameClient, DtDateStart, IntEnd) VALUES(@Id, @User_Id, @StrNameClient, @DtDateStart, @Dtestimated);
				COMMIT TRANSACTION;
			End	
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION
			SELECT 'Ocurrió un error al intentar insertar una cita. Inténtelo de nuevo o contacte con CitasPlus' AS Rpta, '-1' AS Cod
			RETURN
		END CATCH
	SELECT 'Se ha creado con existo la cita para '+StrNameClient AS Rpta, '0' AS Cod FROM tblAppoiment WHERE Id= @Id;
	RETURN;
END
GO
--Eliminar una cita
CREATE PROCEDURE sp_DeleteAppoiment
	@User_Id UNIQUEIDENTIFIER,
	@Appoiment_Id UNIQUEIDENTIFIER
AS
BEGIN
	-- Validar que el usuario exista
	IF not exists(SELECT Id from tblUser WHERE Id = @User_Id)
		BEGIN
			SELECT 'Este usuario no existe' AS Rpta, '-1' AS Cod
			RETURN
		END
	-- Validar que la cita exista
	IF not exists(SELECT Id from tblAppoiment WHERE Id = @Appoiment_Id)
		BEGIN
			SELECT 'Esta cita no existe' AS Rpta, '-1' AS Cod
			RETURN
		END
	-- Validar que la cita exista para este usuario
	IF not exists(SELECT Id from tblAppoiment WHERE Id = @Appoiment_Id AND  UserFk = @User_Id)
		BEGIN
			SELECT 'Esta cita no corresponde a este usuario' AS Rpta, '-1' AS Cod
			RETURN
		END
	--Comienzo a hacer la transacción
	BEGIN TRANSACTION 
		BEGIN TRY
			Begin
				DELETE FROM tblAppoiment Where Id = @Appoiment_Id;
				COMMIT TRANSACTION;
			End	
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION
			SELECT 'Ocurrió un error al intentar eliminar una cita. Inténtelo de nuevo o contacte con CitasPlus' AS Rpta, '-1' AS Cod
			RETURN
		END CATCH
	SELECT 'Se ha eliminado correctamente esta cita' AS Rpta, '0' AS Cod;
	RETURN;
END
GO
--Obtener citas
CREATE PROCEDURE sp_GetAppoimentByUser
	@User_Id UNIQUEIDENTIFIER
AS
BEGIN
	-- Validar que el usuario exista
	IF not exists(SELECT Id from tblUser WHERE Id = @User_Id)
		BEGIN
			SELECT 'Este usuario no existe' AS Rpta, '-1' AS Cod
			RETURN
		END
	SELECT Id, StrNameClient, DtDateStart, IntEnd FROM tblAppoiment where UserFk = @User_Id;
END
GO
select * from tblAppoiment
select * from tblUser d4bbab3d-f15c-47c3-bff6-4eb7bf352124