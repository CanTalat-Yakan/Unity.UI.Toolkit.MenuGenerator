using System;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateOptions(UIMenuGenerator menu, UIMenuOptionsData data)
        {
            var element = menu.Data.OptionsTemplate.CloneTree();
            ConfigureOptionsVisuals(menu.Profile, element, data);
            ConfigureOptionsInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureOptionsVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuOptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var dropdown = element.Q<DropdownField>("Options");

            if (data.Options == null || data.Options.Length == 0)
                return;

            var index = profile.GetData(data.Reference, data.Default);

            dropdown.choices = data.GetChoices();
            dropdown.index = index;
        }

        private static void ConfigureOptionsInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuOptionsData data)
        {
            var dropdown = element.Q<DropdownField>("Options");
            dropdown.RegisterValueChangedCallback(e =>
                profile.OnOptionsValueChanged(data.Reference, dropdown.index));

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var length = data.Options.Length;

                var index = profile.GetData(data.Reference, data.Default);

                index = ProcessIndex(index - 1, length);
                profile.OnOptionsValueChanged(data.Reference, index);

                dropdown.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var length = data.Options.Length;

                var index = profile.GetData(data.Reference, data.Default);

                index = ProcessIndex(index + 1, length);
                profile.OnOptionsValueChanged(data.Reference, index);

                dropdown.index = index;
            };
        }

        private static int ProcessIndex(int index, int maxIndex, int minIndex = 0)
        {
            if (maxIndex <= 0)
                return 0; // Avoid division by zero or negative index

            // Ensure the index is within the bounds
            int range = maxIndex - minIndex;

            if (index < minIndex)
                // Adjust for negative values
                index += range * ((minIndex - index) / range + 1);

            if (index >= maxIndex)
                // Adjust for values larger than maxIndex
                index -= range * ((index - maxIndex) / range + 1);

            return index;
        }
    }
}