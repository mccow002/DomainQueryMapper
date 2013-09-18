using System.Linq.Expressions;

namespace DomainQueryMapper.MappingStrategies
{
    public interface IMappingStrategy
    {
        Expression Map(Expression ex, ParameterExpression pe, string fromName);
    }
}