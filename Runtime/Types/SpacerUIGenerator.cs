using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Style_Spacer_", menuName = "UI/Spacer", order = 3)]
    public class SpacerData : ScriptableObject
    {
        public int Height;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddSpacer(SpacerData data)
        {
            var element = CreateSpacer(data);

            AddElementToScrollView(element);
        }
        
        private void AddSpacer(int height)
        {
            var element = CreateSpacer(height);

            AddElementToScrollView(element);
        }

        private VisualElement CreateSpacer(SpacerData data)
        {
            var element = UIGeneratorData.SpacerTemplate.CloneTree();

            ConfigureOptionsVisuals(element, data);

            return element;
        }

        private VisualElement CreateSpacer(int height)
        {
            var data = ScriptableObject.CreateInstance<SpacerData>();
            data.Height = height;

            return CreateSpacer(data);
        }

        private void ConfigureOptionsVisuals(VisualElement element, SpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }
    }
}