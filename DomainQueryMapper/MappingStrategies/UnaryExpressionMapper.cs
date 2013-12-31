using System;
using System.Linq.Expressions;

namespace DomainQueryMapper.MappingStrategies
{
    public class UnaryExpressionMapper : IMappingStrategy
    {
        public Expression Map(Expression ex, ParameterExpression pe, string fromName, Type propType)
        {
            var expression = (UnaryExpression)ex;
            var memberMapper = new MemberExpressionMapper();
            var result = memberMapper.Map(expression.Operand, pe, fromName, propType);

            if (expression.NodeType == ExpressionType.Not)
                result = Expression.Not(result);

            return result;
        }
    }
}