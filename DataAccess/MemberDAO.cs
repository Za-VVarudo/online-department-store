using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace DataAccess
{
    public class MemberDAO
    {
        private MemberDAO()
        {
            GetAdminInfo();
        }
        private static MemberDAO instance = null;
        private static readonly object instanceLock = new object();
        public static MemberDAO Instance
        {
            get
            {
                lock(instanceLock)
                {
                    return instance == null ? instance = new MemberDAO() : instance;
                }
            }
        }
        private MemberObject admin = new MemberObject();
        private void GetAdminInfo()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json",true,true)
                                                    .Build();
            admin.Email = builder["Admin:Email"];
            admin.Password = builder["Admin:Password"];
        }

        public (MemberObject member, string role) Login(string email, string password)
        {
            MemberObject member = null;
            string role = "None";
            if (email.Equals(admin.Email) && password.Equals(admin.Password))
            {
                member = admin;
                role = "Admin";
            } else
            {
                try
                {
                    using (var context = new SaleContext())
                    {
                        member = context.Members.Where(m => (email.Equals(m.Email) && password.Equals(m.Password))).FirstOrDefault();
                        if (member != null) role = "Member";
                    }
                }
                catch (Exception)
                {

                }
            }
            return (member, role);
        }

        public List<MemberObject> GetMemberList()
        {
            List<MemberObject> list = new List<MemberObject>();
            try
            {
                using (var context = new SaleContext())
                {
                    list = context.Members.ToList();
                }
            } catch (Exception)
            {

            }
            return list;
        }

        public MemberObject GetMemberInfo(int memberID)
        {
            MemberObject member = null;
            try
            {
                using (var context = new SaleContext())
                {
                    member = context.Members.Find(memberID);
                }
            }
            catch (Exception)
            {

            }
            return member;
        }
        public bool Register(MemberObject member)
        {
            bool valid = false;
            if (admin.Email.Equals(member.Email)) return false;
            using (var context = new SaleContext())
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var memberlist = context.Members;
                    memberlist.Add(member);
                    context.SaveChanges();
                    transaction.Commit();
                    valid = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return valid;
        }

        public bool Update(MemberObject member)
        {
            bool valid = false;
            if (admin.Email.Equals(member.Email)) return false;
            using (var context = new SaleContext())
            {
                try
                {
                    var memberlist = context.Members;
                    MemberObject updateMember = null;
                    if ((updateMember = memberlist.Find(member.MemberId)) != null)
                    {
                        updateMember.CompanyName = member.CompanyName;
                        updateMember.City = member.City;
                        updateMember.Country = member.Country;
                        context.SaveChanges();
                        valid = true;
                    }
                }
                catch { }
            }
            return valid;
        }

        public bool UpdateProfile(MemberObject member)
        {
            bool valid = false;
            if (admin.Email.Equals(member.Email)) return false;
            using (var context = new SaleContext())
            {
                try
                {
                    var memberlist = context.Members;
                    MemberObject updateMember = null;
                    if ((updateMember = memberlist.Find(member.MemberId)) != null)
                    {
                        updateMember.CompanyName = member.CompanyName;
                        updateMember.City = member.City;
                        updateMember.Country = member.Country;
                        updateMember.Password = member.Password;
                        context.SaveChanges();
                        valid = true;
                    }
                }
                catch { }
            }
            return valid;
        }

        public bool RemoveMember(int memberID)
        {
            bool valid = false;
            using (var context = new SaleContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Members.Remove(new MemberObject { MemberId = memberID});
                        context.SaveChanges();
                        transaction.Commit();
                        valid = true;
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
            return valid;
        }
    }
}
