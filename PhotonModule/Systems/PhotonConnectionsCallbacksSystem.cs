namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Collections.Generic;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif

    /// <summary>
    /// Class responsible for handling Photon connection callback events.
    /// </summary>
    [Serializable]
    [ECSDI]
    public class PhotonConnectionsCallbacksSystem :
        IEcsInitSystem,
        IEcsRunSystem,
        IConnectionCallbacks
    {
        private EcsWorld _world;
        private List<ConnectionCallbackEvent> _callbackEvents = new(128);
        private PhotonCallbacksAspect _callbacksAspect;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<PhotonAgentComponent>()
                .End();

            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventItem in _callbackEvents)
            {
                foreach (var entity in _filter)
                {
                    switch (eventItem.Type)
                    {
                        case ConnectionsCallbackType.Connected:
                            ref var connectedEvent = ref _callbacksAspect.Connected.GetOrAddComponent(entity);
                            break;
                        case ConnectionsCallbackType.ConnectedToMaster:
                            ref var masterEvent = ref _callbacksAspect.ConnectedToMaster.GetOrAddComponent(entity);
                            break;
                        case ConnectionsCallbackType.Disconnected:
                            ref var disconnectedEvent = ref _callbacksAspect.Disconnected.GetOrAddComponent(entity);
                            disconnectedEvent.Value = (DisconnectCause)eventItem.Data;
                            break;
                        case ConnectionsCallbackType.RegionListReceived:
                            ref var regionListReceivedEvent =
                                ref _callbacksAspect.RegionReceived.GetOrAddComponent(entity);
                            regionListReceivedEvent.Value = (RegionHandler)eventItem.Data;
                            break;
                        case ConnectionsCallbackType.AuthenticationResponse:
                            ref var authenticationResponseEvent = ref _callbacksAspect
                                .AuthenticationResponse
                                .GetOrAddComponent(entity);
                            authenticationResponseEvent.Value = (Dictionary<string, object>)eventItem.Data;
                            break;
                        case ConnectionsCallbackType.AuthenticationFailed:
                            ref var failedEvent = ref _callbacksAspect
                                .AuthenticationFailed
                                .GetOrAddComponent(entity);

                            failedEvent.Value = (string)eventItem.Data;
                            break;
                    }
                }
            }

            _callbackEvents.Clear();
        }

        #region IConnectionCallbacks

        public void OnConnected()
        {
            GameLog.Log("Client Connected to Photon Server",Color.green);

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.Connected,
            });
        }

        public void OnConnectedToMaster()
        {
            GameLog.Log("Client Connected to Photon Master Server",Color.green);

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.ConnectedToMaster,
            });
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            GameLog.Log("Client OnDisconnected From Photon Server",Color.red);

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.Disconnected,
                Data = cause
            });
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            GameLog.Log("Client OnRegionListReceived From Photon Server");

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.RegionListReceived,
                Data = regionHandler
            });
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            GameLog.Log("Client OnCustomAuthenticationResponse From Photon Server");

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.AuthenticationResponse,
                Data = data
            });
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            GameLog.Log("Client OnCustomAuthenticationFailed From Photon Server");

            _callbackEvents.Add(new ConnectionCallbackEvent()
            {
                Type = ConnectionsCallbackType.AuthenticationFailed,
                Data = debugMessage
            });
        }

        #endregion
    }
}