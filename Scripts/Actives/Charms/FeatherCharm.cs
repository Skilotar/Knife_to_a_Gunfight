using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class FeatherCharm : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Feather Charm";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Charm/feather_charm";
            resource = resourceName;
            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<FeatherCharm>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Carried On The Wind";
            string longDesc = "Attaches to your current gun. Only one charm can be equipped to a gun.\n" +
                "Adds Flight while gun is held\n\n" +
                "" +
                "An ornamental gun charm made from a dusty feather. Its Far larger than any bird you've ever seen." +

                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1);

            item.UsesCustomCost = true;
            item.CustomCost = 20;
            item.quality = PickupObject.ItemQuality.D;
            item.numberOfUses = 1;
            item.consumable = true;

            ID = item.PickupObjectId;
            GlobalCharmList.Charmslist.Add(ID);
        }
        public static string resource;
        public static int ID;

        //applies damage on last use
        public override void DoEffect(PlayerController user)
        {
            if (user.CurrentGun.GetComponent<MasterCharmComp>() == null)
            {

                MasterCharmComp charm = user.CurrentGun.gameObject.GetOrAddComponent<MasterCharmComp>();
                charm.baseitem = this;
                charm.attachedGun = user.CurrentGun;
                charm.player = user;
                charm.resource = resource;
                charm.type = MasterCharmComp.CharmType.Feather;
                AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.gameObject);
            }

        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.CurrentGun.GetComponent<MasterCharmComp>() == null;
        }

        public override void Update()
        {

            if (this.LastOwner)
            {
                this.CanBeUsed(this.LastOwner);
            }


            base.Update();
        }

    }
}