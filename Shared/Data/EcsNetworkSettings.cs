namespace Game.Ecs.Network.Shared.Data
{
    using System;
    using NetworkCommands.Data;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class EcsNetworkSettings
    {
        public const string ServerSettings = "server settings";
        public const string ClientSettings = "client settings";
        public const string TypesSettings = "network types";
        public const string CommonSettings = "common settings";
        
        public bool enableDebug = false;
     
        [TitleGroup(ServerSettings)]
        public string serverAddress = "127.0.0.1";
        [TitleGroup(ServerSettings)]
        public ushort serverPort = 0;
        [TitleGroup(ServerSettings)]
        public int networkTickRate = 20;
        [TitleGroup(ServerSettings)]
        public bool AutoStartServer;
        
        [TitleGroup(CommonSettings)]
        [Range(1,50)]
        public int tickHistorySize = 20;
        [TitleGroup(CommonSettings)]
        public NetworkMessageTarget defaultServerTarget = NetworkMessageTarget.NotServer;
        
        [TitleGroup(CommonSettings)]
        [Tooltip("Use hash filtering to reduce network traffic")]
        public bool useHashFiltering = true;
        
        [TitleGroup(CommonSettings)]
        [Tooltip("Use network compression to reduce network traffic")]
        public bool useNetworkCompression = true;
        
        [TitleGroup(CommonSettings)]
        [Tooltip("use multithreaded network processing")]
        public bool multithreaded = true;
        
        [TitleGroup(ClientSettings)]
        [Tooltip("If true, the client will send data each tick, otherwise it will send immediately")]
        public bool useTickSendingOnClient = false;
        [TitleGroup(ClientSettings)]
        public NetworkMessageTarget defaultClientTarget = NetworkMessageTarget.Server;

        [TitleGroup(ClientSettings)]
        public bool AutoStartClient;
        
        [TitleGroup(TypesSettings)]
        [PropertySpace(8)]
        [HideLabel]
        [InlineProperty]
        public NetworkData networkData = new();
        
    }
}