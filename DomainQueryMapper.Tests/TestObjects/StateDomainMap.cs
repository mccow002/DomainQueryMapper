using DomainQueryMapper.Tests.TestObjects.Data;
using DomainQueryMapper.Tests.TestObjects.Domain;

namespace DomainQueryMapper.Tests.TestObjects
{
    public class StateDomainMap : DomainQueryMap<StateObj, State>
    {
         public StateDomainMap()
         {
             MapProperty(x => x.Id)
                 .To(x => x.StateId);
         }
    }
}