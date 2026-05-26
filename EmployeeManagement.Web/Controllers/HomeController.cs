using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Web.Models;

namespace EmployeeManagement.Web.Controllers;

public class HomeController : Controller
{
    private const string SessionLoginUserId = "LoginUserId";

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32(SessionLoginUserId) == null)
        {
            return RedirectToAction("Index", "Login");
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
