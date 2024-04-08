namespace Game.Ecs.Network.NetworkClient
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime;

    [Serializable]
    public class NetworkClientFeature : LeoEcsFeature
    {
        protected sealed override UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }

}