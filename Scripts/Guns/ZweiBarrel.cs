using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
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
    class ZweiBarrel : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Zweibarrel", "Zweibarrel");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:zweibarrel", "ski:zweibarrel");
            gun.gameObject.AddComponent<ZweiBarrel>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("+5 GreatSword");
            gun.SetLongDescription("Attack twice quickly to do a two hit combo. \n" +
                "A fortified Over-Under shotgun with full length barrel blade. \n" +
                "Though much of its shine has tarnished it is a favorite of many hunters and chosen gundead." +

                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Zweibarrel_idle_001", 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 6);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(126) as Gun, true, false);
            
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);

            gun.muzzleFlashEffects = null;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
            gun.SetBaseMaxAmmo(150);
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(7, 0);
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(61/16f, 29/16f, 0f);
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 11f;
            projectile.baseData.speed = 10f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            BaseProj = projectile;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);

            gun.shellsToLaunchOnReload = 2;
            gun.shellCasing = ((Gun)PickupObjectDatabase.GetById(51)).shellCasing;

            projectile.transform.parent = gun.barrelOffset;
            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static int ID;
        public static Projectile BaseProj;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            gun.PreventNormalFireAudio = true;

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            StartCoroutine(ShortPause(projectile));
            
            base.PostProcessProjectile(projectile);
            AkSoundEngine.PostEvent("Play_ENM_chainknight_swing_01", base.gameObject);
            
        }

        private IEnumerator ShortPause(Projectile projectile)
        {
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.DieInAir();
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            yield return new WaitForSeconds(.3f);
            Projectile Proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.position, player, gun);
            ComboManager(Proj);
        }

        public bool Animswap = false;
        public void ComboManager(Projectile projectile)
        {
            if (Animswap) // 2
            {
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Zweibarrel_critical_fire");

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 30;
                if(player.PlayerHasActiveSynergy("Heavy Affinity")) slasher.SlashDamage = 50;
                slasher.SlashRange = 4.5f;
                slasher.slashKnockback = 7f;
                slasher.playerKnockback = 0;
                slasher.soundToPlay = null;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

                Projectile proj = MiscToolMethods.SpawnProjAtPosi(BaseProj, player.CenterPosition, player, this.gun, 180);
                ProjectileSlashingBehaviour slasher2 = proj.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher2.SlashDimensions = 90;
                slasher2.SlashDamageUsesBaseProjectileDamage = false;
                slasher2.SlashDamage = 15;
                slasher2.SlashRange = 3.6f;
                slasher2.slashKnockback = 7f;
                slasher2.playerKnockback = 0;
                slasher2.soundToPlay = null;
                slasher2.InteractMode = SlashDoer.ProjInteractMode.IGNORE;

                AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1.25f, 0, 0), player,   ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1.5f, 0f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1f, .5f, 0), player,     ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1f, .5f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1f, -.5f, 0), player,    ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1f, -.5f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.75f, 1f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.75f, 1f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.75f, -1f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.75f, -1f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                if (player.PlayerHasActiveSynergy("Heavy Affinity"))
                {
                    MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.5f, 1.5f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.5f, 1.5f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                    MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.5f, -1.5f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.5f, -1.5f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                }


            }
            else // 1
            {
                
                StartCoroutine(comboCountdown());
                ComboNumber = 1;
                extend = true;

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 65;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 20;
                if (player.PlayerHasActiveSynergy("Heavy Affinity")) slasher.SlashDamage = 30;
                slasher.SlashRange = 3.6f;
                slasher.playerKnockback = 0;
                slasher.slashKnockback = 7f;
                slasher.soundToPlay = null;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);
                if (player.PlayerHasActiveSynergy("Heavy Affinity"))
                {
                    MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.75f, 1f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.75f, 1f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                    MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(.75f, -1f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(.75f, -1f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                }
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1.25f, 0, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1.5f, 0f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1f, .5f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1f, .5f, 0) - (Vector3)player.CenterPosition)).ToAngle());
                MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.gun.barrelOffset.transform.TransformPoint(1f, -.5f, 0), player, ((Vector2)(this.gun.barrelOffset.transform.TransformPoint(1f, -.5f, 0) - (Vector3)player.CenterPosition)).ToAngle());

            }

        }


        public int ComboNumber = 1;
        public bool extend = false;
        private IEnumerator comboCountdown()
        {
            Animswap = true;
            yield return new WaitForSeconds(1.3f);
            if (ComboNumber == 1)
            {
                Animswap = false;
            }
            if (ComboNumber == 2)
            {
                Animswap = false;
                ComboNumber = 1;
            }

        }

        private bool HasReloaded;


        public override void Update()
        {

            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }



            }




        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                ComboNumber = 1;
                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }


    }
}