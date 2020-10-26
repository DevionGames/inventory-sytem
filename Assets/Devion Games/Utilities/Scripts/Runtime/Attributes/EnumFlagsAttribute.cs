using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public readonly string label;
        public readonly string tooltip;

        public EnumFlagsAttribute():this(string.Empty,string.Empty) {
        }

        public EnumFlagsAttribute(string label) : this(label, string.Empty)
        {
        }

        public EnumFlagsAttribute(string label, string tooltip) {
            this.label = label;
            this.tooltip = tooltip;
        }
    }
}