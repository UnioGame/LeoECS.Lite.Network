namespace Girand.Ecs.Server.Components.Requests
{
    using ExitGames.Client.Photon;
    using Leopotam.EcsLite;
    using Photon.Realtime;

    public struct PhotonEventRequest : IEcsAutoReset<PhotonEventRequest>
    {
        public byte EventCode;
        public object Data;
        public RaiseEventOptions Options;
        public SendOptions SendOptions;
        
        public void AutoReset(ref PhotonEventRequest c)
        {
            c.EventCode = 0;
            c.Data = null;
            c.Options = null;
            c.SendOptions = default;
        }
    }
    
    public struct PhotonOthersEventRequest : IEcsAutoReset<PhotonOthersEventRequest>
    {
        public byte EventCode;
        public object Data;
        
        public void AutoReset(ref PhotonOthersEventRequest c)
        {
            c.EventCode = 0;
            c.Data = null;
        }
    }

    public struct PhotonAllEventRequest : IEcsAutoReset<PhotonAllEventRequest>
    {
        public byte EventCode;
        public object Data;
        
        public void AutoReset(ref PhotonAllEventRequest c)
        {
            c.EventCode = 0;
            c.Data = null;
        }
    }
    
    public struct PhotonServerEventRequest : IEcsAutoReset<PhotonServerEventRequest>
    {
        public byte EventCode;
        public object Data;
        
        public void AutoReset(ref PhotonServerEventRequest c)
        {
            c.EventCode = 0;
            c.Data = null;
        }
    }
}