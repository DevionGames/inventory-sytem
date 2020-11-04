using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    public class SelectableUIStat : UIStat
    {
        protected override StatsHandler handler {
            get {
                if (SelectableObject.current != null) {
                    return SelectableObject.current.GetComponent<StatsHandler>();

                }
                return null;
            }
        }

        protected override Stat stat
        {
            get
            {
                if (handler != null)
                    return handler.GetStat(this.m_StatName);

                return null;
            }
        }
    }
}