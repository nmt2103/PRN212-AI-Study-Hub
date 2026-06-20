CREATE DATABASE AIStudyHubDB;
GO
USE AIStudyHubDB;
GO

-- 1. AppUser: Tài khoản
CREATE TABLE AppUser (
  Id INT PRIMARY KEY IDENTITY(1,1),
  Email VARCHAR(255) UNIQUE NOT NULL,                 
  PasswordHash VARCHAR(255) NOT NULL,                 
  FirstName NVARCHAR(100) NOT NULL,                   
  LastName NVARCHAR(100) NOT NULL,
  [Role] VARCHAR(50) NOT NULL DEFAULT 'Student',     
  IsActive BIT NOT NULL DEFAULT 1,                    
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(7) NULL                         
);

-- 2. Subject: Môn học
CREATE TABLE Subject (
  Id INT PRIMARY KEY IDENTITY(1,1),
  Name NVARCHAR(100) UNIQUE NOT NULL,                 
  Description NVARCHAR(500) NULL,                     
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);

-- 3. Document: Tài liệu môn học
CREATE TABLE Document (
  Id INT PRIMARY KEY IDENTITY(1,1),
  UserId INT NOT NULL,
  SubjectId INT NOT NULL,                             
  Title NVARCHAR(255) NOT NULL,
  FileName NVARCHAR(255) NOT NULL,                    
  StoragePath NVARCHAR(2048) NOT NULL,                
  FileSize BIGINT NOT NULL,                           
  FileExtension VARCHAR(10) NOT NULL,                
  ContentType VARCHAR(100) NOT NULL,                 
  UploadedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  
  -- Khóa ngoại
  CONSTRAINT FK_Document_User FOREIGN KEY (UserId) REFERENCES AppUser(Id) ON DELETE CASCADE,
  CONSTRAINT FK_Document_Subject FOREIGN KEY (SubjectId) REFERENCES Subject(Id) ON DELETE CASCADE
);
GO

-- 4. Indexes
CREATE NONCLUSTERED INDEX IX_Document_UserId ON Document(UserId);
CREATE NONCLUSTERED INDEX IX_Document_SubjectId ON Document(SubjectId);