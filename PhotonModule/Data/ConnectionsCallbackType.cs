namespace Girand.Ecs.GameSettings.Systems
{
    using System;

    [Serializable]
    public enum ConnectionsCallbackType
    {
        Connected,
        ConnectedToMaster,
        Disconnected,
        RegionListReceived,
        AuthenticationResponse,
        AuthenticationFailed,
    }
}