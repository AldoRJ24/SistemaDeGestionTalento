using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("matching")]
    public class Matching
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Vacante")]
        public int VacanteId { get; set; }
        public virtual Vacante Vacante { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public int Porcentaje { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}