#if UNITY_STANDALONE

using System.Threading;
using Unity.Burst;

namespace GalacticBoundStudios.DataScribes.Unmanaged.Utilities
{
    [BurstCompile]
    public struct SpinLock
    {
        private int _lock;

        public void Enter()
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _lock, 1, 0) == 0)
                {
                    return;
                }
            }
        }

        public void Exit()
        {
            Interlocked.Exchange(ref _lock, 0);
        }
    }

    [BurstCompile]
    public struct SpinLock_ThreadCount
    {
        private int _lock;
        private int _waitingThreads;

        public void Enter()
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _lock, 1, 0) == 0)
                {
                    return;
                }

                Interlocked.Increment(ref _waitingThreads);
                while (_lock != 0)
                {
                    Thread.Yield();
                }

                Interlocked.Decrement(ref _waitingThreads);
            }
        }

        public void Exit()
        {
            Interlocked.Exchange(ref _lock, 0);
            if (_waitingThreads > 0)
            {
                Thread.MemoryBarrier();
            }
        }
    }
}

#endif // UNITY_STANDALONE