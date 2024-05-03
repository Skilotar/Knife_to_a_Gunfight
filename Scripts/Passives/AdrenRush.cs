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
    class AdrenalineRush : PassiveItem
    {
        private int m_activations;

        public static void Register()
        {
            //The name of the item
            string itemName = "Oni Mask";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/AdrenalineRush";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<AdrenalineRush>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Adrenaline Rush!";
            string longDesc ="Get kills quickly to trigger an adrenaline rush. Temporarily increase speed and lowers cooldown on active items.\n\n" +
                "" +
                "The mask of a speed demon. Those foolish enough to don this mask are gifted speed and a speedy grave." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            AdrenalineRush.RushPrefab = SpriteBuilder.SpriteFromResource(AdrenalineRush.RushVFX, null);
            AdrenalineRush.RushPrefab.name = AdrenalineRush.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(AdrenalineRush.RushPrefab);
            FakePrefab.MarkAsFakePrefab(AdrenalineRush.RushPrefab);
            AdrenalineRush.RushPrefab.SetActive(false);

            item.quality = PickupObject.ItemQuality.B;
            
        }

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
                if (m_activations >= 3)
                {
                    StartCoroutine(DoSpeedy());
                    m_activations = 0;
                }
            }
        }

        public override void Update()
        {
            if (this.Owner != null)
            {
                if(Speeeeeeeeeeed != true && ComboTimer > 0)
                {

                    ComboTimer -= Time.deltaTime;

                }
                if(ComboTimer <= 0)
                {
                    m_activations = 0;
                }
            }
            base.Update();
        }

        private IEnumerator DoSpeedy()
        {
            Speeeeeeeeeeed = true;

            AkSoundEngine.PostEvent("Play_Speed_Up", base.gameObject);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, 2.5f);
            this.Owner.stats.RecalculateStats(Owner, true);

            foreach (PlayerItem item in this.Owner.activeItems)
            {
               
                float alttime = item.CurrentTimeCooldown - (item.timeCooldown * .25f);
                float altDam = item.CurrentDamageCooldown - (item.damageCooldown * .15f);
                int altRoom = item.CurrentRoomCooldown - 1;

                if (alttime <= 0) alttime = 0;
                if (altDam <= 0) altDam = 0;
                if (altRoom <= 0) altRoom = 0;

                
                item.CurrentTimeCooldown = alttime;
                item.CurrentDamageCooldown = altDam;
                item.CurrentRoomCooldown = altRoom;
                item.DidDamage(this.Owner,1); // For Some reason the Time Cooldown set needs this to update properly??
                // Dog roll Why?!

            }
            GameObject original = AdrenalineRush.RushPrefab;
            tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
            this.Owner.BloopItemAboveHead(OhGeez, "");
            yield return new WaitForSeconds(6f);

            AkSoundEngine.PostEvent("Play_Speed_Down", base.gameObject);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, 0f);
            this.Owner.stats.RecalculateStats(Owner, true);
            m_activations = 0;
            Speeeeeeeeeeed = false;
        }

        public bool Speeeeeeeeeeed = false;
       
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, 0f);
            this.Owner.stats.RecalculateStats(Owner, true);
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

        private static string RushVFX = "Knives/Resources/Rush";
        private static GameObject RushPrefab;
        private static string vfxName = "Rush";
    }
}

