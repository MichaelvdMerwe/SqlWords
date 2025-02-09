USE SqlWords
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SensitiveWord' AND TABLE_SCHEMA = 'dbo')
BEGIN
    CREATE TABLE dbo.SensitiveWord (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Word NVARCHAR(100) NOT NULL UNIQUE,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SensitiveWord_Word' AND object_id = OBJECT_ID('dbo.SensitiveWord'))
BEGIN
    CREATE UNIQUE INDEX IX_SensitiveWord_Word ON dbo.SensitiveWord (Word);
END
GO