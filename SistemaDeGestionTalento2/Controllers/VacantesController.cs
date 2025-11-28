using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.Infrastructure.Data;
using SistemaDeGestionTalento.DTOs;

namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacantesController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public VacantesController(SgiDbContext context)
        {
            _context = context;
        }

        // GET: api/Vacantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vacante>>> GetVacantes()
        {
            return await _context.Vacantes.ToListAsync();
        }

        // GET: api/Vacantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vacante>> GetVacante(int id)
        {
            var vacante = await _context.Vacantes
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.Skill)
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.NivelSkill)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacante == null)
            {
                return NotFound();
            }

            return vacante;
        }

        // GET: api/Vacantes/urgencias
        [HttpGet("urgencias")]
        public async Task<ActionResult<IEnumerable<UrgenciaVacante>>> GetUrgencias()
        {
            return await _context.UrgenciasVacante.ToListAsync();
        }

        // POST: api/Vacantes/1/skills
        [HttpPost("{id}/skills")]
        public async Task<IActionResult> AsignarSkillAVacante(int id, [FromBody] AsignarSkillVacanteDto asignarSkillDto)
        {
            // 1. Verificar si la vacante existe
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                return NotFound(new { message = "Vacante no encontrada" });
            }

            // 2. Verificar si el skill existe
            var skill = await _context.Skills.FindAsync(asignarSkillDto.SkillId);
            if (skill == null)
            {
                return NotFound(new { message = "Skill no encontrado" });
            }

            // 3. Verificar si el nivel de skill existe
            var nivel = await _context.NivelesSkill.FindAsync(asignarSkillDto.NivelId);
            if (nivel == null)
            {
                return NotFound(new { message = "Nivel de skill no encontrado" });
            }

            // 4. Verificar si la asignación ya existe (para evitar duplicados)
            var asignacionExistente = await _context.VacanteSkills
                .FirstOrDefaultAsync(vs => vs.VacanteId == id && vs.SkillId == asignarSkillDto.SkillId);

            if (asignacionExistente != null)
            {
                // Si ya existe, actualizamos el nivel requerido
                asignacionExistente.NivelId = asignarSkillDto.NivelId;
                _context.VacanteSkills.Update(asignacionExistente);
            }
            else
            {
                // Si no existe, creamos la nueva asignación
                var nuevaAsignacion = new VacanteSkill
                {
                    VacanteId = id,
                    SkillId = asignarSkillDto.SkillId,
                    NivelId = asignarSkillDto.NivelId
                };
                _context.VacanteSkills.Add(nuevaAsignacion);
            }

            // 5. Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill requerido asignado/actualizado correctamente" });
        }

        // -----------------------------------------------------------------

        // PUT: api/Vacantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacante(int id, CrearVacanteDto vacanteDto)
        {
            var vacanteExistente = await _context.Vacantes
                .Include(v => v.VacanteSkills)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacanteExistente == null)
            {
                return NotFound();
            }

            // Actualizar campos básicos
            vacanteExistente.Titulo = vacanteDto.Titulo;
            vacanteExistente.Proyecto = vacanteDto.Proyecto;
            vacanteExistente.UrgenciaId = vacanteDto.UrgenciaId;
            vacanteExistente.FechaInicioRequerida = vacanteDto.FechaInicioRequerida;

            // Actualizar Skills
            // 1. Eliminar skills existentes
            _context.VacanteSkills.RemoveRange(vacanteExistente.VacanteSkills);

            // 2. Agregar nuevos skills
            if (vacanteDto.Skills != null && vacanteDto.Skills.Any())
            {
                foreach (var skillDto in vacanteDto.Skills)
                {
                    _context.VacanteSkills.Add(new VacanteSkill
                    {
                        VacanteId = id,
                        SkillId = skillDto.SkillId,
                        NivelId = skillDto.NivelId
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacanteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vacantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vacante>> PostVacante(CrearVacanteDto vacanteDto)
        {
            // Verificamos que el LiderId existe
            var lider = await _context.Usuarios.FindAsync(vacanteDto.LiderId);
            if (lider == null)
            {
                return BadRequest(new { message = "El líder (usuario) no existe." });
            }

            // Mapeamos del DTO al Modelo
            var nuevaVacante = new Vacante
            {
                LiderId = vacanteDto.LiderId,
                Titulo = vacanteDto.Titulo,
                Proyecto = vacanteDto.Proyecto,
                UrgenciaId = vacanteDto.UrgenciaId,
                FechaInicioRequerida = vacanteDto.FechaInicioRequerida,
                Estado = "Abierta",
                FechaCreacion = DateTime.Now
            };

            _context.Vacantes.Add(nuevaVacante);
            await _context.SaveChangesAsync();

            // Guardar Skills Requeridos
            if (vacanteDto.Skills != null && vacanteDto.Skills.Any())
            {
                foreach (var skillDto in vacanteDto.Skills)
                {
                    var vacanteSkill = new VacanteSkill
                    {
                        VacanteId = nuevaVacante.Id,
                        SkillId = skillDto.SkillId,
                        NivelId = skillDto.NivelId
                    };
                    _context.VacanteSkills.Add(vacanteSkill);
                }
                await _context.SaveChangesAsync();
            }

            // Devolvemos el objeto completo, no el DTO
            // (Esto causará el error de "Ciclo Infinito" que arreglamos en Program.cs)
            return CreatedAtAction("GetVacante", new { id = nuevaVacante.Id }, nuevaVacante);
        }

        // DELETE: api/Vacantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacante(int id)
        {
            try
            {
                var vacante = await _context.Vacantes
                    .Include(v => v.VacanteSkills)
                    .Include(v => v.Matchings)
                    .Include(v => v.Notificaciones)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vacante == null)
                {
                    return NotFound();
                }

                // Eliminar relaciones manualmente para evitar error de FK (Restrict)
                if (vacante.VacanteSkills.Any())
                    _context.VacanteSkills.RemoveRange(vacante.VacanteSkills);
                
                if (vacante.Matchings.Any())
                    _context.Matchings.RemoveRange(vacante.Matchings);

                if (vacante.Notificaciones.Any())
                    _context.Notificaciones.RemoveRange(vacante.Notificaciones);

                _context.Vacantes.Remove(vacante);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (console for now)
                Console.WriteLine($"Error deleting vacancy: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { message = $"Error interno al eliminar la vacante: {ex.Message}" });
            }
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }
    }
}
