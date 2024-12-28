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
    public class ArtistRepository : IArtistRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public ArtistRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Artist> AddArtist(ArtistDto artistDto)
        {
            var artist = new Artist
            {
                ArtistId = Guid.NewGuid(),
                Name = artistDto.Name,
                Bio = artistDto.Bio,
                ArtistImage = artistDto.ArtistImage,
                CreatedAt = DateTime.Now,
            };
            await _context.Artists.AddAsync(artist);
            _context.SaveChanges();
            return artist;
        }

        public Task DeleteArtist(Guid id)
        {
            throw new NotImplementedException();
        }

        public  async Task<IEnumerable<object>> GetAllArtist()
        {
            return await _context.Artists.ToListAsync();  
        }

        public async Task<object> GetByIdArtist(Guid id)
        {
            return await _context.Artists.FirstOrDefaultAsync(x => x.ArtistId == id); 
        }

        public async Task<Artist> UpdateArtist(ArtistDto artistDto)
        {
            if (artistDto == null) throw new ArgumentNullException(nameof(artistDto));

            var existingArtists = await _context.Artists
                .FirstOrDefaultAsync(cm => cm.ArtistId == artistDto.ArtistId);

            if (existingArtists == null) throw new InvalidOperationException("Artists không tồn tại.");

            existingArtists.Name = artistDto.Name;
            existingArtists.Bio = artistDto.Bio;
            existingArtists.ArtistImage = artistDto.ArtistImage;
            existingArtists.CreatedAt = artistDto.CreatedAt;
            await _context.SaveChangesAsync();
            return existingArtists;
        }
    }
}
