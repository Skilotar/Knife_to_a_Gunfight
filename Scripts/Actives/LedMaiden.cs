using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;


namespace Knives
{
    public class Led_Maiden : PlayerItem
    { 
        public static void Register()
        {
            string itemName = "Led Maiden";

            string resourceName = "Knives/Resources/led maiden";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Led_Maiden>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Stairwell To Gungeon";
            string longDesc =
                "Temporary orbital protection\n\n" +
                "A song created by a musical alchemist to harness the power of the lead maiden. " + 
                "Its chords becon forth great protection from seemingly nowhere.\n\n" +
                "But gungeoneers beware, at the end of the song the maidens WILL take what you owe them, whether you have payment or not." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 375f);



            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;
        }
        public bool toggle = false;
        public override void  DoEffect(PlayerController user)
        {
                
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_open_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_open_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_open_01", base.gameObject);
            for (int i = 0; i <= 10; i++)
            {
                user.GiveItem("glass_guon_stone");
            }


            float dura = 10f;
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
            toggle = true;

               

        }

       
        protected void EndEffect(PlayerController user)
        {
           
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_shatter_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_shatter_01", base.gameObject);
            
            for (int i = 0; i <= 10; i++)
            {
                user.RemovePassiveItem(565);
            }


            toggle = false;
          
        }

        public override void  OnPreDrop(PlayerController user)
        {
            if (toggle)
            {
                
                toggle = false;
            }


            base.OnPreDrop(user);
        }
    }
}


