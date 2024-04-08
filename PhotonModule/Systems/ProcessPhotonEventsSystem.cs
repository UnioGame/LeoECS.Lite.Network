namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Collections.Generic;
    using Aspects;
    using Components.Events;
    using ExitGames.Client.Photon;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// handle photon event and send to ecs
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class ProcessPhotonEventsSystem : 
        IEcsInitSystem, IEcsRunSystem,
        IEcsDestroySystem,
        IOnEventCallback
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;

        private EcsFilter _filter;
        private List<PhotonNetworkDataEvent> _dataEvents = new List<PhotonNetworkDataEvent>();
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventItem in _dataEvents)
            {
                var eventEntity = _world.NewEntity();
                ref var eventData = ref _photonAspect.DataEvent.Add(eventEntity);
                eventData.Code = eventItem.Code;
                eventData.Data = eventItem.Data;
            }
            
            _dataEvents.Clear();
        }

        public void OnEvent(EventData photonEvent)
        {
            var code = photonEvent.Code;
            var data = photonEvent.CustomData;
            var sender = photonEvent.Sender;
            
            _dataEvents.Add(new PhotonNetworkDataEvent()
            {
                Code = code,
                Data = data,
                Sender = photonEvent.Sender,
                FromServer = sender == 0,
            });
        }

        public void Destroy(IEcsSystems systems)
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}