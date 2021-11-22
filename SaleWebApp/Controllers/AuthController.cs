using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using SaleWebApp.Helpers;
using BusinessObject.Models;
using SaleWepApp.Models;

namespace SaleWebApp.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IMemberRepository memberRepo;
        private readonly IConfiguration builder;
        public AuthController()
        {
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", true, true)
                                                .Build();
            memberRepo = new MemberRepository();
        }
        [HttpGet]
        public IActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("ROLE") != null) return RedirectToAction("Index", "Home");
            return View("Login");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var session = HttpContext.Session;
            if (session.GetString("ROLE") != null) return RedirectToAction("Index", "Home");
            string url = builder["APIController:Member"] + "/login";
            MemberObjectResponse result = null;
            using (var httpClient = new HttpClient())
            {
                var param = new MemberObject{ Email = email, Password = password };
                var task = await httpClient.HttpPostHelper<MemberObjectResponse>(url, param, "memberObject");
                result = task.content;
            }
            if (result != null && result.Member != null)
            {
                session.SetString("ROLE", result.Role);
                session.SetInt32("MEMBER_ID", result.Member.MemberId);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Incorrect Email or Password";
                return Index();
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var session = HttpContext.Session;
            session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
