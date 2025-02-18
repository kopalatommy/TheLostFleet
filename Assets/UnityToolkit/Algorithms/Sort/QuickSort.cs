using System;

namespace ProjectWorlds.Algorithms
{
    public static class QuickSort
    {
        public delegate int CompareFunct<T>(T x, T y);

        public static void Sort<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            if (low < high)
            {
                // Split the array into 2 section
                int p = Partition(array, low, high);

                // Sort ewch section on either side of the partition
                Sort(array, low, p - 1);
                Sort(array, p + 1, high);
            }
        }

        // This function handles correctly sorting a specific element in the array, and then
        // moving all smaller items below it and all greater item above it
        private static int Partition<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            // Choose the last item as the pivot index
            T pivot = array[high];

            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (array[j].CompareTo(pivot) < 0)
                {
                    i++;
                    Swap(array, i, j);
                }
            }
            Swap(array, i + 1, high);
            return i + 1;
        }

        public static void Sort<T>(T[] array, int low, int high, CompareFunct<T> compare)
        {
            if (low < high)
            {
                // Split the array into 2 section
                int p = Partition(array, low, high, compare);

                // Sort ewch section on either side of the partition
                Sort(array, low, p - 1, compare);
                Sort(array, p + 1, high, compare);
            }
        }

        // This function handles correctly sorting a specific element in the array, and then
        // moving all smaller items below it and all greater item above it
        private static int Partition<T>(T[] array, int low, int high, CompareFunct<T> compare)
        {
            // Choose the last item as the pivot index
            T pivot = array[high];

            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (compare(array[j], pivot) < 0)
                {
                    i++;
                    Swap(array, i, j);
                }
            }
            Swap(array, i + 1, high);
            return i + 1;
        }

        private static void Swap<T>(T[] array, int a, int b)
        {
            T temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }
    }
}