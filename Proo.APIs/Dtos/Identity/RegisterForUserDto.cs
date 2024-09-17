using Proo.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class RegisterForUserDto
    {
        [Required]
        [MaxLength(100 , ErrorMessage = "The max length is 100 char")]
        [MinLength(5 , ErrorMessage = "The min length is 5 char")]
        public string FullName { get; set; }

        public string Role { get; set; }
        [Required]
        public Gender Gender { get; set; }
       


    }
    
}
