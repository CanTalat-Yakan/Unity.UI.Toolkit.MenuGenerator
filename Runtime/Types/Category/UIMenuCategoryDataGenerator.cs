using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuCategoryDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuCategoryData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuCategoryData categoryData)
                    return;

                using (var categoryDataGenerator = new UIMenuCategoryDataGenerator())
                    generator.AddElementToScrollView(categoryDataGenerator.CreateElement(generator, categoryData));
            };

        public static readonly string ResourcePath = Path + "Category_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuCategoryData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Populate(false, data.Name, data.Data);
        }

        public void Dispose() { }
    }
}