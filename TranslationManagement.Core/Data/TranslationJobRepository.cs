using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TranslationManagement.Core.Interfaces;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Core.Data
{
    public class TranslationJobRepository : ITranslationJobRepository
    {
        private readonly AppDbContext _dbContext;

        public TranslationJobRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<TranslationJob?> GetByIdAsync(int id)
        {
            return _dbContext.TranslationJobs
                .Include(req => req.Translator)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public Task<List<TranslationJob>> GetByTranslatorAsync(int translatorId)
        {

            return _dbContext.TranslationJobs
                .Where(j => j.Translator != null && j.Translator.Id == translatorId)
                .Include(req => req.Translator)
                .ToListAsync();
        }

        public Task<List<TranslationJob>> ListAsync()
        {
            return _dbContext.TranslationJobs
                .Include(req => req.Translator)
                .ToListAsync();
        }

        public Task<int> AddAsync(TranslationJob job)
        {
            _dbContext.TranslationJobs.Add(job);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(TranslationJob job)
        {
            _dbContext.Entry(job).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }
    }
}
