using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CreateAssetMenu(fileName ="SimplePropertyModifier",menuName = "Devion Games/Inventory System/Modifiers/Property")]
    [System.Serializable]
    public class PropertyModifier : ItemModifier
    {
        [SerializeField]
        protected bool m_ApplyToAll = true;
        [SerializeField]
        protected List<string> m_Properties = new List<string>();
        [SerializeField]
        protected PropertyModifierType m_ModifierType = PropertyModifierType.Flat;
        [MinMaxSlider(-100,100)]
        [SerializeField]
        protected Vector2 m_Range= new Vector2(-10f,10f);

        public override void Modify(Item item)
        {
            List<ObjectProperty> properties = new List<ObjectProperty>();
            if (this.m_ApplyToAll) {
                properties.AddRange(item.GetProperties());
            }else{
                for (int i = 0; i < this.m_Properties.Count; i++) {
                    ObjectProperty property = item.FindProperty(this.m_Properties[i]);
                    if (property == null) {
                        property = new ObjectProperty();
                        property.Name = this.m_Properties[i];
                        property.floatValue = 0f;

                    }
                    properties.Add(property);
                }
            }

            for (int i = 0; i < properties.Count; i++)
            {
                ObjectProperty current = properties[i];
                object value = current.GetValue();
                if (!(UnityTools.IsNumeric(value) && current.show)) continue;

                float currentValue = System.Convert.ToSingle(value);   
                float newValue = currentValue;
                float random = Random.Range(m_Range.x, m_Range.y);

                switch (this.m_ModifierType)
                {
                    case PropertyModifierType.Flat:
                        newValue = currentValue + random;
                        break;
                    case PropertyModifierType.Percent:
                        newValue = currentValue+  currentValue*random*0.01f; 
                        break;
                }

                if (value is float)
                {
                    current.SetValue(newValue);
                }
                else if (value is int)
                {
                    current.SetValue(Mathf.RoundToInt(newValue));
                }
            }
        }

        public enum PropertyModifierType { 
            Flat,
            Percent
        }
    }
}