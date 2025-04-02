using Unity.Burst;

namespace GalacticBoundStudios.DataScribes.Unmanaged
{
    [BurstCompile]
    public struct HeapUtils
    {
        public static int Parent(int index) => (index - 1) / 2;

        public static int LeftChild(int index) => 2 * index + 1;

        public static int RightChild(int index) => 2 * index + 2;

        public static bool IsLeaf(int index, int count) => index >= count / 2 && index < count;
    }
}