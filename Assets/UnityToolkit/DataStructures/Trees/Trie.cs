using System;

namespace ProjectWorlds.DataStructures.Trees
{
    // A trie is a tree-like data structure whose nodes store the letters of an alphabet
    public class Trie : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable
    {
        protected class Node
        {
            // Indicates if a word ends at this node
            public bool isValue;
            // Collection of child nodes
            public System.Collections.Generic.Dictionary<char, Node> children;

            public Node()
            {
                isValue = false;
                children = new System.Collections.Generic.Dictionary<char, Node>();
            }

            public Node(bool isValue)
            {
                this.isValue = isValue;
                children = new  System.Collections.Generic.Dictionary<char, Node>();
            }
        }
        
        protected class TrieEnumerator : System.Collections.IEnumerator, System.Collections.Generic.IEnumerator<string>
        {
            protected ProjectWorlds.DataStructures.Queues.Queue<string> queue = null;
            string value = string.Empty;
            Trie tree;

            public TrieEnumerator(Trie tree)
            {
                this.tree = tree;
                queue = new ProjectWorlds.DataStructures.Queues.Queue<string>();
                System.Collections.Generic.ICollection<string> collection = queue;
                tree.ToCollection(collection);
            }
            
            public string Current
            {
                get
                {
                    return value;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return value;
                }
            }

            public void Dispose()
            {
                queue.Clear();
            }

            public bool MoveNext()
            {
                if (queue.IsEmpty)
                {
                    return false;
                }
                value = queue.Dequeue();
                return true;
            }

            public void Reset()
            {
                queue = new ProjectWorlds.DataStructures.Queues.Queue<string>();
                System.Collections.Generic.ICollection<string> collection = queue;
                tree.ToCollection(collection);
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return count == 0;
            }
        }

        protected Node root;
        protected int count;

        public Trie()
        {
            root = new Node();
            count = 0;
        }

        public void Add(string key)
        {
            if (key == null || key.Length == 0)
            {
                return;
            }

            Node current = root;
            foreach (char c in key)
            {
                if (!current.children.ContainsKey(c))
                {
                    current.children.Add(c, new Node());
                }
                current = current.children[c];
            }

            if (!current.isValue)
            {
                current.isValue = true;
                count++;
            }
        }

        public bool Contains(string key)
        {
            if (key == null || key.Length == 0)
            {
                return false;
            }

            Node current = root;
            foreach (char c in key)
            {
                if (!current.children.ContainsKey(c))
                {
                    return false;
                }
                current = current.children[c];
            }

            return current.isValue;
        }

        public void Remove(string key)
        {
            if (key == null || key.Length == 0)
            {
                return;
            }

            if (RemoveInternal(key))
            {
                count--;
            }
        }

        protected bool RemoveInternal(string key)
        {
            Node current = root;
            foreach (char c in key)
            {
                if (!current.children.ContainsKey(c))
                {
                    return false;
                }
                current = current.children[c];
            }

            if (current.isValue)
            {
                current.isValue = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            root.children.Clear();
            count = 0;
        }

        public void ToCollection(System.Collections.Generic.ICollection<string> collection)
        {
            ToCollection(root, "", collection);
        }

        protected void ToCollection(Node node, string prefix, System.Collections.Generic.ICollection<string> collection)
        {
            if (node.isValue)
            {
                collection.Add(prefix);
            }

            foreach (char c in node.children.Keys)
            {
                ToCollection(node.children[c], prefix + c, collection);
            }
        }

        public System.Collections.Generic.IEnumerator<string> GetEnumerator()
        {
            return new TrieEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new TrieEnumerator(this);
        }
    }
}