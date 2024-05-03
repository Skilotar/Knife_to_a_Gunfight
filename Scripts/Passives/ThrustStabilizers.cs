using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class ThrustStabilizers : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Thrust Stabilizers";


            string resourceName = "Knives/Resources/ThrustStablizer";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<ThrustStabilizers>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Jump Jets";
            string longDesc = "Shoot and Steer While Rolling.\n\n" +
                "A vector stabilization harness made to give the infamous titan pilots a terrifying degree of movement control. \n" +
                "This one was scavenged off the body of one such pilot who had come to an untimely end by the hands of frontier rebels. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");



            //Adds the actual passive effect to the item

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;

        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;

            

            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            RemoveStat(PlayerStats.StatType.Accuracy);
            this.Owner.stats.RecalculateStats(Owner, true);
            doing = false;

            return base.Drop(player);
        }


        bool togglerolleffect = false;
        
        private void Player_OnPreDodgeRoll(PlayerController obj)
        {
            togglerolleffect = true;
        }

        public bool doing = false;
        public override void Update()
        {

            if (this.Owner)
            {
                if (togglerolleffect)
                {
                    if (doing != true)
                    {
                        StartCoroutine(Spread());
                    }
                    if (Time.timeScale <= 0f)
                    {

                    }
                    else
                    {

                        PlayerController player = (PlayerController)this.Owner;
                        if (togglerolleffect && player.IsDodgeRolling)
                        {
                            player.knockbackDoer.ApplyKnockback(player.LastCommandedDirection, .55f * (player.stats.MovementSpeed / 7));


                        }
                        else
                        {
                            if (togglerolleffect == true)
                            {
                                player.knockbackDoer.ApplyKnockback(player.LastCommandedDirection * -1, .35f);
                                togglerolleffect = false;
                            }

                        }
                    }
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.Owner).PlayerIDX);
                    if (instanceForPlayer.ActiveActions.ShootAction.IsPressed && Time.timeScale != 0 && this.Owner.CurrentGun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
                    {
                        if (this.Owner.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.SemiAutomatic) || this.Owner.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Automatic) || this.Owner.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Burst))
                        {
                            this.Owner.CurrentGun.Attack();
                        }

                        if (this.Owner.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Charged))
                        {
                            Holding = true;
                        }

                        
                    }

                    if (instanceForPlayer.ActiveActions.ShootAction.IsPressed == false && Time.timeScale != 0 && this.Owner.CurrentGun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
                    {
                        if (Holding)
                        {
                            if (this.Owner.CurrentGun.HasShootStyle(ProjectileModule.ShootStyle.Charged))
                            {
                                
                                if (this.Owner.CurrentGun.HasChargedProjectileReady)
                                {
                                    this.Owner.CurrentGun.CeaseAttack();
                                }
                               
                            }
                            
                            Holding = false;
                        }
                    }

                        base.Update();
                }
            }
        }

        public bool Holding = false;
        public IEnumerator Spread()
        {
            doing = true;
            
            while (this.Owner.IsDodgeRolling)
            {
                AddStat(PlayerStats.StatType.Accuracy, 1f);
                this.Owner.stats.RecalculateStats(Owner, true);
                yield return null;
            }

            RemoveStat(PlayerStats.StatType.Accuracy);
            this.Owner.stats.RecalculateStats(Owner, true);
            doing = false;
        }

        private void DoBurner(PlayerController player,float angle)
        {


            Projectile projectile1 = ((PickupObjectDatabase.GetById(15) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter - new Vector2(0, .25f), Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : angle), true);
            Projectile component1 = gameObject1.GetComponent<Projectile>();
            ProjectileSlashingBehaviour slash = component1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slash.SlashDimensions = 0;
            slash.SlashRange = 0;
            slash.playerKnockback = 0;
            slash.SlashDamage = 0;
            slash.SlashVFX = ((PickupObjectDatabase.GetById(15) as Gun)).muzzleFlashEffects;
            slash.DestroyBaseAfterFirstSlash = true;
            slash.soundToPlay = null;
            component1.Owner = player;

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

