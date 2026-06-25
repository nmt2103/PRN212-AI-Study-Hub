-- Sử dụng cơ sở dữ liệu AIStudyHubDB
USE AIStudyHubDB;
GO

-- Xóa dữ liệu cũ để tránh trùng lặp khi chạy lại file seed (tùy chọn nhưng an toàn khi phát triển)
-- Xóa theo thứ tự ngược của khóa ngoại: Document -> Subject & AppUser
DELETE FROM Document;
DELETE FROM Subject;
DELETE FROM AppUser;

-- Reset identity (thiết lập lại ID tự tăng về 0 để khi chèn sẽ bắt đầu từ 1)
DBCC CHECKIDENT ('AppUser', RESEED, 0);
DBCC CHECKIDENT ('Subject', RESEED, 0);
DBCC CHECKIDENT ('Document', RESEED, 0);
GO

--------------------------------------------------------------------------------
-- 1. CHÈN DỮ LIỆU MẪU CHO BẢNG AppUser (Tài khoản người dùng)
--   - Role có các giá trị: 'Admin', 'Student' (Có thể mở rộng sau)
--------------------------------------------------------------------------------
INSERT INTO AppUser (Email, PasswordHash, FirstName, LastName, [Role], IsActive, CreatedAt)
VALUES 
(
    'admin@aistudyhub.com', 
    '123456',
    N'Quản', N'Trị Viên', 
    'Admin', 
    1, 
    SYSUTCDATETIME()
),
(
    'thuannm@aistudyhub.com', 
    '123456',
    N'Sinh', N'Viên', 
    'Student', 
    1, 
    SYSUTCDATETIME()
);
GO

--------------------------------------------------------------------------------
-- 2. CHÈN DỮ LIỆU MẪU CHO BẢNG Subject (Môn học)
--------------------------------------------------------------------------------
INSERT INTO Subject (Name, Description, CreatedAt)
VALUES 
(
    'PRN212', 
    N'C# và Windows Presentation Foundation (WPF) - Phát triển ứng dụng Desktop chuyên nghiệp', 
    SYSUTCDATETIME()
),
(
    'PRN221', 
    N'Phát triển ứng dụng Doanh nghiệp với C# và .NET Core (Razor Pages, SignalR)', 
    SYSUTCDATETIME()
),
(
    'PRN231', 
    N'Phát triển Web API và dịch vụ RESTful với ASP.NET Core', 
    SYSUTCDATETIME()
),
(
    'DBW301', 
    N'Hệ quản trị cơ sở dữ liệu SQL Server - Thiết kế và truy vấn nâng cao', 
    SYSUTCDATETIME()
),
(
    'SWE302', 
    N'Kỹ nghệ yêu cầu phần mềm - Quy trình phân tích và thu thập yêu cầu', 
    SYSUTCDATETIME()
);
GO

--------------------------------------------------------------------------------
-- 3. CHÈN DỮ LIỆU MẪU CHO BẢNG Document (Tài liệu học tập)
--------------------------------------------------------------------------------
INSERT INTO Document (UserId, SubjectId, Title, FileName, StoragePath, FileSize, FileExtension, ContentType, UploadedAt)
VALUES
(
    1,
    1,
    N'WPF Lesson 1: Introduction to WPF and XAML',
    'WPF_Lesson1_Intro.pdf',
    '/uploads/documents/prn212/WPF_Lesson1_Intro.pdf',
    1572864,
    '.pdf',
    'application/pdf',
    SYSUTCDATETIME()
),
(
    1,
    1,
    N'Bài giải Lab 1 - Thiết kế giao diện máy tính bỏ túi',
    'Lab1_Calculator_Solution.zip',
    '/uploads/documents/prn212/Lab1_Calculator_Solution.zip',
    4194304,
    '.zip',
    'application/x-zip-compressed',
    SYSUTCDATETIME()
),
(
    1,
    2,
    N'Giáo trình môn học PRN221 - Enterprise Application Development',
    'PRN221_CourseBook.pdf',
    '/uploads/documents/prn221/PRN221_CourseBook.pdf',
    10485760,
    '.pdf',
    'application/pdf',
    SYSUTCDATETIME()
),
(
    1,
    4,
    N'Bảng tra cứu nhanh cú pháp SQL (SQL Cheatsheet)',
    'SQL_Cheatsheet_v2.docx',
    '/uploads/documents/dbw301/SQL_Cheatsheet_v2.docx',
    524288,
    '.docx',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    SYSUTCDATETIME()
),
(
    1,
    3,
    N'Slide 3: Thiết kế chuẩn RESTful Web API',
    'Slide3_RESTful_API_Design.pdf',
    '/uploads/documents/prn231/Slide3_RESTful_API_Design.pdf',
    2097152,
    '.pdf',
    'application/pdf',
    SYSUTCDATETIME()
);
GO
