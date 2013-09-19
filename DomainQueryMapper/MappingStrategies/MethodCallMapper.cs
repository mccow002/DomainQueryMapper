﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DomainQueryMapper.Helpers;

namespace DomainQueryMapper.MappingStrategies
{
    public class MethodCallMapper : IMappingStrategy
    {
        public Expression Map(Expression ex, ParameterExpression pe, string fromName)
        {
            var methodCall = (MethodCallExpression) ex;

            var parts = new Queue<Expression>();
            var propEx = ex;
            parts.Enqueue(propEx);
            while (propEx != null)
            {
                propEx = ExpressionHelpers.GetExpression(propEx);
                if (propEx != null && !(propEx is ParameterExpression))
                    parts.Enqueue(propEx);
            }

            var name = ExpressionHelpers.GetName(methodCall);
            var map = DomainMapperHelpers.GetMap(fromName, name);

            foreach (var part in parts.Reverse())
            {
                
            }



            //    var methodCall = (MethodCallExpression) ex;
            //    var name = ExpressionHelpers.GetName(methodCall);

            //    if (string.IsNullOrWhiteSpace(name))
            //    {
            //        var constant = Expression.Lambda(methodCall, null).Compile().DynamicInvoke();
            //        return Expression.Constant(constant);
            //    }

            //    var map = DomainMapperHelpers.GetMap(fromName, name);
            //    if (map != null)
            //        name = ExpressionHelpers.GetMemberExpression(methodCall).Member.Name;

            //Expression propertyEx = Expression.Property(pe, name);
            //return Expression.Call(propertyEx, methodCall.Method, methodCall.Arguments);
            return null;
        }
    }
}