namespace Game.Ecs.Network.Shared.Profiler
{
    using Unity.Profiling;

    public class EcsNetworkTrafficCounter
    {
        public static readonly ProfilerCategory EcsNetworkCategory = ProfilerCategory.Network;

        public static readonly ProfilerCounter<int> EcsSendTraffic =
            new(EcsNetworkCategory, "ECS Total Send Bytes", ProfilerMarkerDataUnit.Bytes);

        public static ProfilerCounterValue<int> EcsSendTrafficCounter =
            new(EcsNetworkCategory, "ECS Send Byte",
                ProfilerMarkerDataUnit.Bytes, ProfilerCounterOptions.FlushOnEndOfFrame);
    }
}