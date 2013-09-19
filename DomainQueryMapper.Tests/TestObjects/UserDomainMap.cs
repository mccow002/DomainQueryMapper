using DomainQueryMapper.Tests.TestObjects.Data;
using DomainQueryMapper.Tests.TestObjects.Domain;

namespace DomainQueryMapper.Tests.TestObjects
{
    public class UserDomainMap : DomainQueryMap<UserObj, User>
    {
         public UserDomainMap()
         {
             MapProperty(x => x.Id)
                 .To(x => x.UserId);
         }
    }
}