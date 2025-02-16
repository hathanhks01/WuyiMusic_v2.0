using Azure.Messaging;
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
        public async Task<Artist?> GetArtistByUserIdAsync(Guid userId)
        {
            return await _context.Artists
                .Include(a => a.Tracks) 
                .Include(a => a.Albums) 
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }
        public async Task<Artist> AddArtist(ArtistDto artistDto, Guid userId)
        {
            var artist = new Artist
            {
                ArtistId = userId,
                Name = artistDto.Name,
                Bio = artistDto.Bio,
                UserId=userId,
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
           var ArtistExist = _context.Artists.Find(id);
            if (ArtistExist==null)
            {
                throw new Exception("Artist does not exist");
            }
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
            await _context.SaveChangesAsync();
            return existingArtists;
        }
        private static List<Artist> _previouslySelectedArtists = new List<Artist>();

        public async Task<IEnumerable<Artist>> GetRandomArtistsAsync()
        {
            int count = 5;
            var artists = await _context.Artists.Where(a => a.IsVerified == true).ToListAsync();
            artists = artists.Except(_previouslySelectedArtists).ToList();
            if (artists.Count < count)
            {
                count = artists.Count; 
            }

            Random random = new Random();

            var selectedArtists = artists.OrderBy(x => random.Next()).Take(count).ToList();

            // Cập nhật danh sách artist đã chọn
            _previouslySelectedArtists.AddRange(selectedArtists);

            return selectedArtists;
        }


    }
}
