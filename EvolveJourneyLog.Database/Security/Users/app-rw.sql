CREATE USER [app-rw] WITH PASSWORD = 'SOMEPASSWORD';
GO

EXEC sp_addrolemember 'db_datareader', [app-rw];
GO

EXEC sp_addrolemember 'db_datawriter', [app-rw];
GO
