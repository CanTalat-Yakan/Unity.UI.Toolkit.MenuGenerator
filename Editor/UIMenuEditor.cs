using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
                .SetPane(editor.Pane, EditorPaneStyle.Left)
                .SetBody(editor.Body)
                .SetFooter(editor.Footer, EditorWindowStyle.HelpBox)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .ShowUtility();
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

            GUILayout.Label("UI Menu Builder Header", EditorStyles.boldLabel);
            GUILayout.Button("Header", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
        }

        private List<SimpleTreeViewItem> _treeData = CreateTreeData();
        private static List<SimpleTreeViewItem> CreateTreeData()
        {
            // Create items
            var category1 = new SimpleTreeViewItem(1, "Category 1");
            var header1 = new SimpleTreeViewItem(2, "Header 1");
            var category2 = new SimpleTreeViewItem(3, "Category 2");

            // Build hierarchy
            category1.AddChild(header1);

            // Only root items go in the list
            return new List<SimpleTreeViewItem> { category1, category2 };
        }

        private SimpleTreeView _treeView;
        private void Pane()
        {
            _treeView ??= new SimpleTreeView(_treeData);
            _treeView.OnGUI();
        }

        private void Body()
        {
            GUILayout.Button("Test2");
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
        }

        private void Footer()
        {
            GUILayout.Label("UI Menu Builder Footer", EditorStyles.boldLabel);
        }
    }
}