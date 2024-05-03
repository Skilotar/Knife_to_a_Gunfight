using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class tabletech_dizzy : TableFlipItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Table Tech Dizzy";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/tabletech_dizzy";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<tabletech_dizzy>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "You Spin Me Right Round";
            string longDesc =

                "Aim and motorskills way down but increased damage for every table flipped, random duration.\n\n _____________________\n\n" +
                "Developers note: I've heard going into bossrooms with inverted speed softlocks the game so just dont be dizzy when entering a bossfight. thanks" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;


            item.CanBeDropped = false;
            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.D;
        }
      
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.dizzy));
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.boost));
           
        }

        // Token: 0x0600024F RID: 591 RVA: 0x00012C48 File Offset: 0x00010E48
        public override DebrisObject Drop(PlayerController player)
        {
            
            DebrisObject result = base.Drop(player);
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(player.OnTableFlipped, new Action<FlippableCover>(this.dizzy));
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(player.OnTableFlipped, new Action<FlippableCover>(this.boost));
            return result;
        }
        public int dizzyness = 0;
        public int boostamt = 0;
        public int counter = 0;
        private System.Random rng = new System.Random();
        public override void  Update()
        {
            if(this.Owner != null)
            {
                if (dizzyness == 1)
                {
                    if (counter == 25)
                    {
                        ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                        AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Flash_01", base.gameObject);
                        this.Owner.stats.RecalculateStats(Owner, true);
                        counter = 0;
                    }
                    counter++;
                    int random_correct = rng.Next(1, 500);
                    if (random_correct == 1)
                    {
                        dizzyness = 0;
                        boostamt = 0;
                        Statcorrector();
                        AkSoundEngine.PostEvent("Play_NPC_magic_blessing_01", base.gameObject);
                    }
                }

                RemoveStat(PlayerStats.StatType.Damage);
                AddStat(PlayerStats.StatType.Damage, boostamt * 2);
                this.Owner.stats.RecalculateStats(Owner, true);
            }
            
            base.Update();
        }
       
        public void dizzy(FlippableCover obj)
        {
            dizzyness = 1;
            
        }


        public void boost(FlippableCover obj)
        {
            boostamt++;
            
            
            
        }
       
        public void Statcorrector()
        {
            if (!this.Owner || !this.Owner.stats) return;
            float speed = this.Owner.stats.GetStatValue(PlayerStats.StatType.MovementSpeed);
            if (speed < 0)
            {
                ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                this.Owner.stats.RecalculateStats(Owner, true);
            }
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
    }
}
