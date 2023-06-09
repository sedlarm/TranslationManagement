
using System.ComponentModel.DataAnnotations;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Api.DTO
{
    public class TranslationJobUpdate
    {
        public int? TranslatorId { get; set; }
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
