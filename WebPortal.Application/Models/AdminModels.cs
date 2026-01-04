using System;

namespace WebPortal.Application.Models;


public sealed class CategoryUpsertModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}


public sealed class LinkUpsertModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? Tags { get; set; }
    public int Priority { get; set; } = 0;
    public string RolePrefix { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
}
