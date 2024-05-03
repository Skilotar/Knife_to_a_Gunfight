using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Lance2 : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Sir lot o lance Engine", "lnc+upgrade");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:sir_lot_o_lance_engine", "ski:lnc+");
            gun.gameObject.AddComponent<Lance2>();

            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Now With 100% Less Horse");
            gun.SetLongDescription("\nThis is a terribly worn lance made from metal and dragun peices. It seems to have been forged as an attempt to relive adventures of a diffrent time. However, since this is the gungeon, this lance has been equipt with rocket boosters to avoid Kaliber's curse. \n\n" +
                "By far the most effective way to use this weapon is to slightly tilt it toward your adversary whilst you charge them." +
                "\n\n\n - Knife_to_a_Gunfight");

            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "lnc+upgrade_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 2);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("camera_gun", true, true);

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 0f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.DefaultModule.cooldownTime = .01f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;

            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, .5f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= .20f;
            projectile.baseData.range = .001f;
            projectile.PlayerKnockbackForce = -7f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.baseData.force = 45;
            projectile.AppliesStun = true;
            projectile.AppliedStunDuration = .3f;
            projectile.StunApplyChance = 1;


            projectile.transform.parent = gun.barrelOffset;


            ETGMod.Databases.Items.Add(gun, null, "ANY");

            Lance2.lncplusID = gun.PickupObjectId;
        }

        public static int lncplusID;

        public string playerName;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            if (player.IsPrimaryPlayer)
            {
                playerName = "primaryplayer";
            }
            else
            {
                playerName = "secondaryplayer";
            }
            base.OnPickedUpByPlayer(player);
        }
       
        public override void OnPostFired(PlayerController player, Gun gun)
        {

            PlayerController owner = this.gun.CurrentOwner as PlayerController;
            bool Hunter = owner.PlayerHasActiveSynergy("Dragun Slayer");
            if (Hunter)
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                float zRotation = BraveMathCollege.Atan2Degrees(vector);
                player.StartCoroutine(this.HandleSwing(player, vector, (.5f * player.stats.GetStatValue(PlayerStats.StatType.Damage)), 2.5f));
            }
            else
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                float zRotation = BraveMathCollege.Atan2Degrees(vector);
                player.StartCoroutine(this.HandleSwing(player, vector, (.3f * player.stats.GetStatValue(PlayerStats.StatType.Damage)), 2.5f));
            }

           
            gun.PreventNormalFireAudio = true;

        }

        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                {

                    if (user.IsPrimaryPlayer)
                    {

                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "primaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 5);


                    }
                    else
                    {

                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "secondaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 5);

                    }
                }
                SpeculativeRigidbody hitRigidbody2 = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody2 && hitRigidbody2.projectile && hitRigidbody2.projectile.Owner != this.gun.CurrentOwner)
                {
                    PassiveReflectItem.ReflectBullet(hitRigidbody2.projectile, true, this.gun.CurrentOwner, 15f, 1f, 1f, 0f);
                }


                yield return null;
            }
            yield break;
        }


        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }



        private bool HasReloaded;

        //This block of code allows us to change the reload sounds.
        public override void  Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

                PassiveItem.IncrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));

            }

        }
        bool OnHopCooldown = false;
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (!OnHopCooldown)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_mushroom_charge_01", base.gameObject);
                StartCoroutine(Hop(player));
            }
        }

        public IEnumerator Hop(PlayerController player)
        {
            player.SetIsFlying(true, "hop");
            OnHopCooldown = true;
            yield return new WaitForSecondsRealtime(.4f);
            player.SetIsFlying(false, "hop");
            yield return new WaitForSecondsRealtime(.75f);
            OnHopCooldown = false;
            yield break;
        }

    }
}

