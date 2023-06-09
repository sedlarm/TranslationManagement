using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Interfaces
{
    public interface ITranslatorRepository
    {
        Task<Translator?> GetByIdAsync(int id);
        Task<List<Translator>> GetByNameAsync(string name);
        Task<List<Translator>> ListAsync();
        Task<int> AddAsync(Translator translator);
        Task<int> UpdateAsync(Translator translator);
    }
}
