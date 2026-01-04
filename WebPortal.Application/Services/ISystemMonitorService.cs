using System.Threading;
using System.Threading.Tasks;
using WebPortal.Application.Models;

namespace WebPortal.Application.Services;

public interface ISystemMonitorService
{
    Task<SystemStatusModel> GetStatusAsync(CancellationToken ct = default);
}
