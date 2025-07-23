using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuOptionsData : UIMenuTypeDataBase
    {
        [Space]
        public bool Reverse;
        public string[] Options;

        [Space]
        public int Default;

        public string GetOption(int index) =>
            GetChoices()[index] ?? string.Empty;

        public List<string> GetChoices()
        {
            if(Options == null || Options.Length == 0)
                return new List<string>() { "NA" };
            if (Reverse)
                return Options?.Reverse().ToList();
            return Options?.ToList();
        } 

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            Options = Array.Empty<string>();
            Default = 0;
        }
    }
}