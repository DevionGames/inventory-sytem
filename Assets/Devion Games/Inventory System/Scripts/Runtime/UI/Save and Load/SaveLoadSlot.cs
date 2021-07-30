using UnityEngine;
using UnityEngine.EventSystems;
using DevionGames.UIWidgets;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DevionGames.InventorySystem
{
    public class SaveLoadSlot : MonoBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            string key = GetComponentInChildren<Text>().text;
            DialogBox dialogBox = InventoryManager.UI.dialogBox;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ContextMenu menu = InventoryManager.UI.contextMenu;
                menu.Clear();
                menu.AddMenuItem("Load", () => {
                    dialogBox.Show("Load", "Are you sure you want to load this save? ", null, (int result) => {
                        if (result != 0) return;
                        InventoryManager.Load(key);
                    }, "Yes", "No");

                });
                menu.AddMenuItem("Save", ()=> {
                   dialogBox.Show("Save", "Are you sure you want to overwrite this save? ", null, (int result)=> {
                       if (result != 0) return;
                       List<string> keys = PlayerPrefs.GetString("InventorySystemSavedKeys").Split(';').ToList();
                       int index = keys.IndexOf(key);
                       InventoryManager.Delete(key);
                       InventoryManager.Save(DateTime.UtcNow.ToString(), index);
                   }
                   , "Yes", "No");
                });
                menu.AddMenuItem("Delete", () => {
                    dialogBox.Show("Delete", "Are you sure you want to delete this save? ", null, (int result) => {
                        if (result != 0) return;
                        InventoryManager.Delete(key);
                        DestroyImmediate(gameObject);
                    }, "Yes", "No");


                 
                });
                menu.Show();
            }else {
                dialogBox.Show("Load", "Are you sure you want to load this save? ", null, (int result) => { 
                    if (result != 0) return;
                    InventoryManager.Load(key);
                }, "Yes", "No");
               
            }
        }
    }
}