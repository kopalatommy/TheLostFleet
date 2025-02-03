using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace GalacticBoundStudios.DataScribes
{
    [BurstCompile(CompileSynchronously = true)]
    public unsafe struct FixedArray<T> : IDisposable where T : unmanaged
    {
        public int Length => _length;

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= _length)
                {
                    throw new IndexOutOfRangeException();
                }
                return ref _data[index];
            }
        }

        [NativeDisableUnsafePtrRestriction]
        private T* _data;
        private int _length;

        public FixedArray(int length)
        {
            _data = (T*)Marshal.AllocHGlobal(length * sizeof(T));
            _length = length;
        }

        //public FixedArray(T[] array)
        //{
        //    _data = (T*)Marshal.AllocHGlobal(array.Length * sizeof(T));
        //    _length = array.Length;

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        _data[i] = array[i];
        //    }
        //}

        public FixedArray(params T[] items) 
        {
            _data = (T*)Marshal.AllocHGlobal(items.Length * sizeof(T));
            _length = items.Length;
            for (int i = 0; i < items.Length; i++)
            {
                _data[i] = items[i];
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)_data);
            _data = null;
            _length = 0;
        }
    }
}
