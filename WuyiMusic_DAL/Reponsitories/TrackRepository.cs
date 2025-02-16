using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using NAudio.Wave;

namespace WuyiMusic_DAL.Reponsitories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public TrackRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            return await _context.Tracks.Include(t => t.Artist).ToListAsync();
        }

        public async Task<Track> GetByIdAsync(Guid id)
        {
            return await _context.Tracks
                .Include(t => t.Artist)
                .FirstOrDefaultAsync(t => t.TrackId == id);
        }


        public async Task AddAsync(Track track, IFormFile file)
        {
            var tempFilePath = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var reader = new AudioFileReader(tempFilePath))
                {
                    var duration = reader.TotalTime;
                    if (duration.Hours > 0)
                    {
                        track.Duration = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            duration.Hours,
                            duration.Minutes,
                            duration.Seconds);
                    }
                    else
                    {
                        track.Duration = string.Format("{0:D2}:{1:D2}",
                            duration.Minutes,
                            duration.Seconds);
                    }
                }

                await _context.Tracks.AddAsync(track);
                await _context.SaveChangesAsync();
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public async Task UpdateAsync(Track track)
        {
            _context.Tracks.Update(track);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var track = await GetByIdAsync(id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Track>> GetFavoriteTracksAsync(Guid userId)
        {
            return await _context.UserFavoriteTracks
                .Where(uft => uft.UserId == userId)
                .Include(uft => uft.Track)
                .ThenInclude(t => t.Artist)
                .Select(uft => uft.Track)
                .ToListAsync();
        }
        public async Task<IEnumerable<Track>> GetRandom6Track(Guid artistId)
        {
            return await _context.Tracks
                .Where(t => t.ArtistId == artistId)
                .OrderBy(t => Guid.NewGuid()) 
                .Take(6)
                .ToListAsync();
        }

        public async Task<List<Track>> GetTrackRankingByListenCount(DateTime startDate, DateTime endDate, int topCount = 10)
        {
            var rankings = await _context.Tracks
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderByDescending(t => t.ListenCount ?? 0)
                .Take(topCount)
                .Select(t => new Track
                {
                    TrackId = t.TrackId,
                    Title = t.Title,
                    Artist = t.Artist,
                    ListenCount = t.ListenCount ?? 0,
                    TrackImage = t.TrackImage
                })
                .ToListAsync();

            return rankings;
        }

        public async Task<SearchResultDto> SearchAsync(string searchTerm)
        {
            // Normalize search term
            searchTerm = searchTerm.ToLower().Trim();

            // Search Tracks
            var tracks = await _context.Tracks
                .Where(t => t.Title.ToLower().Contains(searchTerm) ||
                            t.Artist.Name.ToLower().Contains(searchTerm))
                .Select(t => new TrackDto
                {
                    Title = t.Title,
                    TrackImage = t.TrackImage,
                    AlbumId = t.AlbumId,
                    ArtistId = t.ArtistId
                })
                .Take(10)
                .ToListAsync();

            // Search Artists
            var artists = await _context.Artists
                .Where(a => a.Name.ToLower().Contains(searchTerm))
                .Select(a => new ArtistDto
                {
                    ArtistId = a.ArtistId,
                    Name = a.Name,
                    Bio = a.Bio,
                    ArtistImage = a.ArtistImage,
                })
                .Take(10)
                .ToListAsync();

            return new SearchResultDto
            {
                Tracks = tracks,
                Artists = artists
            };

        }

        public async Task IncrementListenCount(Guid trackId)
        {
            var track = await _context.Tracks.FindAsync(trackId);
            if (track != null)
            {
                track.ListenCount += 1;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Track with ID {trackId} not found.");
            }
        }


        public class TrackListeningStatistic
        {
            public DateTime Hour { get; set; }
            public int ListenCount { get; set; }
        }
    }
}