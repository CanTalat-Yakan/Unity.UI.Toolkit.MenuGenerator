#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuPrefabSpawner : MonoBehaviour
    {
        [MenuItem("GameObject/ UI Toolkit/ Add Menu", false, priority = 99)]
        private static void InstantiateQuery(MenuCommand menuCommand)
        {
            var go = new GameObject("UI Menu");
            var menu = go.AddComponent<UIMenu>();
            var provider = go.AddComponent<UIMenuProfileProvider>();
            var generator = go.AddComponent<UIMenuGenerator>();
            generator.hideFlags = HideFlags.HideInInspector;
            menu.Initialize();
            Undo.RegisterCreatedObjectUndo(go, "Create UI Menu");
            Selection.activeObject = go;
        }
    }
}
#endif