using ChatApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    // Tables (Users table implicitly created with IdentityDbContext)
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ConversationParticipants Composite Primary Key
        builder.Entity<ConversationParticipant>()
            .HasKey(cp => new { cp.ConversationId, cp.UserId });

        // User -> ConversationParticipant 1-N relationship
        builder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Conversation -> ConversationParticipant 1-N relationship
        builder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message -> Conversation 1-N relationship
        builder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Message 1-N relationship
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}