using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ToolBox.Test
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    [TestFixture]
    public class TestOrderedGroup
    {
        [Test]
        public void SingleElement()
        {
            var users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Admin"
                }
            };
            var user = users.OrderedGroup(u => u.ID).SelectMany(u => u).ToList();
            Assert.AreSame(user[0], users[0]);
            Assert.AreEqual(user.Count, 1);
        }

        [Test]
        public void GroupsAUser()
        {
            var users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Admin"
                },new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Developper"
                }
            };
            var roles = users.OrderedGroup(u => u.ID)
                .Select(group => string.Join(" ", group.Select(u => u.Role)))
                .ToList();
            Assert.AreEqual(1, roles.Count);
            Assert.AreEqual(2, roles[0].Split(' ').Length);
            //not guaranteed
            Assert.AreEqual("Admin Developper", roles[0]);
        }

        [Test]
        public void GroupsMultiUsers()
        {
            var users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Admin"
                },new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Developper"
                },new User()
                {
                    ID = 2,
                    Name = "Ordered Group",
                    Role = "Method"
                },new User()
                {
                    ID = 2,
                    Name = "Ordered Group",
                    Role = "Elite"
                },
            };
            var roles = users.OrderedGroup(u => u.ID)
                .Select(group => string.Join(" ", group.Select(u => u.Role)))
                .ToList();
            Assert.AreEqual(2, roles.Count);
            Assert.AreEqual(2, roles[0].Split(' ').Length);
            Assert.AreEqual(2, roles[1].Split(' ').Length);
            //not guaranteed
            Assert.AreEqual("Admin Developper", roles[0]);
            Assert.AreEqual("Method Elite", roles[1]);
        }

        [Test]
        public void GroupsMultiUsersVsGroupBy()
        {
            var users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Admin"
                },new User()
                {
                    ID = 1,
                    Name = "Bruno Costa",
                    Role = "Developper"
                },new User()
                {
                    ID = 2,
                    Name = "Ordered Group",
                    Role = "Method"
                },new User()
                {
                    ID = 2,
                    Name = "Ordered Group",
                    Role = "Elite"
                },
            };
            for (int i = 0; i < 10; ++i)
            {
                var roles = users.OrderedGroup(u => u.ID)
                    .Select(group => string.Join(" ", group.Select(u => u.Role)))
                    .ToList();
                var roles2 = users.GroupBy(u => u.ID)
                    .Select(group => string.Join(" ", group.Select(u => u.Role)))
                    .ToList();

                Assert.AreEqual(2, roles.Count);
                Assert.AreEqual(2, roles2.Count);
                Assert.AreEqual(2, roles[0].Split(' ').Length);
                Assert.AreEqual(2, roles[1].Split(' ').Length);
                //not guaranteed
                Assert.AreEqual("Admin Developper", roles[0]);
                Assert.AreEqual("Method Elite", roles[1]);
            }
        }
    }
}
