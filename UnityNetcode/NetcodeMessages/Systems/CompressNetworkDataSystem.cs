namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Leopotam.EcsLite;
    using MemoryPack.Compression;
    using Network.Serializer;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityNetcode.Aspects;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CompressNetworkDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _netcodeMessageAspect;
        private NetworkMessageAspect _networkMessageAspect;

        private EcsWorld _world;

        private EcsNetworkSettings _networkSettings;
        private EcsFilter _serializeFilter;
        private ArrayBufferWriter<byte> _arrayBufferWriter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            _arrayBufferWriter = new ArrayBufferWriter<byte>(512);

            _serializeFilter = _world
                .Filter<NetworkSerializationResult>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var useCompression = _networkSettings.useNetworkCompression;
            if(!useCompression) return;
            
            foreach (var entity in _serializeFilter)
            {
                ref var serializeResult = ref _networkMessageAspect
                    .SerializationResult.Get(entity);

                var size = serializeResult.Size;
                var arrayBuffer = serializeResult.Value;
                var fullArraySlice = arrayBuffer.AsSpan()[..size];
                
                _arrayBufferWriter.Clear();
                
                using var compressor = new BrotliCompressor();
                
                compressor.Write(fullArraySlice);
                compressor.CopyTo(_arrayBufferWriter);
                size = _arrayBufferWriter.WrittenCount;
                
                var writtenData = _arrayBufferWriter.WrittenSpan;
                serializeResult.Value.WriteData(ref writtenData, 0);
                serializeResult.Size = size;
                
                // ref var messageData = ref _networkMessageAspect.MessageData.Get(entity);
                // messageData.Size = size;
                // messageData.Value = serializeResult.Value.ToArray();
            }
        }
       
    }
    
   
}