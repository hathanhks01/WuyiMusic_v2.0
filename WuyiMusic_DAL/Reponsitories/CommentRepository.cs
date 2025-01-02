using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Reponsitories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly WuyiMusic_DbContext _context;

        public CommentRepository(WuyiMusic_DbContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddComment(CommentDto commentDto)
        {
            var comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                TrackId = commentDto.TrackId,
                UserId = commentDto.UserId,
                Content = commentDto.Content,
                CreatedAt = DateTime.Now,
            };
            await _context.Comments.AddAsync(comment);
            _context.SaveChanges();
            return comment;
        }

        public Task DeleteComment(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllComment()
        {
            var result = await _context.Comments
        .Include(cm => cm.Track)
            .ThenInclude(tr => tr.Album)
        .Include(cm => cm.Track)
            .ThenInclude(tr => tr.Artist)
        .Include(cm => cm.User)
        .Select(cm => new
        {
            cm.CommentId,
            cm.Content,
            cm.CreatedAt,
            Track = cm.Track == null ? null : new
            {
                cm.Track.TrackId,
                cm.Track.Title,
                cm.Track.Duration,
                cm.Track.FilePath,
                Album = cm.Track.Album == null ? null : new
                {
                    cm.Track.Album.AlbumId,
                    cm.Track.Album.Title
                },
                Artist = cm.Track.Artist == null ? null : new
                {
                    cm.Track.Artist.ArtistId,
                    cm.Track.Artist.Name
                }
            },
            User = cm.User == null ? null : new
            {
                cm.User.UserId,
                cm.User.Username,
                cm.User.Email,
            }
        }).ToListAsync();
            return result;
        }

        public async Task<object> GetByIdComment(Guid id)
        {
            var result = await _context.Comments.Where(cm => cm.CommentId == id).Select(cm => new
            {
                cm.CommentId,
                cm.Content,
                cm.CreatedAt,
                Track = cm.Track == null ? null : new
                {
                    cm.Track.TrackId,
                    cm.Track.Title,
                    cm.Track.Duration,
                    Album = cm.Track.Album == null ? null : new
                    {
                        cm.Track.Album.AlbumId,
                        cm.Track.Album.Title
                    },
                    Artist = cm.Track.Artist == null ? null : new
                    {
                        cm.Track.Artist.ArtistId,
                        cm.Track.Artist.Name
                    }
                },
                User = cm.User == null ? null : new
                {
                    cm.User.UserId,
                    cm.User.Username,
                    cm.User.Email
                }
            }).FirstOrDefaultAsync();         
            return result;
        }

        public async Task<Comment> UpdateComment(CommentDto commentDto)
        {
            if (commentDto == null) throw new ArgumentNullException(nameof(commentDto));

            var existingComment = await _context.Comments
                .FirstOrDefaultAsync(cm => cm.CommentId == commentDto.CommentId);

            if (existingComment == null) throw new InvalidOperationException("Comment không tồn tại.");

            existingComment.TrackId = commentDto.TrackId;
            existingComment.UserId = commentDto.UserId;
            existingComment.Content = commentDto.Content;
            existingComment.CreatedAt = commentDto.CreatedAt;
            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}
