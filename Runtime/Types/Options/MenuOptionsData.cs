using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class MenuOptionsData : MenuTypeDataBase
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
            if (Options == null || Options.Length == 0)
                return new List<string>() { "NA" };
            if (Reverse)
                return Options?.Reverse().ToList();
            return Options?.ToList();
        }

        public override object GetDefault()
        {
            if (Options == null || Options.Length == 0)
                return 0;
            if (Reverse)
                return Options.Length - 1 - Default;
            return Default;
        }

        public override void ApplyDynamicReset()
        {
            Reverse = false;
            Options = Array.Empty<string>();
            Default = 0;
        }
    }
}