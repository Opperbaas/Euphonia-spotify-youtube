using Resonance.BusinessLogicLayer.DTOs;
using System.Threading.Tasks;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Service interface voor Dashboard statistieken en inzichten
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Haalt alle dashboard statistieken op voor een gebruiker
        /// </summary>
        Task<DashboardDto> GetDashboardDataAsync(int userId);
    }
}

