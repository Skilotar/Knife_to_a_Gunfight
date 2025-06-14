using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    class BrokenWill : PassiveItem
    {
        public static void Init()
        {
            string ItemName = "Broken Will";
            string SpriteDirectory = "Knives/Resources/Charm/icons/Meat_pickup";
            GameObject obj = new GameObject();

            var item = obj.AddComponent<BrokenWill>();

            ItemBuilder.AddSpriteToObject(ItemName, SpriteDirectory, obj);

            string shortDesc = "A hero's lamment";

            string longDesc = "An artifact made from the essence of a gungeoneer's being\n\n" +
            "";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "jde");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.D;
        }

        private bool IsPointFirey(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
            }
            return false;
        }

        public bool hasBuff = false;
        public override void Update()
        {
            base.Update();
            if (this.Owner)
            {
                if (IsPointFirey(this.Owner.sprite.WorldCenter) && hasBuff == false)
                {

                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 0.5f);
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, .5f);


                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = true;
                }
                if (!IsPointFirey(this.Owner.sprite.WorldCenter) && hasBuff)
                {

                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 0f);
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, .0f);

                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = false;
                }
            }
        }

        void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
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


        void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }


        public override void Pickup(PlayerController player)
        {

            player.healthHaver.Armor += 1f;
            player.healthHaver.CursedMaximum = 1f;
            if (!m_pickedUpThisRun)
            {
                player.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier
                {
                    damageType = CoreDamageTypes.Fire,
                    damageMultiplier = 0.01f

                    //todo: add goop resistance, thats the fire on the ground, im tired now so fuck u xoxo//
                });

            }
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }

    }
}
