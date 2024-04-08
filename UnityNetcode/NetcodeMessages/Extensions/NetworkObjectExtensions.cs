namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Extensions
{
    using System.Runtime.CompilerServices;
    using NetworkCommands.Data;
    using Unity.Netcode;

    public static class NetworkObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcParams GetRpcTarget(this NetworkBehaviour target,NetworkMessageTarget messageTarget)
        {
            var rpcTarget = target.RpcTarget;
            var targetResult = messageTarget switch
            {
                NetworkMessageTarget.Server => rpcTarget.Server,
                NetworkMessageTarget.NotServer => rpcTarget.NotServer,
                NetworkMessageTarget.All => rpcTarget.Everyone,
                NetworkMessageTarget.Me => rpcTarget.Me,
                NetworkMessageTarget.NotMe => rpcTarget.NotMe,
                _ => rpcTarget.Server,
            };
            return targetResult;
        }
    }
}