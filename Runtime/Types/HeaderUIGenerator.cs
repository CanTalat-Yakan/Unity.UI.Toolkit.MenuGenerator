using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Style_Header_", menuName = "UI/Header", order = 3)]
    public class HeaderData : ScriptableObject
    {
        public string Text;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddHeader(HeaderData data)
        {
            var element = CreateHeader(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateHeader(HeaderData data)
        {
            var element = UIGeneratorData.HeaderTemplate.CloneTree();

            ConfigureHeaderVisuals(element, data);

            return element;
        }

        private void ConfigureHeaderVisuals(VisualElement element, HeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Text.ToUpper();
        }
    }
}