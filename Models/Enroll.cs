#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projekti.Models;

public class Enroll {
    [Key]
    public int Enrollid { get; set; }
    public int Userid { get; set; }
    public int Courseid { get; set; }
    public User? User { get; set; }
    public Course? Course { get; set; }

}