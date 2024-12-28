using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.IReponsitories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<object>> GetAllArtist();
        Task<object> GetByIdArtist(Guid id);
        Task<Artist> AddArtist(ArtistDto artistDto);
        Task<Artist> UpdateArtist(ArtistDto artistDto);
        Task DeleteArtist(Guid id);
    }
}
