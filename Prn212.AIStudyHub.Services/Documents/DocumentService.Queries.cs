using Microsoft.EntityFrameworkCore;
using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Documents;

/// <summary>
/// Phần ĐỌC: danh sách / tìm / lọc / sắp xếp / chi tiết / danh sách môn.
/// NGƯỜI DOC 2 làm ở file này. (cùng class DocumentService với file Commands)
/// </summary>
public partial class DocumentService
{
    public (List<Document> Items, int TotalCount) GetPaged(
        int page, int pageSize, string? keyword = null, int? subjectId = null, string? sortBy = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 10;

        using var context = new AistudyHubDbContext();
        IQueryable<Document> query = context.Documents
            .Include(d => d.Subject)
            .Include(d => d.User);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(d => d.Title.Contains(keyword) || d.FileName.Contains(keyword));

        if (subjectId.HasValue)
            query = query.Where(d => d.SubjectId == subjectId.Value);

        query = sortBy switch
        {
            "name" => query.OrderBy(d => d.Title),
            "size" => query.OrderByDescending(d => d.FileSize),
            _ => query.OrderByDescending(d => d.UploadedAt)
        };

        int total = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, total);
    }

    public Document? GetDetail(int documentId)
    {
        using var context = new AistudyHubDbContext();
        return context.Documents
            .Include(d => d.Subject)
            .Include(d => d.User)
            .FirstOrDefault(d => d.Id == documentId);
    }

    public List<Subject> GetAllSubjects()
    {
        using var context = new AistudyHubDbContext();
        return context.Subjects.OrderBy(s => s.Name).ToList();
    }
}
