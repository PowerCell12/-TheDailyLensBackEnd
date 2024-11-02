using System.ComponentModel.DataAnnotations;

namespace server.Models.AuthModels;


public class AuthFormModel{
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

}