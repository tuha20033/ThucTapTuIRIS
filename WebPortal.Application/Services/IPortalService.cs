using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Application.Models;
using WebPortal.Domain.Enums;

namespace WebPortal.Application.Services;

public interface IPortalService
{
    Task<PortalModel> GetPortalAsync(string? searchText, CancellationToken ct = default);

    Task PinAsync(Guid linkId, CancellationToken ct = default);
    Task UnpinAsync(Guid linkId, CancellationToken ct = default);

    Task SetViewModeAsync(ViewMode mode, CancellationToken ct = default);

    Task SetLinkOrderAsync(IReadOnlyList<Guid> orderedLinkIds, CancellationToken ct = default);
    Task SetCategoryOrderAsync(IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default);

    Task SetCategoryCollapsedAsync(Guid categoryId, bool isCollapsed, CancellationToken ct = default);
}
