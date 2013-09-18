﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DomainQueryMapper.MappingStrategies;

namespace DomainQueryMapper
{
    public class DomainMapper
    {
        public static Expression<Func<TTo, object>> MapProperty<TTo, TFrom>(Expression<Func<TFrom, object>> property)
        {
            if (!(property.Body is MemberExpression))
                throw new ArgumentException("The func must be a member expression", "property");

            var param = Expression.Parameter(typeof(TTo), "x");
            //var exp = MapMemberExpression((MemberExpression)property.Body, param, typeof(TFrom));
            //return Expression.Lambda<Func<TTo, object>>(exp, param);

            return null;
        }

        public static Expression<Func<TTo, bool>> MapQuery<TTo, TFrom>(Expression<Func<TFrom, bool>> query)
        {
            var parts = GetMemberExpressions(query.Body);

            var mappedParts = new List<Expression>();
            var arg = Expression.Parameter(typeof(TTo), "x");
            var fromName = typeof (TFrom).Name;
            foreach (var part in parts)
            {
                var strategy = MappingFactory.GetStrategy(part);

                if(strategy != null)
                    mappedParts.Add(strategy.Map(part, arg, fromName));
                
                //if (part is MethodCallExpression)
                //    mappedParts.Add(MapMethodCallExpression((MethodCallExpression)part, arg, typeof(TFrom)));

                //if (part is MemberExpression)
                //    mappedParts.Add(MapMemberExpression((MemberExpression)part, arg, typeof(TFrom)));
            }

            var exp = MapNodes(query, mappedParts);
            return Expression.Lambda<Func<TTo, bool>>(exp, arg);
        }

        private static Expression MapNodes<TEntity>(Expression<Func<TEntity, bool>> query, List<Expression> mappedParts)
        {
            Expression exp = mappedParts[0];
            var node = query.Body;
            var index = 0;
            while (index < mappedParts.Count())
            {
                if (node.NodeType == ExpressionType.AndAlso)
                    exp = Expression.AndAlso(exp, mappedParts[index]);

                if (node.NodeType == ExpressionType.OrElse)
                    exp = Expression.OrElse(exp, mappedParts[index]);

                if (node.NodeType == ExpressionType.Not)
                    exp = Expression.Not(mappedParts[index]);

                index = index + 1;

                if (node is BinaryExpression)
                    node = ((BinaryExpression)node).Right;
            }

            return exp;
        }

        private static Expression MapMethodCallExpression(MethodCallExpression methodCall, ParameterExpression pe, Type fromType)
        {
            var name = GetName(methodCall);

            if (string.IsNullOrWhiteSpace(name))
            {
                var constant = Expression.Lambda(methodCall, null).Compile().DynamicInvoke();
                return Expression.Constant(constant);
            }

            var map = DomainQueryMapper.Maps.FirstOrDefault(x => x.Key == fromType.Name);
            if (map != null)
            {
                var dataProperty = map.Maps.FirstOrDefault(x => x.Key == name);
                if (dataProperty != null)
                {
                    var body = dataProperty.DataProperty;
                    if (body is UnaryExpression)
                        body = ((UnaryExpression)body).Operand;
                    name = ((MemberExpression)body).Member.Name;
                }
            }

            Expression propertyEx = Expression.Property(pe, name);
            return Expression.Call(propertyEx, methodCall.Method, methodCall.Arguments);
        }

        private static string GetName(Expression property)
        {
            var body = property;
            if (body is UnaryExpression)
                body = ((UnaryExpression)body).Operand;

            return (body is MemberExpression) ? ((MemberExpression)body).Member.Name : string.Empty;
        }

        private static Expression GetPropertyExpression(Expression ex, ParameterExpression pe)
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


        private static IEnumerable<Expression> GetMemberExpressions(Expression body)
        {
            // A Queue preserves left to right reading order of expressions in the tree
            var candidates = new Queue<Expression>(new[] { body });
            while (candidates.Count > 0)
            {
                var expr = candidates.Dequeue();
                if (expr is MemberExpression)
                {
                    yield return expr;
                }
                else if (expr is UnaryExpression)
                {
                    candidates.Enqueue(((UnaryExpression)expr).Operand);
                }
                else if (expr is BinaryExpression)
                {
                    var binary = expr as BinaryExpression;

                    if (!(binary.Left is BinaryExpression) && !(binary.Right is BinaryExpression))
                        if (IsValidType(binary.NodeType))
                            yield return binary;
                        else
                        {
                            candidates.Enqueue(binary.Left);
                            candidates.Enqueue(binary.Right);
                        }
                }
                else if (expr is MethodCallExpression)
                {
                    var method = expr as MethodCallExpression;
                    foreach (var argument in method.Arguments)
                    {
                        candidates.Enqueue(argument);
                    }
                }
                else if (expr is LambdaExpression)
                {
                    candidates.Enqueue(((LambdaExpression)expr).Body);
                }
            }
        }

        private static bool IsValidType(ExpressionType nodeType)
        {
            return nodeType == ExpressionType.Equal || nodeType == ExpressionType.NotEqual ||
                nodeType == ExpressionType.GreaterThan || nodeType == ExpressionType.GreaterThanOrEqual ||
                nodeType == ExpressionType.LessThan || nodeType == ExpressionType.LessThanOrEqual;
        }
    }
}