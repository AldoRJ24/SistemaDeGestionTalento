using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.Infrastructure.Data;
using SistemaDeGestionTalento.DTOs;

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
        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.ColaboradorSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(u => u.ColaboradorSkills)
                    .ThenInclude(cs => cs.NivelSkill)
                .Include(u => u.Certificaciones)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5/opentowork
        [HttpPut("{id}/opentowork")]
        public async Task<IActionResult> UpdateOpenToWork(int id, [FromBody] OpenToWorkDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.OpenToWork = dto.OpenToWork;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Estado OpenToWork actualizado", openToWork = usuario.OpenToWork });
        }

        // POST: api/Usuarios/5/certificaciones
        [HttpPost("{id}/certificaciones")]
        public async Task<IActionResult> AddCertificacion(int id, [FromBody] CertificacionDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var certificacion = new Certificacion
            {
                UsuarioId = id,
                Nombre = dto.Nombre,
                Entidad = dto.Entidad,
                Url = dto.Url,
                FechaObtencion = dto.FechaObtencion
            };

            _context.Certificaciones.Add(certificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Certificación agregada", certificacion });
        }

        // PUT: api/Usuarios/5/certificaciones/1
        [HttpPut("{id}/certificaciones/{certId}")]
        public async Task<IActionResult> UpdateCertificacion(int id, int certId, [FromBody] CertificacionDto dto)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(c => c.Id == certId && c.UsuarioId == id);
            if (certificacion == null) return NotFound();

            certificacion.Nombre = dto.Nombre;
            certificacion.Entidad = dto.Entidad;
            certificacion.Url = dto.Url;
            certificacion.FechaObtencion = dto.FechaObtencion;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Certificación actualizada", certificacion });
        }

        // DELETE: api/Usuarios/5/certificaciones/1
        [HttpDelete("{id}/certificaciones/{certId}")]
        public async Task<IActionResult> DeleteCertificacion(int id, int certId)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(c => c.Id == certId && c.UsuarioId == id);
            if (certificacion == null) return NotFound();

            _context.Certificaciones.Remove(certificacion);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Certificación eliminada" });
        }

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

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserProfileDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nombre = dto.Nombre ?? usuario.Nombre;
            usuario.Apellido = dto.Apellido ?? usuario.Apellido;
            usuario.PuestoActual = dto.PuestoActual;
            usuario.DescripcionPerfil = dto.DescripcionPerfil;
            usuario.FotoPerfil = dto.FotoPerfil;
            usuario.UpdatedBy = usuario.Correo; // Audit
            usuario.Timestamp = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Perfil actualizado", usuario });
        }

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
