using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using NUnit.Framework;
using Toolbox.Rbac;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestSpecializedRbac
    {
        private Dictionary<string, IPrincipal> Users;
        private Rbac _rbac;
        [SetUp]
        public void Init()
        {
            Users = new Dictionary<string, IPrincipal>();
            Users.Add("owner", new Principal
            {
                Roles = { "owner", "member", "user" }
            });
            Users.Add("member", new Principal
            {
                Roles = { "member", "user" }
            });
            Users.Add("user", new Principal
            {
                Roles = { "user" }
            });
            Users.Add("evaluator", new Principal
            {
                Roles = { "evaluator" }
            });
            Users.Add("teacher", new Principal
            {
                Roles = { "teacher" }
            });
            Users.Add("Bob", new Principal
            {
                Name = "Bob",
                Roles = { "owner" }
            });

            _rbac = new Rbac(new SpecializedRbacSession());
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
            Assert.IsTrue(_rbac.Is.User(Users["owner"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(Users["member"]).A("member").Result);
            Assert.IsTrue(_rbac.Is.User(Users["member"]).A("user").Result);
            Assert.IsFalse(_rbac.Is.User(Users["member"]).A("owner").Result);
            Assert.IsTrue(_rbac.Is.User(Users["user"]).A("user").Result);
            Assert.IsFalse(_rbac.Is.User(Users["user"]).A("member").Result);
            Assert.IsTrue(_rbac.Is.User(Users["Bob"]).A("mantainer").Result);
        }

        [Test]
        public void TestCanDo()
        {
            Assert.IsTrue(_rbac.Can.User(Users["owner"]).Do("Delete").Result);
            Assert.IsTrue(_rbac.Can.User(Users["owner"]).Do("transfer").Result);
            Assert.IsTrue(_rbac.Can.User(Users["owner"]).Do("comment").Result);
            Assert.IsTrue(_rbac.Can.User(Users["owner"]).Do("Create").Result);
            Assert.IsFalse(_rbac.Can.User(Users["owner"]).Do("Maintnance").Result);

            Assert.IsTrue(_rbac.Can.User(Users["member"]).Do("Create").Result);
            Assert.IsTrue(_rbac.Can.User(Users["member"]).Do("read").Result);

            Assert.IsTrue(_rbac.Can.User(Users["user"]).Do("read").Result);
            Assert.IsFalse(_rbac.Can.User(Users["user"]).Do("Delete").Result);
            Assert.IsFalse(_rbac.Can.User(Users["user"]).Do("transfer").Result);

            Assert.IsTrue(_rbac.Can.User(Users["Bob"]).Do("Maintnance").Result);

            Assert.IsTrue(_rbac.Can.User(Users["evaluator"]).Do("Evaluation").The("anything :p"));
            Assert.IsTrue(_rbac.Can.User(Users["evaluator"]).Do("Evaluation").The(1));
            Assert.IsTrue(_rbac.Can.User(Users["teacher"]).Do("grading").The("the exam"));
            Assert.IsTrue(_rbac.Can.User(Users["user"]).Do("grading").The("Hello world"));
            Assert.IsFalse(_rbac.Can.User(Users["user"]).Do("grading").The("protected resource to other user"));
        }

        [Test]
        public void TestCanUserDo()
        {
            var actions = new[] { "Read", "Transfer", "Delete", "Comment", "Create" };
            Assert.IsTrue(_rbac.What.Can.User.Do(Users["owner"]).ToList().All(a => actions.Contains(a)));
        }

        [Test]
        public void TestCanRoleDo()
        {
            var actions = new[] { "Read", "Transfer", "Delete", "Comment", "Create" };
            Assert.IsTrue(_rbac.What.Can.Role.Do("owner").ToList().All(a => actions.Contains(a)));
        }
    }
}
