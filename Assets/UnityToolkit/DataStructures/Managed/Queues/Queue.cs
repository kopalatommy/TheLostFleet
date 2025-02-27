namespace GalacticBoundStudios.DataScribes.Managed.Queues
{
    public class Queue<T> : GalacticBoundStudios.DataScribes.Managed.Lists.LinkedList<T>, IQueue<T>
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