using DomainQueryMapper.Tests.TestObjects.Data;
using DomainQueryMapper.Tests.TestObjects.Domain;

namespace DomainQueryMapper.Tests.TestObjects
{
    public static class TestValueMethods
    {
         public static int AppraiserId()
         {
             return 1;
         }

        public static User GetUser()
        {
            return new User{UserId = 1};
        }
    }
}