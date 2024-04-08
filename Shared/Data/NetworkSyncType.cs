namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using Shared.Data;
    using Sirenix.OdinInspector;
    using UniGame.Core.Runtime.SerializableType;
    using UnityEngine;

    [Serializable]
    public struct NetworkSyncType : ISearchFilterable
    {
        public int id;
        public string name;
        public SType type;
        public int size;
        public bool isBlittable;
        
        [SerializeReference]
        public IEcsTypeSerializer serializer;

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            
            return name.Contains(searchString,StringComparison.OrdinalIgnoreCase);
        }
    }
}