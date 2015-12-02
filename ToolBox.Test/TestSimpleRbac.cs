using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Toolbox.Rbac;

namespace ToolBox.Test
{
    public class Principal : IPrincipal, IIdentity
    {
        private HashSet<string> _roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

    [TestFixture]
    public class TestSimpleRbac
    {
        private Dictionary<string, IPrincipal> Users = new Dictionary<string, IPrincipal>(); 
        private Rbac _rbac;
        [SetUp]
        public void Init()
        {
            Users.Add("owner", new Principal()
            {
                Roles = {"owner", "member", "user"}
            });
            Users.Add("member", new Principal()
            {
                Roles = {"member", "user" }
            });
            Users.Add("user", new Principal()
            {
                Roles = { "user" }
            });
            Users.Add("Bob", new Principal()
            {
                Name = "Bob",
                Roles = { "owner" }
            });

            _rbac = new Rbac(new SimpleRbacSession());
            _rbac.Do.A("Delete").Requires("owner");
            _rbac.Do.A("Transfer").Requires("owner");
            _rbac.Do.A("Comment").Requires("member");
            _rbac.Do.A("Create").Requires("member");
            _rbac.Do.A("Read").Requires("user");
            _rbac.Do.A("Maintnance").Requires("mantainer");
            _rbac.User.Is("mantainer").If(u => u.Identity.Name == "Bob");
        }

        [Test]
        public void TestHasRole()
        {
            Assert.IsTrue(_rbac.Is.User(Users["owner"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(Users["member"]).A("member").Result);
            Assert.IsTrue(_rbac.Is.User(Users["member"]).A("user").Result);
            Assert.IsFalse(_rbac.Is.User(Users["member"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(Users["user"]).A("user").Result);
            Assert.IsFalse(_rbac.Is.User(Users["user"]).A("member").Result);
            Assert.IsTrue(_rbac.Is.User(Users["Bob"]).A("mantainer").Result);
        }
    }
}
