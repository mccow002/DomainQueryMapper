using System;
using System.Linq;
using System.Linq.Expressions;

namespace DomainQueryMapper
{
    public static class DomainQueryMapExtensions
    {
        public static IQueryable<TTo> MappedWhere<TTo, TFrom>(this IQueryable<TTo> source, Expression<Func<TFrom, bool>> query)
        {
            var mappedQuery = DomainMapper.MapQuery<TTo, TFrom, bool>(query);
            return source.Where(mappedQuery);
        }

        public static IQueryable<TTo> MappedOrderBy<TTo, TFrom, T>(this IQueryable<TTo> source,
            Expression<Func<TFrom, T>> orderBy)
        {
            var mappedQuery = DomainMapper.MapQuery<TTo, TFrom, T>(orderBy);
            return source.OrderBy(mappedQuery);
        }
    }
}