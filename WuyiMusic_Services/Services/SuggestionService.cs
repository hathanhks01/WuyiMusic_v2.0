using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _suggestionRepo;
        public SuggestionService(ISuggestionRepository suggestionRepo)
        {
            _suggestionRepo = suggestionRepo;
        }
        public async Task<Suggestion> AddSuggestion(SuggestionDto suggestionDto)
        {
            return await _suggestionRepo.AddSuggestion(suggestionDto);
        }

        public Task DeleteSuggestion(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllSuggestion()
        {
            return await _suggestionRepo.GetAllSuggestion();
        }

        public async Task<object> GetByIdSuggestion(Guid id)
        {
           return await _suggestionRepo.GetByIdSuggestion(id);
        }

        public async Task<Suggestion> UpdateSuggestion(SuggestionDto suggestionDto)
        {
            return await _suggestionRepo.UpdateSuggestion(suggestionDto);
        }
    }
}
