using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Data;
using SistemaDeGestionTalento.Models;
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
            var vacante = await _context.Vacantes.FindAsync(id);

            if (vacante == null)
            {
                return NotFound();
            }

            return vacante;
        }
        // --- PEGA ESTE MÉTODO DENTRO DE TU CLASE VacantesController ---

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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacante(int id, Vacante vacante)
        {
            if (id != vacante.Id)
            {
                return BadRequest();
            }

            _context.Entry(vacante).State = EntityState.Modified;

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
        public async Task<ActionResult<Vacante>> PostVacante(Vacante vacante)
        {
            _context.Vacantes.Add(vacante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVacante", new { id = vacante.Id }, vacante);
        }

        // DELETE: api/Vacantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacante(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                return NotFound();
            }

            _context.Vacantes.Remove(vacante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }
    }
}
