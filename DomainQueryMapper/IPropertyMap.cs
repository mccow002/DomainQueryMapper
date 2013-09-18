using System.Linq.Expressions;

namespace DomainQueryMapper
{
    public interface IPropertyMap
    {
        string Key { get; }
        Expression DataProperty { get; }
    }
}