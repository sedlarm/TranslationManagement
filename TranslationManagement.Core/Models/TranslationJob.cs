using TranslationManagement.Core.Models.Common;

namespace TranslationManagement.Core.Models
{
    public class TranslationJob : BaseEntity
    {
        public TranslationJob(Translator translator, string customerName) : this()
        {
            Translator = translator;
            CustomerName = customerName;
        }

        public TranslationJob() 
        {
        }

        public Translator? Translator { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = TranslationJobStatus.New;
        public string OriginalContent { get; set; } = string.Empty;
        public string TranslatedContent { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;

        public bool ChangeStatus(string newStatus)
        {
            if ((Status == TranslationJobStatus.New && newStatus == TranslationJobStatus.Completed) ||
                (Status == TranslationJobStatus.Completed || newStatus == TranslationJobStatus.New)) 
            {
                Status = newStatus;

                return true;
            }

            return false;
        }

        public void UpdatePriceByPricePerCharacter(double pricePerCharacter)
        {
            Price = pricePerCharacter * OriginalContent.Length;
        }
    }
}
