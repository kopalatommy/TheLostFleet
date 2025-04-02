// using Unity.Collections;
// using Unity.Collections.LowLevel.Unsafe;
// using System;

// namespace GalacticBoundStudios.DataScribes.Unmanaged
// {
//     [NativeContainer]
//     public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
//     {
//         public int Length => length;

//         [NativeDisableUnsafePtrRestriction]
//         private T* buffer;
//         private int length;

//         public T this[int index]
//         {
//             get
//             {
//                 if (index < 0 || index >= length)
//                 {
//                     throw new IndexOutOfRangeException();
//                 }

//                 return buffer[index];
//             }
//             set
//             {
//                 if (index < 0 || index >= length)
//                 {
//                     throw new IndexOutOfRangeException();
//                 }

//                 buffer[index] = value;
//             }
//         }

//         public NativeArray(int length)
//         {
//             if (length <= 0)
//             {
//                 throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0");
//             }

//             this.length = length;
//             buffer = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * length, UnsafeUtility.AlignOf<T>(), Allocator.Persistent);
//         }

//         public static void Resize(ref NativeArray<T> array, int newLength)
//         {
//             if (newLength <= 0)
//             {
//                 throw new ArgumentOutOfRangeException(nameof(newLength), "Length must be greater than 0");
//             }

//             T* newBuffer = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * newLength, UnsafeUtility.AlignOf<T>(), Allocator.Persistent);

//             for (int i = 0; i < Math.Min(array.length, newLength); i++)
//             {
//                 newBuffer[i] = array.buffer[i];
//             }

//             UnsafeUtility.Free(array.buffer, Allocator.Persistent);
//             array.buffer = newBuffer;
//             array.length = newLength;
//         }

//         public void Dispose()
//         {
//             UnsafeUtility.Free(buffer, Allocator.Persistent);
//             buffer = null;
//             length = 0;
//         }
//     }
// }