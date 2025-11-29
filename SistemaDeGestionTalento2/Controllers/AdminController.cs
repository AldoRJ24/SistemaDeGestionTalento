using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.DTOs;
using SistemaDeGestionTalento.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "AdminRRHH")] // Uncomment when roles are fully set up
    public class AdminController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public AdminController(SgiDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var stats = new DashboardStatsDto();

            // 1. Totales
            stats.TotalVacantes = await _context.Vacantes.CountAsync();
            stats.TotalColaboradores = await _context.Usuarios
                .Include(u => u.Rol)
                .CountAsync(u => u.Rol.Nombre == "Colaborador");

            // 2. Top Requested Skills (from VacanteSkills)
            var topRequested = await _context.VacanteSkills
                .Include(vs => vs.Skill)
                .GroupBy(vs => vs.Skill.Nombre)
                .Select(g => new StatItemDto { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            stats.TopRequestedSkills = topRequested;

            // 3. Top Collaborator Skills (from ColaboradorSkills)
            var topCollabSkills = await _context.ColaboradoresSkills
                .Include(cs => cs.Skill)
                .GroupBy(cs => cs.Skill.Nombre)
                .Select(g => new StatItemDto { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            stats.TopCollaboratorSkills = topCollabSkills;

            // 4. Vacancies by Status
            var vacanciesByStatus = await _context.Vacantes
                .GroupBy(v => v.Estado)
                .Select(g => new StatItemDto { Name = g.Key, Count = g.Count() })
                .ToListAsync();
            stats.VacanciesByStatus = vacanciesByStatus;

            // 5. Supply vs Demand Trend (Last 6 Months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            
            var vacanciesTrend = await _context.Vacantes
                .Where(v => v.FechaCreacion >= sixMonthsAgo)
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var collaboratorsTrend = await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Colaborador" && u.FechaCreacion >= sixMonthsAgo)
                .GroupBy(u => new { u.FechaCreacion.Year, u.FechaCreacion.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var trend = new List<TrendItemDto>();
            for (int i = 0; i < 6; i++)
            {
                var date = DateTime.Now.AddMonths(-i);
                var vCount = vacanciesTrend.FirstOrDefault(x => x.Year == date.Year && x.Month == date.Month)?.Count ?? 0;
                var cCount = collaboratorsTrend.FirstOrDefault(x => x.Year == date.Year && x.Month == date.Month)?.Count ?? 0;
                
                trend.Add(new TrendItemDto 
                { 
                    Period = date.ToString("MMM yyyy"), 
                    Vacancies = vCount, 
                    Collaborators = cCount 
                });
            }
            stats.SupplyDemandTrend = trend.OrderBy(t => DateTime.Parse(t.Period)).ToList();

            return Ok(stats);
        }
    }
}
