using System;
using System.Runtime.InteropServices;

namespace ProjectWorlds.DataStructures.Unsafe
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        public int Length => length;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= length)
                {
                    throw new IndexOutOfRangeException();
                }

                return dataPtr[index];
            }
            set
            {
                if (index < 0 || index >= length)
                {
                    throw new IndexOutOfRangeException();
                }

                dataPtr[index] = value;
            }
        }

        private T* dataPtr;
        private int length;

        public UnsafeArray(int length)
        {
            // Make sure that the length is greater than 0
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0");
            }
            
            this.length = length;
            dataPtr = (T*)Marshal.AllocHGlobal(sizeof(T) * length);

            // Initialize the array with default values
            for (int i = 0; i < length; i++)
            {
                dataPtr[i] = default;
            }
        }

        public static void Resize(ref UnsafeArray<T> array, int newLength)
        {
            if (newLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newLength), "Length must be greater than 0");
            }

            if (array.length == newLength)
            {
                return;
            }

            T* newDataPtr = (T*)Marshal.AllocHGlobal(sizeof(T) * newLength);

            // Copy the old data to the new data
            for (int i = 0; i < Math.Min(array.length, newLength); i++)
            {
                newDataPtr[i] = array.dataPtr[i];
            }

            // Initialize the rest of the new data with default values
            for (int i = array.length; i < newLength; i++)
            {
                newDataPtr[i] = default;
            }

            // Free the old data
            Marshal.FreeHGlobal((IntPtr)array.dataPtr);

            // Update the array
            array.dataPtr = newDataPtr;
            array.length = newLength;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)dataPtr);
            dataPtr = null;
            length = 0;
        }
    }
}