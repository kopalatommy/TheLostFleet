namespace ProjectWorlds.DataStructures.Trees
{
    public interface ITree<T>
    {
        int Count { get; }

        bool IsEmpty { get; }

        void Add(T value);

        void Clear();

        bool Contains(T value);

        void Remove(T value);

        public T MinValue { get; }

        public T MaxValue { get; }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection);

        public System.Collections.Generic.IEnumerator<T> GetEnumerator();
    }
}