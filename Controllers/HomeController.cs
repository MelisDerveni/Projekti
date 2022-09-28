
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projekti.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Projekti.Controllers;
    
public class HomeController : Controller
{
    private MyContext _context;
    private readonly IWebHostEnvironment WebHostEnvironment;
    
    public HomeController(MyContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        WebHostEnvironment = webHostEnvironment;
    }
    
    [HttpGet("")]
    public IActionResult Index()
    {
        

        return View();
    }
    [HttpGet("Index")]
    public IActionResult ReturnIndex()
    {
        return RedirectToAction("Index");
    }
    [HttpGet ("AboutUs")]
    public IActionResult AboutUs()
    {
        return View();
    }
    [HttpGet("Services")]
    public IActionResult Services()
    {
        return View();
    }

    [HttpGet("ContactUs")]
    public IActionResult ContactUs()
    {
        return View();
    }
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("Login")]
    public IActionResult LoginUser(LoginUser UserSubmission)
    {
        if(ModelState.IsValid)
    {
        // If initial ModelState is valid, query for a user with provided email
        var userInDb = _context.Users.FirstOrDefault(u => u.EmailAddress == UserSubmission.EmailAddress);
        // If no user exists with provided email
        if(userInDb == null)
        {
            // Add an error to ModelState and return to View!
            ModelState.AddModelError("EmailAddress", "Invalid Email/Password");
            return View("Login");
        }
            
        // Initialize hasher object
        var hasher = new PasswordHasher<LoginUser>();
        // verify provided password against hash stored in db
        var result = hasher.VerifyHashedPassword(UserSubmission, userInDb.password, UserSubmission.Password);
        
        // result can be compared to 0 for failure
        if(result == 0)
        {
            // handle failure (this should be similar to how "existing email" is handled)
            ModelState.AddModelError("password", "Invalid Password");
            return View("Login");
        }
        else
        {
        HttpContext.Session.SetInt32("id", userInDb.id);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
        }
    }
    else{
    return RedirectToAction("Login");
    }
        
    }

    [HttpGet ("Register")]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost("Register")]
    public IActionResult Register(User CurrentUser)
    {
        
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(u=>u.EmailAddress == CurrentUser.EmailAddress)){
                ModelState.AddModelError("EmailAddress", "Email already in use!");
                return View("Register");
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            CurrentUser.password = Hasher.HashPassword(CurrentUser, CurrentUser.password);
            _context.Users.Add(CurrentUser);
            
            
            _context.SaveChanges();
            User FromDb = _context.Users.First(c=>c.EmailAddress == CurrentUser.EmailAddress );
            HttpContext.Session.SetInt32("id", FromDb.id);
            

            return RedirectToAction("Dashboard");
            

        }
        else{
            return View("Register");
        }
        
    }





    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        ViewBag.LoggedInUser = _context.Users.First(c=>c.id == HttpContext.Session.GetInt32("id"));
        ViewBag.AllCourses = _context.Courses.ToList();
        ViewBag.Images = _context.Images.Include(c=>c.Course).Where(c=>c.CourseId == c.ImageId).ToList();
        return View();
    }

    [HttpGet("CreateCourse")]
    public IActionResult CreateCourse()
    {
        return View();
    }
    [HttpPost("CreateCourse")]
    public IActionResult NewCourse(Course FromView)
    {

        {
        _context.Courses.Add(FromView);
        _context.SaveChanges();

        string StringFileName = UploadFile(FromView);
        var image = new Image( ){
            ImagePath = StringFileName,
            // UserId = (int)HttpContext.Session.GetInt32("id"),
            CourseId = FromView.Courseid

        
        };
        _context.Images.Add(image);
        _context.SaveChanges();
        if(ModelState.IsValid)
            return RedirectToAction("Dashboard");
        }

        return View("CreateCourse");
    }
    private string UploadFile(Course marrNgaView )
    {

        
    string fileName = null;
    if(marrNgaView.UploadImage!=null){
        string Uploaddir = Path.Combine(WebHostEnvironment.WebRootPath,"Images");
        fileName = Guid.NewGuid().ToString() + "-" + marrNgaView.UploadImage.FileName;
        string filePath = Path.Combine(Uploaddir,fileName);
        using (var filestream = new FileStream(filePath,FileMode.Create))
        {
                marrNgaView.UploadImage.CopyTo(filestream);
        }
    }
    return fileName;
    }

    [HttpGet("Course/{id}")]
    public IActionResult DisplayCourse(int id)
    {   
        int SessionId = (int)HttpContext.Session.GetInt32("id");
        ViewBag.DisplayCourse = _context.Courses.First(c=>c.Courseid == id);
        ViewBag.Images = _context.Images.Include(c=>c.Course).Where(c=>c.CourseId == c.ImageId).ToList();
        ViewBag.Enrolls = _context.Enrolls.Include(c=>c.User).Include(c=>c.Course).Where(c=>(c.Courseid == id) && (c.Userid == (int)HttpContext.Session.GetInt32("id")));
        ViewBag.IsStudnet= _context.Courses.Include(c=>c.Users_Who_Enroll).Where(e=>e.Courseid== id).Where(e=> e.Users_Who_Enroll.Any(e=>e.Userid == SessionId) == true).Any();
        Console.WriteLine(ViewBag.IsStudnet);
        return View();
    }
    [HttpGet("Course/AddUserToCourse/{id}")]
    public IActionResult AddUser( int id)
    {
        int SessionId = (int)HttpContext.Session.GetInt32("id");
        User AddtoCourse = _context.Users.First(c=>c.id == SessionId);
        if(_context.Enrolls.Include(c=>c.User).Include(c=>c.Course).Where(c=>(c.Courseid == id) && (c.Userid == (int)HttpContext.Session.GetInt32("id"))).Count() == 0)
        {
            Enroll NewEnroll = new Enroll{
            Userid = SessionId,
            Courseid = id
            };
        _context.Enrolls.Add(NewEnroll);
        _context.SaveChanges();
        }


        return RedirectToAction("Dashboard");
    }
    [HttpGet("Course/Leave/{id}")]
    
    public IActionResult Leave(int id)
    {
        int SessionId = (int)HttpContext.Session.GetInt32("id");
        Enroll ThisEnroll = _context.Enrolls.Include(c=>c.User).Include(c=>c.Course).First(c=>(c.Courseid == id) && (c.Userid == (int)HttpContext.Session.GetInt32("id")));
        _context.Remove(ThisEnroll);
        _context.SaveChanges();
        return RedirectToAction("DashBoard");
    }
}

