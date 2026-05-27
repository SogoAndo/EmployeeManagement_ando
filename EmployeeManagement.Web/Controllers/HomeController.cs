using System.Diagnostics;
using EmployeeManagement.Web.Applications.Filters;
using EmployeeManagement.Web.Applications.Security;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

public class HomeController : Controller
{
    [LoginRequired]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) == null)
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
