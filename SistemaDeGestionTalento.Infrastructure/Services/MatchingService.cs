using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Core.DTOs;
using SistemaDeGestionTalento.Core.Interfaces;
using SistemaDeGestionTalento.Core.Entities;
using SistemaDeGestionTalento.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeGestionTalento.Infrastructure.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly SgiDbContext _context;

        public MatchingService(SgiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchResultDto>> ObtenerCandidatosParaVacante(int vacanteId)
        {
            var vacante = await _context.Vacantes
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.Skill)
                .Include(v => v.VacanteSkills)
                    .ThenInclude(vs => vs.NivelSkill)
                .FirstOrDefaultAsync(v => v.Id == vacanteId);

            if (vacante == null) return new List<MatchResultDto>();

            // Filtrar candidatos OpenToWork y que sean Colaboradores
            var candidatos = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.ColaboradorSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(u => u.ColaboradorSkills)
                    .ThenInclude(cs => cs.NivelSkill)
                .Where(u => u.Rol.Nombre == "Colaborador" && u.OpenToWork)
                .ToListAsync();

            var resultados = new List<MatchResultDto>();

            foreach (var candidato in candidatos)
            {
                var score = CalcularScore(vacante, candidato);
                if (score.Total > 0)
                {
                    resultados.Add(new MatchResultDto
                    {
                        UsuarioId = candidato.Id,
                        NombreUsuario = $"{candidato.Nombre} {candidato.Apellido}",
                        Email = candidato.Correo,
                        PuestoActual = candidato.PuestoActual ?? "N/A",
                        PorcentajeCoincidencia = Math.Round(score.Total, 1),
                        ScoreTecnico = Math.Round(score.Tecnico, 1),
                        ScoreHabilidadesBlandas = Math.Round(score.Blando, 1),
                        ScoreSeniority = Math.Round(score.Seniority, 1),
                        SkillsCoincidentes = score.Coincidentes,
                        SkillsFaltantes = score.Faltantes
                    });
                }
            }

            return resultados.OrderByDescending(r => r.PorcentajeCoincidencia);
        }

        private (double Total, double Tecnico, double Blando, double Seniority, List<string> Coincidentes, List<string> Faltantes) CalcularScore(Vacante vacante, Usuario candidato)
        {
            double puntajeTecnico = 0;
            double puntajeBlando = 0;
            double puntajeSeniority = 0;

            var skillsTecnicosReq = vacante.VacanteSkills.Where(s => s.Skill.Categoria == "Tecnica").ToList();
            var skillsBlandosReq = vacante.VacanteSkills.Where(s => s.Skill.Categoria == "Blanda").ToList();

            var coincidentes = new List<string>();
            var faltantes = new List<string>();

            // 1. Skills Técnicos (70%)
            if (skillsTecnicosReq.Any())
            {
                int matches = 0;
                foreach (var req in skillsTecnicosReq)
                {
                    var tieneSkill = candidato.ColaboradorSkills.FirstOrDefault(cs => cs.SkillId == req.SkillId);
                    if (tieneSkill != null)
                    {
                        matches++;
                        // Bonus por Seniority (20% del total)
                        if (tieneSkill.NivelSkill.Orden >= req.NivelSkill.Orden)
                        {
                            puntajeSeniority += (100.0 / skillsTecnicosReq.Count);
                        }
                        coincidentes.Add(req.Skill.Nombre);
                    }
                    else
                    {
                        faltantes.Add(req.Skill.Nombre);
                    }
                }
                puntajeTecnico = (double)matches / skillsTecnicosReq.Count * 100;
            }
            else
            {
                puntajeTecnico = 100; // Si no pide técnicos, tiene el 100% de ese rubro
                puntajeSeniority = 100;
            }

            // 2. Skills Blandos (10%)
            if (skillsBlandosReq.Any())
            {
                int matches = 0;
                foreach (var req in skillsBlandosReq)
                {
                    if (candidato.ColaboradorSkills.Any(cs => cs.SkillId == req.SkillId))
                    {
                        matches++;
                        coincidentes.Add(req.Skill.Nombre);
                    }
                    else
                    {
                        faltantes.Add(req.Skill.Nombre);
                    }
                }
                puntajeBlando = (double)matches / skillsBlandosReq.Count * 100;
            }
            else
            {
                puntajeBlando = 100;
            }

            // Ponderación Final
            // Tecnico 70%, Seniority 20%, Blando 10%
            // Nota: El Seniority se calculó basado en los skills técnicos coincidentes que cumplen el nivel
            double total = (puntajeTecnico * 0.7) + (puntajeSeniority * 0.2) + (puntajeBlando * 0.1);

            return (total, puntajeTecnico, puntajeBlando, puntajeSeniority, coincidentes, faltantes);
        }
    }
}
