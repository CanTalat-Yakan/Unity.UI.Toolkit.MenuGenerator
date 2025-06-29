#if UNITY_EDITOR
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

        public Action<UIMenuData> SetUIMenuData;

        private UIMenuData _data;
        private SimpleTreeView _treeView;

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            UIMenu.ShowUIBuilder = (menu) => ShowUtility(menu.Data, menu.SetUIMenuData);

        private static void ShowUtility(UIMenuData data, Action<UIMenuData> action, UIMenuEditor editor = null)
        {
            editor ??= new UIMenuEditor();
            editor.SetUIMenuData = action;
            editor._data = data;

            editor._treeView = new SimpleTreeView(data.Name);
            editor._treeView.OnRename = (item) =>
            {
                SetSerializedObjectName(item.UserData as ScriptableObject, item.Name, item.UniqueName);
            };

            foreach (var item in data?.Root)
                editor._treeView?.AddItem(UIMenuEditorUtilities.CreateItem(item, editor._treeView));

            editor.Window ??= new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800)).ShowUtility();
            editor.Window
                .SetPreProcess(editor._treeView.PreProcess)
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left, genericMenu: UIMenuEditorUtilities.GetGenericMenu(editor._treeView))
                .SetBody(editor.Body)
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
                UIMenuEditorUtilities.GetGenericMenu(_treeView).DropDown(lastRect);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("  Revert  ", EditorStyles.toolbarButton))
                ShowUtility(_data, SetUIMenuData, this);
            if (GUILayout.Button("  Apply  ", EditorStyles.toolbarButton))
            {
                UIMenuEditorAssetSerializer.Save(_data, _treeView);
                UIMenuEditorUtilities.PopulateDataFromTree(_data, _treeView);
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

            CreateDynamicBox(item);

            if (item.UserData is UIMenuCategoryData category || isRoot)
                foreach (var child in item.Children)
                    CreateDynamicBox(child);

            if (item.SupportsChildren)
            {
                GUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Item", GUILayout.Width(200), GUILayout.Height(24)))
                        if (item.ContextMenu != null)
                            item.ContextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                        else UIMenuEditorUtilities.GetGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
            }
        }

        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly Dictionary<ScriptableObject, bool> _foldoutStates = new();
        private CustomScriptableObjectDrawer _customScriptableObjectDrawer = new();
        private void CreateDynamicBox(SimpleTreeViewItem item)
        {
            var itemData = item.UserData as ScriptableObject;
            if (itemData == null)
                return;

            if (!_foldoutStates.TryGetValue(itemData, out bool isExpanded))
            {
                isExpanded = true;
                _foldoutStates[itemData] = isExpanded;
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    var label = item.Name == string.Empty ? item.UserTag : $"{item.Name} ({item.UserTag})";
                    isExpanded = EditorGUILayout.Foldout(isExpanded, label, true);
                    _foldoutStates[itemData] = isExpanded;
                }

                EditorGUI.indentLevel++;
                {
                    if (isExpanded)
                    {
                        if (!_editorCache.TryGetValue(itemData, out Editor editor) || editor == null)
                        {
                            editor = Editor.CreateEditor(itemData);
                            _editorCache[itemData] = editor;
                        }

                        var drawArrays = true;
                        drawArrays &= itemData is not UIMenuCategoryData;
                        drawArrays &= itemData is not UIMenuSelectionDataCategory;

                        _customScriptableObjectDrawer.Draw(editor, drawArrays);

                        GUILayout.Space(10);
                    }
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