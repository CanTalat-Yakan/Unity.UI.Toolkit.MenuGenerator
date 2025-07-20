using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuOptionsData : UIMenuTypeDataBase
    {
        [Space]
        public string[] Options;

        [Space]
        public int Default;

        public string GetOption(int index) =>
            GetChoices()[index] ?? string.Empty;

        public List<string> GetChoices() => 
            Options?.ToList() ?? new List<string>() { "NA" };

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            Options = Array.Empty<string>();
            Default = 0;
        }
    }
}