namespace ProjectWorlds.DataStructures.Queues
{
    public class Queue<T> : ProjectWorlds.DataStructures.Lists.LinkedList<T>, IQueue<T>
    {
        // Add an item to the queue
        public void Enqueue(T item)
        {
            Add(item);
        }

        // Remove and return the first item in the queue
        public T Dequeue()
        {
            return TakeFirst();
        }

        // Return the first item in the queue
        public T Peek()
        {
            return Get(0);
        }
    }
}