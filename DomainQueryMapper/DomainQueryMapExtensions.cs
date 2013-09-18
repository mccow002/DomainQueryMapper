using System;
using System.Linq;
using System.Linq.Expressions;

namespace DomainQueryMapper
{
    public static class DomainQueryMapExtensions
    {
        public static IQueryable<TTo> MappedWhere<TTo, TFrom>(this IQueryable<TTo> source, Expression<Func<TFrom, bool>> query)
        {
            var mappedQuery = DomainMapper.MapQuery<TTo, TFrom>(query);
            return source.Where(mappedQuery);
        }
    }
}