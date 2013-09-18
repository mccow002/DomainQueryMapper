using System.Collections.Generic;

namespace DomainQueryMapper
{
    public interface IDomainQueryMap
    {
        string Key { get; }
        List<IPropertyMap> Maps { get; }
    }
}