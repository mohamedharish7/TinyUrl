-- Create database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinyUrl')
    CREATE DATABASE TinyUrl;
GO

USE TinyUrl;
GO

-- Create table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UrlEntries' AND xtype='U')
CREATE TABLE UrlEntries (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    ShortCode   NVARCHAR(50)  NOT NULL,
    OriginalUrl NVARCHAR(2048) NOT NULL,
    CreatedAt   DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    HitCount    INT           NOT NULL DEFAULT 0,
    CONSTRAINT UQ_UrlEntries_ShortCode UNIQUE (ShortCode)
);
GO

-- Sample data
INSERT INTO UrlEntries (ShortCode, OriginalUrl, CreatedAt, HitCount) VALUES
('abc123', 'https://www.google.com',         GETUTCDATE(), 10),
('xyz789', 'https://www.github.com',         GETUTCDATE(), 5),
('ms2024', 'https://www.microsoft.com',      GETUTCDATE(), 3),
('awsdoc', 'https://docs.aws.amazon.com',    GETUTCDATE(), 8),
('ytmain', 'https://www.youtube.com',        GETUTCDATE(), 20);
GO

SELECT * FROM UrlEntries;
