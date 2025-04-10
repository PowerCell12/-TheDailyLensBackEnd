using System.ComponentModel.DataAnnotations;

namespace server.Models.UserModels;

public class EditProfiileModel{

    public string? UserName {get; set;}

    public string? FullName {get;set;}

    public string? Country {get; set;}

    [Required]
    [EmailAddress]
    public string Email {get; set;}

    [MaxLength(250)]
    public string? Bio {get; set;}

}