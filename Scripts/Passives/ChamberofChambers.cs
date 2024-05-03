
   using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class ChamberofChambers : PassiveItem
    {

        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Chamber of Chambers";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Chamber of Chambers";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<ChamberofChambers>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "5 Out Of 6 Chambers Recommend";
            string longDesc = "A strange invention created by the Duke of the chamber. This chamber has 6 smaller chambers inlayed into itself. \n\nWith this you can instantly reload 6 times before having to refill the whole chamber. " +
                "\n\nIf a quick shootout is what you're looking for this is the chamber for you. Just make sure you finish it in 6 clips or its gonna be one heck of a reload." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ReloadSpeed, 4);


            //Set the rarity of the item
            
            item.quality = PickupObject.ItemQuality.B;

            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.OnReloadedGun = (Action<PlayerController, Gun>)Delegate.Combine(player.OnReloadedGun, new Action<PlayerController, Gun>(this.painfulreload));
            base.Pickup(player);
        }

        private void painfulreload(PlayerController arg1, Gun arg2)
        {
            float timetoreload = this.Owner.CurrentGun.reloadTime * arg1.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);
            arg1.CurrentStoneGunTimer = timetoreload;
            arg1.IsGunLocked = true;
            StartCoroutine(unlocktimer(arg1));
        }

        private IEnumerator unlocktimer(PlayerController arg1)
        {
            yield return new WaitForSeconds(this.Owner.CurrentGun.reloadTime * arg1.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed));
            arg1.IsGunLocked = false;
            token = 6;
        }

        public int token = 6;
        

        public override void  Update()
        {
            if (this.Owner)
            {
                if(this.Owner.CurrentGun.ClipShotsRemaining == 0)
                {
                    if(token != 0)
                    {
                        this.Owner.CurrentGun.MoveBulletsIntoClip(this.Owner.CurrentGun.ClipCapacity);
                        token--;
                    }
                }
                    
            }
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReloadedGun = (Action<PlayerController, Gun>)Delegate.Remove(player.OnReloadedGun, new Action<PlayerController, Gun>(this.painfulreload));
            return base.Drop(player);
        }
    }

}

