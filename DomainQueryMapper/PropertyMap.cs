using System;
using System.Linq.Expressions;

namespace DomainQueryMapper
{
    public class PropertyMap<TTo, TFrom> : IPropertyMap
    {
        public PropertyMap(Expression<Func<TTo, object>> dataProperty)
        {
            DataExpression = dataProperty;
        }

        public PropertyMap<TTo, TFrom> To(Expression<Func<TFrom, object>> domainProperty)
        {
            Key = GetName(domainProperty);
            DomainExpression = domainProperty;
            return this;
        }

        public Expression<Func<TTo, object>> DataExpression { get; private set; }
        public Expression<Func<TFrom, object>> DomainExpression { get; private set; }

        private string GetName<T>(Expression<Func<T, object>> property)
        {
            var body = property.Body;
            if (body is UnaryExpression)
                body = ((UnaryExpression)body).Operand;

            return ((MemberExpression)body).Member.Name;
        }

        public string Key { get; private set; }
        public Expression DataProperty { get { return DataExpression.Body; } }
    }
}