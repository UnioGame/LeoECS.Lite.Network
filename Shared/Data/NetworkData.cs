namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using Sirenix.OdinInspector;

    [Serializable]
    public class NetworkData
    {
        public int defaultBufferSize = 512;
        public int serializationEntityChunkSize = 1028;
        
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ListDrawerSettings(ListElementLabelName = "@name")]
        public NetworkSyncType[] networkTypes = Array.Empty<NetworkSyncType>();
        
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ListDrawerSettings(ListElementLabelName = "@name")]
        public NetworkSyncType[] clientTypes = Array.Empty<NetworkSyncType>();
    }
}