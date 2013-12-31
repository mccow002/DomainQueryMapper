using System;
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
            var mapper = new MemberExpressionMapper();
            var exp = mapper.Map(property.Body, param, typeof(TFrom).Name, typeof(object));
            return Expression.Lambda<Func<TTo, object>>(exp, param);
        }

        public static Expression<Func<TTo, T>> MapQuery<TTo, TFrom, T>(Expression<Func<TFrom, T>> query)
        {
            var parts = GetMemberExpressions(query.Body).ToList();
            parts.Reverse();

            var mappedParts = new List<Expression>();
            var arg = Expression.Parameter(typeof(TTo));
            var fromName = typeof(TFrom).Name;
            foreach (var part in parts)
            {
                var strategy = MappingFactory.GetStrategy(part);

                if (strategy != null)
                    mappedParts.Add(strategy.Map(part, arg, fromName, typeof(T)));

                //if (part is MethodCallExpression)
                //    mappedParts.Add(MapMethodCallExpression((MethodCallExpression)part, arg, typeof(TFrom)));

                //if (part is MemberExpression)
                //    mappedParts.Add(MapMemberExpression((MemberExpression)part, arg, typeof(TFrom)));
            }

            var exp = MapNodes(query, mappedParts);
            return Expression.Lambda<Func<TTo, T>>(exp, arg);
        }

        private static Expression MapNodes<TEntity, T>(Expression<Func<TEntity, T>> query, List<Expression> mappedParts)
        {
            var node = query.Body;

            var index = mappedParts.Count() - 1;
            var exp = mappedParts[index];
            index = index - 1;

            while (index >= 0)
            {
                var part = mappedParts[index];
                var unary = DetermineUnaryType(node, exp, part);
                if (unary != null) 
                    exp = unary;

                index = index - 1;

                if (node is BinaryExpression)
                    node = ((BinaryExpression)node).Left;
            }

            return exp;
        }

        private static Expression DetermineUnaryType(Expression node, Expression exp, Expression part)
        {
            if (node.NodeType == ExpressionType.AndAlso)
                return Expression.AndAlso(exp, part);

            if (node.NodeType == ExpressionType.OrElse)
                return Expression.OrElse(exp, part);

            if (node.NodeType == ExpressionType.Not)
                return Expression.Not(part);

            return null;
        }

        private static IEnumerable<Expression> GetMemberExpressions(Expression body)
        {
            // A Queue preserves left to right reading order of expressions in the tree
            var candidates = new Queue<Expression>(new[] { body });
            while (candidates.Count > 0)
            {
                var expr = candidates.Dequeue();
                if (expr is MemberExpression) 
                    yield return expr;
                else if (expr is UnaryExpression) 
                    yield return expr;
                    //candidates.Enqueue(((UnaryExpression)expr).Operand);
                else if (expr is BinaryExpression)
                {
                    var binary = expr as BinaryExpression;

                    if (binary.NodeType == ExpressionType.AndAlso || binary.NodeType == ExpressionType.OrElse)
                    {
                        candidates.Enqueue(binary.Right);
                        candidates.Enqueue(binary.Left);
                    }

                    if (!(binary.Left is BinaryExpression) && !(binary.Right is BinaryExpression))
                        if (IsValidType(binary.NodeType)) 
                            yield return binary;
                        else
                        {
                            candidates.Enqueue(binary.Right);
                            candidates.Enqueue(binary.Left);
                        }
                }
                else if (expr is MethodCallExpression)
                {
                    if (expr.NodeType == ExpressionType.Call) yield return expr;

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