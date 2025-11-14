using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("kpi_logs")]
    public class KpiLog
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? NombreKpi { get; set; }

        [Column(TypeName = "numeric(10, 2)")]
        public decimal? Valor { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}