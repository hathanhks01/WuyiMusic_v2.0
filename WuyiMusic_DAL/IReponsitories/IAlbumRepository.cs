using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<object>> GetAllAlbum();
        Task<object> GetByIdAlbum(Guid id);
        Task<Album> AddComment(AlbumDto albumDto);
        Task<Album> UpdateAlbum(AlbumDto albumDto);
        Task DeleteAlbum(Guid id);
    }
}
