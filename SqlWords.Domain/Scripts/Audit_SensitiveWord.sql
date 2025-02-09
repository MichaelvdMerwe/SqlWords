-- TODO : enhancement, i want to create an AuditSensitiveWord entity to experiment with joins using dapper

CREATE TABLE Audit_SensitiveWord (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ActionType NVARCHAR(50) NOT NULL,
    Word NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ModifiedBy NVARCHAR(100) NOT NULL
);

CREATE INDEX IX_Audit_SensitiveWord_CreatedAt ON Audit_SensitiveWord (CreatedAt);