using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DomainQueryMapper.Tests.TestObjects;
using DomainQueryMapper.Tests.TestObjects.Data;
using DomainQueryMapper.Tests.TestObjects.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainQueryMapper.Tests
{
    [TestClass]
    public class Tests
    {
        private List<AppraiserObj> _dataSource;

        [TestInitialize]
        public void Setup()
        {
            DomainQueryMapper.AddMap(new AppraiserDomainMap());

            _dataSource = new List<AppraiserObj>
                              {
                                  new AppraiserObj{Id = 1, Name = "Chris", User = new UserObj{Id = 1, IsActive = true, Username = "cmccown"}},
                                  new AppraiserObj{Id = 2, Name = "Gill", User = new UserObj{Id = 2, IsActive = false, Username = "gmccown"}},
                                  new AppraiserObj{Id = 3, Name = "Stu", User = new UserObj{Id = 3, IsActive = true, Username = "shartley"}}
                              };
        }

        [TestMethod]
        public void PropertyToProperty_WithMappedProperty()
        {
            var results = Query<Appraiser>(x => x.AppraiserId == 1).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyToProperty_NotMappedProperty()
        {
            var results = Query<Appraiser>(x => x.Name == "Gill").ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 2);
            Assert.AreEqual(results.First().Name, "Gill");
        }

        [TestMethod]
        public void SubPropertyToSubProperty_WithMappedProperty()
        {
            var results = Query<Appraiser>(x => x.User.UserId == 1).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void SubPropertyToSubProperty_NotMappedProperty()
        {
            var results = Query<Appraiser>(x => x.User.Username == "cmccown").ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void SubPropertyToProperty_Binary()
        {
            var results = Query<Appraiser>(x => x.IsActive == false).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 2);
            Assert.AreEqual(results.First().Name, "Gill");
        }

        [TestMethod]
        public void SubPropertyToProperty_Member_False()
        {
            var results = Query<Appraiser>(x => !x.IsActive).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 2);
            Assert.AreEqual(results.First().Name, "Gill");
        }

        [TestMethod]
        public void SubPropertyToProperty_Member_True()
        {
            var results = Query<Appraiser>(x => x.IsActive).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyToMethodResult_NotMappedProperty()
        {
            var results = Query<Appraiser>(x => x.AppraiserId == TestValueMethods.AppraiserId()).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyToMethodResultProperty_WithMappedProperty()
        {
            var results = Query<Appraiser>(x => x.User.UserId == TestValueMethods.GetUser().UserId).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        private IEnumerable<AppraiserObj> Query<TTo>(Expression<Func<TTo, bool>> query)
        {
            return _dataSource.AsQueryable().MappedWhere(query).ToList();
        }
    }
}
