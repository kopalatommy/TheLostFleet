namespace ProjectWorlds.DataStructures.Queues
{
    public interface IQueue<T> : ProjectWorlds.DataStructures.Lists.IListExtended<T>
    {
        // Add an item to the queue
        public void Enqueue(T item);

        // Remove and return the first item in the queue
        public T Dequeue();

        // Return the first item in the queue
        public T Peek();
    }
}