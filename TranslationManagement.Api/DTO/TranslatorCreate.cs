using System.ComponentModel.DataAnnotations;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Api.DTO
{
    public class TranslatorCreate
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double HourlyRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreditCardNumber { get; set; } = string.Empty;
    }
}
