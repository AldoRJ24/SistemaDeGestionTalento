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

            return Ok(stats);
        }
    }
}
