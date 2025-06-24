using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuEditorUtilities : MonoBehaviour
    {
        public static GenericMenu GetPaneGenericMenu(SimpleTreeView treeView)
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Category"), false, () => AddCategory(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Header"), false, () => AddHeader(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Space"), false, () => AddSpace(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddSeparator(string.Empty);
            menu.AddItem(new("Add Button"), false, () => AddButton(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Options"), false, () => AddOptions(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Input"), false, () => AddInput(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Slider"), false, () => AddSlider(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Toggle"), false, () => AddToggle(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Selection Group"), false, () => AddSelectionCollectionGroup(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(treeView, parent: treeView.GetSelectedItem()?.id));
            return menu;
        }

        public static GenericMenu GetBodyGenericMenu(SimpleTreeView treeView)
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Button"), false, () => AddButton(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Options"), false, () => AddOptions(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Input"), false, () => AddInput(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Slider"), false, () => AddSlider(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Toggle"), false, () => AddToggle(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Selection Group"), false, () => AddSelectionCollectionGroup(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(treeView, parent: treeView.GetSelectedItem()?.id));
            return menu;
        }

        public static GenericMenu GetSelectionGenericMenu(SimpleTreeView treeView)
        {
            var menu = new GenericMenu();
            menu.AddItem(new("Add Selection Collection"), false, () => AddSelectionCollection(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Picker"), false, () => AddColorPicker(treeView, parent: treeView.GetSelectedItem()?.id));
            menu.AddItem(new("Add Color Slider"), false, () => AddColorSlider(treeView, parent: treeView.GetSelectedItem()?.id));
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

        public static void AddCategory(SimpleTreeView treeView, string name = "Category", int? parent = null) => treeView.AddItem(CreateCategory(), parent);
        public static SimpleTreeViewItem CreateCategory(string name = "Category") =>
            new SimpleTreeViewItem(name, FolderIcon).SetUserTag(UIMenuDataTypes.Category.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuCategoryData>());

        public static void AddHeader(SimpleTreeView treeView, string name = "Header", int? parent = null) => treeView.AddItem(CreateHeader(name), parent);
        public static SimpleTreeViewItem CreateHeader(string name = "Header") =>
            new SimpleTreeViewItem(name, HeaderIcon).Support(false).SetUserTag(UIMenuDataTypes.Header.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuHeaderData>());

        public static void AddSpace(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateSpace(), parent);
        public static SimpleTreeViewItem CreateSpace() =>
            new SimpleTreeViewItem(string.Empty).Support(false, false).SetUserTag(UIMenuDataTypes.Space.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSpacerData>());

        public static void AddButton(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateButton(), parent, false);
        public static SimpleTreeViewItem CreateButton(string name = "Button") =>
            new SimpleTreeViewItem(name, ButtonIcon).Support(false).SetUserTag(UIMenuDataTypes.Button.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorSliderData>());

        public static void AddOptions(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateOptions(), parent, false);
        public static SimpleTreeViewItem CreateOptions(string name = "Options") =>
            new SimpleTreeViewItem(name, OptionsIcon).Support(false).SetUserTag(UIMenuDataTypes.Options.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuOptionsData>());

        public static void AddInput(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateInput(), parent, false);
        public static SimpleTreeViewItem CreateInput(string name = "Input") =>
            new SimpleTreeViewItem(name, InputIcon).Support(false).SetUserTag(UIMenuDataTypes.Input.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuInputData>());

        public static void AddSlider(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateSlider(), parent, false);
        public static SimpleTreeViewItem CreateSlider(string name = "Slider") =>
            new SimpleTreeViewItem(name, SliderIcon).Support(false).SetUserTag(UIMenuDataTypes.Slider.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSliderData>());

        public static void AddToggle(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateToggle(), parent, false);
        public static SimpleTreeViewItem CreateToggle(string name = "Toggle") =>
            new SimpleTreeViewItem(name, ToggleIcon).Support(false).SetUserTag(UIMenuDataTypes.Toggle.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuToggleData>());

        public static void AddSelectionCollection(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateSelectionCollection(), parent, false);
        public static SimpleTreeViewItem CreateSelectionCollection(string name = "Selection Collection") =>
            new SimpleTreeViewItem(name, SelectionIcon).Support(false).SetUserTag(UIMenuDataTypes.Selection.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSelectionDataCollection>());

        public static void AddSelectionCollectionGroup(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateSelectionCollectionGroup(), parent, false);
        public static SimpleTreeViewItem CreateSelectionCollectionGroup(string name = "Selection Group") =>
            new SimpleTreeViewItem(name, SelectionIcon).Support(true).SetUserTag(UIMenuDataTypes.Selection.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuSelectionDataCollectionGroup>());

        public static void AddColorPicker(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateColorPicker(), parent, false);
        public static SimpleTreeViewItem CreateColorPicker(string name = "Color Picker") =>
            new SimpleTreeViewItem(name, ColorPickerIcon).Support(false).SetUserTag(UIMenuDataTypes.ColorPicker.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorPickerData>());

        public static void AddColorSlider(SimpleTreeView treeView, int? parent = null) => treeView.AddItem(CreateColorSlider(), parent, false);
        public static SimpleTreeViewItem CreateColorSlider(string name = "Color Slider") =>
            new SimpleTreeViewItem(name, ColorSliderIcon).Support(false).SetUserTag(UIMenuDataTypes.ColorSlider.ToString())
                .SetUserData(ScriptableObject.CreateInstance<UIMenuColorSliderData>());

    }
}
