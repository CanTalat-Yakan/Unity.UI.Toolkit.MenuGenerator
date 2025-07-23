using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuOptionsDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuOptionsData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuOptionsData typedData)
                    using (var generator = new UIMenuOptionsDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Options_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuOptionsData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuOptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var dropdown = element.Q<DropdownField>("Options");

            if (data.Options == null || data.Options.Length == 0)
                return;

            var index = menu.Profile.Get<int>(data);
            if (data.Reverse)
                index = (data.Options.Length - 1) - index;

            dropdown.choices = data.GetChoices();
            dropdown.index = index;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuOptionsData data)
        {
            var dropdown = element.Q<DropdownField>("Options");
            dropdown.RegisterValueChangedCallback((evt) =>
            {
                var index = dropdown.index;
                if (data.Reverse)
                    index = (data.Options.Length - 1) - index;

                menu.Profile.Set(data.Reference, index);
            });

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var index = menu.Profile.Get<int>(data);
                if (data.Reverse)
                    index = (data.Options.Length - 1) - index;

                index = ProcessIndex(index - 1, data.Options.Length);
                dropdown.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var index = menu.Profile.Get<int>(data);
                if (data.Reverse)
                    index = (data.Options.Length - 1) - index;

                index = ProcessIndex(index + 1, data.Options.Length);
                dropdown.index = index;
            };
        }

        private static int ProcessIndex(int index, int maxIndex, int minIndex = 0)
        {
            if (maxIndex <= 0)
                return 0;

            int range = maxIndex - minIndex;

            if (index < minIndex)
                index += range * ((minIndex - index) / range + 1);

            if (index >= maxIndex)
                index -= range * ((index - maxIndex) / range + 1);

            return index;
        }

        public void Dispose() { }
    }
}