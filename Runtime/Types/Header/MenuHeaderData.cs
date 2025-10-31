namespace UnityEssentials
{
    public class MenuHeaderData : MenuTypeDataBase
    {
        public int MarginTop = 20;

        public override object GetDefault() => null;

        public override void ApplyDynamicReset()
        {
            MarginTop = 20;
            Name = "Header";
        }
    }
}