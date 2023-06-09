
using System.ComponentModel.DataAnnotations;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Api.DTO
{
    public class TranslationJobCreate
    {
        public int? TranslatorId { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = TranslationJobStatus.New;
        [Required]
        public string OriginalContent { get; set; } = string.Empty;
        public string TranslatedContent { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;
    }
}
