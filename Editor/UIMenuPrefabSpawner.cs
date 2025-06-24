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
            var generator = go.AddComponent<UIMenuGenerator>();
            generator.hideFlags = HideFlags.HideInInspector;
            generator.Initialize();
            menu.OnTypeValueChanged();
            generator.Fetch();
            Undo.RegisterCreatedObjectUndo(go, "Create UI Menu");
            Selection.activeObject = go;
        }
    }
}
#endif