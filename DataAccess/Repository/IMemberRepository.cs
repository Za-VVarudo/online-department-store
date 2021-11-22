using BusinessObject.Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public interface IMemberRepository
    {
        (MemberObject member, string role) Login(string email, string password);
        IEnumerable<MemberObject> GetMemberList();
        MemberObject GetMemberInfo(int memberID);
        bool Register(MemberObject member);
        bool Update(MemberObject member);
        bool UpdateProfile(MemberObject member);
        bool Remove(int memberID);
    }
}
