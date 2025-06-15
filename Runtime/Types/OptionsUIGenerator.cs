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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddOptions(OptionsData data)
        {
            var element = CreateOptions(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateOptions(OptionsData data)
        {
            var element = UIGeneratorData.OptionsTemplate.CloneTree();

            ConfigureOptionsVisuals(element, data);
            ConfigureOptionsInteraction(element, data);

            return element;
        }

        private void ConfigureOptionsVisuals(VisualElement element, OptionsData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var dropdown = element.Q<DropdownField>("Options");

            Profile.OptionsDataDictionary.TryGetValue(data.Reference, out var dropDownindex);
            dropdown.choices = data.Options.ToList();
            dropdown.index = dropDownindex;
            dropdown.value = data.Options[dropDownindex];
        }

        private void ConfigureOptionsInteraction(VisualElement element, OptionsData data)
        {
            var dropdownField = element.Q<DropdownField>("Options");
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                Profile.OnOptionsChange(data.Reference, dropdownField.index);
            });

            var buttonLeft = element.Q<Button>("Left");
            buttonLeft.clicked += () =>
            {
                var length = data.Options.Length;

                var index = 0;
                Profile.OptionsDataDictionary.TryGetValue(data.Reference, out index);

                index = ProcessIndex(!data.Reverse ? index - 1 : index + 1, length);
                Profile.OnOptionsChange(data.Reference, ProcessIndex(index, length));

                dropdownField.index = index;
            };

            var buttonRight = element.Q<Button>("Right");
            buttonRight.clicked += () =>
            {
                var length = data.Options.Length;

                var index = 0;
                Profile.OptionsDataDictionary.TryGetValue(data.Reference, out index);

                index = ProcessIndex(!data.Reverse ? index + 1 : index - 1, length);
                Profile.OnOptionsChange(data.Reference, index);

                dropdownField.index = index;
            };
        }

        private int ProcessIndex(int index, int maxIndex, int minIndex = 0)
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