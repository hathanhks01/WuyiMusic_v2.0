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
    public class AlbumRepository : IAlbumRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public AlbumRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Album> AddAlbum(Album album)
        {   
            await _context.Albums.AddAsync(album);
            _context.SaveChanges();
            return album;
        }
        public async Task DeleteAlbum(Guid id)
        {
            var album = await _context.Albums
                                      .Include(a => a.Tracks)
                                      .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album == null)
            {
                throw new Exception("Album không tồn tại.");
            }

            if (album.Tracks != null && album.Tracks.Any())
            {
                _context.Tracks.RemoveRange(album.Tracks);
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Album>> GetAllAlbum()
        {
            var result = await _context.Albums
                .Include(a => a.Tracks)
                .Include(a => a.Artist)
                .ToListAsync();
            return result;
        }

        public async Task<object> GetByIdAlbum(Guid id)
        {
            var result = await _context.Albums.Where(alb => alb.AlbumId == id).Select(alb => new
            {
                alb.AlbumId,
                alb.Title,
                alb.ReleaseDate,

                Artist = alb.Artist == null ? null : new
                {
                    alb.Artist.ArtistId,
                    alb.Artist.Name
                }
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Album> UpdateAlbum(AlbumDto albumDto)
        {
            if (albumDto == null) throw new ArgumentNullException(nameof(albumDto));

            var existingAlbum = await _context.Albums
                .FirstOrDefaultAsync(alb => alb.AlbumId == albumDto.AlbumId);

            if (existingAlbum == null) throw new InvalidOperationException("Album không tồn tại.");

            existingAlbum.ArtistId = albumDto.ArtistId;
            existingAlbum.Title = albumDto.Title;
            existingAlbum.ReleaseDate = albumDto.ReleaseDate;
            await _context.SaveChangesAsync();
            return existingAlbum;
        }
    }

}
