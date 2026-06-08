namespace ChatApp.Api.Models;

/// <summary>
/// Join Table between User and Conversation to store more metadata
/// </summary>
public class ConversationParticipant
{
    // Foreign Keys
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }

    // Metadata
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReadAt { get; set; }
    public bool IsAdmin { get; set; }

    // References
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}