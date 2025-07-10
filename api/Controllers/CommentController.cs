using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<User> _userManager;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<User> userManager)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comments = await _commentRepository.GetAllAsync();
            var commentsDto = comments.Select(x => x.ToCommentDto());
            return Ok(commentsDto);
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentRequestDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _stockRepository.StockExists(stockId))
            {
                return BadRequest("Stock does not exist!");
            }

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username); //pobieramy użytkownika po username

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.UserId = user.Id;
            await _commentRepository.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, UpdateCommentRequestDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _commentRepository.UpdateAsync(id, commentDto);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _commentRepository.DeleteAsync(id);
            if (response == null)
            {
                return NotFound("Comment does not exist!");
            }

            return NoContent();
        }
    }
}