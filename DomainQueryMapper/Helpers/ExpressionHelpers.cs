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

         public static Expression BuildPropertyExpression(Expression ex, ParameterExpression pe)
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

         public static bool IsParameterExpression(Expression ex)
         {
             var innerEx = ex;
             while (innerEx != null)
             {
                 if (innerEx is ParameterExpression)
                     break;

                 innerEx = GetExpression(innerEx);
             }

             return innerEx is ParameterExpression;
         }

         public static Expression GetExpression(Expression ex)
         {
             if (ex is ParameterExpression)
                 return null;
             if (ex is MemberExpression)
                 return ((MemberExpression)ex).Expression;
             if (ex is MethodCallExpression)
                 return ((MethodCallExpression)ex).Object;
             if (ex is UnaryExpression)
                 return ((UnaryExpression)ex).Operand;

             return null;
         }

         public static string GetName(Expression property)
         {
             var propEx = property;
             Expression memberEx = null;
             while (propEx != null)
             {
                 propEx = GetExpression(propEx);
                 if (propEx != null && !(propEx is ParameterExpression))
                     memberEx = propEx;
             }

             var body = memberEx;
             if (body is UnaryExpression)
                 body = ((UnaryExpression)body).Operand;

             if (body is MethodCallExpression)
             {
                 var methodCall = (MethodCallExpression) body;
                 body = methodCall.Object ?? methodCall.Arguments[0];
             }

             return (body is MemberExpression) ? ((MemberExpression)body).Member.Name : string.Empty;
         }
    }
}