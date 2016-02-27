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

        private RbacPrincipal _owner;
        private RbacPrincipal _member;
        private RbacPrincipal _user;
        private RbacPrincipal _evaluator;
        private RbacPrincipal _teacher;
        private RbacPrincipal _bob;
        private RbacSession _rbac;
        [SetUp]
        public void Init()
        {
            _rbac = new RbacSession();
            _rbac.AddPermission("owner", "Delete");
            _rbac.AddPermission("owner", "Transfer");
            _rbac.AddPermission("member", "Comment");
            _rbac.AddPermission("member", "Create");
            _rbac.AddPermission("user", "Read");
            _rbac.AddPermission("mantainer", "Maintnance");
            _rbac.UserIsInRoleIf("mantainer", u => u.Identity.Name == "Bob");

            _rbac.AddPermission("mantainer", "Maintnance");
            _rbac.AddPermission("Teacher", "Grading");
            _rbac.AddPermission("Evaluator", "evaluation");
            _rbac.AddUserRoleForTypeIf<string>("Teacher", (user, resource) => resource == "Hello world");

            _owner = new RbacPrincipal(new Principal
            {
                Roles = { "owner", "member", "user" }
            }, _rbac);
            _member = new RbacPrincipal(new Principal
            {
                Roles = { "member", "user" }
            }, _rbac);
            _user = new RbacPrincipal(new Principal
            {
                Roles = { "member", "user" }
            }, _rbac);
            _evaluator = new RbacPrincipal(new Principal
            {
                Roles = { "evaluator" }
            }, _rbac);
            _teacher = new RbacPrincipal(new Principal
            {
                Roles = { "teacher" }
            }, _rbac);
            _bob = new RbacPrincipal(new Principal
            {
                Name = "Bob",
                Roles = { "owner" }
            }, _rbac);
        }


        [Test]
        public void TestHasRole()
        {
            Assert.IsTrue(_owner.Is["owner"]);
            Assert.IsTrue(_member.Is["member"]);
            Assert.IsTrue(_member.Is["User"]);
            Assert.IsFalse(_member.Is["owner"]);
            Assert.IsTrue(_user.Is["User"]);
            Assert.IsTrue(_user.Is["member"]);
            Assert.IsTrue(_bob.Is["mantainer"]);

            Assert.IsTrue(_teacher.Is["Teacher","the exam"]);
            Assert.IsFalse(_user.Is["Teacher", "the exam"]);
            Assert.IsTrue(_user.Is["Teacher", "Hello world"]);
        }

        [Test]
        public void TestCanDo()
        {
            Assert.IsTrue(_owner.CanDo["Delete"]);
            Assert.IsTrue(_owner.CanDo["transfer"]);
            Assert.IsTrue(_owner.CanDo["comment"]);
            Assert.IsTrue(_owner.CanDo["Create"]);
            Assert.IsFalse(_owner.CanDo["Maintnance"]);
            
            Assert.IsTrue(_member.CanDo["Create"]);
            Assert.IsTrue(_member.CanDo["read"]);

            Assert.IsTrue(_user.CanDo["Create"]);
            Assert.IsFalse(_user.CanDo["Delete"]);
            Assert.IsFalse(_user.CanDo["transfer"]);

            Assert.IsTrue(_bob.CanDo["Maintnance"]);
            
            Assert.IsTrue(_evaluator.CanDo["Evaluation", "anything :p"]);
            Assert.IsTrue(_evaluator.CanDo["Evaluation", 1]);
            Assert.IsTrue(_teacher.CanDo["grading", "the exam"]);
            Assert.IsTrue(_user.CanDo["grading", "Hello world"]); 
            Assert.IsFalse(_user.CanDo["grading", "protected resource to other user"]);

            Assert.IsFalse(_user.CanDo["Delete"]);
            Assert.IsFalse(_user.CanDo["transfer"]);
            Assert.IsTrue(_user.CanDo["Create"]);
        }
    }
}
