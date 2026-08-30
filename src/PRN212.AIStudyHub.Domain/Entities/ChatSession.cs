using System;
using System.Collections.Generic;

namespace PRN212.AIStudyHub.Domain.Entities;

public partial class ChatSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessage { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatSessionDocument> ChatSessionDocument { get; set; } = new List<ChatSessionDocument>();

    public virtual AppUser User { get; set; } = null!;
}
