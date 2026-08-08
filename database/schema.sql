CREATE DATABASE AIStudyHubDB;
GO
USE AIStudyHubDB;
GO

-- =============================================
-- 1. AppUser: Quản lý tài khoản người dùng
-- =============================================
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
GO

-- =============================================
-- 2. RefreshToken: Quản lý phiên xác thực JWT
-- =============================================
CREATE TABLE RefreshToken (
  Id INT PRIMARY KEY IDENTITY(1,1),
  UserId INT NOT NULL,
  Token VARCHAR(500) UNIQUE NOT NULL,
  ExpiresAt DATETIME2(7) NOT NULL,
  IsRevoked BIT NOT NULL DEFAULT 0,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_RefreshToken_AppUser FOREIGN KEY (UserId) REFERENCES AppUser(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 3. Subject: Danh mục Môn học
-- =============================================
CREATE TABLE Subject (
  Id INT PRIMARY KEY IDENTITY(1,1),
  Name NVARCHAR(100) UNIQUE NOT NULL,
  Description NVARCHAR(500) NULL,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- =============================================
-- 4. Document: Quản lý Tài liệu học tập
-- =============================================
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
  IsCloudStored BIT NOT NULL DEFAULT 0,
  CloudPublicId NVARCHAR(500) NULL,
  IsPublic BIT NOT NULL DEFAULT 0,
  ProcessingStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',
  IsDeleted BIT NOT NULL DEFAULT 0,
  DeletedAt DATETIME2(7) NULL,

  CONSTRAINT FK_Document_AppUser FOREIGN KEY (UserId) REFERENCES AppUser(Id) ON DELETE CASCADE,
  CONSTRAINT FK_Document_Subject FOREIGN KEY (SubjectId) REFERENCES Subject(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 5. DocumentSummary: Tóm tắt nội dung tài liệu AI
-- =============================================
CREATE TABLE DocumentSummary (
  Id INT PRIMARY KEY IDENTITY(1,1),
  DocumentId INT UNIQUE NOT NULL,
  SummaryContent NVARCHAR(MAX) NOT NULL,
  KeyTakeaways NVARCHAR(MAX) NULL,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(7) NULL,
  CONSTRAINT FK_DocumentSummary_Document FOREIGN KEY (DocumentId) REFERENCES Document(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 6. ChatSession: Phiên hỏi đáp với AI Chatbot
-- =============================================
CREATE TABLE ChatSession (
  Id INT PRIMARY KEY IDENTITY(1,1),
  UserId INT NOT NULL,
  Title NVARCHAR(255) NOT NULL,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(7) NULL,
  CONSTRAINT FK_ChatSession_AppUser FOREIGN KEY (UserId) REFERENCES AppUser(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 7. ChatSessionDocument: Bảng trung gian Phiên Chat - Tài liệu (N-N)
-- =============================================
CREATE TABLE ChatSessionDocument (
  SessionId INT NOT NULL,
  DocumentId INT NOT NULL,
  AttachedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  
  CONSTRAINT PK_ChatSessionDocument PRIMARY KEY (SessionId, DocumentId),
  CONSTRAINT FK_CSD_ChatSession FOREIGN KEY (SessionId) REFERENCES ChatSession(Id) ON DELETE CASCADE,
  CONSTRAINT FK_CSD_Document FOREIGN KEY (DocumentId) REFERENCES Document(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 8. ChatMessage: Nhật ký tin nhắn Chatbot
-- =============================================
CREATE TABLE ChatMessage (
  Id INT PRIMARY KEY IDENTITY(1,1),
  SessionId INT NOT NULL,
  Sender VARCHAR(20) NOT NULL, -- 'User' hoặc 'Assistant'
  Content NVARCHAR(MAX) NOT NULL,
  SentAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_ChatMessage_ChatSession FOREIGN KEY (SessionId) REFERENCES ChatSession(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 9. FlashcardSet: Bộ thẻ ghi nhớ AI
-- =============================================
CREATE TABLE FlashcardSet (
  Id INT PRIMARY KEY IDENTITY(1,1),
  UserId INT NOT NULL,
  DocumentId INT NULL,
  Title NVARCHAR(255) NOT NULL,
  Description NVARCHAR(500) NULL,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_FlashcardSet_AppUser FOREIGN KEY (UserId) REFERENCES AppUser(Id) ON DELETE CASCADE,
  CONSTRAINT FK_FlashcardSet_Document FOREIGN KEY (DocumentId) REFERENCES Document(Id) ON DELETE SET NULL
);
GO

-- =============================================
-- 10. FlashcardItem: Chi tiết thẻ Flashcard
-- =============================================
CREATE TABLE FlashcardItem (
  Id INT PRIMARY KEY IDENTITY(1,1),
  SetId INT NOT NULL,
  Question NVARCHAR(MAX) NOT NULL,
  Answer NVARCHAR(MAX) NOT NULL,
  IsMastered BIT NOT NULL DEFAULT 0,
  CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_FlashcardItem_FlashcardSet FOREIGN KEY (SetId) REFERENCES FlashcardSet(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- CHỈ MỤC PHỤ (NON-CLUSTERED INDEXES) TỐI ƯU TRA CỨU
-- =============================================
CREATE NONCLUSTERED INDEX IX_RefreshToken_UserId ON RefreshToken(UserId);

CREATE NONCLUSTERED INDEX IX_Document_UserId ON Document(UserId);
CREATE NONCLUSTERED INDEX IX_Document_SubjectId ON Document(SubjectId);
CREATE NONCLUSTERED INDEX IX_Document_IsDeleted_SubjectId ON Document(IsDeleted, SubjectId);

CREATE NONCLUSTERED INDEX IX_DocumentSummary_DocumentId ON DocumentSummary(DocumentId);

CREATE NONCLUSTERED INDEX IX_ChatSession_UserId ON ChatSession(UserId);
CREATE NONCLUSTERED INDEX IX_ChatSessionDocument_DocumentId ON ChatSessionDocument(DocumentId);
CREATE NONCLUSTERED INDEX IX_ChatMessage_SessionId ON ChatMessage(SessionId);

CREATE NONCLUSTERED INDEX IX_FlashcardSet_UserId ON FlashcardSet(UserId);
CREATE NONCLUSTERED INDEX IX_FlashcardSet_DocumentId ON FlashcardSet(DocumentId);
CREATE NONCLUSTERED INDEX IX_FlashcardItem_SetId ON FlashcardItem(SetId);
GO