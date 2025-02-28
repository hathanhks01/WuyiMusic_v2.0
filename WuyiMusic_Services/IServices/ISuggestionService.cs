using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface ISuggestionService
    {
        Task<IEnumerable<object>> GetAllSuggestion();
        Task<object> GetByIdSuggestion(Guid id);
        Task<Suggestion> AddSuggestion(SuggestionDto suggestionDto);
        Task<Suggestion> UpdateSuggestion(SuggestionDto suggestionDto);
        Task DeleteSuggestion(Guid id);
    }
}
