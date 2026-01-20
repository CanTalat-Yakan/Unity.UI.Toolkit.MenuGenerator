using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuCategoryDataGenerator : MenuTypeDataGeneratorBase<MenuCategoryData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuCategoryData typedData)
                    using (var generator = new MenuCategoryDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Category_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuCategoryData data)
        {
            var element = AssetResolver.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name;

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Populate(false, data.Name, data.Data);
        }

        public void Dispose() { }
    }
}