using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{ 
    class nano_boost :PlayerItem
    {
        public static void Register()
        {
            string itemName = "Nano Boost";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/nano_boost";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<nano_boost>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "That Goood Stuff.";
            string longDesc = "Temporary boost to speed, damage and defense.\n\n" +
                "A chemical mixture of literally just electricity and extremely sugary cereal that causes your central nervous system to temporarily go crazy. \n\n" +
                "Normally a trained medical professional would administer a small dose but you just take the whole thing at once you freaking addict." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 600f);


            item.quality = PickupObject.ItemQuality.B;


            item.numberOfUses = 1;
            item.UsesNumberOfUsesBeforeCooldown = true;

        }

        public bool toggle = false;
        public override void  DoEffect(PlayerController user)
        {

            float dura = 10f;

            user.GiveItem("ski:nanostone");
            user.GiveItem("ski:nanostone");
            user.GiveItem("ski:nanostone");

            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed , 4f);
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, .5f);
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));

            toggle = true;
        }

   
        protected void EndEffect(PlayerController user)
        {
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            RemoveStat(PlayerStats.StatType.Damage);
            breakThis();
            toggle = false;
        }

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }

        private void breakThis()
        {
            base.LastOwner.RemovePassiveItem(nanostone.ID);
            base.LastOwner.RemovePassiveItem(nanostone.ID);
            base.LastOwner.RemovePassiveItem(nanostone.ID);

        }

        public override void  OnPreDrop(PlayerController user)
        {
            if (toggle)
            {
                breakThis();
                RemoveStat(PlayerStats.StatType.MovementSpeed);
                RemoveStat(PlayerStats.StatType.Damage);
                toggle = false;
            }
           
            base.OnPreDrop(user);
        }
    }
}
