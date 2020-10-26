using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames.InventorySystem
{
    public class DisplayCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //Cursor image to show
        [SerializeField]
        protected Sprite m_Sprite;
        //Size of the imgae
        [SerializeField]
        protected Vector2 m_Size = new Vector2(32f, 32f);
        //Cursor animation, leave empty for none, 
        [SerializeField]
        protected string m_AnimatorState = "Cursor";

        protected virtual void DoDisplayCursor(bool state)
        {
            if (state)
            {
                UICursor.Set(this.m_Sprite, this.m_Size, false, this.m_AnimatorState);
            }
            else
            {
                UICursor.Clear();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!UnityTools.IsPointerOverUI())
            {
                DoDisplayCursor(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!UnityTools.IsPointerOverUI())
            {
                DoDisplayCursor(false);
            }
        }


    }
}