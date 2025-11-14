using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeGestionTalento.Models
{
    [Table("roles")]
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        // Propiedad de navegación: Un rol tiene muchos usuarios
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}