namespace WebPortal.Domain.Entities;

public sealed class LinkSequence
{
    public string UserId { get; set; } = default!;
    public System.Guid LinkId { get; set; }
    public int OrderIndex { get; set; }

    public User? User { get; set; }
    public Link? Link { get; set; }
}
