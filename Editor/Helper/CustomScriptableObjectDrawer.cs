#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class CustomScriptableObjectDrawer : MonoBehaviour
    {
        private CustomReorderableListDrawer _customReorderableListDrawer = new();

        public void Draw(Editor scriptableEditor, bool drawArrays = true)
        {
            if (scriptableEditor == null)
                return;

            var serializedObject = scriptableEditor.serializedObject;
            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                // Skip the internal script reference and "Name" property
                if (iterator.name == "m_Script" || iterator.name == "Name")
                    continue;

                // Skip arrays if requested, but allow normal strings
                if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String && !drawArrays)
                {
                    enterChildren = false;
                    continue;
                }

                if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String)
                {
                    var arrayProperty = iterator.Copy();
                    if (arrayProperty.arraySize >= 0)
                        _customReorderableListDrawer.Draw(serializedObject, arrayProperty);
                }
                else EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
#endif