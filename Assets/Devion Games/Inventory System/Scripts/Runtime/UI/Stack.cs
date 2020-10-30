using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(Spinner))]
    public class Stack : UIWidget
    {
        [HideInInspector]
        public Item item;
        private Canvas canvas;
        private Spinner spinner;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            this.canvas = GetComponentInParent<Canvas>();
            this.spinner = GetComponent<Spinner>();
        }

        public void SetItem(Item item)
        {
            if (item != null)
            {
                this.item = item;
                this.spinner.min = 1;
                this.spinner.max = item.Stack;
                this.spinner.step = 1;

                Show();
                UpdatePosition();
            }
        }

        public void Unstack() {
            if (item != null) {
                int amount = Mathf.RoundToInt(spinner.current);
                item.Stack -= amount;
                Item newItem = (Item)Instantiate(item);
                newItem.Rarity = item.Rarity;
                newItem.Stack = amount;
                item = newItem;
                UICursor.Set(item.Icon);
                base.Close();
            }
        }

        public void Cancel() {
            item = null;
        }

        private void UpdatePosition()
        {
            Vector3 currentPosition = GetCurrentPosition();
            transform.position = currentPosition;
            Focus();
        }

        private Vector3 GetCurrentPosition()
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            Vector2 offset = Vector2.zero;

            if (Input.mousePosition.x < m_RectTransform.sizeDelta.x)
            {
                offset += new Vector2(m_RectTransform.sizeDelta.x * 0.5f, 0);
            }
            else
            {
                offset += new Vector2(-m_RectTransform.sizeDelta.x * 0.5f, 0);
            }
            if (Screen.height - Input.mousePosition.y > m_RectTransform.sizeDelta.y)
            {
                offset += new Vector2(0, m_RectTransform.sizeDelta.y * 0.5f);
            }
            else
            {
                offset += new Vector2(0, -m_RectTransform.sizeDelta.y * 0.5f);
            }
            pos = pos + offset;

            return canvas.transform.TransformPoint(pos);
        }
    }
}