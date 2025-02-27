// Only include is unsafe compilation is allowed
#if UNSAFE

using System.Runtime.InteropServices;
using Unity.Burst;

[BurstCompile]
public unsafe struct FixedArray<T> where T : unmanaged
{
    public ref T this[int index] => ref _data[index];

    private T* _data;
    private int _length;

    public FixedArray(int length)
    {
        _data = (T*)Marshal.AllocHGlobal(length * sizeof(T));
        _length = length;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal((IntPtr)_data);
        _data = null;
        _length = 0;
    }
}

#endif // UNSAFE