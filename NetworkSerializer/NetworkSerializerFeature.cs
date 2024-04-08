namespace Girand.Ecs.GameSettings
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Feature/Network Serializer Feature", fileName = "Network Serializer Feature")]
    public class NetworkSerializerFeature : BaseLeoEcsFeature
    {
        public sealed override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            
            return UniTask.CompletedTask;
        }
    }

}