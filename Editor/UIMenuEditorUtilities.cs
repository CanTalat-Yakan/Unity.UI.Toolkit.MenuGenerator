#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

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

    public class UIMenuEditorUtilities : MonoBehaviour
    {
        public static void PopulateDataFromTree(UIMenuData data, SimpleTreeView treeView)
        {
            if (treeView?.RootItem == null)
                return;

            data.Root = treeView.RootItem.Children.Select(i => i.UserData as ScriptableObject).ToArray();
            EditorUtility.SetDirty(data); 
            AssetDatabase.SaveAssets();

            void Traverse(SimpleTreeViewItem item)
            {
                if (item == null)
                    return;

                var data = item.UserData as ScriptableObject;
                if (data != null)
                {
                    var dataField = data.GetType().GetField("Data");
                    if (dataField != null)
                    {
                        var childrenData = item.Children.Select(child => child.UserData as ScriptableObject).ToArray();
                        dataField.SetValue(data, childrenData);
                    }
                }

                foreach (var child in item.Children)
                    Traverse(child);
            }

            Traverse(treeView.RootItem);
        }

        public static SimpleTreeViewItem CreateItem(ScriptableObject data, SimpleTreeView treeView, SimpleTreeViewItem parent = null) =>
            data switch
            {
                UIMenuCategoryData categoryData => CreateItemRecursive(CreateCategory(treeView, categoryData.Name), categoryData, treeView, parent),
                UIMenuHeaderData headerData => CreateItemRecursive(CreateHeader(treeView, headerData.Name), headerData, treeView, parent),
                UIMenuSpacerData spacerData => CreateItemRecursive(CreateSpace(), spacerData, treeView, parent),
                UIMenuButtonData buttonData => CreateItemRecursive(CreateButton(treeView, buttonData.Name), buttonData, treeView, parent),
                UIMenuOptionsData optionsData => CreateItemRecursive(CreateOptions(treeView, optionsData.Name), optionsData, treeView, parent),
                UIMenuInputData inputData => CreateItemRecursive(CreateInput(treeView, inputData.Name), inputData, treeView, parent),
                UIMenuSliderData sliderData => CreateItemRecursive(CreateSlider(treeView, sliderData.Name), sliderData, treeView, parent),
                UIMenuToggleData toggleData => CreateItemRecursive(CreateToggle(treeView, toggleData.Name), toggleData, treeView, parent),
                UIMenuSelectionDataCategory selectionDataCategory => CreateItemRecursive(CreateSelectionCategory(treeView, selectionDataCategory.Name), selectionDataCategory, treeView, parent),
                UIMenuSelectionDataGroup selectionDataGroup => CreateItemRecursive(CreateSelectionGroup(treeView, selectionDataGroup.Name), selectionDataGroup, treeView, parent),
                UIMenuColorPickerData colorPickerData => CreateItemRecursive(CreateColorPicker(treeView, colorPickerData.Name), colorPickerData, treeView, parent),
                UIMenuColorSliderData colorSliderData => CreateItemRecursive(CreateColorSlider(treeView, colorSliderData.Name), colorSliderData, treeView, parent),
                _ => null
            };

        public static SimpleTreeViewItem CreateItemRecursive<T>(SimpleTreeViewItem item, T data, SimpleTreeView treeView, SimpleTreeViewItem parent) where T : ScriptableObject
        {
            if (item == null || data == null)
                return null;

            item.SetUserData(data);

            if (parent != null)
                item.Parent = parent;

            var dataField = data.GetType().GetField("Data");
            var dataValue = dataField?.GetValue(data) as ScriptableObject[];

            if (dataValue != null)
                foreach (var child in dataValue)
                    CreateItem(child, treeView, item);

            treeView.SetExpanded(item.id, true);

            return item;
        }

        public static GenericMenu GetGenericMenu(SimpleTreeView treeView)
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Category"), false, () => AddCategory(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Header"), false, () => AddHeader(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Space"), false, () => AddSpace(treeView, treeView.GetSelectedItem()?.id));
            menu.AddSeparator("");
            menu.AddItem(new("Add Button"), false, () => AddButton(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Options"), false, () => AddOptions(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Input"), false, () => AddInput(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Slider"), false, () => AddSlider(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Toggle"), false, () => AddToggle(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Selection Category"), false, () => AddSelectionCategory(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(treeView, treeView.GetSelectedItem()?.id));
            return menu;
        }

        public static GenericMenu GetSelectionGenericMenu(SimpleTreeView treeView)
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Header"), false, () => AddHeader(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Space"), false, () => AddSpace(treeView, treeView.GetSelectedItem()?.id));
            menu.AddSeparator("");
            menu.AddItem(new("Add Selection Group"), false, () => AddSelectionGroup(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(treeView, treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(treeView, treeView.GetSelectedItem()?.id));
            return menu;
        }

        public static Texture2D FolderIcon = EditorIconUtilities.GetTexture(EditorIconNames.VerticalLayoutGroupIcon);
        public static Texture2D HeaderIcon = EditorIconUtilities.GetTexture(EditorIconNames.TextIcon);
        public static Texture2D ButtonIcon = EditorIconUtilities.GetTexture(EditorIconNames.ButtonIcon);
        public static Texture2D OptionsIcon = EditorIconUtilities.GetTexture(EditorIconNames.DropdownIcon);
        public static Texture2D InputIcon = EditorIconUtilities.GetTexture(EditorIconNames.InputFieldIcon);
        public static Texture2D SliderIcon = EditorIconUtilities.GetTexture(EditorIconNames.SliderIcon);
        public static Texture2D ToggleIcon = EditorIconUtilities.GetTexture(EditorIconNames.ToggleIcon);
        public static Texture2D SelectionIcon = EditorIconUtilities.GetTexture(EditorIconNames.SelectableIcon);
        public static Texture2D ColorPickerIcon = EditorIconUtilities.GetTexture(EditorIconNames.GridPickingTool);
        public static Texture2D ColorSliderIcon = EditorIconUtilities.GetTexture(EditorIconNames.GridPaintTool);

        public static void  AddCategory(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateCategory(treeView), parent);
        public static SimpleTreeViewItem CreateCategory(SimpleTreeView treeView, string name = "Category") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(FolderIcon)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Category.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuCategoryData>(name, uniqueName));

        public static void AddHeader(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateHeader(treeView), parent, false);
        public static SimpleTreeViewItem CreateHeader(SimpleTreeView treeView, string name = "Header") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(HeaderIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Header.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuHeaderData>(name, uniqueName, hasReference: false));

        public static void AddSpace(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateSpace(), parent, false);
        public static SimpleTreeViewItem CreateSpace() =>
            new SimpleTreeViewItem()
                .SetName(string.Empty)
                .Support(allowChildren: false, allowRenaming: false)
                .SetUserTag(UIMenuDataTypes.Space.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuSpacerData>(hasReference: false));

        public static void AddButton(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateButton(treeView), parent, false);
        public static SimpleTreeViewItem CreateButton(SimpleTreeView treeView, string name = "Button") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(ButtonIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Button.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuButtonData>(name, uniqueName));

        public static void AddOptions(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateOptions(treeView), parent, false);
        public static SimpleTreeViewItem CreateOptions(SimpleTreeView treeView, string name = "Options") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(OptionsIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Options.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuOptionsData>(name, uniqueName));

        public static void AddInput(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateInput(treeView), parent, false);
        public static SimpleTreeViewItem CreateInput(SimpleTreeView treeView, string name = "Input") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(InputIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Input.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuInputData>(name, uniqueName));

        public static void AddSlider(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateSlider(treeView), parent, false);
        public static SimpleTreeViewItem CreateSlider(SimpleTreeView treeView, string name = "Slider") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(SliderIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Slider.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuSliderData>(name, uniqueName));

        public static void AddToggle(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateToggle(treeView), parent, false);
        public static SimpleTreeViewItem CreateToggle(SimpleTreeView treeView, string name = "Toggle") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(ToggleIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Toggle.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuToggleData>(name, uniqueName));

        public static void AddSelectionCategory(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateSelectionCategory(treeView), parent, true);
        private static Type[] SupportedTypes = { typeof(UIMenuHeaderData), typeof(UIMenuSpacerData),
              typeof(UIMenuSelectionDataGroup), typeof(UIMenuColorSliderData), typeof(UIMenuColorPickerData) };
        public static SimpleTreeViewItem CreateSelectionCategory(SimpleTreeView treeView, string name = "Selection Category") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(SelectionIcon)
                .Support(SupportedTypes)
                .SetContextMenu(GetSelectionGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Selection.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuSelectionDataCategory>(name, uniqueName));

        public static void AddSelectionGroup(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateSelectionGroup(treeView), parent, false);
        public static SimpleTreeViewItem CreateSelectionGroup(SimpleTreeView treeView, string name = "Selection Group") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(SelectionIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.Selection.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuSelectionDataGroup>(name, uniqueName, hasReference: false));

        public static void AddColorPicker(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateColorPicker(treeView), parent, false);
        public static SimpleTreeViewItem CreateColorPicker(SimpleTreeView treeView, string name = "Color Picker") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(ColorPickerIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.ColorPicker.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuColorPickerData>(name, uniqueName));

        public static void AddColorSlider(SimpleTreeView treeView, int? parent = null) =>
            treeView.AddItem(CreateColorSlider(treeView), parent, false);
        public static SimpleTreeViewItem CreateColorSlider(SimpleTreeView treeView, string name = "Color Slider") =>
            new SimpleTreeViewItem()
                .SetName(name, out var uniqueName)
                .SetIcon(ColorSliderIcon)
                .Support(allowChildren: false)
                .SetContextMenu(GetGenericMenu(treeView))
                .SetUserTag(UIMenuDataTypes.ColorSlider.ToString())
                .SetUserData(UIGeneratorTypeTemplate.Initialize<UIMenuColorSliderData>(name, uniqueName));
    }
}
#endif