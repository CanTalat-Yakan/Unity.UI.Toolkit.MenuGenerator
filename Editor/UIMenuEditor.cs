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

        private SimpleTreeView _treeView;
        private UIMenuData _data = new();
        public List<ScriptableObject> Root = new();

        private static Texture2D FolderIcon = EditorIconUtilities.GetTexture(EditorIconNames.VerticalLayoutGroupIcon);
        private static Texture2D HeaderIcon = EditorIconUtilities.GetTexture(EditorIconNames.TextIcon);
        private static GUIContent ToolbarIcon = EditorIconUtilities.GetContent(EditorIconNames.ToolbarPlus);

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            UIMenu.ShowUIBuilder = ShowWindow;

        [MenuItem("Tools/ UI Menu Builder %g", false, priority = 1003)]
        private static void ShowWindow()
        {
            var editor = new UIMenuEditor();
            editor._treeView = new SimpleTreeView(editor.FetchData(), "Menu");
            editor._treeView.ContextMenu = editor.GetPaneGenericMenu();
            editor.Window = new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left, genericMenu: editor.GetPaneGenericMenu())
                .SetBody(editor.Body, genericMenu: editor.GetBodyGenericMenu())
                .SetFooter(editor.Footer, EditorWindowStyle.HelpBox)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .ShowUtility();

            editor.Window.SplitterPosition = 200;
        }

        private void Header()
        {
            if (EditorGUILayout.DropdownButton(ToolbarIcon, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                _treeView.ClearAllSelections();
                GetPaneGenericMenu().DropDown(GetLastRect());
            }

            GUILayout.Label("UI Menu Builder Header", EditorStyles.boldLabel);
            GUILayout.Button("Header", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
        }

        private void Pane()
        {
            _treeView.OnGUI();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                _treeView.ClearAllSelections();

            Window.ContextMenuHandled = _treeView.ContextMenuEnabled;
        }

        private void Body()
        {
            var item = _treeView.GetSelectedItem();
            if (item == null)
                return;

            var icon = item.Icon;
            string name = item.Name;
            string type = item.UserData?.ToString();
            if (!string.IsNullOrEmpty(type))
                type = $" ({type})";

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (icon != null)
            {
                GUILayout.Label(icon, GUILayout.Width(24), GUILayout.Height(24));
                GUILayout.Space(4);
            }
            GUILayout.Label(name + type, EditorStyles.boldLabel, GUILayout.Height(24));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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

        private Rect GetLastRect()
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.y += 20;
            return lastRect;
        }

        private GenericMenu GetPaneGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Category"), false, () => AddCategory(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Header"), false, () => AddHeader(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Space"), false, () => AddSpace(parent: _treeView.GetSelectedItem()?.id));
            return menu;
        }

        private GenericMenu GetBodyGenericMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Button"), false, () => { });
            menu.AddItem(new("Add Options"), false, () => { });
            menu.AddItem(new("Add Input"), false, () => { });
            menu.AddItem(new("Add Slider"), false, () => { });
            menu.AddItem(new("Add Toggle"), false, () => { });
            menu.AddItem(new("Add Selection"), false, () => { });
            menu.AddItem(new("Add Color Picker"), false, () => { });
            menu.AddItem(new("Add Color Slider"), false, () => { });
            return menu;
        }

        private void AddCategory(string name = "Category", int? parent = null) => _treeView.AddItem(CreateCategory(), parent);
        private SimpleTreeViewItem CreateCategory(string name = "Category") => new SimpleTreeViewItem(name, FolderIcon).SetUserData(UIMenuDataTypes.Category);

        private void AddHeader(string name = "Header", int? parent = null) => _treeView.AddItem(CreateHeader(name), parent);
        private SimpleTreeViewItem CreateHeader(string name = "Header") => new SimpleTreeViewItem(name, HeaderIcon).Support(false).SetUserData(UIMenuDataTypes.Header);

        private void AddSpace(int? parent = null) => _treeView.AddItem(CreateSpace(), parent);
        private SimpleTreeViewItem CreateSpace() => new SimpleTreeViewItem(string.Empty).Support(false, false).SetUserData(UIMenuDataTypes.Space);

    }
}
#endif