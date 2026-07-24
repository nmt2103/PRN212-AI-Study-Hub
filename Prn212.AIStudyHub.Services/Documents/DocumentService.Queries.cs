using Microsoft.EntityFrameworkCore;
using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Documents;

/// <summary>
/// Phần ĐỌC: danh sách / tìm / lọc / sắp xếp / chi tiết / danh sách môn.
/// NGƯỜI DOC 2 làm ở file này. (cùng class DocumentService với file Commands)
/// </summary>
public partial class DocumentService
{
  /// <summary>
  /// Tìm kiếm tài liệu theo keyword trên name, subject name, subject description
  /// </summary>
  public (List<Document> Items, int TotalCount) SearchDocuments(
          string? keyword = null, int? subjectId = null, int? userId = null, int page = 1, int pageSize = 10, string? sortBy = null)
  {
    page = Math.Max(1, page);
    pageSize = Math.Max(1, pageSize);

    using var context = new AistudyHubDbContext();
    IQueryable<Document> query = context.Documents
        .Include(d => d.Subject)
        .Include(d => d.User);

    if (!string.IsNullOrWhiteSpace(keyword))
    {
      string lowerKeyword = keyword.ToLower();
      query = query.Where(d =>
          d.Title.ToLower().Contains(lowerKeyword) ||
          d.FileName.ToLower().Contains(lowerKeyword) ||
          d.Subject.Name.ToLower().Contains(lowerKeyword) ||
          (d.Subject.Description != null && d.Subject.Description.ToLower().Contains(lowerKeyword))
      );
    }

    if (subjectId.HasValue && subjectId.Value != -1)
    {
      query = query.Where(d => d.SubjectId == subjectId.Value);
    }

    if (userId.HasValue)
    {
      query = query.Where(d => d.UserId == userId.Value);
    }

    query = sortBy switch
    {
      "name" => query.OrderBy(d => d.Title),
      "size" => query.OrderByDescending(d => d.FileSize),
      "subject" => query.OrderBy(d => d.Subject.Name),
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
