using System.ComponentModel.DataAnnotations;
namespace Projekti.Models;
#pragma warning disable CS8618
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


public  class Image
{
    [Key]
    public int ImageId { get; set; }
    public string ImagePath { get; set; }

    // [Required]
    // public int UserId { get; set; }
    // Navigation property for related User object
    public int CourseId {get;set;}
    public Course? Course {get;set;}
    // public User? Creator { get; set; }

    [NotMapped]
    public IFormFile UploadImage { get; set;}


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}