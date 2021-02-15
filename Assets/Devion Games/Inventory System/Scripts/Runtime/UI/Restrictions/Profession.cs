using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem.Restrictions
{
    public class Profession : Restriction
    {
        public override bool CanAddItem(Item item)
        {
            string profession = PlayerPrefs.GetString("Profession");

            if (item == null || !(item is EquipmentItem equipmentItem)) { return false; }

            if (string.IsNullOrEmpty(profession)) return true;

            ObjectProperty property = item.FindProperty("Profession");
            if (property == null) return true;

            string[] professions = property.stringValue.Split(';');
            for (int i = 0; i < professions.Length; i++) {
                if (PlayerPrefs.GetString("Profession") == professions[i]) {
                    return true;
                }
            }
            return false;
        }
    }
}