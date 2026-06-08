namespace ChatApp.Api.Models;

public class Message
{
    // Attributes
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Foreign Keys
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }

    // References
    public Conversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
}