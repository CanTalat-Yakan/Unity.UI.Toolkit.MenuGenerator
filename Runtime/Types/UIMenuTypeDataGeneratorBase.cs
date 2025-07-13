using UnityEngine.UIElements;

namespace UnityEssentials
{
    public abstract class UIMenuTypeDataGeneratorBase<T> where T : UIMenuTypeDataBase
    {
        public const string Path = "UIToolkit/UXML/Templates_Types_UI_";

        public abstract VisualElement CreateElement(UIMenuGenerator menu, T data);
        public abstract void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, T data);
        public abstract void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, T data);
    }
}
