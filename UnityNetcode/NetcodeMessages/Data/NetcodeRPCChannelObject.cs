namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Data
{
    using Components;
    using MemoryPack;
    using NetworkCommands.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using Unity.Netcode;
    using UnityEngine;

    public class NetcodeRPCChannelObject : NetworkBehaviour
    {
        
        [Rpc(SendTo.NotServer,AllowTargetOverride = true)]
        public void SendMessageRPC(byte[] data,RpcParams rpcParams)
        {
            var result = MemoryPackSerializer.Deserialize<string>(data);
            Debug.Log($"SendFromServerRPC: {result}");
        }
        
        [Rpc(SendTo.NotServer,AllowTargetOverride = true)]
        public void SendRPC(byte[] data,int size,RpcParams rpcParams)
        {
            var world = LeoEcsGlobalData.World;
            var entity = world.NewEntity();
            
            ref var rpcDataComponent = ref world.AddComponent<NetworkMessageDataComponent>(entity);
            rpcDataComponent.Value = data;
            rpcDataComponent.Size = size;
        }
        
        [Rpc(SendTo.Server)]
        public void SendToServerRPC(byte[] data)
        {
            
        }
        
    }
}