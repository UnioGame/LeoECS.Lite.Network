namespace Game.Ecs.Network.NetworkCommands.Extensions
{
    using System.Runtime.CompilerServices;
    using Data;
    using Leopotam.EcsLite;
    using Systems;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif
    
    public static class EcsNetworkCommandsExtensions
    {
        public static NetworkMessageToolsSystem messageTools;
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkMessage<TComponent>(this EcsPool<TComponent> pool, 
            NetworkMessageTarget target, ulong targetId) where TComponent : struct
        {
            return ref messageTools.AddNetworkMessage<TComponent>(target, targetId);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkMessage<TComponent>(this EcsPool<TComponent> pool)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkMessage<TComponent>();
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkMessage<TComponent>(this EcsWorld world, NetworkMessageTarget target) 
            where TComponent : struct
        {
            return ref messageTools.AddNetworkMessage<TComponent>(target);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkMessage<TComponent>(this EcsWorld world)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkMessage<TComponent>();
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkComponent<TComponent>(this EcsWorld world,int entity)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkComponent<TComponent>(entity);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkComponent<TComponent>(this EcsWorld world,int entity, NetworkMessageTarget target)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkComponent<TComponent>(entity,target);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkComponent<TComponent>(this EcsPool<TComponent> pool,int entity)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkComponent<TComponent>(entity);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TComponent AddNetworkComponent<TComponent>(this EcsPool<TComponent> pool,int entity, NetworkMessageTarget target)
            where TComponent : struct
        {
            return ref messageTools.AddNetworkComponent<TComponent>(entity,target);
        }
    }
}