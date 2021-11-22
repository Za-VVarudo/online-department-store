using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository;
using SaleWebApp.Models;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using BusinessObject;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SaleWebApp.Helpers;

namespace SaleWebApp.Controllers
{
    [Route("[controller]")]
    public class StatisticController : Controller
    {
        private IOrderRepository orderRepo;
        private MemberObject loginMember;
        private readonly IConfiguration builder;
        public StatisticController()
        {
            orderRepo = new OrderRepository();
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", true, true)
                                                .Build();
        }
        [HttpGet]
        public IActionResult Index()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            return View("DailySaleStatistic");
        }
        public async Task<IActionResult> DailySaleStatistic([FromForm] StatisticParamsModel param)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();           
            var from = param.From;
            var to = param.To;
            if (from.CompareTo(to) > 0)
            {
                from = to;
                to = param.From;
            }
            to = to.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<DailySaleStatisticResult> saleStatistic;
            var url = builder["APIController:Order"] + "/statistic";
            using (var httpClient = new HttpClient())
            {
                param.From = from;
                param.To = to;
                var result = await httpClient.HttpPostHelper<List<DailySaleStatisticResult>>(url, param, "statistic");
                saleStatistic = result.content;
            }
            TempData["fromDate"] = from.ToString("yyyy-MM-dd");
            TempData["toDate"] = to.ToString("yyyy-MM-dd");
            return View(saleStatistic);
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
