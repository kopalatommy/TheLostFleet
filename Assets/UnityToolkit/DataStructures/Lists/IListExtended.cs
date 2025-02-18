namespace ProjectWorlds.DataStructures.Lists
{
    public interface IListExtended<T> : System.Collections.Generic.IList<T>
    {
        // Helper function that checks if the list is empty
        public bool IsEmpty { get; }

        // Set the value at a specific index
        public void Set(int index, T value);

        // Returns the item at the given index
        public T Get(int index);

        // Add a range of values to the list
        public void AddRange(System.Collections.Generic.IEnumerable<T> range);

        // Insert a range of items into the list at the given index
        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range);

        // Insert x items from a range of items into the list at the given index
        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> range, int length);

        // Remove and return the first item in the list
        public T TakeFirst();

        // Remove and return the last item in the list
        public T TakeLast();

        // Remove the first item in the list
        public void RemoveFirst();

        // Remove the last item in the list
        public void RemoveLast();

        // Return the first item in the list
        public T First();

        // Return the last item in the list
        public T Last();

        // Remove a range of item from the list
        public void RemoveRange(int index, int length);

        // Remove all instances of value from the list
        public void RemoveAll(T value);

        // Removes the first instance of value. Returns true if the value was removed
        public bool RemoveFirst(T value);

        // Removes the last instance of the value. Returns true if the value was removed
        public bool RemoveLast(T value);

        // ToDo, AddAfter, add item after a specific item
    }
}