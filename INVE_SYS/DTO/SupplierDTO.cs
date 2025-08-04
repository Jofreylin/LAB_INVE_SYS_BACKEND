using System;
using System.ComponentModel.DataAnnotations;

namespace INVE_SYS.DTO
{
    public class SupplierDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(300, ErrorMessage = "El nombre no puede exceder los 300 caracteres")]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }

    public class SupplierResponse
    {
        public int SupplierId { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
