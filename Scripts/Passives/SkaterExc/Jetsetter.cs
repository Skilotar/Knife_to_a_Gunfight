using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;
using SaveAPI;

namespace Knives
{
    class JetSetterRadio : PassiveItem
    {
        private int m_activations;

        public static void Register()
        {
            //The name of the item
            string itemName = "JetSetter Radio";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/JetSetter_Radio";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<JetSetterRadio>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Funky Radio";
            string longDesc = "Gain a small stacking speed boost for every hit or projectile rolled over. This effect is lost on taking damage.\n\n" +
                "A walkman style radio playing nothing but funky jams. Get into the zone and let the beats f" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.C;

            ID = item.PickupObjectId;
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.SKATER_JETSET, true);

        }
        public static int ID;


        public override void Pickup(PlayerController player)
        {

            base.Pickup(player);

            player.OnDealtDamage += Player_OnDealtDamage;
            player.OnReceivedDamage += Player_OnReceivedDamage;
            
            player.OnDodgedProjectile += this.OnDodgedProjectile;
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;
        }

        public bool CurrentlyRolling = false;
        public bool Onebullet = false;
        public int Combo = 0;
        private void Player_OnPreDodgeRoll(PlayerController obj)
        {

            CurrentlyRolling = true;
            Onebullet = false;
        }

        private void OnDodgedProjectile(Projectile projectile)
        {
            if (this.Owner != null)
            {
                if (projectile.Owner != this.Owner)
                {
                    if (CurrentlyRolling == true && Onebullet == false)
                    {
                        combo += 5;
                        Onebullet = true;
                    }
                }

            }

        }


        private void Player_OnReceivedDamage(PlayerController obj)
        {
            if (obj != null)
            {
                AkSoundEngine.PostEvent("Play_Speed_Down", base.gameObject);
                combo = 0;
            }
        }

        private void Player_OnDealtDamage(PlayerController arg1, float arg2)
        {
            if (combo < 150)
            {
                combo++;
            }
            else
            {
                combo = 150;
            }
        }

        private int lastknownCombo = 0;

        public override void Update()
        {
            if (this.Owner != null)
            {
                if(lastknownCombo != combo)
                {
                    lastknownCombo = combo;

                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, combo/150f);
                    this.Owner.stats.RecalculateStats(Owner, true);
                }

                if (Time.timeScale != 0)
                {
                    if (CurrentlyRolling == true)// roll was initiated
                    {
                        if (this.Owner.IsDodgeRolling != true)//roll ended
                        {
                            CurrentlyRolling = false;
                            Onebullet = false;
                        }
                    }

                }
            }
            base.Update();
        }

        
        public int combo = 1;

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);

            player.OnDealtDamage -= Player_OnDealtDamage;
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            player.OnDodgedProjectile -= this.OnDodgedProjectile;
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            return debrisObject;
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


