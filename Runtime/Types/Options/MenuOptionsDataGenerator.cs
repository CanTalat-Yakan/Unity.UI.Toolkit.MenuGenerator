using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuOptionsDataGenerator : MenuTypeDataGeneratorBase<MenuOptionsData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuOptionsData typedData)
                    using (var generator = new MenuOptionsDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Options_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuOptionsData data)
        {
            var element = ResourceLoader.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuOptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var dropdown = element.Q<DropdownField>("Options");

            if (data.Options == null || data.Options.Length == 0)
                return;

            var index = menu.Profile.Value.Get(data.Reference, data.Default);
            if (data.Reverse)
                index = (data.Options.Length - 1) - index;

            dropdown.choices = data.GetChoices();
            dropdown.index = index;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuOptionsData data)
        {
            var dropdown = element.Q<DropdownField>("Options");
            dropdown.RegisterValueChangedCallback((evt) =>
            {
                var index = dropdown.index;
                if (data.Reverse)
                    index = (data.Options.Length - 1) - index;

                menu.Profile.Value.Set(data.Reference, index);
            });

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var index = menu.Profile.Value.Get(data.Reference, data.Default);
                if (data.Reverse)
                    index = (data.Options.Length - 1) - index;

                index = ProcessIndex(index - 1, data.Options.Length);
                dropdown.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var index = menu.Profile.Value.Get(data.Reference, data.Default);
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