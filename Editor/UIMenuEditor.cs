#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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
            UIMenu.ShowUIBuilder = (data) => ShowUtility(data ?? new());

        private static void ShowUtility(UIMenuData data)
        {
            var editor = new UIMenuEditor();
            editor._data = data;
            editor._treeView = new SimpleTreeView(editor.FetchData(), data.Name);
            editor._treeView.ContextMenu = editor.GetPaneGenericMenu();
            editor._treeView.OnRename = (item) =>
            {
                var itemData = item.UserData as ScriptableObject;
                if (itemData == null)
                    return;

                itemData.GetType().GetField("Reference")?.SetValue(itemData, item.Name);
            };
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

            SimpleTreeViewBreadcrumbs.Draw(item, clickedItem =>
            {
                _treeView.SetSelectedItems(clickedItem.id);
                _treeView.Repaint();
            });

            CreateDynamicBox(item);
            if (item.UserData is UIMenuCategoryData category)
            {
                foreach (var child in item.Children)
                    CreateDynamicBox(child);

                GUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Item", GUILayout.Width(200), GUILayout.Height(24)))
                        GetBodyGenericMenu().DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                    GUILayout.FlexibleSpace();
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

            foreach (var data in _data.Root)
                items.Add(CreateItem(data, null));

            if (items.Count == 0)
                items.Add(CreateCategory());

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
                UIMenuSelectionData selectionData => AttachUserData(CreateSelection(selectionData.Name), selectionData, parent),
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
            menu.AddItem(new("Add Button"), false, () => AddButton(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Options"), false, () => AddOptions(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Input"), false, () => AddInput(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Slider"), false, () => AddSlider(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Toggle"), false, () => AddToggle(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Selection"), false, () => AddSelection(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(parent: _treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(parent: _treeView.GetSelectedItem()?.id));
            return menu;
        }

        private static Texture2D FolderIcon = EditorIconUtilities.GetTexture(EditorIconNames.VerticalLayoutGroupIcon);
        private static Texture2D HeaderIcon = EditorIconUtilities.GetTexture(EditorIconNames.TextIcon);
        private static Texture2D ButtonIcon = EditorIconUtilities.GetTexture(EditorIconNames.ButtonIcon);
        private static Texture2D OptionsIcon = EditorIconUtilities.GetTexture(EditorIconNames.DropdownIcon);
        private static Texture2D InputIcon = EditorIconUtilities.GetTexture(EditorIconNames.InputFieldIcon);
        private static Texture2D SliderIcon = EditorIconUtilities.GetTexture(EditorIconNames.SliderIcon);
        private static Texture2D ToggleIcon = EditorIconUtilities.GetTexture(EditorIconNames.ToggleIcon);
        private static Texture2D SelectionIcon = EditorIconUtilities.GetTexture(EditorIconNames.SelectableIcon);
        private static Texture2D ColorPickerIcon = EditorIconUtilities.GetTexture(EditorIconNames.GridPickingTool);
        private static Texture2D ColorSliderIcon = EditorIconUtilities.GetTexture(EditorIconNames.GridPaintTool);

        private void AddCategory(string name = "Category", int? parent = null) => _treeView.AddItem(CreateCategory(), parent);
        private SimpleTreeViewItem CreateCategory(string name = "Category") =>
            new SimpleTreeViewItem(name, FolderIcon).SetUserTag(UIMenuDataTypes.Category.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuCategoryData>());

        private void AddHeader(string name = "Header", int? parent = null) => _treeView.AddItem(CreateHeader(name), parent);
        private SimpleTreeViewItem CreateHeader(string name = "Header") =>
            new SimpleTreeViewItem(name, HeaderIcon).Support(false).SetUserTag(UIMenuDataTypes.Header.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuHeaderData>());

        private void AddSpace(int? parent = null) => _treeView.AddItem(CreateSpace(), parent);
        private SimpleTreeViewItem CreateSpace() =>
            new SimpleTreeViewItem(string.Empty).Support(false, false).SetUserTag(UIMenuDataTypes.Space.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSpacerData>());

        private void AddButton(int? parent = null) => _treeView.AddItem(CreateButton(), parent, false);
        private SimpleTreeViewItem CreateButton(string name = "Button") =>
            new SimpleTreeViewItem(name, ButtonIcon).Support(false).SetUserTag(UIMenuDataTypes.Button.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorSliderData>());

        private void AddOptions(int? parent = null) => _treeView.AddItem(CreateOptions(), parent, false);
        private SimpleTreeViewItem CreateOptions(string name = "Options") =>
            new SimpleTreeViewItem(name, OptionsIcon).Support(false).SetUserTag(UIMenuDataTypes.Options.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuOptionsData>());

        private void AddInput(int? parent = null) => _treeView.AddItem(CreateInput(), parent, false);
        private SimpleTreeViewItem CreateInput(string name = "Input") =>
            new SimpleTreeViewItem(name, InputIcon).Support(false).SetUserTag(UIMenuDataTypes.Input.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuInputData>());

        private void AddSlider(int? parent = null) => _treeView.AddItem(CreateSlider(), parent, false);
        private SimpleTreeViewItem CreateSlider(string name = "Slider") =>
            new SimpleTreeViewItem(name, SliderIcon).Support(false).SetUserTag(UIMenuDataTypes.Slider.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSliderData>());

        private void AddToggle(int? parent = null) => _treeView.AddItem(CreateToggle(), parent, false);
        private SimpleTreeViewItem CreateToggle(string name = "Toggle") =>
            new SimpleTreeViewItem(name, ToggleIcon).Support(false).SetUserTag(UIMenuDataTypes.Toggle.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuToggleData>());

        private void AddSelection(int? parent = null) => _treeView.AddItem(CreateSelection(), parent, false);
        private SimpleTreeViewItem CreateSelection(string name = "Selection") =>
            new SimpleTreeViewItem(name, SelectionIcon).Support(false).SetUserTag(UIMenuDataTypes.Selection.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSelectionData>());

        private void AddColorPicker(int? parent = null) => _treeView.AddItem(CreateColorPicker(), parent, false);
        private SimpleTreeViewItem CreateColorPicker(string name = "Color Picker") =>
            new SimpleTreeViewItem(name, ColorPickerIcon).Support(false).SetUserTag(UIMenuDataTypes.ColorPicker.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorPickerData>());

        private void AddColorSlider(int? parent = null) => _treeView.AddItem(CreateColorSlider(), parent, false);
        private SimpleTreeViewItem CreateColorSlider(string name = "Color Slider") =>
            new SimpleTreeViewItem(name, ColorSliderIcon).Support(false).SetUserTag(UIMenuDataTypes.ColorSlider.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorSliderData>());

        private readonly Dictionary<ScriptableObject, Editor> _editorCache = new();
        private readonly Dictionary<ScriptableObject, bool> _foldoutStates = new();
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
                    isExpanded = EditorGUILayout.Foldout(isExpanded, $"{item.Name} ({item.UserTag})", true);
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

                        var drawArrays = itemData is not UIMenuCategoryData;
                        DrawScriptableObjectInline(editor, drawArrays);

                        GUILayout.Space(10);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawScriptableObjectInline(Editor scriptableEditor, bool drawArrays = true)
        {
            if (scriptableEditor == null)
                return;

            var so = scriptableEditor.serializedObject;
            so.Update();

            SerializedProperty iterator = so.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                // Skip the internal script reference and "Name" property
                if (iterator.name == "m_Script" || iterator.name == "Name")
                    continue;

                // Skip arrays if requested, but allow normal strings
                if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String && !drawArrays)
                {
                    enterChildren = false;
                    continue;
                }

                // If it's an actual array (not a string), draw reorderable list
                if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String)
                {
                    var arrayProperty = iterator.Copy();
                    if (arrayProperty.arraySize >= 0)
                        DrawReorderablePropertyList(so, arrayProperty);
                }
                else EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            so.ApplyModifiedProperties();
        }

        private readonly Dictionary<string, ReorderableList> _listsCache = new();
        private void DrawReorderablePropertyList(SerializedObject so, SerializedProperty property)
        {
            if (property == null || !property.isArray || property.propertyType == SerializedPropertyType.String)
                return;

            // Compose a unique key using the target object's instance ID and the property path
            string cacheKey = $"{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}";

            ReorderableList reorderableList = null;
            if (!_listsCache.TryGetValue(cacheKey, out reorderableList))
            {
                reorderableList = new ReorderableList(so, property, true, true, true, true);
                _listsCache.Add(cacheKey, reorderableList);
            }

            reorderableList.drawHeaderCallback = (Rect position) =>
            {
                position.x -= 15;
                EditorGUI.LabelField(position, property.displayName);
            };

            reorderableList.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                position.y += 2;
                position.height -= 4;
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };

            var listRect = EditorGUILayout.GetControlRect(false, reorderableList.GetHeight());
            listRect = EditorGUI.IndentedRect(listRect);
            reorderableList.DoList(listRect);
        }
    }
}
#endif