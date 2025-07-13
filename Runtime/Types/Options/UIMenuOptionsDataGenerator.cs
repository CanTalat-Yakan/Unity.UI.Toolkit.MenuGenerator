using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuOptionsDataGenerator : UIMenuGeneratorTypeBase<UIMenuOptionsData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuOptionsData optionsData)
                    return;

                using (var optionsDataGenerator = new UIMenuOptionsDataGenerator())
                    generator.AddElementToScrollView(optionsDataGenerator.CreateElement(generator, optionsData));
            };

        public static readonly string ResourcePath = Path + "Options_UXML";
        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuOptionsData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuOptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var dropdown = element.Q<DropdownField>("Options");

            if (data.Options == null || data.Options.Length == 0)
                return;

            var index = menu.Profile.GetData(data.Reference, data.Default);

            dropdown.choices = data.GetChoices();
            dropdown.index = index;
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuOptionsData data)
        {
            var dropdown = element.Q<DropdownField>("Options");
            dropdown.RegisterValueChangedCallback(e =>
                menu.Profile.OnOptionsValueChanged(data.Reference, dropdown.index));

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var length = data.Options.Length;

                var index = menu.Profile.GetData(data.Reference, data.Default);

                index = ProcessIndex(index - 1, length);
                menu.Profile.OnOptionsValueChanged(data.Reference, index);

                dropdown.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var length = data.Options.Length;

                var index = menu.Profile.GetData(data.Reference, data.Default);

                index = ProcessIndex(index + 1, length);
                menu.Profile.OnOptionsValueChanged(data.Reference, index);

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