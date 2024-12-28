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
    public class RatingRepository : IRatingRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public RatingRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Rating> AddRating(RatingDto ratingDto)
        {
            var rating = new Rating
            {
                RatingId = Guid.NewGuid(),
                TrackId = ratingDto.TrackId,
                UserId = ratingDto.UserId,
                Score = ratingDto.Score,
            };
            await _context.Ratings.AddAsync(rating);
            _context.SaveChanges();
            return rating;
        }

        public Task DeleteRating(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllRating()
        {
            var result = await _context.Ratings
        .Include(rt => rt.Track)
            .ThenInclude(tr => tr.Album)
        .Include(rt => rt.Track)
            .ThenInclude(tr => tr.Artist)
        .Include(rt => rt.User)
        .Select(rt => new
        {
            rt.RatingId,
            rt.Score,
            Track = rt.Track == null ? null : new
            {
                rt.Track.TrackId,
                rt.Track.Title,
                rt.Track.Duration,
                rt.Track.FilePath,
                Album = rt.Track.Album == null ? null : new
                {
                    rt.Track.Album.AlbumId,
                    rt.Track.Album.Title
                },
                Artist = rt.Track.Artist == null ? null : new
                {
                    rt.Track.Artist.ArtistId,
                    rt.Track.Artist.Name
                }
            },
            User = rt.User == null ? null : new
            {
                rt.User.UserId,
                rt.User.Username,
                rt.User.Email,
            }
        }).ToListAsync();
            return result;
        }

        public async Task<object> GetByIdRating(Guid id)
        {
            var result = await _context.Ratings.Where(rt => rt.RatingId == id).Select(rt => new
            {
                rt.RatingId,
                rt.Score,
                Track = rt.Track == null ? null : new
                {
                    rt.Track.TrackId,
                    rt.Track.Title,
                    rt.Track.Duration,
                    rt.Track.FilePath,
                    Album = rt.Track.Album == null ? null : new
                    {
                        rt.Track.Album.AlbumId,
                        rt.Track.Album.Title
                    },
                    Artist = rt.Track.Artist == null ? null : new
                    {
                        rt.Track.Artist.ArtistId,
                        rt.Track.Artist.Name
                    }
                },
                User = rt.User == null ? null : new
                {
                    rt.User.UserId,
                    rt.User.Username,
                    rt.User.Email,
                }
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Rating> UpdateRating(RatingDto ratingDto)
        {
            if (ratingDto == null) throw new ArgumentNullException(nameof(ratingDto));

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(rt => rt.RatingId == ratingDto.RatingId);

            if (existingRating == null) throw new InvalidOperationException("Rating không tồn tại.");

            existingRating.TrackId = ratingDto.TrackId;
            existingRating.UserId = ratingDto.UserId;
            existingRating.Score = ratingDto.Score;
            await _context.SaveChangesAsync();
            return existingRating;
        }
    }

}
