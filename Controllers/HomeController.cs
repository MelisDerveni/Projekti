
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projekti.Models;
namespace Monsters.Controllers;
    
public class HomeController : Controller
{
    private MyContext _context;
    
    
    public HomeController(MyContext context)
    {
        _context = context;
    }
    
    [HttpGet("")]
    public IActionResult Index()
    {
        

        return View();
    }
}

