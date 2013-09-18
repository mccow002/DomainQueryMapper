using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DomainQueryMapper.Helpers
{
    public static class ExpressionHelpers
    {
         public static MemberExpression GetMemberExpression(Expression ex)
         {
             if (ex is UnaryExpression)
                 return (MemberExpression)((UnaryExpression) ex).Operand;

             return (MemberExpression) ex;
         }

         public static Expression GetPropertyExpression(Expression ex, ParameterExpression pe)
         {
             var propEx = (MemberExpression)ex;

             var exps = new Queue<Expression>();
             while (propEx != null)
             {
                 exps.Enqueue(propEx);
                 if (propEx.Expression is MemberExpression)
                     propEx = (MemberExpression)propEx.Expression;
                 else
                     propEx = null;
             }

             Expression finalExp = null;
             foreach (var exp in exps.Reverse())
             {
                 var memberExp = (MemberExpression)exp;
                 if (finalExp == null)
                 {
                     finalExp = Expression.Property(pe, memberExp.Member.Name);
                     continue;
                 }

                 finalExp = Expression.Property(finalExp, memberExp.Member.Name);
             }

             return finalExp;
         }
    }
}