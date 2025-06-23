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

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            UIMenu.ShowUIBuilder = (data) => ShowUtility(data ?? new());

        private static void ShowUtility(UIMenuData data)
        {
            var editor = new UIMenuEditor();
            editor._data = data;
            editor._treeView = new SimpleTreeView(editor.FetchData(), data.Name);
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

        private static GUIContent ToolbarIcon = EditorIconUtilities.GetContent(EditorIconNames.ToolbarPlus);
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
            _treeView.Draw();

            Window.ContextMenuHandled = _treeView.ContextMenuHandled;
        }

        private void Body()
        {
            var item = _treeView.GetSelectedItem();
            if (item == null)
                return;

            SimpleTreeViewBreadcrumbs.Draw(item, clickedItem =>
            {
                _treeView.SetSelectedItems(clickedItem.id);
                _treeView.Repaint();
            });

            var icon = item.Icon;
            string name = item.Name;
            string type = item.UserTag;
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

            if (item.UserData is UIMenuCategoryData categoryData)
                foreach (var data in categoryData.Data)
                {
                    switch (data)
                    {
                        case UIMenuButtonData buttonData:
                            GUILayout.Label($"Button: {buttonData.Name}", EditorStyles.label);
                            EditorGUILayout.ObjectField("Button", buttonData, typeof(UIMenuButtonData), false);
                            break;
                        default:
                            break;
                    }
                }
        }

        private void Footer()
        {
            GUILayout.Label("UI Menu Builder Footer", EditorStyles.boldLabel);
        }

        private SimpleTreeViewItem[] FetchData()
        {
            var items = new List<SimpleTreeViewItem>();

            foreach (var item in _data.Root)
                items.Add(item switch
                {
                    UIMenuCategoryData categoryData => CreateCategoryAndData(categoryData),
                    UIMenuHeaderData headerData => CreateHeader(headerData.Name),
                    UIMenuSpacerData spacerData => CreateSpace(),
                    _ => null
                });

            if (items.Count == 0)
                items.Add(CreateCategory());

            return items.ToArray();
        }

        private SimpleTreeViewItem CreateCategoryAndData(UIMenuCategoryData category)
        {
            var parent = CreateCategory(category.Name);

            var items = new List<SimpleTreeViewItem>();
            foreach (var item in category.Data)
                items.Add(item switch
                {
                    UIMenuCategoryData categoryData => SetParent(CreateCategoryAndData(categoryData), parent).SetUserData(categoryData),
                    UIMenuHeaderData headerData => SetParent(CreateHeader(headerData.Name), parent).SetUserData(headerData),
                    UIMenuSpacerData spacerData => SetParent(CreateSpace(), parent).SetUserData(spacerData),
                    _ => null
                });

            return parent;
        }

        private SimpleTreeViewItem SetParent(SimpleTreeViewItem item, SimpleTreeViewItem parent)
        {
            var categoryItem = CreateCategory(item.Name);
            categoryItem.Parent = parent;
            return categoryItem;
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
        private static Texture2D FolderIcon = EditorIconUtilities.GetTexture(EditorIconNames.VerticalLayoutGroupIcon);
        private SimpleTreeViewItem CreateCategory(string name = "Category") =>
            new SimpleTreeViewItem(name, FolderIcon).SetUserTag(UIMenuDataTypes.Category.ToString());

        private void AddHeader(string name = "Header", int? parent = null) => _treeView.AddItem(CreateHeader(name), parent);
        private static Texture2D HeaderIcon = EditorIconUtilities.GetTexture(EditorIconNames.TextIcon);
        private SimpleTreeViewItem CreateHeader(string name = "Header") =>
            new SimpleTreeViewItem(name, HeaderIcon).Support(false).SetUserTag(UIMenuDataTypes.Header.ToString());

        private void AddSpace(int? parent = null) => _treeView.AddItem(CreateSpace(), parent);
        private SimpleTreeViewItem CreateSpace() =>
            new SimpleTreeViewItem(string.Empty).Support(false, false).SetUserTag(UIMenuDataTypes.Space.ToString());

        private void AddButton(int? parent = null) => _treeView.AddItem(CreateButton(), parent);
        private static Texture2D ButtonIcon = EditorIconUtilities.GetTexture(EditorIconNames.ButtonIcon);
        private SimpleTreeViewItem CreateButton(string name = "Button") =>
            new SimpleTreeViewItem().Support(false, false).SetUserTag(UIMenuDataTypes.Space.ToString());
    }
}
#endif