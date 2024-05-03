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
    class CheatersDrawArm : PassiveItem
    {
        private int m_activations;

        public static void Register()
        {
            //The name of the item
            string itemName = "Cheater's Draw Arm";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cheater's_Draw_arm";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<CheatersDrawArm>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Whiplash Wrist";
            string longDesc = "For a second after switching weapons gain accuracy and firerate. \n\n" +
                "A robotic arm fine tuned for gunslinger duels. The arm is neatly concealed in any long sleeve jacket and precalculates the aim for its shot.\n" +
                "The arm's inventor is shunned by other gunslingers for going against the nature of the sport." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            CheatersDrawArm.RushPrefab = SpriteBuilder.SpriteFromResource(CheatersDrawArm.RushVFX, null);
            CheatersDrawArm.RushPrefab.name = CheatersDrawArm.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(CheatersDrawArm.RushPrefab);
            FakePrefab.MarkAsFakePrefab(CheatersDrawArm.RushPrefab);
            CheatersDrawArm.RushPrefab.SetActive(false);


            item.quality = PickupObject.ItemQuality.D;

            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
            {
                return;
            }
            player.GunChanged += Player_GunChanged;
            base.Pickup(player);
            
        }

        private void Player_GunChanged(Gun arg1, Gun arg2, bool arg3)
        {
            if (Speeeeeeeeeeed != true) StartCoroutine(DoSpeedy());
        }
        public bool Smoking = false;
        public override void Update()
        {
            if(this.Owner.PlayerHasActiveSynergy("Smoking Joe"))
            {
                Smoking = true;
            }
            else
            {
                Smoking = false;
            }
            base.Update();
        }
        public bool hasbuff = false;
        private IEnumerator DoSpeedy()
        {
            Speeeeeeeeeeed = true;
            

            this.Owner.PostProcessProjectile += Owner_PostProcessProjectile;
            hasbuff = true;
            RemoveStat(PlayerStats.StatType.RateOfFire);
            AddStat(PlayerStats.StatType.RateOfFire, 1f);
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, .2f);
            RemoveStat(PlayerStats.StatType.Accuracy);
            AddStat(PlayerStats.StatType.Accuracy, -.9f);
            this.Owner.stats.RecalculateStats(Owner, true);
            AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Arm_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Flash_01", base.gameObject);
            GlobalSparksDoer.DoRandomParticleBurst(6, this.Owner.sprite.WorldCenter, this.Owner.sprite.WorldCenter, Vector3.up, 360, 5, null, null, ExtendedColours.lime, GlobalSparksDoer.SparksType.SOLID_SPARKLES);

            yield return new WaitForSeconds(1f);
            this.Owner.PostProcessProjectile -= Owner_PostProcessProjectile;
            hasbuff = false;
            RemoveStat(PlayerStats.StatType.RateOfFire); 
            AddStat(PlayerStats.StatType.RateOfFire, 0f); 
            RemoveStat(PlayerStats.StatType.Accuracy);
            AddStat(PlayerStats.StatType.Accuracy, 0f);
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, 0f);
            this.Owner.stats.RecalculateStats(Owner, true);

            if(Smoking == true)
            {
                yield return new WaitForSeconds(4f);
            }
            else
            {
                yield return new WaitForSeconds(6f);
            }
            Speeeeeeeeeeed = false;
            AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Charge_01",base.gameObject);
            GameObject original = CheatersDrawArm.RushPrefab;
            tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
            this.Owner.BloopItemAboveHead(OhGeez, "");
        }

        private void Owner_PostProcessProjectile(Projectile arg1, float arg2)
        {
            arg1.HasDefaultTint = true;
            arg1.DefaultTintColor = ExtendedColours.lime;
            ImprovedAfterImage trail = arg1.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .1f;
            trail.shadowTimeDelay = .1f;
            trail.dashColor = ExtendedColours.lime;
            trail.maxEmission = 100;
            trail.spawnShadows = true;

            if (Smoking)
            {

                arg1.AppliesFire = true;
                arg1.FireApplyChance = 100;
                arg1.fireEffect = StaticStatusEffects.hotLeadEffect;
                
                arg1.DefaultTintColor = ExtendedColours.carrionRed;
                trail.dashColor = ExtendedColours.carrionRed;
            }

        }

        public bool Speeeeeeeeeeed = false;

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            if(hasbuff == true)
            {
                this.Owner.PostProcessProjectile -= Owner_PostProcessProjectile;
                RemoveStat(PlayerStats.StatType.RateOfFire);
                RemoveStat(PlayerStats.StatType.Accuracy);
                RemoveStat(PlayerStats.StatType.Damage);
            }
            
            this.Owner.stats.RecalculateStats(Owner, true);
            
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

        private static string RushVFX = "Knives/Resources/Cheater";
        private static GameObject RushPrefab;
        private static string vfxName = "Cheater";
    }
}

