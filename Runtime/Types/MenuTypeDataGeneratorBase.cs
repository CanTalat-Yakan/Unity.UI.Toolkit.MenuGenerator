using UnityEngine.UIElements;

namespace UnityEssentials
{
    public abstract class MenuTypeDataGeneratorBase<T> where T : IMenuTypeData
    {
        public static readonly string Path = "UIToolkit/UXML/Templates_Types_UI_";

        public abstract VisualElement CreateElement(MenuGenerator menu, T data);
        public abstract void ConfigureVisuals(MenuGenerator menu, VisualElement element, T data);
        public abstract void ConfigureInteraction(MenuGenerator menu, VisualElement element, T data);
    }
}