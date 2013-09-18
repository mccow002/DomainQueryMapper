using System.Linq.Expressions;
using DomainQueryMapper.Helpers;

namespace DomainQueryMapper.MappingStrategies
{
    public class BinaryExpressionMapper : IMappingStrategy
    {
        public Expression Map(Expression ex, ParameterExpression pe, string fromName)
        {
            var binary = (BinaryExpression)ex;
            var name = ((MemberExpression)binary.Left).Member.Name;
            var map = DomainMapperHelpers.GetMap(fromName, name);

            var leftEx = map == null ? binary.Left : ExpressionHelpers.GetMemberExpression(map.DataProperty);
            var left = ExpressionHelpers.GetPropertyExpression(leftEx, pe);

            var right = binary.Right;

            if (right is MemberExpression)
            {
                
            }

            if (right is MethodCallExpression)
                right = Expression.Constant(Expression.Lambda(right, null).Compile().DynamicInvoke());

            return Expression.MakeBinary(binary.NodeType, left, right);
        }
    }
}