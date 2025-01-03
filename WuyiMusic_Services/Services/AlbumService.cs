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
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepo;   
        public AlbumService(IAlbumRepository albumRepo) 
        { 
            _albumRepo = albumRepo;
        }

        public async Task<Album> AddAlbum(AlbumDto albumDto)
        {
            return await _albumRepo.AddAlbum(albumDto);
        }

        public Task DeleteAlbum(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllAlbum()
        {
            return await _albumRepo.GetAllAlbum();
        }

        public async Task<object> GetByIdAlbum(Guid id)
        {
            return await _albumRepo.GetByIdAlbum(id);
        }

        public async Task<Album> UpdateAlbum(AlbumDto albumDto)
        {
            return await _albumRepo.UpdateAlbum(albumDto);
        }
    }
}
