using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuPrefabSpawner : MonoBehaviour
    {
        [MenuItem("GameObject/ UI Toolkit/ Add UI Menu Generator", false, priority = 99)]
        private static void InstantiateQuery(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("UI Menu Generator");
            go.AddComponent<UIMenuGenerator>();
            Undo.RegisterCreatedObjectUndo(go, "Create UI Menu Generator");
            Selection.activeObject = go;
        }
    }
}