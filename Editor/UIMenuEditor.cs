#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public enum UIMenuDataTypes
    {
        Category,
        Header,
        Space,
    }

    public class UIMenuEditor
    {
        public EditorWindowDrawer Window;
        public Action Repaint;
        public Action Close;

        private UIMenuData _data = new();
        public List<ScriptableObject> Root = new();

        private static Texture2D FolderIcon = EditorIconUtilities.GetTexture(EditorIconNames.VerticalLayoutGroupIcon);
        private static Texture2D HeaderIcon = EditorIconUtilities.GetTexture(EditorIconNames.TextIcon);
        private static GUIContent ToolbarIcon = EditorIconUtilities.GetContent(EditorIconNames.ToolbarPlus);

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            UIMenu.ShowUIBuilder = () =>
            {
                Initialize();
                ShowWindow();
            };

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

            editor.Window.SplitterPosition = 200;
        }

        private void Initialization()
        {
            _treeView ??= new SimpleTreeView(FetchData(), "Menu");
            _treeView.CustomContextMenuAction = new (string, Action<SimpleTreeViewItem>)[]
            {
                ("Add Category", (item) => AddCategory(parent: item)),
                ("Add Header", (item) => AddHeader(parent: item)),
                ("Add Space", (item) => AddSpace(parent: item))
            };
        }

        private void Header()
        {
            if (EditorGUILayout.DropdownButton(ToolbarIcon, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new("Add Category"), false, () => AddCategory());
                menu.AddItem(new("Add Header"), false, () => AddHeader());
                menu.AddItem(new("Add Space"), false, () => AddSpace());
                menu.DropDown(GUILayoutUtility.GetLastRect());
            }

            GUILayout.Label("UI Menu Builder Header", EditorStyles.boldLabel);
            GUILayout.Button("Header", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
        }

        private SimpleTreeView _treeView;
        private void Pane()
        {
            _treeView?.OnGUI();
        }

        private void Body()
        {
            string type = _treeView.GetSelectedItem()?.UserData?.ToString();
            if (!string.IsNullOrEmpty(type))
                type = $" ({_treeView.GetSelectedItem()?.UserData})";

            GUILayout.Button(_treeView.GetSelectedItem()?.Name + type);
        }

        private void Footer()
        {
            GUILayout.Label("UI Menu Builder Footer", EditorStyles.boldLabel);
        }

        private SimpleTreeViewItem[] FetchData()
        {
            var items = new List<SimpleTreeViewItem>();
            items.Add(CreateCategory());
            return items.ToArray();
        }

        private SimpleTreeViewItem CreateCategory(string name = "Category") =>
            new SimpleTreeViewItem(name, FolderIcon).SetUserData(UIMenuDataTypes.Category);

        private void AddCategory(string name = "Category", SimpleTreeViewItem parent = null) =>
            _treeView.AddItem(CreateCategory(), parent);

        private SimpleTreeViewItem CreateHeader(string name = "Header") =>
            new SimpleTreeViewItem(name, HeaderIcon).SetUserData(UIMenuDataTypes.Header);

        private void AddHeader(string name = "Header", SimpleTreeViewItem parent = null) =>
            _treeView.AddItem(CreateHeader(name), parent);

        private SimpleTreeViewItem CreateSpace() =>
            new SimpleTreeViewItem(string.Empty, HeaderIcon).SetUserData(UIMenuDataTypes.Space);

        private void AddSpace(SimpleTreeViewItem parent = null) =>
            _treeView.AddItem(CreateSpace(), parent);
    }
}
#endif