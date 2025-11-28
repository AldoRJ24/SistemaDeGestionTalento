using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.Infrastructure.Data;
using System.Security.Claims;

namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public NotificacionesController(SgiDbContext context)
        {
            _context = context;
        }

        // GET: api/Notificaciones/mis-notificaciones
        [HttpGet("mis-notificaciones")]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetMyNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            return await _context.Notificaciones
                .Include(n => n.Vacante) // Incluir info de la vacante
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();
        }

        // PUT: api/Notificaciones/responder-invitacion/5
        [HttpPut("responder-invitacion/{id}")]
        public async Task<IActionResult> RespondInvitation(int id, [FromBody] InvitationResponseDto response)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null) return NotFound();

            if (notificacion.UsuarioId != userId) return Unauthorized();

            if (notificacion.Tipo != "Invitacion") return BadRequest("Esta notificación no es una invitación.");

            notificacion.Estado = response.Estado; // "Aceptada" o "Rechazada"
            notificacion.Leido = true;

            if (response.Estado == "Aceptada")
            {
                // Unir al equipo del líder
                var vacante = await _context.Vacantes.FindAsync(notificacion.VacanteId);
                if (vacante != null)
                {
                    // Verificar si ya es parte del equipo
                    var exists = await _context.LiderColaborador
                        .AnyAsync(lc => lc.LiderId == vacante.LiderId && lc.ColaboradorId == userId);

                    if (!exists)
                    {
                        var relacion = new LiderColaborador
                        {
                            LiderId = vacante.LiderId,
                            ColaboradorId = userId
                        };
                        _context.LiderColaborador.Add(relacion);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Invitación {response.Estado}" });
        }
    }

    public class InvitationResponseDto
    {
        public string Estado { get; set; }
    }
}
