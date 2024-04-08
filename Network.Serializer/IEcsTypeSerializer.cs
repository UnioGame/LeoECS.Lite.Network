namespace Game.Ecs.Network.Shared.Data
{
    using System;
    using System.Buffers;
    using Leopotam.EcsLite;
    using Unity.Collections;

    public interface IEcsTypeSerializer
    {
        bool Serialize(EcsWorld world, int entity,IBufferWriter<byte> writer);

        int Serialize(EcsWorld world, int entity, ref NativeArray<byte> stream,int offset);

        int Deserialize(EcsWorld world, int entity, ref ReadOnlySpan<byte> buffer);

        int Deserialize(EcsWorld world, int entity, ref NativeSlice<byte> buffer);

        int Deserialize(EcsWorld world, int entity, ref NativeArray<byte> buffer, int offset);
    }
}