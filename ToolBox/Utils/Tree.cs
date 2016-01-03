using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ToolBox.Annotations;

namespace ToolBox.Utils
{
    public interface ITreeItem
    {
        object Key { get;  }
        object ParentKey { get; set; }
    }
    
    public class Tree<T> : IEnumerable<T>
        where T: ITreeItem
    {
        private readonly IDictionary<object, Tree<T>> _items = new Dictionary<object, Tree<T>>(7);
        public T Item { get; set; }

        public Tree()
        {
        }  
             
        public Tree(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                var parent = Parent(value);
                if (parent == null)
                {
                    throw new ArgumentException("Couldn't find the parent of element with key " + value.Key);
                }
                Parent(value)?.AddAsSubTree(value);
            }
        }

        public IEnumerable<Tree<T>> Children
        {
            get { return _items.Values; }
        }

        /// <summary>
        /// Finds the parent of an item.
        /// If the item.ParentKey == null then the parent is the self
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Tree<T> Parent(T item)
        {
            if (item.ParentKey == null)
            {
                return this;
            }
            if(_items.Count == 0)
            {
                return null;
            }
            var child = _items.TryGetOrValue(item.ParentKey, null);
            if (child == null)
            {
                return _items.Values
                    .Select(v => v.Parent(item))
                    .FirstOrDefault(v => v != null);
            }
            else
            {
                return child;
            }
        }

        public Tree<T> Parent(T item, int level)
        {
            if (level == 1)
            {
                return this;
            }
            return ChildrenInLevel(level - 1)
                .FirstOrDefault(t => t._items.ContainsKey(item.ParentKey));
        } 

        protected Tree<T> AddAsSubTree(T item)
        {
            var tree = new Tree<T>
            {
                Item = item
            };
            _items.Add(item.Key, tree);
            return tree;
        }

        protected IEnumerable<T> AllChildren(Tree<T> root)
        {
            foreach (var item in root._items.Values)
            {
                yield return item.Item;
                foreach (var aux in AllChildren(item))
                {
                    yield return aux;
                }
            }
        }

        protected IEnumerable<Tree<T>> ChildrenInLevel(int level)
        {
            var children = Children;
            while (--level > 0)
            {
                children = children.SelectMany(t => t._items.Values);
            }
            return children;
        }

        /// <summary>
        /// Adds the item to its parent
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The subtree of the item</returns>
        public Tree<T> Add(T item)
        {
            return Parent(item).AddAsSubTree(item);
        }

        /// <summary>
        /// Adds the item to its parent
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The subtree of the item</returns>
        public Tree<T> Add(T item, int level)
        {
            return Parent(item, level).AddAsSubTree(item);
        }

        private bool RemoveItemAndChildren(Tree<T> parent, T item)
        {
            if (parent._items.Remove(item.Key))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Detaches a sub tree, that is, removes the item and all children.
        /// Does not update the ParentKey
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItemAndChildren(T item)
        {
            return RemoveItemAndChildren(Parent(item), item);
        }

        /// <summary>
        /// Detaches a sub tree, that is, removes the item and all children.
        /// Does not update the ParentKey
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the item was sucessfully removed</returns>
        public bool RemoveItemAndChildren(T item, int level)
        {
            return RemoveItemAndChildren(Parent(item, level), item);
        }

        private IEnumerable<T> Remove(Tree<T> parent, T item)
        {
            if (parent == null)
            {
                yield break;
            }
            var children = parent._items[item.Key].Children;
            parent._items.Remove(item.Key);
            item.ParentKey = null;
            yield return item;
            foreach (var child in children)
            {
                parent._items.Add(child.Item.Key, child);
                child.Item.ParentKey = item.ParentKey;
                yield return child.Item;
            }
        }

        /// <summary>
        /// Removes the item from a tree, moving all children of that item to the parent.
        /// This method updates the Parentkey of the item children and of the item removed as well
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns>All modified items</returns>
        public IEnumerable<T> Remove(T item)
        {
            return Remove(Parent(item), item).ToList();
        }

        /// <summary>
        /// Removes the item from a tree level, moving all children of that item to the parent.
        /// This method updates the Parentkey of the item children and of the item removed as well
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns>All modified items</returns>
        public IEnumerable<T> Remove(T item, int level)
        {
            return Remove(Parent(item, level), item).ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return AllChildren(this).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
