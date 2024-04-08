namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// update photon info
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateCurrentRoomStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private PhotonRoomAspect _roomAspect;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world
                .Filter<PhotonAgentComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var photonAgent = ref _photonAspect.PhotonAgent.Get(entity);
                ref var statusComponent = ref _roomAspect.Status.GetOrAddComponent(entity);
                ref var roomComponent = ref _roomAspect.Room.GetOrAddComponent(entity);

                var room =  PhotonNetwork.CurrentRoom;
                roomComponent.Value = room;
                
                statusComponent.IsInRoom = PhotonNetwork.InRoom;
                statusComponent.IsConnecting =  statusComponent is { IsInRoom: false, IsConnecting: true };
                statusComponent.IsOffline = room is { IsOffline: true };
                statusComponent.Name = room == null ? string.Empty : room.Name;
                statusComponent.IsOpen = room is { IsOpen: true };
                statusComponent.IsVisible = room is { IsVisible: true };
                statusComponent.MaxPlayers = room?.MaxPlayers ?? 0;
                statusComponent.PlayerCount = room?.PlayerCount ?? 0;
            }
        }
    }
}