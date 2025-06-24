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

        private SimpleTreeView _treeView;
        private UIMenuData _data;

        [InitializeOnLoadMethod()]
        public static void Initialize() =>
            UIMenu.ShowUIBuilder = (data) => ShowUtility(data);

        private static void ShowUtility(UIMenuData data, UIMenuEditor editor = null)
        {
            editor ??= new UIMenuEditor();
            editor._data = data;
            editor._treeView = new SimpleTreeView(editor.FetchData(), data.Name);
            editor._treeView.ContextMenu = UIMenuEditorUtilities.GetPaneGenericMenu(editor._treeView);
            editor._treeView.OnRename = (item) =>
            {
                SetSerializedObjectName(item.UserData as ScriptableObject, item.Name);
            };
            editor.Window ??= new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left, genericMenu: UIMenuEditorUtilities.GetPaneGenericMenu(editor._treeView))
                .SetBody(editor.Body, genericMenu: UIMenuEditorUtilities.GetBodyGenericMenu(editor._treeView))
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
                var lastRect = GUILayoutUtility.GetLastRect();
                lastRect.y += 20;
                _treeView.ClearAllSelections();
                UIMenuEditorUtilities.GetPaneGenericMenu(_treeView).DropDown(lastRect);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Button("Revert", EditorStyles.toolbarButton);
            GUILayout.Button("Load", EditorStyles.toolbarButton);
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
                SimpleTreeViewBreadcrumbs.Draw(item, clickedItem =>
                {
                    _treeView.SetSelectedItems(clickedItem.id);
                    _treeView.Repaint();
                });

            CreateDynamicBox(item);
            if (item.UserData is UIMenuCategoryData category || isRoot)
            {
                foreach (var child in item.Children)
                    CreateDynamicBox(child);
            }

            if (item.SupportsChildren)
            {
                GUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Item", GUILayout.Width(200), GUILayout.Height(24)))
                        if (item.UserData is UIMenuSelectionDataCollectionGroup)
                            UIMenuEditorUtilities.GetSelectionGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                        else UIMenuEditorUtilities.GetBodyGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
            }
        }

        private void Footer()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUIStyle italicLabelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic };
                GUILayout.Label("§", italicLabelStyle);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Revert", GUILayout.Width(100)))
                    ShowUtility(_data, this);
                if (GUILayout.Button("Apply", GUILayout.Width(100)))
                {
                    UIMenuEditorAssetSerializer.Save(_data, _treeView);
                    UIMenuEditorUtilities.PopulateCategoryDataFromTree(_data, _treeView);
                }
            }
        }

        private SimpleTreeViewItem[] FetchData()
        {
            var items = new List<SimpleTreeViewItem>();

            foreach (var item in _data?.Root)
                if (item != null)
                    items.Add(UIMenuEditorUtilities.CreateItem(item, null));

            return items.ToArray();
        }

        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly Dictionary<ScriptableObject, bool> _foldoutStates = new();
        private CustomScriptableObjectDrawer _customScriptableObjectDrawer = new();
        private void CreateDynamicBox(SimpleTreeViewItem item)
        {
            var itemData = item.UserData as ScriptableObject;
            if (itemData == null)
                return;

            SetSerializedObjectName(itemData, item.Name);

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
                        drawArrays &= itemData is not UIMenuSelectionDataCollectionGroup;

                        _customScriptableObjectDrawer.Draw(editor, drawArrays);

                        GUILayout.Space(10);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private static void SetSerializedObjectName(ScriptableObject item, string name)
        {
            if (item == null)
                return;

            item.GetType().GetMethod("SetName")?.Invoke(item, new object[] { name });
        }
    }
}
#endif