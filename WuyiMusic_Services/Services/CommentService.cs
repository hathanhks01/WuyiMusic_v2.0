using AutoMapper;
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
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IMapper _mapper;
        public CommentService(ICommentRepository commentRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _mapper = mapper;
        }
        public async Task<Comment> AddComment(CommentDto commentDto)
        {
            return await _commentRepo.AddComment(commentDto);
        }

        public Task DeleteComment(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllComment()
        {
            return await _commentRepo.GetAllComment();
        }

        public async Task<object> GetByIdComment(Guid id)
        {
            return await _commentRepo.GetByIdComment(id);
        }

        public async Task<Comment> UpdateComment(CommentDto commentDto)
        {
            return await _commentRepo.UpdateComment(commentDto);    
        }
    }
}
