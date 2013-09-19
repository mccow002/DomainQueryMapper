using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DomainQueryMapper.Helpers;

namespace DomainQueryMapper.MappingStrategies
{
    public class BinaryExpressionMapper : IMappingStrategy
    {
        public Expression Map(Expression ex, ParameterExpression pe, string fromName)
        {
            var binary = (BinaryExpression)ex;
            var propName = ExpressionHelpers.GetLowestLevelPropertyName(binary.Left);
            var map = DomainMapperHelpers.GetMap(fromName, propName);

            //This need to be modified to be more recursive. Current only support one-level sub properties
            if (map == null)
            {
                var highestPropName = ExpressionHelpers.GetHighestLevelPropertyName(binary.Left);
                map = DomainMapperHelpers.GetMap(fromName, highestPropName);

                if (map == null)
                {
                    var lowestLevelProp = ExpressionHelpers.GetLowestLevelProperty(binary.Left);
                    var typeMap = DomainMapperHelpers.GetMapFromType(lowestLevelProp.Type);
                    if (typeMap != null)
                    {
                        map = typeMap.Maps.FirstOrDefault(x => x.Key == highestPropName);
                    }
                }
            }

            var leftEx = map == null ? binary.Left : ExpressionHelpers.GetExpression(map.DataProperty);

            var left = DetermineLeftMapper(leftEx)(leftEx, pe, propName);
            
            var right = binary.Right;
            if (!ExpressionHelpers.IsParameterExpression(right))
                right = Expression.Constant(Expression.Lambda(right, null).Compile().DynamicInvoke());

            return Expression.MakeBinary(binary.NodeType, left, right);
        }

        private Func<Expression, ParameterExpression, string, Expression> DetermineLeftMapper(Expression left)
        {
            if (left is MemberExpression)
                return MemberMapper;

            if (left is MethodCallExpression)
            {
                var methodEx = (MethodCallExpression) left;
                var declaringType = methodEx.Method.DeclaringType;

                return declaringType == typeof (Enumerable)
                           ? (Func<Expression, ParameterExpression, string, Expression>) LinqMethodMapper
                           : SimpleMethodMapper;
            }
            return null;
        }

        private Expression MemberMapper(Expression memberEx, ParameterExpression pe, string name)
        {
            return ExpressionHelpers.BuildPropertyExpression(memberEx, pe);
        }

        private Expression SimpleMethodMapper(Expression methodEx, ParameterExpression pe, string name)
        {
            var methodCall = (MethodCallExpression)methodEx;
            var propertyEx = Expression.Property(pe, name);
            return Expression.Call(propertyEx, methodCall.Method, methodCall.Arguments);
        }

        private Expression LinqMethodMapper(Expression methodEx, ParameterExpression pe, string name)
        {
            var methodCall = (MethodCallExpression) methodEx;
            var newArgs = new List<Expression>();

            Type toType = null;
            foreach (var argument in methodCall.Arguments)
            {
                if(argument.NodeType == ExpressionType.MemberAccess)
                    newArgs.Add(new MemberExpressionMapper().Map(argument, pe, name));

                if (argument.NodeType == ExpressionType.Lambda)
                {
                    var lambdaEx = (LambdaExpression) argument;
                    var fromType = lambdaEx.Parameters.First().Type;
                    var map = DomainMapperHelpers.GetMapFromType(fromType);

                    if(map == null)
                        throw new Exception(string.Format("You must define a map for type {0}", fromType.Name));

                    toType = map.ToType;
                    var mi = typeof (DomainMapper).GetMethod("MapQuery");
                    mi = mi.MakeGenericMethod(new[] {toType, fromType});
                    var mappedQuery = mi.Invoke(null, new[] {lambdaEx});

                    newArgs.Add((Expression) mappedQuery);

                }
            }

            var propertyEx = Expression.Property(pe, name);
            return Expression.Call(typeof(Enumerable), methodCall.Method.Name, new[] { toType }, newArgs.ToArray());
            //return Expression.Call(propertyEx, method, newArgs.ToArray());
        }
    }
}