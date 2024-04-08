namespace Game.Ecs.Network.Profiler
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;

    [Serializable]
    public sealed class NetworkEcsProfilerFeature : LeoEcsFeature
    {
        protected override UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();

            ecsSystems.Add(new UpdateEcsNetworkTrafficCounterSystem());

            return UniTask.CompletedTask;
        }
    }

}