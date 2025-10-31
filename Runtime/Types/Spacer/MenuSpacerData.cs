namespace UnityEssentials
{
    public class MenuSpacerData : MenuTypeDataBase
    {
        public int Height = 20;

        public override object GetDefault() => null;

        public override void ApplyDynamicReset() =>
            Height = 20;
    }
}