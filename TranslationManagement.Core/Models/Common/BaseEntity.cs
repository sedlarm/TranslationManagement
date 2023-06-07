using System.ComponentModel.DataAnnotations;

namespace TranslationManagement.Core.Models.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
