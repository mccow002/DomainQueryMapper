using System.Linq.Expressions;

namespace DomainQueryMapper.MappingStrategies
{
    public static class MappingFactory
    {
        public static IMappingStrategy GetStrategy(Expression ex)
        {
            if (ex is BinaryExpression)
                return new BinaryExpressionMapper();

            if (ex is MemberExpression)
                return new MemberExpressionMapper();

            if (ex is MethodCallExpression)
                return new MethodCallMapper();

            if (ex is UnaryExpression)
                return new UnaryExpressionMapper();

            return null;
        }
    }
}