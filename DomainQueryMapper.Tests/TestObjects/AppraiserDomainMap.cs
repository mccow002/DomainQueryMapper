using DomainQueryMapper.Tests.TestObjects.Data;
using DomainQueryMapper.Tests.TestObjects.Domain;

namespace DomainQueryMapper.Tests.TestObjects
{
    public class AppraiserDomainMap : DomainQueryMap<AppraiserObj, Appraiser>
    {
         public AppraiserDomainMap()
         {
             MapProperty(x => x.Id)
                 .To(x => x.AppraiserId);

             MapProperty(x => x.User.Id)
                 .To(x => x.User.UserId);

             MapProperty(x => x.User.IsActive)
                 .To(x => x.IsActive);
         }
    }
}