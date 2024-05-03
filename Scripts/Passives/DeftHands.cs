using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;

namespace Knives
{
    class Deft_Hands : PassiveItem
    {
        private int m_activations;

        public static void Register()
        {
            //The name of the item
            string itemName = "Deft Hands";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/DeftHands";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Deft_Hands>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Slight of Hand";
            string longDesc = "Clip does not deplete for a second after getting a kill.\n\n" +
                "Highly dexterous hands provide a skillfull reload even while still shooting." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");


            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
            {
                return;
            }
            base.Pickup(player);
            player.OnKilledEnemy += this.OnKilledEnemy;

        }



        public float ComboTimer = 0;
        public void OnKilledEnemy(PlayerController source)
        {
            if (Speeeeeeeeeeed != true)
            {
                ComboTimer += 3.5f;
                m_activations++;
                if (m_activations >= 1)
                {
                    bool syn = false;
                    if (source.PlayerHasActiveSynergy("Daft Hands")) syn = true;
                    StartCoroutine(DoSpeedy(syn));
                    m_activations = 0;
                }
            }
        }

        public override void Update()
        {
            if (this.Owner != null)
            {
                if (Speeeeeeeeeeed != true && ComboTimer > 0)
                {

                    ComboTimer -= Time.deltaTime;

                }
                if (ComboTimer <= 0)
                {
                    m_activations = 0;
                }

                if (Speeeeeeeeeeed == true)
                {
                    if (CurrentGunID != this.Owner.CurrentGun.PickupObjectId)
                    {
                        CurrentGunAmmo = this.Owner.CurrentGun.ClipShotsRemaining;
                        CurrentGunID = this.Owner.CurrentGun.PickupObjectId;
                    }
                    else
                    {
                        if (CurrentGunAmmo > this.Owner.CurrentGun.ClipShotsRemaining)
                        {
                            this.Owner.CurrentGun.MoveBulletsIntoClip(1);
                            this.Owner.CurrentGun.GainAmmo(1);
                        }
                    }
                }
            }
            base.Update();
        }

        public int CurrentGunID;
        public int CurrentGunAmmo;

        private IEnumerator DoSpeedy(bool syn)
        {
            Speeeeeeeeeeed = true;
            if (syn)
            {
                
                RemoveStat(PlayerStats.StatType.RateOfFire);
                AddStat(PlayerStats.StatType.RateOfFire, .2f);
                this.Owner.stats.RecalculateStats(Owner, true);

            }

            yield return new WaitForSeconds(1f);

            if (syn)
            {
                
                RemoveStat(PlayerStats.StatType.RateOfFire);
                AddStat(PlayerStats.StatType.RateOfFire, 0f);
                this.Owner.stats.RecalculateStats(Owner, true);
            }

            CurrentGunID = -1;
            m_activations = 0;
            Speeeeeeeeeeed = false;
        }


        public bool Speeeeeeeeeeed = false;

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);

            player.OnKilledEnemy -= this.OnKilledEnemy;
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

