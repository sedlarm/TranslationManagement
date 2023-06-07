using TranslationManagement.Core.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranslationManagement.Core.Models
{
    public class Translator : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public double HourlyRate { get; set; }
        public string Status { get; set; } = TranslatorStatus.Applicant;
        public string? CreditCardNumber { get; set; }
    }
}
