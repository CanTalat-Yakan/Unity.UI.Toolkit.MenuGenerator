#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class MenuEditor
    {
        public EditorWindowBuilder Window;
        public Action Repaint;
        public Action Close;

        public Action<MenuData> SetUIMenuData;

        private MenuData _data;
        private SimpleTreeView _treeView;

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            Menu.ShowEditor = (menu) => ShowUtility(menu.Data, menu.SetData);

        private static void ShowUtility(MenuData data, Action<MenuData> action, MenuEditor editor = null)
        {
            editor ??= new MenuEditor();
            editor.SetUIMenuData = action;
            editor._data = data;

            editor._treeView = new SimpleTreeView(data.Name);
            editor._treeView.OnRename = (item) =>
                SetSerializedObjectName(item.UserData as ScriptableObject, item.Name, item.UniqueName);

            foreach (var item in data?.Root)
                if (item != null)
                    editor._treeView?.AddItem(MenuEditorUtilities.CreateItem(item, editor._treeView));

            editor._treeView?.SetSelectedItems(0);
            editor._treeView?.BeginRename(0);

            editor.Window ??= EditorWindowBuilder
                .CreateInstance("UI Menu Builder", new(300, 400), new(600, 800))
                .ShowAsUtility();
            editor.Window
                .SetPreProcess(editor._treeView.PreProcess)
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left, genericMenu: MenuEditorUtilities.GetGenericMenu(editor._treeView))
                .SetBody(editor.Body, EditorWindowStyle.Dark)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close);

            editor.Window.SplitterPosition = 200;
        }

        private static GUIContent ToolbarIcon = EditorIconUtilities.GetContent(EditorIconNames.ToolbarPlus);
        private void Header()
        {
            if (EditorGUILayout.DropdownButton(ToolbarIcon, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                lastRect.y += 20;
                _treeView.ClearAllSelections();
                MenuEditorUtilities.GetGenericMenu(_treeView).DropDown(lastRect);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("  Revert  ", EditorStyles.toolbarButton))
                ShowUtility(_data, SetUIMenuData, this);
            if (GUILayout.Button("  Apply  ", EditorStyles.toolbarButton))
            {
                _data.Name = _treeView.RootItem.Name;
                MenuEditorAssetSerializer.Save(_data, _treeView);
                MenuEditorUtilities.PopulateDataFromTree(_data, _treeView);
                SetUIMenuData?.Invoke(_data);
            }
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

            var isRoot = item == _treeView.RootItem;

            if (!isRoot)
                SimpleTreeViewItemBreadcrumbs.Draw(item, clickedItem =>
                {
                    _treeView.SetSelectedItems(clickedItem.id);
                    _treeView.Repaint();
                });

            CreateDynamicBox(item, true);

            if (item.UserData is MenuCategoryData category || isRoot)
                foreach (var child in item.Children)
                    CreateDynamicBox(child);

            if (item.SupportsChildren)
            {
                GUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Item", GUILayout.Width(200), GUILayout.Height(24)))
                    {
                        if (item.ContextMenu != null)
                            item.ContextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                        else MenuEditorUtilities.GetGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));

                        Window.BodyScrollPosition = new Vector2(0, float.MaxValue);
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
            }
        }

        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly Dictionary<ScriptableObject, bool> _foldoutStates = new();
        private CustomScriptableObjectDrawer _customScriptableObjectDrawer = new();
        private void CreateDynamicBox(SimpleTreeViewItem item, bool isFocused = false)
        {
            var itemData = item.UserData as ScriptableObject;
            if (itemData == null)
                return;

            if (!_foldoutStates.TryGetValue(itemData, out bool isExpanded))
            {
                isExpanded = isFocused;
                _foldoutStates[itemData] = isExpanded;
            }
            else if (isFocused && !isExpanded)
                isExpanded = true;

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (!isFocused && item.icon != null)
                    {
                        const float iconSize = 24f;
                        GUILayout.Space(20f);
                        GUILayout.Label(item.icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
                        GUILayout.Space(-44f);
                    }

                    var label = item.Name == string.Empty ? item.UserTag : $"{item.Name} ({item.UserTag})";
                    var labelOffset = isFocused || item.icon == null ? " " : "       ";
                    var guiContent = new GUIContent(labelOffset + label);

                    if (!isFocused)
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            _foldoutStates[itemData] = isExpanded = EditorGUILayout.Foldout(isExpanded, guiContent, true);
                    else GUILayout.Label(guiContent, EditorStyles.boldLabel);
                }

                EditorGUI.indentLevel++;
                if (isExpanded)
                {
                    if (!_editorCache.TryGetValue(itemData, out Editor editor) || editor == null)
                    {
                        editor = Editor.CreateEditor(itemData);
                        _editorCache[itemData] = editor;
                    }

                    var drawArrays = true;
                    drawArrays &= itemData is not MenuCategoryData;
                    drawArrays &= itemData is not MenuSelectionCategoryData;

                    var drawReference = true;
                    if (itemData is MenuTypeDataBase data)
                        drawReference = data.HasReference;

                    _customScriptableObjectDrawer.Draw(editor, drawArrays, drawReference);

                    GUILayout.Space(10);
                }
                EditorGUI.indentLevel--;
            }
        }

        private static void SetSerializedObjectName(ScriptableObject item, string name, string uniqueName)
        {
            if (item == null)
                return;

            if (name == string.Empty)
                name = uniqueName;

            item.GetType().GetMethod("SetName")?.Invoke(item, new object[] { name, uniqueName });
        }
    }
}
#endif