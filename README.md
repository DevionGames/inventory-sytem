# Item & Inventory System 2.1.1 preview
Inventory System is a highly flexible tool for unity. It can be used in any type of game genres such as RPG, FPS, RTS, Platformer and many more. It comes with full source code, allowing you to change anything and extend it as you wish.
<br><br>• Organized Project
<br>• Intuitive Editor
<br>• Triggering System
<br>• Visual Scripting
<br>• Stat System
<br>• Saving and Loading
<br>• Multiple Inventories and Windows
<br>• Animated Windows
<br>• Currency System
<br>• Vendor System
<br>• Gathering Resources
<br>• Crafting
<br>• Restrictions
<br>• Item Property Generators
<br>• Easy to modify and extend
<br>• Source code included
<br>• Examples
<br><br><b><a href="https://deviongames.com/inventory-system/getting-started/">Documentation</a> | <a href="https://discord.gg/y4fMXpZ">Discord</a> | <a href="https://assetstore.unity.com/packages/tools/gui/item-inventory-system-45568">Asset Store</a></b>

<b>Changelog:</b>
<br>- Remove item references in MoveItem()
<br>- Lock containers if Pause Item Update is true in ThirdPersonController motion
<br>- Unstacking event is implemented now. Key + Click / Key + Drag (See Settings > Input)
<br>- Changed Trigger Type to LeftClick = 1, RightClick = 2, MiddleClick = 4, Key = 8, OnTriggerEnter = 16, Raycast = 32
<br>- Fixed Animation stuck when trigger is used (Cutting trees)
<br>- Fixed PickupItem action droping the game object always at y=1, now it does a raycast.
<br>- You can't drop items in cooldown anymore.
<br>- Fixed OnTriggerEnter option never executing in Trigger component.
<br>- Fixed WidgetUtility returning same windows if there are multiple parent canvases.
<br>- Added ICondition interface to some Trigger actions.
<br>- ItemContainer.GetItemAmount(string windowName, string nameOrId)
<br>- Fixed Jump on steep slope
<br>- Saving skill progress
<br>- InventoryManager.HasSavedData()
<br>- ShowProgressbar action. You can use this instead of Wait. 
<br>- Item.AddProperty(string name, object value) and Item.RemoveProperty(string name) to add and remove properties at runtime.
<br>- Created properties at runtime are saved now. This requires a diffrent serialization. Please delete all saved items.
<br>- Item has an option when sold -> Can Buy Back. 
<br>- When item is sold with Can Buy Back option set to true, it will be added to the vendors inventory and the player can buy back the item.
<br>- Trigger collider is created as child
<br>- Adds automaticaly TriggerRaycaster to main camera if null
<br>- Requires now TriggerRaycaster for any TriggerType.
<br>- Combined DisplayName.DisplayType OnMouseOver = 8, CameraRaycast = 16, MouseRaycast = 32 to Raycast = 8
<br>- Above Trigger changes allow now to have a collider with Is Trigger set to true and still receive events. (Bush example, you can now walk through it)
<br>- Fixed item swap when in cooldown
<br>- Fixed Unstack when in cooldown
