namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System.Runtime.CompilerServices;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.IL2CPP.CompilerServices;
    using Unity.Jobs;

    public static class ByteHashCalculator
    {
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ComputeHash(ref NativeStream.Reader data,int length)
        {
            unchecked
            {
                unsafe
                {
                    var size = length;
                    var dataPtr = data.ReadUnsafePtr(size);
                    const int p = 16777619;
                    var hash = (int)2166136261;

                    for (var i = 0; i < size; i++)
                        hash = (hash ^ dataPtr[i]) * p;

                    return hash;
                }
            }
        }
        
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ComputeHash(ref DataStreamWriter data)
        {
            var dataArray = data.AsNativeArray();
            return ComputeHash(ref dataArray,0, data.Length);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ComputeHash(ref NativeArray<byte> data,int from,int length)
        {
            unchecked
            {
                const int p = 16777619;
                var hash = (int)2166136261;

                for (var i = from; i < length; i++)
                    hash = (hash ^ data[i]) * p;

                return hash;
            }
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ComputeHash(ref NativeList<byte> data,int length)
        {
            unchecked
            {
                const int p = 16777619;
                var hash = (int)2166136261;

                for (var i = 0; i < length; i++)
                    hash = (hash ^ data[i]) * p;

                return hash;
            }
        }
        
        [BurstCompile]
        public struct ByteHashCalculatorJobFor : IJobFor
        {
            [ReadOnly]
            public NativeArray<byte> Data;
            [ReadOnly]
            public NativeArray<ByteRegions> Regions;
            
            public NativeArray<int> Results;
            
            public void Execute(int index)
            {
                var start = Regions[index].Start;
                var end = Regions[index].End;
                
                unchecked
                {
                    const int p = 16777619;
                    var hash = (int)2166136261;

                    for (int i = start; i < end; i++)
                    {
                        hash = (hash ^ Data[i]) * p;
                    }

                    Results[index] = hash;
                }
            }
        }
        
        [BurstCompile]
        public struct ByteHashCalculatorJob : IJob
        {
            [ReadOnly]
            public NativeArray<byte> Data;
            public int Result;
            
            public void Execute()
            {
                unchecked
                {
                    const int p = 16777619;
                    var hash = (int)2166136261;

                    for (var i = 0; i < Data.Length; i++)
                    {
                        hash = (hash ^ Data[i]) * p;
                    }

                    Result = hash;
                }
            }
        }
        
        public struct ByteRegions
        {
            public int Start;
            public int End;
        }
    }
}