using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace DevionGames.InventorySystem
{
    public abstract class VisibleItem : CallbackHandler
    {

        public override string[] Callbacks => new string[] { "OnEquip", "OnUnEquip" };


        [ItemPicker]
        public Item item;

        [SerializeField]
        public Attachment[] attachments;

        protected Animator m_CharacterAnimator;
        protected Transform m_CameraTransform;
        protected Camera m_Camera;
        protected Collider[] m_CharacterColliders;
        protected EquipmentHandler m_Handler;
        protected Item m_CurrentEquipedItem;

        protected virtual void Start() {
          
        }

        protected virtual void Awake(){
            this.m_Handler = GetComponent<EquipmentHandler>();
            this.m_CharacterAnimator = GetComponentInParent<Animator>();
            m_CharacterColliders = GetComponentsInChildren<Collider>(true);
            this.m_Camera = Camera.main;
            this.m_CameraTransform = this.m_Camera.transform;
        }

        protected virtual void Update() {}

        public virtual void OnItemEquip(Item item) {
            enabled = true;
            this.m_CurrentEquipedItem = item;
            foreach (Attachment att in this.attachments)
            {
                if (att.gameObject != null)
                {
                    att.gameObject.SetActive(true);
                }else {
                    att.Instantiate(this.m_Handler);
                }
            }
            CallbackEventData data = new CallbackEventData();
            data.AddData("Item", item);
            data.AddData("Attachments", this.attachments);
            Execute("OnEquip", data);
        }

        public virtual void OnItemUnEquip(Item item) {
            this.m_CurrentEquipedItem = null;
            enabled = false;
            foreach (Attachment att in attachments)
            {
                if (att.gameObject != null)
                {
                    att.gameObject.SetActive(false);
                }
            }
            CallbackEventData data = new CallbackEventData();
            data.AddData("Item", item);
            data.AddData("Attachments", this.attachments);
            Execute("OnUnEquip", data);
        }

        protected void IgnoreCollision(GameObject gameObject) {
            Collider collider = gameObject.GetComponent<Collider>();
            for (int i = 0; i < this.m_CharacterColliders.Length; i++) {
                Physics.IgnoreCollision(this.m_CharacterColliders[i],collider);
            }
            collider.enabled = true;
        }

        [System.Serializable]
        public class Attachment {
            [EquipmentPicker]
            public EquipmentRegion region;
            public GameObject prefab;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale = Vector3.one;

            [HideInInspector]
            public GameObject gameObject;

            public GameObject Instantiate(EquipmentHandler handler) {
                gameObject = GameObject.Instantiate(prefab, handler.GetBone(region));
                gameObject.SetActive(true);
                //Calean prefab, not the best way, but keeps the project clean from duplicate prefabs.
                Trigger trigger = gameObject.GetComponent<Trigger>();
                if (trigger != null) {
                    Destroy(trigger);
                }
                IGenerator[] generators = gameObject.GetComponents<IGenerator>();
                for (int i = 0; i < generators.Length; i++) {
                    Destroy((generators[i] as Component));
                }
                ItemCollection collection = gameObject.GetComponent<ItemCollection>();
                if (collection != null){
                    Destroy(collection);
                }

                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    Destroy(rigidbody);
                }
                Collider[] colliders = gameObject.GetComponents<Collider>();
                for (int i = 0; i < colliders.Length; i++) {
                    Destroy(colliders[i]);
                }
                gameObject.transform.localPosition = position;
                gameObject.transform.localEulerAngles = rotation;
                gameObject.transform.localScale = scale;
                return gameObject;
            }
        }
    }
}