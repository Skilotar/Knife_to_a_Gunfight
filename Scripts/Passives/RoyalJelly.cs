using System;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using MonoMod;
using System.Runtime.CompilerServices;
using HarmonyLib;
using System.Collections.Generic;

namespace Knives
{
    public class RoyalJelly : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Royal Jelly";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<RoyalJelly>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);


            string shortDesc = "Hail To The King";
            string longDesc = "Charms All Non Royal Blobulons" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";



            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");



            item.quality = PickupObject.ItemQuality.EXCLUDED;
            itemID = item.PickupObjectId;
        }
        public static int itemID;

        public static PlayerController man;
        public override void Pickup(PlayerController player)
        {
            man = player;
            base.Pickup(player);
        }

        public override void Update()
        {
            if(this.Owner != null)
            {
                if(this.Owner.CurrentRoom != null)
                {
                    if (this.Owner.IsInCombat)
                    {
                       
                    }
                }
            }
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            
            return base.Drop(player);
        }

        public List<string> blobs = new List<string>
        {
            "0239c0680f9f467dbe5c4aab7dd1eca6",
            "042edb1dfb614dc385d5ad1b010f2ee3",
            "42be66373a3d4d89b91a35c9ff8adfec", 
            "e61cab252cfb435db9172adc96ded75f",
            "fe3fe59d867347839824d5d9ae87f244",
            "b8103805af174924b578c98e95313074", 
            "022d7c822bc146b58fe3b0287568aaa2",
            "ccf6d241dad64d989cbcaca2a8477f01",
            "062b9b64371e46e195de17b6f10e47c8",
            "116d09c26e624bca8cca09fc69c714b3",
            "864ea5a6a9324efc95a0dd2407f42810",
            "0b547ac6b6fc4d68876a241a88f5ca6a",
            "1bc2a07ef87741be90c37096910843ab"
        };
        
    }
}
