using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Reflection;
using System.Collections;

namespace Knives
{
    class IceBoots : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Ice Boots";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/IceBoots";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<IceBoots>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Cool Kicks";
            string longDesc = "Immunity to Ice slipping, speedboost while on slippery terrain. + 1 coolness, freezes liquids.\n\n" +
                "Spiked Ice Boots with a tag inside labeled -Popo. These boots are a bit big for you but they get the job done." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1);

            item.quality = PickupObject.ItemQuality.D;
        }

        private ImprovedAfterImage zoomy;
        public override void Pickup(PlayerController player)
        {
            zoomy = player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            zoomy.dashColor = new Color(220f / 255f, 220f / 255f, 220f / 255f);
            zoomy.spawnShadows = false;
            zoomy.shadowTimeDelay = .1f;
            zoomy.shadowLifetime = .5f;
            zoomy.minTranslation = 0.1f;
            zoomy.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");

            base.Pickup(player);
        }

        public float internalCooldown;
        public float dir;
        public override void Update()
        {
            if (this.Owner != null)
            {
                if(GetPrivateType<PlayerController, float>(this.Owner, "m_maxIceFactor") > 0)
                {
                    SetPrivateType<PlayerController>(this.Owner, "m_maxIceFactor", 0);
                    if(this.Owner.Velocity.magnitude > 0)
                    {
                        if (zoomy.spawnShadows == false)
                        {
                            StartCoroutine(Boost());
                            dir = 1.5f;
                        }
                        if(internalCooldown <= 0)
                        {
                            GlobalSparksDoer.DoLinearParticleBurst(6, this.Owner.specRigidbody.UnitBottomLeft, this.Owner.specRigidbody.UnitBottomRight, 0, 5, 0, null, null, ExtendedColours.freezeBlue, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                            internalCooldown = .5f;
                        }
                    }
                }

                if (IsPointWatery(this.Owner.sprite.WorldCenter))
                {
                    DeadlyDeadlyGoopManager.FreezeGoopsCircle(this.Owner.sprite.WorldCenter, 2f);
                }


                if (internalCooldown > 0)
                {
                    internalCooldown -= BraveTime.DeltaTime;
                }
            }

           

            base.Update();
        }

        private bool IsPointWatery(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionInGoop(testPos);
            }
            return false;
        }

        public IEnumerator Boost()
        {
            zoomy.spawnShadows = true;
            AddStat(PlayerStats.StatType.MovementSpeed, 2f);
            this.Owner.stats.RecalculateStats(Owner, true);
            while(dir > 0)
            {
                dir -= BraveTime.DeltaTime;
                yield return null;
            }

            RemoveStat(PlayerStats.StatType.MovementSpeed);
            this.Owner.stats.RecalculateStats(Owner, true);
            zoomy.spawnShadows = false;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            this.Owner.stats.RecalculateStats(Owner, true);
            zoomy.spawnShadows = false;
            return base.Drop(player);

        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
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
