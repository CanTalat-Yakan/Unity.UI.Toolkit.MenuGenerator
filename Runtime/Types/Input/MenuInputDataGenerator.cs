using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuInputDataGenerator : MenuTypeDataGeneratorBase<MenuInputData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuInputData typedData)
                    using (var generator = new MenuInputDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Input_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuInputData data)
        {
            var element = AssetResolver.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuInputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var inputField = element.Q<TextField>("Input");

            var input = menu.Profile.Value.Get(data.Reference, data.Default);

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
                menu.Profile.Value.Set(data.Reference, evt.newValue));
        }

        public void Dispose() { }
    }
}