using System.ComponentModel.DataAnnotations;

namespace TechMoveGLMS.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        public Contract Contract { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000)]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        [Display(Name = "Cost (ZAR)")]
        public decimal CostZAR { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
