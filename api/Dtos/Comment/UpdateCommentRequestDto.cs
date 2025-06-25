using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class UpdateCommentRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Title must be 3 characters long.")]
        [MaxLength(200, ErrorMessage = "Title cannot be over 200 characters long.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Content must be 5 characters long.")]
        [MaxLength(500, ErrorMessage = "Content must be 500 characters long.")]
        public string Content { get; set; } = string.Empty;
    }
}