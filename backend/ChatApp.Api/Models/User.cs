using Microsoft.AspNetCore.Identity;

namespace ChatApp.Api.Models;

public class User : IdentityUser<Guid>
{
    // Attributes
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    
    // References
    public ICollection<ConversationParticipant> Participations { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
}