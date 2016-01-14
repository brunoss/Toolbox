using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Toolbox.Rbac;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestRbac
    {
        public class Principal : IPrincipal, IIdentity
        {
            private readonly HashSet<string> _roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public IIdentity Identity
            {
                get
                {
                    return this;
                }
            }

            public ICollection<string> Roles { get { return _roles; } }

            public bool IsInRole(string role)
            {
                return _roles.Contains(role);
            }

            public string Name { get; set; }
            public string AuthenticationType { get; set; }
            public bool IsAuthenticated { get; set; }
        }

        private Dictionary<string, IPrincipal> _users;
        private Rbac _rbac;
        [SetUp]
        public void Init()
        {
            _users = new Dictionary<string, IPrincipal>();
            _users.Add("owner", new Principal
            {
                Roles = { "owner", "member", "user" }
            });
            _users.Add("member", new Principal
            {
                Roles = { "member", "user" }
            });
            _users.Add("user", new Principal
            {
                Roles = { "user" }
            });
            _users.Add("evaluator", new Principal
            {
                Roles = { "evaluator" }
            });
            _users.Add("teacher", new Principal
            {
                Roles = { "teacher" }
            });
            _users.Add("Bob", new Principal
            {
                Name = "Bob",
                Roles = { "owner" }
            });

            _rbac = new Rbac(new RbacSession());
            _rbac.Do.A("Delete").Requires("owner");
            _rbac.Do.A("Transfer").Requires("owner");
            _rbac.Do.A("Comment").Requires("member");
            _rbac.Do.A("Create").Requires("member");
            _rbac.Do.A("Read").Requires("user");
            _rbac.Do.A("Maintnance").Requires("mantainer");
            _rbac.User.Is("mantainer").If(u => u.Identity.Name == "Bob");

            _rbac.Do.A("Evaluation").Requires("Evaluator");
            _rbac.Do.A("Grading").Requires("Teacher");
            _rbac.User.Is("Teacher").Of<string>().If((user, resource) => resource=="Hello world");
        }


        [Test]
        public void TestHasRole()
        {
            Assert.IsTrue(_rbac.Is.User(_users["owner"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(_users["member"]).A("Member").Result);
            Assert.IsTrue(_rbac.Is.User(_users["member"]).A("User").Result);
            Assert.IsFalse(_rbac.Is.User(_users["member"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(_users["user"]).A("user").Result);
            Assert.IsFalse(_rbac.Is.User(_users["user"]).A("member").Result);
            Assert.IsTrue(_rbac.Is.User(_users["Bob"]).A("mantainer").Result);

            Assert.IsTrue(_rbac.Is.User(_users["teacher"]).A("Teacher").Of("the exam"));
            Assert.IsFalse(_rbac.Is.User(_users["user"]).A("teacher").Of("the exam"));
            Assert.IsTrue(_rbac.Is.User(_users["user"]).A("Teacher").Of("Hello world"));
        }

        [Test]
        public void TestCanDo()
        {
            Assert.IsTrue(_rbac.Can.User(_users["owner"]).Do("Delete").Result);
            Assert.IsTrue(_rbac.Can.User(_users["owner"]).Do("transfer").Result);
            Assert.IsTrue(_rbac.Can.User(_users["owner"]).Do("comment").Result);
            Assert.IsTrue(_rbac.Can.User(_users["owner"]).Do("Create").Result);
            Assert.IsFalse(_rbac.Can.User(_users["owner"]).Do("Maintnance").Result);

            Assert.IsTrue(_rbac.Can.User(_users["member"]).Do("Create").Result);
            Assert.IsTrue(_rbac.Can.User(_users["member"]).Do("read").Result);

            Assert.IsTrue(_rbac.Can.User(_users["user"]).Do("read").Result);
            Assert.IsFalse(_rbac.Can.User(_users["user"]).Do("Delete").Result);
            Assert.IsFalse(_rbac.Can.User(_users["user"]).Do("transfer").Result);

            Assert.IsTrue(_rbac.Can.User(_users["Bob"]).Do("Maintnance").Result);

            Assert.IsTrue(_rbac.Can.User(_users["evaluator"]).Do("Evaluation").The("anything :p"));
            Assert.IsTrue(_rbac.Can.User(_users["evaluator"]).Do("Evaluation").The(1));
            Assert.IsTrue(_rbac.Can.User(_users["teacher"]).Do("grading").The("the exam"));
            Assert.IsTrue(_rbac.Can.User(_users["user"]).Do("grading").The("Hello world"));
            Assert.IsFalse(_rbac.Can.User(_users["user"]).Do("grading").The("protected resource to other user"));
        }
    }
}
