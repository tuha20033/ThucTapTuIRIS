namespace WebPortal.Domain.Entities;

public sealed class CategorySequence
{
    public string UserId { get; set; } = default!;
    public System.Guid CategoryId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsCollapsed { get; set; }
    // Navigation
    public User? User { get; set; }
    public Category? Category { get; set; }
}
