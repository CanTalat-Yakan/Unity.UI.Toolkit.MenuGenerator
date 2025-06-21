using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEssentials
{
    public class UIMenuEditor
    {
        public EditorWindowDrawer Window;
        public Action Repaint;
        public Action Close;

        private UIMenuData _data = new();

        public List<ScriptableObject> Root = new();

        [MenuItem("Tools/ UI Menu Builder %g", false, priority = 1003)]
        private static void ShowWindow()
        {
            var editor = new UIMenuEditor();
            editor.Window = new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetBody(editor.Body)
                .SetFooter(editor.Footer, EditorWindowStyle.HelpBox)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .ShowWindow();
        }

        private void Header()
        {
            if (EditorGUILayout.DropdownButton(EditorGUIUtility.IconContent("Toolbar Plus"), FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Category"), false, () => Root.Add(ScriptableObject.CreateInstance<CategoryData>()));
                menu.AddItem(new GUIContent("Header"), false, () => Debug.Log("Option 2 selected"));
                menu.AddItem(new GUIContent("Space"), false, () => Debug.Log("Option 2 selected"));
                menu.DropDown(GUILayoutUtility.GetLastRect());
            }

            GUILayout.FlexibleSpace();
        }

        private void Body()
        {
            foreach (var rootObject in Root)
            {
            }
        }

        private void Footer()
        {
        }
    }
}