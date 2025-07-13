using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public abstract class UIMenuGeneratorTypeBase<T> where T : UIMenuTypeBase
    {
        public const string Path = "UIToolkit/UXML/Templates_Types_UI_";

        public abstract VisualElement CreateElement(UIMenuDataGenerator menu, T data);
        public abstract void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, T data);
        public abstract void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, T data);
    }
}
