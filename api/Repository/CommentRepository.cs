using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;

        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var response = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (response == null)
            {
                return null;
            }

            _context.Comments.Remove(response);
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentRequestDto commentDto)
        {
            var response = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (response == null)
            {
                return null;
            }

            _context.Entry(response).CurrentValues.SetValues(commentDto);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }
    }
}