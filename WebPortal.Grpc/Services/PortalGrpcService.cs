using Grpc.Core;
using WebPortal.Application.Services;
using WebPortal.Domain.Enums;
using WebPortal.Grpc.Contracts;
using ProtoViewMode = WebPortal.Grpc.Contracts.ViewMode;

namespace WebPortal.Grpc.Services;

public sealed class PortalGrpcService : PortalGrpc.PortalGrpcBase
{
    private readonly IPortalService _portal;

    public PortalGrpcService(IPortalService portal) => _portal = portal;

    public override async Task<GetPortalResponse> GetPortal(GetPortalRequest request, ServerCallContext context)
    {
        var model = await _portal.GetPortalAsync(request.SearchText, context.CancellationToken);

        var resp = new GetPortalResponse
        {
            UserId = model.UserId,
            UserName = model.UserName,
            ViewMode = ToProtoViewMode(model.ViewMode),
            GeneratedAtUtc = model.GeneratedAtUtc.ToString("O")
        };

        resp.PinnedLinks.AddRange(model.PinnedLinks.Select(ToProtoLink));
        resp.Categories.AddRange(model.Categories.Select(ToProtoCategory));

        return resp;
    }

    public override async Task<Empty> PinLink(LinkIdRequest request, ServerCallContext context)
    {
        await _portal.PinAsync(System.Guid.Parse(request.LinkId), context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> UnpinLink(LinkIdRequest request, ServerCallContext context)
    {
        await _portal.UnpinAsync(System.Guid.Parse(request.LinkId), context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> SetViewMode(SetViewModeRequest request, ServerCallContext context)
    {
        var mode = FromProtoViewMode(request.Mode);
        await _portal.SetViewModeAsync(mode, context.CancellationToken);
        return new Empty();
    }

    private static ProtoViewMode ToProtoViewMode(Domain.Enums.ViewMode mode)
        => mode == Domain.Enums.ViewMode.List ? (ProtoViewMode)1 : (ProtoViewMode)0;

    private static Domain.Enums.ViewMode FromProtoViewMode(ProtoViewMode mode)
        => (int)mode == 1 ? Domain.Enums.ViewMode.List : Domain.Enums.ViewMode.Grid;

    public override async Task<Empty> SetLinkOrder(SetOrderRequest request, ServerCallContext context)
    {
        var ids = request.Ids.Select(System.Guid.Parse).ToList();
        await _portal.SetLinkOrderAsync(ids, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> SetCategoryOrder(SetOrderRequest request, ServerCallContext context)
    {
        var ids = request.Ids.Select(System.Guid.Parse).ToList();
        await _portal.SetCategoryOrderAsync(ids, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> SetCategoryCollapsed(SetCategoryCollapsedRequest request, ServerCallContext context)
    {
        await _portal.SetCategoryCollapsedAsync(System.Guid.Parse(request.CategoryId), request.IsCollapsed, context.CancellationToken);
        return new Empty();
    }

    private static PortalLink ToProtoLink(WebPortal.Application.Models.PortalLinkModel m)
        => new()
        {
            Id = m.Id.ToString(),
            Name = m.Name,
            Url = m.Url,
            Icon = m.Icon ?? "",
            Color = m.Color ?? "",
            Tags = m.Tags ?? "",
            RolePrefix = m.RolePrefix,
            CategoryId = m.CategoryId.ToString(),
            IsPinned = m.IsPinned,
            Domain = m.Domain ?? ""
        };

    private static PortalCategory ToProtoCategory(WebPortal.Application.Models.PortalCategoryModel c)
    {
        var pc = new PortalCategory
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Description = c.Description ?? "",
            DefaultPriority = c.DefaultPriority,
            IsCollapsed = c.IsCollapsed
        };
        pc.Links.AddRange(c.Links.Select(ToProtoLink));
        return pc;
    }
}
