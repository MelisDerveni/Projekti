#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projekti.Models;

public class Course {
    [Key]
    public int Courseid{get;set;}
    [Required]
    public string CourseDescription{get;set;}
    [Required]
    public string CourseName{get;set;}
    public List<Enroll> Users_Who_Enroll { get; set; } = new List<Enroll>();
    public List<Image> Images {get;set;} = new List<Image>();
    [NotMapped]
    [Required]
    public IFormFile UploadImage { get; set;}

}