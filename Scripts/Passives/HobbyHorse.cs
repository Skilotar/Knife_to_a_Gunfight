using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class HobbyHorse : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Noble Steed";


            string resourceName = "Knives/Resources/HobbyHorse";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<HobbyHorse>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Clippy Clop";
            string longDesc = "Rolling provides a short firerate and speed boost.\n\n" +
                "A fake horse for knights in training. It boasts a sturdy wooden frame and half a horsepower.\n" +
                "It was customary for a squire to follow closely behind the knight in training clopping two coconut halves together." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            


            //Adds the actual passive effect to the item

            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;

        }
        public static int ID;

        private ImprovedAfterImage zoomy;
        public override void Pickup(PlayerController player)
        {
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;

            zoomy = player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            zoomy.dashColor = new Color(220f / 255f, 220f / 255f, 220f / 255f);
            zoomy.spawnShadows = false;
            zoomy.shadowTimeDelay = .1f;
            zoomy.shadowLifetime = 1f;
            zoomy.minTranslation = 0.1f;
            zoomy.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");

            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            RemoveStat(PlayerStats.StatType.RateOfFire);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            this.Owner.stats.RecalculateStats(Owner, true);
            zoomy.spawnShadows = false;

            return base.Drop(player);
        }
       
        

        private void Player_OnPreDodgeRoll(PlayerController obj)
        {
            if(zoomy.spawnShadows == false)
            {
                StartCoroutine(Boost());
            }

        }

        public IEnumerator Boost()
        {
            zoomy.spawnShadows = true;
            yield return new WaitForSeconds(.1f);
            while (this.Owner.IsDodgeRolling) yield return null;

            AddStat(PlayerStats.StatType.MovementSpeed, .7f);
            AddStat(PlayerStats.StatType.RateOfFire, .5f);
            this.Owner.stats.RecalculateStats(Owner, true);
            if(Owner.PlayerHasActiveSynergy("200% More Horse")) yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(1.5f);
            RemoveStat(PlayerStats.StatType.RateOfFire);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            this.Owner.stats.RecalculateStats(Owner, true);
            zoomy.spawnShadows = false;
        }

        public override void Update()
        {

            if (this.Owner)
            {
                

                base.Update();
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

