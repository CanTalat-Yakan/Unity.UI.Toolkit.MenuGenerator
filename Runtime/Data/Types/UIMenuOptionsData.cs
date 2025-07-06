using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuOptionsData : UIGeneratorTypeTemplate
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
            var choices = Options.ToList();
            if (Reverse) choices?.Reverse();
            return choices ?? new List<string>() { "NA" };
        }

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.Options.Add(Reference, Default);

        public override void ApplyDynamicReset() =>
            Options = Array.Empty<string>();
    }
}