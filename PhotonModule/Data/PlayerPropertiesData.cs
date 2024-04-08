namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using ExitGames.Client.Photon;
    using Photon.Realtime;

    [Serializable]
    public struct PlayerPropertiesData
    {
        public Player TargetPlayer;
        public Hashtable ChangedProps;
    }
}