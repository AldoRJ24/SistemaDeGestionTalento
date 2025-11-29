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
                        SkillsFaltantes = score.Faltantes,
                        SkillDetails = score.Details
                    });
                }
            }

            return resultados.OrderByDescending(r => r.PorcentajeCoincidencia);
        }

        private (double Total, double Tecnico, double Blando, double Seniority, List<string> Coincidentes, List<string> Faltantes, List<SkillMatchDetailDto> Details) CalcularScore(Vacante vacante, Usuario candidato)
        {
            double puntajeTecnico = 0;
            double puntajeBlando = 0;
            double puntajeSeniority = 0;

            var skillsTecnicosReq = vacante.VacanteSkills.Where(s => s.Skill.Categoria == "Tecnica").ToList();
            var skillsBlandosReq = vacante.VacanteSkills.Where(s => s.Skill.Categoria == "Blanda").ToList();

            var coincidentes = new List<string>();
            var faltantes = new List<string>();
            var details = new List<SkillMatchDetailDto>();

            // Helper para procesar skills
            void ProcesarSkill(VacanteSkill req, bool esTecnico)
            {
                var tieneSkill = candidato.ColaboradorSkills.FirstOrDefault(cs => cs.SkillId == req.SkillId);
                var detail = new SkillMatchDetailDto
                {
                    SkillNombre = req.Skill.Nombre,
                    NivelRequerido = req.NivelSkill.Nombre
                };

                if (tieneSkill != null)
                {
                    detail.NivelCandidato = tieneSkill.NivelSkill.Nombre;
                    coincidentes.Add(req.Skill.Nombre);

                    // Calcular porcentaje
                    if (tieneSkill.NivelSkill.Orden >= req.NivelSkill.Orden)
                    {
                        detail.Porcentaje = 100;
                        detail.Color = "green";
                        if (esTecnico) puntajeSeniority += (100.0 / skillsTecnicosReq.Count);
                    }
                    else
                    {
                        // Regla de 3 simple basada en el orden
                        // Ejemplo: Req=3 (Avanzado), Tiene=1 (Básico) -> 1/3 = 33%
                        detail.Porcentaje = Math.Round(((double)tieneSkill.NivelSkill.Orden / req.NivelSkill.Orden) * 100, 0);
                        
                        if (detail.Porcentaje >= 66) detail.Color = "green"; // Casi cumple
                        else if (detail.Porcentaje >= 33) detail.Color = "orange"; // Parcial
                        else detail.Color = "red"; // Muy bajo
                    }
                }
                else
                {
                    detail.NivelCandidato = "Ninguno";
                    detail.Porcentaje = 0;
                    detail.Color = "red";
                    faltantes.Add(req.Skill.Nombre);
                }
                details.Add(detail);
            }

            // 1. Procesar todos los skills (Técnicos y Blandos juntos)
            var allSkillsReq = vacante.VacanteSkills.ToList();
            double sumPercentages = 0;

            if (allSkillsReq.Any())
            {
                foreach (var req in allSkillsReq)
                {
                    // Determinar si es técnico solo para estadísticas internas si se requiere, 
                    // pero para el total usamos la misma lógica.
                    bool esTecnico = req.Skill.Categoria == "Tecnica";
                    ProcesarSkill(req, esTecnico);
                }

                // Sumar los porcentajes calculados en ProcesarSkill
                sumPercentages = details.Sum(d => d.Porcentaje);
            }
            else
            {
                // Si no hay skills requeridos, asumimos 100% de coincidencia (o 0%, según negocio, pero 100 es más amigable si no hay requisitos)
                return (100, 100, 100, 100, coincidentes, faltantes, details);
            }

            // Cálculo Final: Promedio Simple
            double total = sumPercentages / allSkillsReq.Count;

            // Mantener valores legacy por si el frontend los usa, aunque ya no sean weighted
            puntajeTecnico = total; 
            puntajeBlando = total;
            puntajeSeniority = total;

            return (total, puntajeTecnico, puntajeBlando, puntajeSeniority, coincidentes, faltantes, details);
        }
    }
}
