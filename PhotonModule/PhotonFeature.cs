namespace Girand.Ecs.GameSettings
{
    using Components.Events;
    using Components.Events.Room;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using Server.Components.Requests;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Features/Photon", fileName = "Photon Feature")]
    public class PhotonFeature : BaseLeoEcsFeature
    {
        public sealed override UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            //room callbacks
            ecsSystems.DelHere<MasterClientSwitchedEvent>();
            ecsSystems.DelHere<PlayerEnterRoomEvent>();
            ecsSystems.DelHere<PlayerLeftRoomEvent>();
            ecsSystems.DelHere<PlayerPropertiesUpdateEvent>();
            ecsSystems.DelHere<RoomPropertiesEvent>();
            
            //connection callbacks
            ecsSystems.DelHere<NetworkAuthenticationFailedSelfEvent>();
            ecsSystems.DelHere<NetworkAuthenticationResponseSelfEvent>();
            ecsSystems.DelHere<NetworkConnectedSelfEvent>();
            ecsSystems.DelHere<NetworkConnectedToMasterSelfEvent>();
            ecsSystems.DelHere<NetworkDisconnectedSelfEvent>();
            ecsSystems.DelHere<NetworkRegionListReceivedSelfEvent>();
            
            ecsSystems.Add(new CreatePhotonAgentSystem()); 
            
            //get all connection callbacks from photon
            ecsSystems.Add(new PhotonConnectionsCallbacksSystem());    
            ecsSystems.Add(new PhotonRoomCallbacksSystem());  
            
            ecsSystems.Add(new UpdatePhotonStatusSystem());    
            ecsSystems.Add(new UpdateCurrentRoomStatusSystem());   
            ecsSystems.Add(new PlayersProcessEventsSystem());   
            ecsSystems.Add(new PlayersUpdateDataSystem());   
            
            ecsSystems.Add(new ConnectToPhotonSystem());    
            ecsSystems.Add(new MakeNewServerRoomSystem());    
            ecsSystems.Add(new JoinGameRoomSystem());    
            ecsSystems.Add(new RiseEventSystem());

            ecsSystems.DelHere<PhotonNetworkDataEvent>();
            ecsSystems.Add(new ProcessPhotonEventsSystem());
            
            ecsSystems.Add(new LoadPhotonGameSceneSystem());
            
            ecsSystems.DelHere<CreatePhotonAgentSelfRequest>();
            ecsSystems.DelHere<ConnectToPhotonSelfRequest>();
            ecsSystems.DelHere<MakeNewRoomSelfRequest>();
            ecsSystems.DelHere<LoadNetworkSceneRequest>();
            ecsSystems.DelHere<JoinRoomRequest>();
            
            return UniTask.CompletedTask;
        }
    }

}