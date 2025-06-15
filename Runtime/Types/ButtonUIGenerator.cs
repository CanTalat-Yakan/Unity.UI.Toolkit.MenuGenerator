using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Button_", menuName = "UI/Button", order = 2)]
    public class ButtonData : ScriptableObject
    {
        public string Name;

        [Space]
        public Texture2D Texture;

        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        public void InvokeEvent() =>
            Event?.Invoke();

        public void InvokeAltEvent() =>
            AltEvent?.Invoke();
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddButton(ButtonData data)
        {
            var element = CreateButton(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateButton(ButtonData data)
        {
            var element = UIGeneratorData.CategoryTemplate.CloneTree();

            ConfigureButtonVisuals(element, data);
            ConfigureButtonInteraction(element, data);

            return element;
        }

        private void ConfigureButtonVisuals(VisualElement element, ButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        private void ConfigureButtonInteraction(VisualElement element, ButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => data.InvokeEvent();
        }
    }
}