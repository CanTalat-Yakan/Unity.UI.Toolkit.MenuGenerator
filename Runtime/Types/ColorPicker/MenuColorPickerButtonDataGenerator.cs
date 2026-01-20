using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuColorPickerButtonDataGenerator : MenuTypeDataGeneratorBase<MenuColorPickerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuColorPickerData typedData)
                    using (var generator = new MenuColorPickerButtonDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "ColorPickerButton_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuColorPickerData data)
        {
            var element = ResourceLoader.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuColorPickerData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name;

            var color = menu.Profile.Value.Get(data.Reference, data.Default);
            var colorElement = element.Q<VisualElement>("Color");
            colorElement.SetBackgroundColor(color);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuColorPickerData data)
        {
            var colorElement = element.Q<VisualElement>("Color");
            var button = element.Q<Button>("Button");
            button.clicked += () =>
            {
                menu.Populate(false, data.Name, null, () =>
                {
                    using (var colorPickerGenerator = new MenuColorPickerDataGenerator())
                        menu.AddToScrollView(colorPickerGenerator.CreateElement(menu, data));
                });
            };
        }

        public void Dispose() { }
    }
}