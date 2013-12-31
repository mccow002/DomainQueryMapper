using System.Collections.Generic;

namespace DomainQueryMapper.Tests.TestObjects.Data
{
    public class AppraiserObj
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string Title { get; set; }

        public int Age { get; set; }

        public UserObj User { get; set; }

        public List<StateObj> States { get; set; }
    }
}