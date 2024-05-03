using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class MeatCharm : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Meat Charm";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Charm/Meat_Charm";
            resource = resourceName;
            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MeatCharm>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            List<String> cook = new List<string>
            {
                "Well Done",
                "Medium Well",
                "Medium Rare",
                "Rare",
                "Raw",
                "Questionably Sourced"
            };

            string shortDesc = cook[UnityEngine.Random.Range(0, cook.Count - 1)];
            string longDesc = "Attaches to your current gun. Only one charm can be equipped to a gun.\n" +
                "Kills may drop Meat. Meat temporarily gives you berzerkers rage and increased stamina.\n\n" +
                "" +
                "An ornamental gun charm made from the meat of a Popo. These large tusked creatures roam the tundras of the Hoarfrost Reach." +

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
                charm.type = MasterCharmComp.CharmType.Meat;
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