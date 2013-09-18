﻿using System.Collections.Generic;

namespace DomainQueryMapper
{
    public static class DomainQueryMapper
    {
        static DomainQueryMapper()
        {
            Maps = new List<IDomainQueryMap>();
        }

        public static void AddMap(IDomainQueryMap map)
        {
            Maps.Add(map);
        }

        public static List<IDomainQueryMap> Maps { get; private set; }
    }
}