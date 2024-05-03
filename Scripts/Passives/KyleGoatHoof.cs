using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class KylesGoatHoof : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Kyles Goat Hoof"; //The name of the item
            string resourceName = "Knives/Resources/Kyle'sGoatHoof"; //Refers to an embedded png in the project. Make sure to embed your resources!

            GameObject obj = new GameObject();
            var item = obj.AddComponent<KylesGoatHoof>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Walked So We Could Run.";
            string longDesc = "Speed Buff Scales with current health\n\n" +
                "A Hoof From My Favorite GOAT. He was the fastest of the whole flock. So fast in fact that the yearly goat race judges asked for a handicap to be put on him.\n\n" +
                "We cut one leg off and he still won the race. What a GOAT!" +
                "\n\n\n - Knife_to_a_Gunfight";
            ;

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, .2f);
            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnHealthChanged += HealthHaver_OnHealthChanged;
            DoStatChange(player);
            base.Pickup(player);
        }

        private void HealthHaver_OnHealthChanged(float resultValue, float maxValue)
        {
            DoStatChange(this.Owner);
        }

        public void DoStatChange(PlayerController player)
        {

            GetHealthAsNumberOfHits(player);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, HealthAsHits * .2f);

            player.stats.RecalculateStats(player, true);


        }

        public override void Update()
        {
            base.Update();
           

        }

        private float HealthAsHits = 0;
       

        public float GetHealthAsNumberOfHits(PlayerController player)
        {
            float Health = (player.healthHaver.GetCurrentHealth() * 2);
            //ETGModConsole.Log(Health);
            float Armor = (player.healthHaver.Armor);
            //ETGModConsole.Log(Armor);
            HealthAsHits = Health + Armor;
            return HealthAsHits;
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