namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class EcsNetworkData
    {
        public Dictionary<int,byte[]> SyncTypes = new();
        public Type[] IdTypeMap = Array.Empty<Type>();
        public Dictionary<Type,int> TypesMap = new();
        public NetworkSyncType[] Types = Array.Empty<NetworkSyncType>();
        
        public Type[] ClientIsTypeMap  = Array.Empty<Type>();
        public NetworkSyncType[] ClientTypes = Array.Empty<NetworkSyncType>();
        public Dictionary<Type,int> ClientTypeMap = new();
        public Dictionary<int,int> ClientIdTypeMap = new();

        public bool TryGetServerType(int id, out NetworkSyncType networkSyncType)
        {
            return TryGetValidatedType(id, Types, out networkSyncType);
        }
        
        public bool TryGetClientType(int id, out NetworkSyncType networkSyncType)
        {
            return TryGetValidatedType(id, ClientTypes, out networkSyncType);
        }
        
        private bool TryGetValidatedType(int id,NetworkSyncType[] types, out NetworkSyncType syncType)
        {
            if(id < 0 || id >= types.Length)
            {
                syncType = default;
                return false;
            }
            
            syncType = types[id];
            return syncType.serializer != null;
        }
    }
}