
using System.ComponentModel.DataAnnotations;

namespace TranslationManagement.Core.Models.DTO
{
    public class TranslationJobUpdate
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = TranslationJobStatus.New;
        [Required]
        public string OriginalContent { get; set; } = string.Empty;
        [Required]
        public string TranslatedContent { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; } = 0.0;
    }
}
