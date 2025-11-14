using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("urgencias_vacante")]
    public class UrgenciaVacante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }
    }
}