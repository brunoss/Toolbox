using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ToolBox.Utils;

namespace ToolBox.Test
{
    public class Model : ITreeItem
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Text { get; set; }
        public object Key
        {
            get { return ID; }
        }

        public object ParentKey
        {
            get { return ParentID; }
            set { ParentID = (int?)value; }
        }
    }

    public class TestTree
    {
        private Tree<Model> _tree;
        private List<Model> _list; 

        [SetUp]
        public void Init()
        {
            _list = new List<Model>(8)
            {
                new Model() {ID = 1, Text = "Item 1"},
                new Model() {ID = 2, Text = "Item 2"},
                new Model() {ID = 3, Text = "Item 1.1", ParentID = 1},
                new Model() {ID = 4, Text = "Item 1.2", ParentID = 1},
                new Model() {ID = 7, Text = "Item 1.2.1", ParentID = 4},
                new Model() {ID = 5, Text = "Item 2.1", ParentID = 2},
                new Model() {ID = 6, Text = "Item 1.2.2", ParentID = 4},
                new Model() {ID = 8, Text = "Item 1.2.1.1", ParentID = 7}
            };
            _tree = new Tree<Model>(_list);
        }

        [Test]
        public void HasSameCount()
        {
            Assert.AreEqual(_list.Count, _tree.Count());
        }

        [Test]
        public void HasParentElements()
        {
            var parents = _tree.Children;
            Assert.AreEqual(2, parents.Count());
            Assert.AreEqual(3, parents.Sum(p => p.Item.ID));
        }

        [Test]
        public void HasChildren()
        {
            var children = _tree.Children.SelectMany(t => t.Children);
            Assert.AreEqual(3, children.Count());
            Assert.AreEqual(12, children.Sum(p => p.Item.ID));

            children = children.SelectMany(t => t.Children);
            Assert.AreEqual(2, children.Count());
            Assert.AreEqual(13, children.Sum(p => p.Item.ID));
        }

        [Test]
        public void TestAdd()
        {
            _tree = new Tree<Model>();
            foreach (var item in _list)
            {
                _tree.Add(item);
            }
            HasSameCount();
            HasParentElements();
            HasChildren();
        }

        [Test]
        public void TestRemoveAll()
        {
            foreach (var item in _list)
            {
                _tree.Remove(item);
            }
            Assert.AreEqual(0, _tree.Count());
        }

        [Test]
        public void TestRemoveLast()
        {
            var last = _list.Last();
            var removed = _tree.Remove(last);
            Assert.AreEqual(1, removed.Count());
            Assert.AreSame(removed.FirstOrDefault(), last);
        }

        [Test]
        public void TestRemoveItemAndChildren()
        {
            var item = _list[3];
            Assert.IsTrue(_tree.RemoveItemAndChildren(item));
            Assert.AreEqual(_list.Count - 4, _tree.Count());
        }
    }
}
