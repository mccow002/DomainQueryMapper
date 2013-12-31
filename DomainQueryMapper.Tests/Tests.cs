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
            DomainQueryAtlas.AddMap(new AppraiserDomainMap());
            DomainQueryAtlas.AddMap(new StateDomainMap());
            DomainQueryAtlas.AddMap(new UserDomainMap());

            BuildMockDataSource();
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
        [Description("x => x.IsActive")]
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

        [TestMethod]
        public void PropertyGreaterThanCheckAndToLocalVariable_NotMappedProperty()
        {
            var appraiserId = TestValueMethods.AppraiserId();
            var results = this.Query<Appraiser>(x => x.AppraiserId > 0 && x.AppraiserId == appraiserId).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyGreaterThanCheckOrToLocalVariable_NotMappedProperty()
        {
            var appraiserId = TestValueMethods.AppraiserId();
            var results = this.Query<Appraiser>(x => x.AppraiserId > 2 || x.AppraiserId == appraiserId).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Id, 1);
            Assert.AreEqual(results[0].Name, "Chris");
            Assert.AreEqual(results[1].Id, 3);
            Assert.AreEqual(results[1].Name, "Stu");
        }

        [TestMethod]
        public void PropertyFalseAndPropertyEquals_MappedProperty()
        {
            var results = this.Query<Appraiser>(x => !x.IsActive && x.AppraiserId == 2).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 2);
            Assert.AreEqual(results.First().Name, "Gill");
        }

        [TestMethod]
        public void PropertyMethodCall_NoMappedProperty()
        {
            var results = Query<Appraiser>(x => x.Name.Contains("Ch")).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyChainedMethodCall_NoMappedProperty()
        {
            var results = Query<Appraiser>(x => x.Name.ToLower().Contains("ch")).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyMethodCallWithNestedCalls_NoMappedProperty()
        {
            var results = Query<Appraiser>(x => x.Name.ToLower() == "CHRIS".ToLower()).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void ListPropertySearch_Equals_WithMappedProperty()
        {
            var results = Query<Appraiser>(x => x.States.FirstOrDefault(y => y.StateId == 1) != null).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void ListPropertySearch_Contains_WithMappedProperty()
        {
            var results = Query<Appraiser>(x => x.States.FirstOrDefault(y => y.Name.Contains("Al")) != null).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.First().Id, 1);
            Assert.AreEqual(results.First().Name, "Chris");
        }

        [TestMethod]
        public void PropertyEqualThreeParts_ReturnsOne()
        {
            var results = this.Query<Appraiser>(x => x.IsActive && x.Title == "Developer" && x.Age == 28).ToList();

            Assert.AreEqual(results.Count(), 1);
            Assert.AreEqual(results.First().Id, 3);
            Assert.AreEqual(results.First().Name, "Stu");
        }

        [TestMethod]
        public void PropertyEqualThreeParts_ReturnsNone()
        {
            var results = this.Query<Appraiser>(x => x.Title == "Developer" && !x.IsActive && x.Age == 28).ToList();

            Assert.AreEqual(results.Count(), 0);
        }

        [TestMethod]
        public void PropertyTrueAndPropertyEqualsOrPropertyEquals_ReturnsTwo()
        {
            var results = this.Query<Appraiser>(x => !x.IsActive && (x.Title == "Developer" || x.Age == 28)).ToList();

            Assert.AreEqual(1, results.Count());
        }

        private IEnumerable<AppraiserObj> Query<TTo>(Expression<Func<TTo, bool>> query)
        {
            return _dataSource.AsQueryable().MappedWhere(query).ToList();
        }

        private void BuildMockDataSource()
        {
            var alabama = new StateObj { Id = 1, Name = "Alabama" };
            var georgia = new StateObj { Id = 2, Name = "Georgia" };
            var tennessee = new StateObj { Id = 3, Name = "Tennesee"};

            _dataSource = new List<AppraiserObj>
                {
                    new AppraiserObj
                        {
                            Id = 1,
                            Name = "Chris",
                            Age = 27,
                            Title = "Developer",
                            User = new UserObj
                                {
                                    Id = 1,
                                    IsActive = true,
                                    Username = "cmccown"
                                },
                            States = new List<StateObj>
                                {
                                    alabama,
                                    georgia
                                }
                        },
                    new AppraiserObj
                        {
                            Id = 2,
                            Name = "Gill",
                            Age = 28,
                            Title = "Boss",
                            User = new UserObj
                                {
                                    Id = 2,
                                    IsActive = false,
                                    Username = "gmccown"
                                }
                            ,
                            States = new List<StateObj>
                                {
                                    alabama,
                                    georgia,
                                    tennessee
                                }
                        },
                    new AppraiserObj
                        {
                            Id = 3,
                            Name = "Stu",
                            Age = 28,
                            Title = "Developer",
                            User = new UserObj
                                {
                                    Id = 3,
                                    IsActive = true,
                                    Username = "shartley"
                                }
                            ,
                            States = new List<StateObj>
                                {
                                    georgia
                                }
                        }
                };
        }
    }
}
