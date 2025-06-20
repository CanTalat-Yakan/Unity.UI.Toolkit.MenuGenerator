using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "OptionsData_", menuName = "UI/Data/Options", order = 2)]
    public class OptionsData : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public string[] Options;
        public bool Reverse;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateOptions(UIMenuGenerator menu, OptionsData data)
        {
            var element = menu.UIGeneratorData.OptionsTemplate.CloneTree();

            ConfigureOptionsVisuals(menu.Profile, element, data);
            ConfigureOptionsInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureOptionsVisuals(UIMenuDataProfile profile, VisualElement element, OptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var dropdown = element.Q<DropdownField>("Options");

            profile.OptionsDataDictionary.TryGetValue(data.Reference, out var dropDownindex);

            dropdown.choices = data.Options.ToList();
            dropdown.index = dropDownindex;
            dropdown.value = data.Options[dropDownindex];
        }

        private static void ConfigureOptionsInteraction(UIMenuDataProfile profile, VisualElement element, OptionsData data)
        {
            var dropdownField = element.Q<DropdownField>("Options");
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                profile.OnOptionsChange(data.Reference, dropdownField.index);
            });

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var length = data.Options.Length;

                var index = 0;
                profile.OptionsDataDictionary.TryGetValue(data.Reference, out index);

                index = ProcessIndex(!data.Reverse ? index - 1 : index + 1, length);
                profile.OnOptionsChange(data.Reference, ProcessIndex(index, length));

                dropdownField.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var length = data.Options.Length;

                var index = 0;
                profile.OptionsDataDictionary.TryGetValue(data.Reference, out index);

                index = ProcessIndex(!data.Reverse ? index + 1 : index - 1, length);
                profile.OnOptionsChange(data.Reference, index);

                dropdownField.index = index;
            };
        }

        private static int ProcessIndex(int index, int maxIndex, int minIndex = 0)
        {
            // Ensure the index is within the bounds
            int range = maxIndex - minIndex;

            if (index < minIndex)
                index += range * ((minIndex - index) / range + 1); // Adjust for negative values

            if (index >= maxIndex)
                index -= range * ((index - maxIndex) / range + 1); // Adjust for values larger than maxIndex

            return index;
        }
    }
}