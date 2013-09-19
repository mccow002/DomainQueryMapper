using System;
using System.Linq;

namespace DomainQueryMapper.Helpers
{
    public class DomainMapperHelpers
    {
        public static IPropertyMap GetMap(string fromName, string propName)
        {
            var map = DomainQueryMapper.Maps.FirstOrDefault(x => x.Key == fromName);
            return map != null ? map.Maps.FirstOrDefault(x => x.Key == propName) : null;
        }

        public static IDomainQueryMap GetMapFromType(Type fromType)
        {
            return DomainQueryMapper.Maps.FirstOrDefault(x => x.FromType == fromType);
        }
    }
}