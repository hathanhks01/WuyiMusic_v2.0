using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public SuggestionRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Suggestion> AddSuggestion(SuggestionDto suggestionDto)
        {
            var suggestion = new Suggestion
            {
                SuggestionId = Guid.NewGuid(),
                TrackId = suggestionDto.TrackId,
                UserId = suggestionDto.UserId,
            };
            await _context.Suggestions.AddAsync(suggestion);
            _context.SaveChanges();
            return suggestion;
        }

        public Task DeleteSuggestion(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllSuggestion()
        {
            var result = await _context.Suggestions
        .Include(sg => sg.Track)
            .ThenInclude(tr => tr.Album)
        .Include(sg => sg.Track)
            .ThenInclude(tr => tr.Artist)
        .Include(sg => sg.User)
        .Select(sg => new
        {
            sg.SuggestionId,
            Track = sg.Track == null ? null : new
            {
                sg.Track.TrackId,
                sg.Track.Title,
                sg.Track.Duration,
                sg.Track.FilePath,
                Album = sg.Track.Album == null ? null : new
                {
                    sg.Track.Album.AlbumId,
                    sg.Track.Album.Title
                },
                Artist = sg.Track.Artist == null ? null : new
                {
                    sg.Track.Artist.ArtistId,
                    sg.Track.Artist.Name
                }
            },
            User = sg.User == null ? null : new
            {
                sg.User.UserId,
                sg.User.Username,
                sg.User.Email,
            }
        }).ToListAsync();
            return result;
        }

        public async Task<object> GetByIdSuggestion(Guid id)
        {
            var result = await _context.Suggestions.Where(sg => sg.SuggestionId == id).Select(sg => new
            {
                sg.SuggestionId,
                Track = sg.Track == null ? null : new
                {
                    sg.Track.TrackId,
                    sg.Track.Title,
                    sg.Track.Duration,
                    sg.Track.FilePath,
                    Album = sg.Track.Album == null ? null : new
                    {
                        sg.Track.Album.AlbumId,
                        sg.Track.Album.Title
                    },
                    Artist = sg.Track.Artist == null ? null : new
                    {
                        sg.Track.Artist.ArtistId,
                        sg.Track.Artist.Name
                    }
                },
                User = sg.User == null ? null : new
                {
                    sg.User.UserId,
                    sg.User.Username,
                    sg.User.Email,
                }
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Suggestion> UpdateSuggestion(SuggestionDto suggestionDto)
        {
            if (suggestionDto == null) throw new ArgumentNullException(nameof(suggestionDto));

            var existingSuggestion = await _context.Suggestions
                .FirstOrDefaultAsync(sg => sg.SuggestionId == suggestionDto.SuggestionId);

            if (existingSuggestion == null) throw new InvalidOperationException("Suggestion không tồn tại.");

            existingSuggestion.TrackId = suggestionDto.TrackId;
            existingSuggestion.UserId = suggestionDto.UserId;
            await _context.SaveChangesAsync();
            return existingSuggestion;
        }
    }

}
