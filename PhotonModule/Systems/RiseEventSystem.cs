namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using Aspects;
    using ExitGames.Client.Photon;
    using Leopotam.EcsLite;
    using Newtonsoft.Json;
    using Photon.Pun;
    using Photon.Realtime;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// send photon events
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class RiseEventSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;

        private EcsFilter _filter;
        private EcsFilter _baseFilter;
        private EcsFilter _serverFilter;
        private EcsFilter _allFilter;
        
        private RaiseEventOptions _othersOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache
        };
        
        private RaiseEventOptions _toServerOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.MasterClient,
            CachingOption = EventCaching.DoNotCache
        };

        private RaiseEventOptions _toAllOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache,
        };

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<PhotonEventRequest>()
                .End();
            
            _baseFilter = _world
                .Filter<PhotonOthersEventRequest>()
                .End();
            
            _serverFilter = _world
                .Filter<PhotonServerEventRequest>()
                .End();
            
            _allFilter = _world
                .Filter<PhotonAllEventRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            if (!PhotonNetwork.IsConnectedAndReady) return;
            
            foreach (var entity in _filter)
            {
                ref var eventRequest = ref _photonAspect.SendEvent.Get(entity);
                var data = eventRequest.Data;
                var serializedData = data is string 
                    ? data 
                    : JsonConvert.SerializeObject(eventRequest.Data);
                
                var accepted = PhotonNetwork.RaiseEvent(
                    eventRequest.EventCode,
                    serializedData,
                    eventRequest.Options,
                    eventRequest.SendOptions);

                _photonAspect.SendEvent.Del(entity);
            }

            foreach (var eventEntity in _baseFilter)
            {
                ref var eventRequest = ref _photonAspect.ToOthersEvent.Get(eventEntity);
                var data = eventRequest.Data;
                var serializedData = data is string 
                    ? data 
                    : JsonConvert.SerializeObject(eventRequest.Data);
                
                var accepted = PhotonNetwork.RaiseEvent(
                    eventRequest.EventCode,
                    serializedData,
                    _othersOptions,
                    SendOptions.SendReliable);

                _photonAspect.ToOthersEvent.Del(eventEntity);
            }
            
            foreach (var eventEntity in _serverFilter)
            {
                ref var eventRequest = ref _photonAspect.ToServerEvent.Get(eventEntity);
                var data = eventRequest.Data;
                var serializedData = data is string 
                    ? data 
                    : JsonConvert.SerializeObject(eventRequest.Data);
                
                var accepted = PhotonNetwork.RaiseEvent(
                    eventRequest.EventCode,
                    serializedData,
                    _toServerOptions,
                    SendOptions.SendReliable);

                _photonAspect.ToServerEvent.Del(eventEntity);
            }
            
            foreach (var eventEntity in _allFilter)
            {
                ref var eventRequest = ref _photonAspect.ToAllEvent.Get(eventEntity);

                var data = eventRequest.Data;
                var serializedData = data is string 
                    ? data 
                    : JsonConvert.SerializeObject(eventRequest.Data);
                
                var accepted = PhotonNetwork.RaiseEvent(
                    eventRequest.EventCode,
                    serializedData,
                    _toAllOptions,
                    SendOptions.SendReliable);

                _photonAspect.ToAllEvent.Del(eventEntity);
            }
        }
    }
}