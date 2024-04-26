namespace Game.Ecs.Network.Shared.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using NetworkCommands.Data;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.Utils;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;
    using System;

#if UNITY_EDITOR
    using UnityCodeGen;
    using UnityEditor;
    using UniModules.Editor;
#endif
    
    [CreateAssetMenu(menuName = "Game/Configurations/Network Settings", fileName = "Network Settings")]
    public class EcsNetworkSettingsAsset : ScriptableObject
    {
        [HideLabel]
        [InlineProperty]
        public EcsNetworkSettings networkSettings = new();
        
        private Dictionary<Type,bool> cachedTypes = new();
        
#if UNITY_EDITOR

        
        [Button]
        public void Rebuild()
        {
            UnityCodeGenUtility.Generate();
            BakeNetworkData();
            SetDirty();
        }
        
        [Button]
        public void BakeNetworkData()
        {
            var data = networkSettings.networkData;
            
            Bake<IEcsNetworkValue>(ref data.networkTypes);
            //Bake<IEcsClientNetworkValue>(data.networkTypes);
            BakeClient<IEcsClientNetworkValue>(ref data.clientTypes);
            
            this.MarkDirty();
        }
        
        public IEcsTypeSerializer GetNetworkSerializer(Type type)
        {
            var typeName = type.Name;
            var targetTypes = TypeCache.GetTypesDerivedFrom(typeof(IEcsTypeSerializer));
            foreach (var serializerType in targetTypes)
            {
                if(serializerType.IsAbstract || serializerType.IsInterface) continue;
                if(!serializerType.Name.Contains(typeName,StringComparison.OrdinalIgnoreCase))
                    continue;
                var instance = serializerType.CreateWithDefaultConstructor();
                return instance as IEcsTypeSerializer;
            }

            return default;
        }

        public NetworkSyncType[] Bake<TSourceType>(ref NetworkSyncType[] typeItems)
        {
            var types = TypeCache.GetTypesDerivedFrom(typeof(TSourceType));
            var dataItems = types
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .ToArray();
            
            typeItems = new NetworkSyncType[dataItems.Length];

            for (var i = 0; i < dataItems.Length; i++)
            {
                var type = dataItems[i];
                if (type.IsAbstract || type.IsInterface) continue;

                var id = i;
                var isBlittable = UnsafeUtility.IsBlittable(type);
                typeItems[i] = new NetworkSyncType
                {
                    id = id,
                    type = type,
                    name = type.Name,
                    serializer = GetNetworkSerializer(type),
                    isBlittable = isBlittable,
                    size = isBlittable ? UnsafeUtility.SizeOf(type) : 0,
                };
            }

            return typeItems;
        }
        
        public NetworkSyncType[] BakeClient<TSourceType>(ref NetworkSyncType[] typeItems)
        {
            var types = TypeCache.GetTypesDerivedFrom(typeof(TSourceType))
                .Where(x => x.IsAbstract == false && x.IsInterface == false)
                .ToArray();
            
            var data = networkSettings.networkData;
            var networkTypes = data.networkTypes;
            
            typeItems = new NetworkSyncType[types.Length];
            
            if(types.Length <= 0) return typeItems;

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var networkSyncType = networkTypes
                    .FirstOrDefault(x => x.type == type);

                if (networkSyncType.type == null) continue;

                typeItems[i] = networkSyncType;
            }

            return typeItems;
        }
#endif
    }
}