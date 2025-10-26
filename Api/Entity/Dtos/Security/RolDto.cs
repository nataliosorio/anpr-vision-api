using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Security
{
    public class RolDto : GenericDto
    {
        [Required(ErrorMessage = "La descripción del rol es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        //public string Description { get; set; }
        public string Description { get; set; } = null!;

    }
}
