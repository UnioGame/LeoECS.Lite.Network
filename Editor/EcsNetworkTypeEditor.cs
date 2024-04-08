namespace Game.Ecs.Network.Editor
{
    using Shared.Data;
    using Sirenix.OdinInspector;
    using UnityCodeGen;

#if UNITY_EDITOR
    using UnityEngine;
#endif
    
    [CreateAssetMenu(menuName = "Game/Ecs/Network/Editor/EcsNetworkTypeEditor",fileName = "EcsNetworkTypeEditor")]
    public class EcsNetworkTypeEditorAsset : ScriptableObject
    {
        [TitleGroup("Network Settings")]
        [InlineEditor]
        [HideLabel]
        public EcsNetworkSettingsAsset networkSettings;
        
        [Button]
        public void GenerateNetworkTypes()
        {
            UnityCodeGenUtility.Generate();
        }


        [OnInspectorInit]
        public void Initialize()
        {
            
        }

        
    }
}