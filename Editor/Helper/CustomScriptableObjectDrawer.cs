using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEssentials
{
    public class CustomScriptableObjectDrawer : MonoBehaviour
    {
        public void Draw(Editor scriptableEditor, bool drawArrays = true)
        {
            if (scriptableEditor == null)
                return;

            var so = scriptableEditor.serializedObject;
            so.Update();

            SerializedProperty iterator = so.GetIterator();
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
                        DrawReorderablePropertyList(so, arrayProperty);
                }
                else
                    EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            so.ApplyModifiedProperties();
        }

        private readonly Dictionary<string, ReorderableList> _listsCache = new();
        private void DrawReorderablePropertyList(SerializedObject so, SerializedProperty property)
        {
            if (property == null || !property.isArray || property.propertyType == SerializedPropertyType.String)
                return;

            string cacheKey = property.serializedObject.targetObject.GetInstanceID().ToString() + property.propertyPath;

            ReorderableList reorderableList = null;
            if (!_listsCache.TryGetValue(cacheKey, out reorderableList))
            {
                reorderableList = new ReorderableList(so, property, true, true, true, true);
                _listsCache.Add(cacheKey, reorderableList);
            }

            reorderableList.drawHeaderCallback = (Rect position) =>
            {
                float buttonWidth = 70f;
                Rect labelRect = new Rect(position.x, position.y, position.width - buttonWidth - 5, position.height);
                Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

                EditorGUI.LabelField(labelRect, property.displayName);

                if (GUI.Button(buttonRect, "Clear All", EditorStyles.toolbarButton))
                {
                    property.ClearArray();
                    property.serializedObject.ApplyModifiedProperties();
                }
            };

            reorderableList.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                position.y += 2;
                position.height -= 4;
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };

            var listRect = EditorGUILayout.GetControlRect(false, reorderableList.GetHeight());
            listRect = EditorGUI.IndentedRect(listRect);
            reorderableList.DoList(listRect);
            HandleDragAndDrop(listRect, property);
        }

        private void HandleDragAndDrop(Rect dropArea, SerializedProperty arrayProperty)
        {
            Event evt = Event.current;

            if (!dropArea.Contains(evt.mousePosition))
                return;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    // You can restrict accepted object types here
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var obj in DragAndDrop.objectReferences)
                            {
                                int index = arrayProperty.arraySize;
                                arrayProperty.InsertArrayElementAtIndex(index);
                                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(index);
                                if (element.propertyType == SerializedPropertyType.ObjectReference)
                                    element.objectReferenceValue = obj;
                            }
                            arrayProperty.serializedObject.ApplyModifiedProperties();
                        }

                        Event.current.Use();
                    }
                    break;
            }
        }
    }
}
