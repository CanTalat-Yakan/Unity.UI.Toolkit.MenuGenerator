using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuCategoryDataGenerator : UIMenuGeneratorTypeBase<UIMenuCategoryData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuCategoryData categoryData)
                    return;

                using (var categoryDataGenerator = new UIMenuCategoryDataGenerator())
                    generator.AddElementToScrollView(categoryDataGenerator.CreateElement(generator, categoryData));
            };
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuCategoryData data)
        {
            const string ResourcePath = Path + "Category_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Populate(false, data.Name, data.Data);
        }

        public void Dispose() { }
    }
}