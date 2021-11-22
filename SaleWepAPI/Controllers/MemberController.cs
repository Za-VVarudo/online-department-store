using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private IMemberRepository memberRepo;
        public MemberController() : base()
        {
            memberRepo = new MemberRepository();
        }
        [HttpGet]
        public IActionResult GetMembers()
        {
            var list = memberRepo.GetMemberList();
            return new JsonResult(new { MemberList = list , Message = "" });
        }

        [HttpPost("login")]
        public IActionResult Login(MemberObject account)
        {
            string email = account.Email;
            string password = account.Password;
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var (member, role) = memberRepo.Login(email, password);
                return new JsonResult(new { MemberObject = new { Member = member, Role = role }, Message = "" });
            }
            else
            {
                this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(StatusCodes.Status400BadRequest);
            }

        }

        [HttpGet("{memberId:int}")]
        public IActionResult GetInfo(int memberId)
        {
            var member = memberRepo.GetMemberInfo(memberId);
            return new JsonResult(new { Member = member , Message = "" });
        }
        [HttpPost]
        public IActionResult CreateMember(MemberInsertModel model)
        {
            bool valid = false;
            if (ModelState.IsValid)
            {
                var member = new MemberObject
                {
                    Email = model.Email,
                    Password = model.Password,
                    CompanyName = model.CompanyName,
                    City = model.City,
                    Country = model.Country
                };
                valid = memberRepo.Register(member);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }
        [HttpPut]
        public IActionResult EditMember(MemberUpdateModel model)
        {
            bool valid = false;
            if (ModelState.IsValid)
            {
                var member = new MemberObject
                {
                    MemberId = model.MemberId,
                    CompanyName = model.CompanyName,
                    City = model.City,
                    Country = model.Country
                };
                valid = memberRepo.Update(member);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }
        [HttpDelete]
        public IActionResult DeleteMember(int memberId)
        {
            bool valid = false;
            if (memberId > 0)
            {
                valid = memberRepo.Remove(memberId);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }
    }
}
