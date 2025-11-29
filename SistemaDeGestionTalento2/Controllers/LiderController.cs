using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.DTOs;
using SistemaDeGestionTalento.Infrastructure.Data;
using System.Security.Claims;
using System;
using System.Linq;

namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure user is logged in
    public class LiderController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public LiderController(SgiDbContext context)
        {
            _context = context;
        }

        // GET: api/Lider/team
        [HttpGet("team")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetMyTeam()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var team = await _context.LiderColaborador
                .Where(lc => lc.LiderId == userId)
                .Include(lc => lc.Colaborador)
                .Select(lc => lc.Colaborador)
                .ToListAsync();

            return Ok(team);
        }

        // GET: api/Lider/colaborador/5/skills
        [HttpGet("colaborador/{colabId}/skills")]
        public async Task<ActionResult<IEnumerable<object>>> GetCollaboratorSkills(int colabId)
        {
            // Verify if the collaborator belongs to the leader (Optional but recommended security check)
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isMyCollab = await _context.LiderColaborador
                .AnyAsync(lc => lc.LiderId == userId && lc.ColaboradorId == colabId);

            // For now, we might skip strict check to allow viewing, or enforce it.
            // if (!isMyCollab) return Forbid();

            var skills = await _context.ColaboradoresSkills
                .Where(cs => cs.UsuarioId == colabId)
                .Include(cs => cs.Skill)
                .Include(cs => cs.NivelSkill)
                .Select(cs => new
                {
                    cs.Id,
                    SkillNombre = cs.Skill.Nombre,
                    NivelNombre = cs.NivelSkill.Nombre,
                    cs.Estado, // Pendiente, Validado, Rechazado
                    cs.FechaEvaluacion
                })
                .ToListAsync();

            return Ok(skills);
        }

        // PUT: api/Lider/skill/5/validate
        [HttpPut("skill/{colabSkillId}/validate")]
        public async Task<IActionResult> ValidateSkill(int colabSkillId, [FromBody] ValidateSkillDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var colabSkill = await _context.ColaboradoresSkills.FindAsync(colabSkillId);
            if (colabSkill == null) return NotFound();

            colabSkill.Estado = dto.Estado;
            colabSkill.EvaluadorId = userId;
            colabSkill.FechaEvaluacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Skill actualizado a {dto.Estado}" });
        }

        // GET: api/Lider/vacancies
        [HttpGet("vacancies")]
        public async Task<ActionResult<IEnumerable<Vacante>>> GetMyVacancies()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Console.WriteLine($"[GetMyVacancies] User Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

            if (userIdClaim == null) 
            {
                Console.WriteLine("[GetMyVacancies] NameIdentifier claim not found.");
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            Console.WriteLine($"[GetMyVacancies] Requesting vacancies for LiderId: {userId}");

            var vacancies = await _context.Vacantes
                .Where(v => v.LiderId == userId)
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.Skill)
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.NivelSkill)
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();

            Console.WriteLine($"[GetMyVacancies] Found {vacancies.Count} vacancies.");

            return Ok(vacancies);
        }

        [HttpPost("invitar")]
        public async Task<IActionResult> InviteCandidate([FromBody] InviteCandidateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int liderId = int.Parse(userIdClaim.Value);

            var vacante = await _context.Vacantes.FindAsync(dto.VacanteId);
            if (vacante == null) return NotFound("Vacante no encontrada");

            if (vacante.LiderId != liderId) return Unauthorized("No eres el dueño de esta vacante");

            // Verificar si ya existe invitación pendiente
            var existing = await _context.Notificaciones
                .AnyAsync(n => n.VacanteId == dto.VacanteId && n.UsuarioId == dto.UsuarioId && n.Tipo == "Invitacion" && n.Estado == "Enviada");
            
            if (existing) return BadRequest("Ya existe una invitación pendiente para este usuario.");

            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                VacanteId = dto.VacanteId,
                Mensaje = $"Has sido invitado a aplicar a la vacante: {vacante.Titulo}",
                Tipo = "Invitacion",
                Estado = "Enviada",
                Fecha = DateTime.Now,
                Leido = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invitación enviada correctamente" });
        }

        // DELETE: api/Lider/team/5
        [HttpDelete("team/{colaboradorId}")]
        public async Task<IActionResult> RemoveCollaborator(int colaboradorId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int liderId = int.Parse(userIdClaim.Value);

            // Buscar la relación
            var relacion = await _context.LiderColaborador
                .FirstOrDefaultAsync(lc => lc.LiderId == liderId && lc.ColaboradorId == colaboradorId);

            if (relacion == null)
            {
                return NotFound("El colaborador no está en tu equipo.");
            }

            // Obtener nombre del líder para la notificación
            var lider = await _context.Usuarios.FindAsync(liderId);
            string nombreLider = lider != null ? $"{lider.Nombre} {lider.Apellido}" : "Tu líder";

            // Eliminar relación
            _context.LiderColaborador.Remove(relacion);

            // Crear notificación
            var notificacion = new Notificacion
            {
                UsuarioId = colaboradorId,
                Mensaje = $"Has sido eliminado del equipo del líder {nombreLider}.",
                Tipo = "Expulsion",
                Estado = "Enviada",
                Fecha = DateTime.Now,
                Leido = false
            };
            _context.Notificaciones.Add(notificacion);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Colaborador eliminado del equipo correctamente." });
        }
    }

    public class InviteCandidateDto
    {
        public int VacanteId { get; set; }
        public int UsuarioId { get; set; }
    }
}
