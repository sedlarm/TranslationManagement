using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Interfaces
{
    public interface ITranslationJobRepository
    {
        Task<TranslationJob?> GetByIdAsync(int id);
        Task<List<TranslationJob>> GetByTranslatorAsync(int translatorId);
        Task<List<TranslationJob>> ListAsync();
        Task<int> AddAsync(TranslationJob job);
        Task<int> UpdateAsync(TranslationJob job);
        Task<int> DeleteAsync(TranslationJob job);
    }
}
