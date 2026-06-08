namespace ChatApp.Api.Models;

public class Conversation
{
    // Attributes
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } // null for non group
    public bool IsGroup { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; } // null before first message

    // References
    public ICollection<ConversationParticipant> Participants { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}
