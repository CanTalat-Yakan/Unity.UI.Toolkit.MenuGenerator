using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuToggleDataGenerator : MenuTypeDataGeneratorBase<MenuToggleData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuToggleData typedData)
                    using (var generator = new MenuToggleDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Toggle_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuToggleData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var value = menu.Profile.Value.Get(data.Reference, data.Default);

            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback((evt) =>
            {
                menu.Profile.Value.Set(data.Reference, evt.newValue);
            });
        }

        public void Dispose() { }
    }
}