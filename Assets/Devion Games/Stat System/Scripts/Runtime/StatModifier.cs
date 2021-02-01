using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    public enum StatModType
    {
        Flat,
        PercentAdd,
        PercentMult,
    }

    public class StatModifier
    {
        public object source;

        private float m_Value;
        public float Value
        {
            get
            {
                return this.m_Value;
            }
        }

        private StatModType m_Type;
        public StatModType Type
        {
            get
            {
                return this.m_Type;
            }
        }

        public StatModifier() : this(0f, StatModType.Flat, null)
        {
        }

        public StatModifier(float value, StatModType type, object source)
        {
            this.m_Value = value;
            this.m_Type = type;
            this.source = source;
        }
    }
}