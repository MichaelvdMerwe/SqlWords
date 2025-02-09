IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SqlWords')
BEGIN
	CREATE DATABASE SqlWords;
END
GO