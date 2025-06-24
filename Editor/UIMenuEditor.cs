#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEssentials.UIMenuEditorUtilities;

namespace UnityEssentials
{
    public enum UIMenuDataTypes
    {
        Category,
        Header,
        Space,
        Button,
        Options,
        Input,
        Slider,
        Toggle,
        Selection,
        ColorPicker,
        ColorSlider,
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
            UIMenu.ShowUIBuilder = (data) => ShowUtility(data);

        private static void ShowUtility(UIMenuData data)
        {
            var editor = new UIMenuEditor();
            editor._data = data;
            editor._treeView = new SimpleTreeView(editor.FetchData(), data.Name);
            editor._treeView.ContextMenu = GetPaneGenericMenu(editor._treeView);
            editor._treeView.OnRename = (item) =>
            {
                var itemData = item.UserData as ScriptableObject;
                if (itemData == null)
                    return;

                itemData.GetType().GetField("Reference")?.SetValue(itemData, item.Name);
            };
            editor.Window = new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left, genericMenu: GetPaneGenericMenu(editor._treeView))
                .SetBody(editor.Body, genericMenu: GetBodyGenericMenu(editor._treeView))
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
                GetPaneGenericMenu(_treeView).DropDown(GetLastRect());
            }

            GUILayout.FlexibleSpace();
            GUILayout.Button("Revert", EditorStyles.toolbarButton);
            GUILayout.Button("Apply", EditorStyles.toolbarButton);
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
                            GetSelectionGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                        else GetBodyGenericMenu(_treeView).DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
            }
        }

        private void Footer()
        {
            GUILayout.Label("UI Menu Builder Footer", EditorStyles.boldLabel);
        }

        private SimpleTreeViewItem[] FetchData()
        {
            var items = new List<SimpleTreeViewItem>();

            foreach (var data in _data?.Root)
                items.Add(CreateItem(data, null));

            return items.ToArray();
        }

        public SimpleTreeViewItem CreateItem(ScriptableObject data, SimpleTreeViewItem parent = null) =>
            data switch
            {
                UIMenuCategoryData categoryData => CreateCategoryAndData(categoryData, parent),
                UIMenuHeaderData headerData => AttachUserData(CreateHeader(headerData.Name), headerData, parent),
                UIMenuSpacerData spacerData => AttachUserData(CreateSpace(), spacerData, parent),
                UIMenuButtonData buttonData => AttachUserData(CreateButton(buttonData.Name), buttonData, parent),
                UIMenuOptionsData optionsData => AttachUserData(CreateOptions(optionsData.Name), optionsData, parent),
                UIMenuInputData inputData => AttachUserData(CreateInput(inputData.Name), inputData, parent),
                UIMenuSliderData sliderData => AttachUserData(CreateSlider(sliderData.Name), sliderData, parent),
                UIMenuToggleData toggleData => AttachUserData(CreateToggle(toggleData.Name), toggleData, parent),
                UIMenuSelectionDataCollectionGroup selectionDataGroup => AttachUserData(CreateSelectionCollectionGroup(selectionDataGroup.Name), selectionDataGroup, parent),
                UIMenuSelectionDataCollection selectionData => AttachUserData(CreateSelectionCollectionGroup(selectionData.Name), selectionData, parent),
                UIMenuColorPickerData colorPickerData => AttachUserData(CreateColorPicker(colorPickerData.Name), colorPickerData, parent),
                UIMenuColorSliderData colorSliderData => AttachUserData(CreateColorSlider(colorSliderData.Name), colorSliderData, parent),
                _ => null
            };

        private SimpleTreeViewItem CreateCategoryAndData(UIMenuCategoryData categoryData, SimpleTreeViewItem parent)
        {
            var categoryItem = AttachUserData(CreateCategory(categoryData.Name), categoryData, parent);
            foreach (var item in categoryData.Data)
                CreateItem(item, categoryItem);
            return categoryItem;
        }

        private SimpleTreeViewItem AttachUserData(SimpleTreeViewItem item, ScriptableObject data, SimpleTreeViewItem parent)
        {
            if (parent != null)
                item.Parent = parent;
            item.SetUserData(data);
            return item;
        }

        private Rect GetLastRect()
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.y += 20;
            return lastRect;
        }

        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly Dictionary<ScriptableObject, bool> _foldoutStates = new();
        private CustomScriptableObjectDrawer _customScriptableObjectDrawer = new();
        private void CreateDynamicBox(SimpleTreeViewItem item)
        {
            var itemData = item.UserData as ScriptableObject;
            if (itemData == null)
                return;

            itemData.GetType().GetField("Name")?.SetValue(itemData, item.Name);

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

    }
}
#endif