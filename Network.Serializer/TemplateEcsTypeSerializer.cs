namespace Game.Ecs.Network.Shared.Data
{ 
    //====NAMESPACE_START====//
    
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using Leopotam.EcsLite;
    using MemoryPack;
    using Unity.Collections;
    using Game.Ecs.Network.Shared.Data;
    using Unity.Collections.LowLevel.Unsafe;
    using Game.Ecs.Network.Network.Serializer;
    using UniGame.LeoEcs.Shared.Extensions;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif
    
    //====NAMESPACE_END====//
    
    //====TEMPLATE_START====//
    
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class TemplateEcsTypeSerializer : IEcsTypeSerializer
    {

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(EcsWorld world, int entity,IBufferWriter<byte> writer)
        {
            var pool = world.GetPool<TemplateSerializeType>();
            if (!pool.Has(entity)) return false;
            ref var component = ref pool.Get(entity);
            //var size = Unsafe.SizeOf<TemplateSerializeType>();
            MemoryPackSerializer.Serialize(writer, component, MemoryPackSerializerOptions.Utf16);
            
            return true;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Serialize(EcsWorld world, int entity,ref NativeArray<byte> buffer,int offset)
        {
            var pool = world.GetPool<TemplateSerializeType>();
            if (!pool.Has(entity)) return -1;
            
            ref var component = ref pool.Get(entity);
            // Get a writer for the stream
            // Write some data into the stream
            var size = buffer.Serialize<TemplateSerializeType>(ref component,offset);
            return size;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Deserialize(EcsWorld world, int entity,ref ReadOnlySpan<byte> buffer)
        {
            ref var component = ref world.GetOrAddComponent<TemplateSerializeType>(entity);
            // Get a writer for the stream
            // Write some data into the stream
            var size = buffer.Deserialize<TemplateSerializeType>(ref component);
            return size;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Deserialize(EcsWorld world, int entity,ref NativeSlice<byte> buffer)
        {
            ref var component = ref world.GetOrAddComponent<TemplateSerializeType>(entity);
            // Get a writer for the stream
            // Write some data into the stream
            var size = buffer.Deserialize<TemplateSerializeType>(ref component);
            return size;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Deserialize(EcsWorld world, int entity,ref NativeArray<byte> buffer,int offset)
        {
            // Get a writer for the stream
            // Write some data into the stream
            var slice = new NativeSlice<byte>(buffer, offset);
            return Deserialize(world, entity, ref slice);
        }
    }
    
    //====TEMPLATE_END====//
    
    [Serializable]
    public struct TemplateSerializeType
    {
        public int Value;
        public int Value2;
    }
    
    [Serializable]
    public struct TemplateNonBlitableType
    {
        public int Value;
        public int Value2;
        public string Value3;
        public string Value4;
    }
}