#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class CustomScriptableObjectDrawer : MonoBehaviour
    {
        private CustomReorderableList _customReorderableList = new();

        public void Draw(Editor scriptableEditor, bool drawArrays = true, bool drawReference = true)
        {
            if (scriptableEditor == null)
                return;

            var serializedObject = scriptableEditor.serializedObject;
            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name == "m_Script")
                    continue;

                if (iterator.name == "Reference" && !drawReference)
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
                        _customReorderableList.Draw(serializedObject, arrayProperty);
                }
                else EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
#endif