namespace Game.Ecs.Network.Shared
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Feature/Network/Network Shared Feature", fileName = "Network Shared Feature")]
    public class NetworkSharedFeature : BaseLeoEcsFeature
    {
        public sealed override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();

            return UniTask.CompletedTask;
        }
    }

}