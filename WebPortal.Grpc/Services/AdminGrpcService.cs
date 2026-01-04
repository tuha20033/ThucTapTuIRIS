using Grpc.Core;
using WebPortal.Application.Models;
using WebPortal.Application.Services;
using WebPortal.Grpc.Contracts;

namespace WebPortal.Grpc.Services;

public sealed class AdminGrpcService : AdminGrpc.AdminGrpcBase
{
    private readonly ICatalogAdminService _admin;

    public AdminGrpcService(ICatalogAdminService admin) => _admin = admin;

    public override async Task<ListCategoriesResponse> ListCategories(Empty request, ServerCallContext context)
    {
        var items = await _admin.GetCategoriesAsync(context.CancellationToken);
        var resp = new ListCategoriesResponse();
        resp.Items.AddRange(items.Select(c => new CategoryDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Description = c.Description ?? "",
            Priority = c.Priority,
            IsActive = c.IsActive
        }));
        return resp;
    }

    public override async Task<ListLinksResponse> ListLinks(Empty request, ServerCallContext context)
    {
        var items = await _admin.GetLinksAsync(context.CancellationToken);
        var resp = new ListLinksResponse();
        resp.Items.AddRange(items.Select(l => new LinkDto
        {
            Id = l.Id.ToString(),
            Name = l.Name,
            Url = l.Url,
            Icon = l.Icon ?? "",
            Color = l.Color ?? "",
            Tags = l.Tags ?? "",
            Priority = l.Priority,
            RolePrefix = l.RolePrefix,
            IsActive = l.IsActive,
            CategoryId = l.CategoryId.ToString()
        }));
        return resp;
    }

    public override async Task<CategoryDto> UpsertCategory(CategoryUpsertRequest request, ServerCallContext context)
    {
        var model = new CategoryUpsertModel
        {
            Id = string.IsNullOrWhiteSpace(request.Id) ? null : System.Guid.Parse(request.Id),
            Name = request.Name,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            Priority = request.Priority,
            IsActive = request.IsActive
        };

        var result = await _admin.UpsertCategoryAsync(model, context.CancellationToken);

        return new CategoryDto
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Description = result.Description ?? "",
            Priority = result.Priority,
            IsActive = result.IsActive
        };
    }

    public override async Task<LinkDto> UpsertLink(LinkUpsertRequest request, ServerCallContext context)
    {
        var model = new LinkUpsertModel
        {
            Id = string.IsNullOrWhiteSpace(request.Id) ? null : System.Guid.Parse(request.Id),
            Name = request.Name,
            Url = request.Url,
            Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon,
            Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color,
            Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags,
            Priority = request.Priority,
            RolePrefix = request.RolePrefix,
            IsActive = request.IsActive,
            CategoryId = System.Guid.Parse(request.CategoryId)
        };

        var result = await _admin.UpsertLinkAsync(model, context.CancellationToken);

        return new LinkDto
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Url = result.Url,
            Icon = result.Icon ?? "",
            Color = result.Color ?? "",
            Tags = result.Tags ?? "",
            Priority = result.Priority,
            RolePrefix = result.RolePrefix,
            IsActive = result.IsActive,
            CategoryId = result.CategoryId.ToString()
        };
    }

    public override async Task<Empty> SetCategoryActive(SetActiveRequest request, ServerCallContext context)
    {
        await _admin.SetCategoryActiveAsync(System.Guid.Parse(request.Id), request.IsActive, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> SetLinkActive(SetActiveRequest request, ServerCallContext context)
    {
        await _admin.SetLinkActiveAsync(System.Guid.Parse(request.Id), request.IsActive, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> DeleteCategory(IdRequest request, ServerCallContext context)
    {
        await _admin.DeleteCategoryAsync(System.Guid.Parse(request.Id), context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> DeleteLink(IdRequest request, ServerCallContext context)
    {
        await _admin.DeleteLinkAsync(System.Guid.Parse(request.Id), context.CancellationToken);
        return new Empty();
    }
}
