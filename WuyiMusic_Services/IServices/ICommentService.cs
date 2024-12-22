using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_Services.IServices
{
    public interface ICommentService
    {
        Task<IEnumerable<object>> GetAllComment();
        Task<object> GetByIdComment(Guid id);
        Task<Comment> AddComment(CommentDto commentDto);
        Task<Comment> UpdateComment(CommentDto commentDto);
        Task DeleteComment(Guid id);
    }
}
