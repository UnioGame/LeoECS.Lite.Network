namespace Game.Ecs.Network.NetworkCommands
{
    using System;
    using Cysharp.Threading.Tasks;
    using Data;
    using Extensions;
    using Leopotam.EcsLite;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;

    [Serializable]
    public class NetworkCommandsFeature : LeoEcsFeature
    {

        protected sealed override UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();
            
            var messageToolsSystem = new NetworkMessageToolsSystem();
            EcsNetworkCommandsExtensions.messageTools = messageToolsSystem;
            
            world.SetGlobal(messageToolsSystem);
            
            //process network commands and create ecs data
            ecsSystems.Add(messageToolsSystem);
            
            return UniTask.CompletedTask;
        }
    }

}