using System.ComponentModel.DataAnnotations;

namespace TechMoveGLMS.Models
{
    public class ContractFile
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        public Contract Contract { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
