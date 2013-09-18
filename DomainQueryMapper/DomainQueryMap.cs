using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DomainQueryMapper
{
    public class DomainQueryMap<TTo, TFrom> : IDomainQueryMap
    {
        protected List<IPropertyMap> _propertyMaps = new List<IPropertyMap>();

        protected PropertyMap<TTo, TFrom> MapProperty(Expression<Func<TTo, object>> domainProperty)
        {
            var propMap = new PropertyMap<TTo, TFrom>(domainProperty);
            _propertyMaps.Add(propMap);
            return propMap;
        }

        public List<IPropertyMap> Maps { get { return _propertyMaps; } }

        public string Key { get { return typeof(TFrom).Name; } }
    }
}