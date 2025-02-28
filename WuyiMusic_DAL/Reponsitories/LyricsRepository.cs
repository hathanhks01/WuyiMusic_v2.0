using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class LyricsRepository : ILyricsRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public LyricsRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Lyrics> AddLyrics(LyricsDto lyricsDto)
        {
            var lyrics = new Lyrics
            {
                LyricsId = Guid.NewGuid(),
                TrackId = lyricsDto.TrackId,
                Content = lyricsDto.Content,
            };
            await _context.Lyrics.AddAsync(lyrics);
            _context.SaveChanges();
            return lyrics;
        }

        public Task DeleteLyrics(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllLyrics()
        {
            var result = await _context.Lyrics
            .Include(lr => lr.Track)
                .ThenInclude(tr => tr.Album)
            .Include(lr => lr.Track)
                .ThenInclude(tr => tr.Artist)
            .Select(lr => new
            {
                lr.LyricsId,
                lr.Content,
                Track = lr.Track == null ? null : new
                {
                    lr.Track.TrackId,
                    lr.Track.Title,
                    lr.Track.Duration,
                    lr.Track.FilePath,
                    Album = lr.Track.Album == null ? null : new
                    {
                        lr.Track.Album.AlbumId,
                        lr.Track.Album.Title
                    },
                    Artist = lr.Track.Artist == null ? null : new
                    {
                        lr.Track.Artist.ArtistId,
                        lr.Track.Artist.Name
                    }
                }
            }).ToListAsync();

            return result;
        }

        public async Task<object> GetByIdLyrics(Guid id)
        {
            var result = await _context.Lyrics.Where(lr => lr.LyricsId == id).Select(lr => new
            {
                lr.LyricsId,
                lr.Content,
                Track = lr.Track == null ? null : new
                {
                    lr.Track.TrackId,
                    lr.Track.Title,
                    lr.Track.Duration
                },               
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Lyrics> UpdateLyrics(LyricsDto lyricsDto)
        {
            if (lyricsDto == null) throw new ArgumentNullException(nameof(lyricsDto));

            var existingLyrics = await _context.Lyrics
                .FirstOrDefaultAsync(lr => lr.LyricsId == lyricsDto.LyricsId);

            if (existingLyrics == null) throw new InvalidOperationException("Comment không tồn tại.");

            existingLyrics.TrackId = lyricsDto.TrackId;
            existingLyrics.Content = lyricsDto.Content;
            await _context.SaveChangesAsync();
            return existingLyrics;
        }
    }

}
