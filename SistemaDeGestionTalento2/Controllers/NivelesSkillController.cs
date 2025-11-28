using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.Infrastructure.Data;

namespace SistemaDeGestionTalento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NivelesSkillController : ControllerBase
    {
        private readonly SgiDbContext _context;

        public NivelesSkillController(SgiDbContext context)
        {
            _context = context;
        }

        // GET: api/NivelesSkill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NivelSkill>>> GetNivelesSkill()
        {
            return await _context.NivelesSkill.OrderBy(n => n.Orden).ToListAsync();
        }
    }
}
