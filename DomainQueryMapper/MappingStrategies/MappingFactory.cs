using System.Linq.Expressions;

namespace DomainQueryMapper.MappingStrategies
{
    public static class MappingFactory
    {
         public static IMappingStrategy GetStrategy(Expression ex)
         {
             if(ex is BinaryExpression)
                 return new BinaryExpressionMapper();

             if(ex is MemberExpression)
                 return new MemberExpressionMapper();

             return null;
         }
    }
}