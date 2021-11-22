using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repository;
using BusinessObject.Models;
using SaleWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using SaleWebApp.Helpers;

namespace SaleWebApp.Controllers
{
    public class MemberController : Controller
    {
        private IMemberRepository memberRepo;
        private MemberObject loginMember;
        private readonly IConfiguration builder;
        public MemberController()
        {
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", true, true)
                                                .Build();
            memberRepo = new MemberRepository();
        }
        public async Task<IActionResult> GetMembers()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            var list = new List<MemberObject>();
            string url = builder["APIController:Member"];
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpGetHelper<List<MemberObject>>(url, "memberList");
                list = task.content;
            }
            return View(list);
        }
        [HttpGet]
        public IActionResult EditMemberInfo(int MemberId )
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            var info = memberRepo.GetMemberInfo(MemberId);
            if (info != null) return View(info);
            return RedirectToHomePage();
        }
        [HttpPost]
        public async Task<IActionResult> EditMemberInfo(MemberUpdateModel model)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            if (!ModelState.IsValid) return View();
            string message = "Update Failed";
            var memberObject = new MemberObject()
            {
                MemberId = model.MemberId,
                Email = model.Email,
                CompanyName = model.CompanyName,
                City = model.City,
                Country = model.Country
            };
            bool valid = false;
            string url = builder["APIController:Member"];
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpPutHelper<bool>(url, memberObject, "valid");
                valid = task.content;
            }
            if (valid) message = "Update Successfully";
            ViewBag.Message = message;
            return View(memberObject);
        }

        public IActionResult EditProfile()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId == 0)) return RedirectToHomePage();
            var info = memberRepo.GetMemberInfo(loginMember.MemberId);
            return View(info);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(MemberUpdateModel model)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && (loginMember.MemberId == 0 || loginMember.MemberId != model.MemberId) )) return RedirectToHomePage();
            if (!ModelState.IsValid) return View();
            string message = "Update Failed";
            var memberObject = new MemberObject()
            {
                MemberId = model.MemberId,
                Email = model.Email,
                CompanyName = model.CompanyName,
                City = model.City,
                Country = model.Country
            };
            bool valid = false;
            string url = builder["APIController:Member"];
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpPutHelper<bool>(url, memberObject, "valid");
                valid = task.content;
            }
            if (valid) message = "Update Successfully";
            ViewBag.Message = message;
            return View(memberObject);
        }
        public async Task<IActionResult> RemoveMember(int MemberId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            bool valid = false;
            string url = builder["APIController:Member"] + "?memberId=" + MemberId;
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpDeleteHelper<bool>(url, "valid");
                valid = task.content;
            }
            if (valid)
            {
                TempData["Message"] = $"Member {MemberId} Removed"; 
            }
            else
            {
                TempData["Message"] = $"Failed to remove. Member {MemberId} can not be deleted or not found !";
            }
            return RedirectToAction("GetMembers");
        }

        public IActionResult AddNewMember()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewMember(MemberInsertModel insertMember)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            if (!ModelState.IsValid) return View();
            var memberObject = new MemberObject()
            {
                Email = insertMember.Email,
                Password = insertMember.Password,
                CompanyName = insertMember.CompanyName,
                City = insertMember.City,
                Country = insertMember.Country
            };
            bool valid = false;
            string url = builder["APIController:Member"];          
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpPostHelper<bool>(url, memberObject, "valid");
                valid = task.content;
            }
            if (valid)
            {
                return RedirectToAction("GetMembers");
            }
            ViewBag.Message = $"Failed to add new Member - Duplicate Email {memberObject.Email}";
            return View();
        }
        private MemberObject GetSessionRole(HttpContext context)
        {
            var session = context.Session;
            MemberObject member = null;
            string role = session.GetString("ROLE");            
            if (role != null)
            {
                member = new MemberObject
                {
                    MemberId = session.GetInt32("MEMBER_ID") ?? 0
                };
            }
            return member;
        }

        private IActionResult RedirectToHomePage()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
