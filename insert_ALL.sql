USE [li4]
GO

INSERT INTO [dbo].[utilizador]
           ([handle]
           ,[birth_date]
           ,[password]
           ,[email]
           ,[nationality])
     VALUES
/******(<handle, nvarchar(20),>,<birth_date, date,>,<password, nvarchar(50),>,<email, nvarchar(50),>,<nationality, nchar(3),>)******/

           ('john_doe', '1990-01-01', 'hashed_password', 'john.doe@email.com', 'USA'),
           ('jane_smith', '1985-05-15', 'another_password', 'jane.smith@email.com', 'CAN'),
           ('bob_jones', '1995-08-22', 'secure_pw', 'bob.jones@email.com', 'GB'),
		   ('alice_wonder', '1988-03-15', 'alice_password', 'alice@email.com', 'US'),
           ('charlie_brown', '1992-09-22', 'charlie_pw', 'charlie@email.com', 'CA'),
           ('emma_sanders', '1985-06-10', 'emma_pass', 'emma@email.com', 'UK'),
           ('david_smith', '1990-12-05', 'david123', 'david@email.com', 'AU'),
           ('frank_jackson', '1982-02-28', 'frank_pass', 'frank@email.com', 'NZ');

GO

INSERT INTO [dbo].[admin]
           ([id_user])
     VALUES
/******(<id_user, int,>)******/

           (1);

GO

INSERT INTO [dbo].[vendedor]
           ([id_user])
     VALUES
/******(<id_user, int,>)******/

           (2),
           (3);

GO

INSERT INTO [dbo].[comprador]
           ([id_user])
     VALUES
/******(<id_user, int,>)******/

           (4),
           (5),
		   (6);

GO

INSERT INTO [dbo].[denuncia]
           ([motivo]
           ,[id_denunciado]
           ,[id_denunciador])
     VALUES
/******(<id, int,>,<motivo, nvarchar(50),>,<id_denunciado, int,>,<id_denunciador, int,>)******/

           ('Inappropriate behavior', 2, 3),
           ('Scam report', 1, 2),
           ('Spamming', 3, 1);
GO

INSERT INTO [dbo].[sala]
           ([estado]
           ,[titulo]
           ,[descricao]
           ,[id_comprador])
     VALUES
/******(<estado, bit,>,<titulo, nvarchar(50),>,<descricao, nvarchar(200),>,<id_comprador, int,>)******/

           (1, 'Sala 1', 'Description of Sala 1', 1),
           (1, 'Sala 2', 'Description of Sala 2', 2),
           (1, 'Sala 3', 'Description of Sala 3', 3);

GO

INSERT INTO [dbo].[chat]
           ([data]
           ,[mensagem]
           ,[id_utilizador]
           ,[id_sala])
     VALUES
/******(<data, datetime,>,<mensagem, nvarchar(100),>,<id_utilizador, int,>,<id_sala, int,>)******/

           ('2023-01-01 12:00:00', 'Hello, how are you?', 1, 1),
           ('2023-01-02 14:30:00', 'Fine, thanks!', 2, 2),
           ('2023-01-03 16:45:00', 'Great!', 3, 3);

GO

INSERT INTO [dbo].[venda]
           ([payment_method]
           ,[date]
           ,[verified]
           ,[id_sala])
     VALUES
/******(<payment_method, nvarchar(20),>,<date, datetime,>,<verified, bit,>,<id_sala, int,>)******/

           ('Credit Card', '2023-01-02 14:30:00', 1, 1),
           ('PayPal', '2023-01-03 16:45:00', 0, 2),
           ('Cash', '2023-01-04 18:00:00', 1, 3);

GO


INSERT INTO [dbo].[vendedor_has_sala]
           ([id_vendedor]
           ,[id_sala])
     VALUES
/******(<id_vendedor, int,>,<id_sala, int,>)******/

           (1, 1),
           (2, 2),
           (2, 3);

GO

SELECT [id] AS 'User ID', [handle] AS 'User Handle' FROM [dbo].[utilizador];
SELECT [id_user] AS 'Admin ID' FROM [dbo].[admin];
SELECT [id_user] AS 'Vendedor ID' FROM [dbo].[vendedor];
SELECT [id_user] AS 'Comprador ID' FROM [dbo].[comprador];
SELECT [id] AS 'Denuncia ID' FROM [dbo].[denuncia];
SELECT [id] AS 'Sala ID', [titulo] AS 'Sala Title' FROM [dbo].[sala];
SELECT [id] AS 'Chat ID', [mensagem] AS 'Chat Message' FROM [dbo].[chat];
SELECT [payment_method] AS 'Payment Method', [date] AS 'Sale Date' FROM [dbo].[venda];
SELECT [id_vendedor] AS 'Vendedor ID', [id_sala] AS 'Sala ID' FROM [dbo].[vendedor_has_sala];
