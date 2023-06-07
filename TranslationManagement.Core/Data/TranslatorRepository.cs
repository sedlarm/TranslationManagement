using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TranslationManagement.Core.Interfaces;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Data
{
    public class TranslatorRepository : ITranslatorRepository
    {
        private readonly AppDbContext _dbContext;

        public TranslatorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Translator?> GetByIdAsync(int id)
        {
            return _dbContext.Translators.FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<List<Translator>> GetByNameAsync(string name)
        {
            return _dbContext.Translators.Where(t => t.Name == name).ToListAsync();
        }

        public Task<List<Translator>> ListAsync()
        {
            return _dbContext.Translators.AsQueryable().ToListAsync();
        }
        public Task<int> AddAsync(Translator translator)
        {
            _dbContext.Translators.Add(translator);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(Translator translator)
        {
            _dbContext.Entry(translator).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }

    }
}
