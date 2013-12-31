using System;
using System.Linq.Expressions;
using DomainQueryMapper.Helpers;

namespace DomainQueryMapper.MappingStrategies
{
    public class MemberExpressionMapper : IMappingStrategy
    {
        public Expression Map(Expression ex, ParameterExpression pe, string fromName, Type propType)
        {
            var member = (MemberExpression)ex;
            var name = member.Member.Name;
            var map = DomainMapperHelpers.GetMap(fromName, name);

            return map != null ? 
                ExpressionHelpers.BuildPropertyExpression(ExpressionHelpers.GetMemberExpression(map.DataProperty), pe) : 
                Expression.Property(pe, name);
        }
    }
}