namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct SerializationTaskData
    {
        public int Id;
        public int Entity;
    }
}