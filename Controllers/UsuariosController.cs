using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Data;
using SistemaDeGestionTalento.DTOs;
using SistemaDeGestionTalento.Models;
    
namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public UsuariosController(SgiDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        // --- PEGA ESTE MÉTODO DENTRO DE TU CLASE UsuariosController ---

        // POST: api/Usuarios/5/skills
        [HttpPost("{id}/skills")]
        public async Task<IActionResult> AsignarSkillAUsuario(int id, [FromBody] AsignarSkillDto asignarSkillDto)
        {
            // 1. Verificar si el usuario existe
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
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

            // 4. (Opcional) Verificar si el evaluador existe, si se proporcionó uno
            if (asignarSkillDto.EvaluadorId.HasValue)
            {
                var evaluador = await _context.Usuarios.FindAsync(asignarSkillDto.EvaluadorId.Value);
                if (evaluador == null)
                {
                    return NotFound(new { message = "Evaluador no encontrado" });
                }
            }

            // 5. Verificar si la asignación ya existe (para evitar duplicados)
            var asignacionExistente = await _context.ColaboradoresSkills
                .FirstOrDefaultAsync(cs => cs.UsuarioId == id && cs.SkillId == asignarSkillDto.SkillId);

            if (asignacionExistente != null)
            {
                // Si ya existe, tal vez solo queramos actualizar el nivel
                asignacionExistente.NivelId = asignarSkillDto.NivelId;
                asignacionExistente.EvaluadorId = asignarSkillDto.EvaluadorId;
                asignacionExistente.FechaEvaluacion = DateTime.Now;
                _context.ColaboradoresSkills.Update(asignacionExistente);
            }
            else
            {
                // Si no existe, creamos la nueva asignación
                var nuevaAsignacion = new ColaboradorSkill
                {
                    UsuarioId = id,
                    SkillId = asignarSkillDto.SkillId,
                    NivelId = asignarSkillDto.NivelId,
                    EvaluadorId = asignarSkillDto.EvaluadorId,
                    FechaEvaluacion = DateTime.Now
                };
                _context.ColaboradoresSkills.Add(nuevaAsignacion);
            }

            // 6. Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill asignado/actualizado correctamente" });
        }

        // -----------------------------------------------------------------

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
