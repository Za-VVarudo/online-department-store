using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private MemberDAO instance = MemberDAO.Instance;
        public (MemberObject member, string role) Login(string email, string password) => instance.Login(email, password);
        public IEnumerable<MemberObject> GetMemberList() => instance.GetMemberList();
        public MemberObject GetMemberInfo(int memberID) => instance.GetMemberInfo(memberID);
        public bool Register(MemberObject member) => instance.Register(member);
        public bool Update(MemberObject member) => instance.Update(member);
        public bool UpdateProfile(MemberObject member) => instance.UpdateProfile(member);
        public bool Remove(int memberID) => instance.RemoveMember(memberID);
    }
}
