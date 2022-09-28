#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projekti.Models;
public class User {
    [Key]
    public int id {get; set;}
    [Required]
    [MinLength(2)]
    public string Username {get; set;}
    [Required]
    [EmailAddress]
    
    public string EmailAddress  {get; set;}
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string password {get; set;}

    public List<Enroll> Courses_I_Enroll { get; set; } = new List<Enroll>();


    [NotMapped]
    [Required]
    [Compare("password")]
    [DataType(DataType.Password)]
    public string Confirm { get; set; } 
    public DateTime create_time {get; set;}

}
public class LoginUser {
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } 

}