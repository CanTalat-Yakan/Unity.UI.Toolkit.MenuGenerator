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

        private static Texture2D FolderIcon = EditorIconUtilities.GetIconTexture(EditorIconNames.VerticalLayoutGroupIcon);
        private static Texture2D HeaderIcon = EditorIconUtilities.GetIconTexture(EditorIconNames.TextIcon);

        [MenuItem("Tools/ UI Menu Builder %g", false, priority = 1003)]
        private static void ShowWindow()
        {
            var editor = new UIMenuEditor();
            editor.Window = new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetInitialization(editor.Initialization)
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left)
                .SetBody(editor.Body)
                .SetFooter(editor.Footer, EditorWindowStyle.HelpBox)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .ShowUtility();
        }

        private void Initialization()
        {
            _treeView ??= new SimpleTreeView(CreateDefaultTreeData(), "Menu");
            _treeView.CustomContextMenuAction = new (string, Action<SimpleTreeViewItem>)[]
            {
                ("Add Category", (parent) => _treeView.AddItem(new SimpleTreeViewItem("Category", FolderIcon), parent)),
                ("Add Header", (parent) => _treeView.AddItem(new SimpleTreeViewItem("Header", HeaderIcon).Support(false), parent)),
                ("Add Space", (parent) => _treeView.AddItem(new SimpleTreeViewItem(string.Empty).Support(false, false), parent))
            };
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

        private static SimpleTreeViewItem[] CreateDefaultTreeData()
        {

            var category1 = new SimpleTreeViewItem("Category 1", FolderIcon);
            var header1 = new SimpleTreeViewItem("Header 1", HeaderIcon);
            var category2 = new SimpleTreeViewItem("Category 2", FolderIcon);
            var category3 = new SimpleTreeViewItem("Category 3", FolderIcon);

            header1.SupportsChildren = false;
            header1.Parent = category1;

            SimpleTreeViewItem[] rootChildren = { category1, category2, category3 };
            return rootChildren;
        }

        private SimpleTreeView _treeView;
        private void Pane()
        {
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