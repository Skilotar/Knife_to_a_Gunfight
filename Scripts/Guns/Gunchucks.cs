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
    class GunChucks : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Gunchucks", "Gunchucks");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:gunchucks", "ski:gunchucks");
            gun.gameObject.AddComponent<GunChucks>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Gunchaku");
            gun.SetLongDescription("A pair of custom built revolvers created for practicing the experimental art of gunchaku.\n\n" +
                "A delicate dance of martial arts allows for the use of these as both melee and ranged combat tools.\n\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Gunchucks_idle_001", 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 12);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .5f, StatModifier.ModifyMethod.ADDITIVE);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 5;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;


            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
            gun.carryPixelOffset = new IntVector2(10, 3);
            gun.muzzleFlashEffects = null;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 5f;
            projectile.baseData.speed = 20f;
            projectile.baseData.range = 30f;
            projectile.baseData.force = 5;
            projectile.gameObject.GetOrAddComponent<projectileStates>().Gunchuck = true;
            gun.DefaultModule.projectiles[0] = projectile;

            gun.CanBeDropped = false;
            fakeproj = (PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0].projectile;

            MiscToolMethods.TrimAllGunSprites(gun);
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public static Projectile fakeproj;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);

            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

            if(projectile.gameObject.GetComponent<projectileStates>() != null)
            {
                
                if (projectile.gameObject.GetComponent<projectileStates>().Gunchuck == true)
                {
                    Alt = false;
                    ComboNumber++;
                   
                    ComboDropTimer += .6f;
                    if (ComboDropTimer > .75f) ComboDropTimer = .75f;
                    DoingCombo = true;
                    if ((this.gun.CurrentOwner as PlayerController).HasPassiveItem(241)) Alt = true;
                    
                   
                    StartCoroutine(ManageComboAnimation(projectile));
                    

                }
                
            }
           

        }

        public bool Alt = false;

        private IEnumerator ManageComboAnimation(Projectile projectile)
        {
            // check combo number
            // stop clip and play proper animation.
            UnityEngine.Object.Destroy(projectile.gameObject);
            int StupidScatterShotWhyAreYouSoWierd = 1;
            if (Alt)
            {
                StupidScatterShotWhyAreYouSoWierd = 3;
            }

            if (ComboNumber <= 1 * StupidScatterShotWhyAreYouSoWierd)
            {
                
                ComboNumber = 1 * StupidScatterShotWhyAreYouSoWierd;
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Gunchucks_fire");
                yield return new WaitForSeconds(.10f);
                GameObject gameObject = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = (this.Owner as PlayerController);
                ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 30;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 6;
                slasher.SlashRange = 4f;
                slasher.slashKnockback = 7f;
                slasher.playerKnockback = 0;
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                slasher.SlashVFX.type = VFXPoolType.None;

                Vector3 vector = new Vector3(1.8f, .5f, 0);
                if(this.gun.sprite.FlipY == true) vector = new Vector3(1.8f, -.5f, 0);

                GameObject gameObject2 = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.TransformPoint(vector), Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                component2.Owner = (this.Owner as PlayerController);
                component2.baseData.damage = 6f;
                component2.baseData.speed = 20f;
                component2.baseData.range = 30f;
                component2.baseData.force = 5;
                if (Alt)
                {
                    ProjectileSplitController split = component2.gameObject.GetOrAddComponent<ProjectileSplitController>();
                    split.distanceBasedSplit = true;
                    split.distanceTillSplit = 0f;
                    split.dmgMultAfterSplit = .5f;
                    split.amtToSplitTo = 2;
                    split.splitAngles = 25f;
                }

                if (UnityEngine.Random.Range(0, 5) <= 1)
                {
                    (this.Owner as PlayerController).DoPostProcessProjectile(component2);
                }
                
                AkSoundEngine.PostEvent("Play_BlackBetty_fire_001", base.gameObject);


            }
            if (ComboNumber == 2 * StupidScatterShotWhyAreYouSoWierd)
            {

                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Gunchucks_critical_fire");
                yield return new WaitForSeconds(.10f);

                GameObject gameObject = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = (this.Owner as PlayerController);
                ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 30;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 6;
                slasher.SlashRange = 4f;
                slasher.slashKnockback = 7f;
                slasher.playerKnockback = 0;
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                slasher.SlashVFX.type = VFXPoolType.None;

                GameObject gameObject2 = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.TransformPoint(2.5f,0,0), Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                component2.Owner = (this.Owner as PlayerController);
                component2.baseData.damage = 6f;
                component2.baseData.speed = 20f;
                component2.baseData.range = 30f;
                component2.baseData.force = 5;
                if (UnityEngine.Random.Range(0, 5) <= 1)
                {
                    (this.Owner as PlayerController).DoPostProcessProjectile(component2);
                }
                AkSoundEngine.PostEvent("Play_BlackBetty_fire_001", base.gameObject);

                if (Alt)
                {
                    ProjectileSplitController split = component2.gameObject.GetOrAddComponent<ProjectileSplitController>();
                    split.distanceBasedSplit = true;
                    split.distanceTillSplit = 0f;
                    split.dmgMultAfterSplit = .5f;
                    split.amtToSplitTo = 2;
                    split.splitAngles = 25f;
                }
            }
            if (ComboNumber == 3 * StupidScatterShotWhyAreYouSoWierd)
            {
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("Gunchucks_charge");
                
                AkSoundEngine.PostEvent("Play_BlackBetty_fire_001", base.gameObject);
                GameObject gameObject = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = (this.Owner as PlayerController);
                component.baseData.damage = 8f;
                component.baseData.speed = 20f;
                component.baseData.range = 30f;
                component.baseData.force = 5;

                if (Alt)
                {
                    ProjectileSplitController split = component.gameObject.GetOrAddComponent<ProjectileSplitController>();
                    split.distanceBasedSplit = true;
                    split.distanceTillSplit = 0f;
                    split.dmgMultAfterSplit = .5f;
                    split.amtToSplitTo = 2;
                    split.splitAngles = 25f;
                }
                if (UnityEngine.Random.Range(0, 5) <= 1)
                {
                    (this.Owner as PlayerController).DoPostProcessProjectile(component);
                }
                yield return new WaitForSeconds(.15f);

                Vector3 vector = new Vector3(0f, .5f, 0);
                if (this.gun.sprite.FlipY == true) vector = new Vector3(0f, -.5f, 0);
                GameObject gameObject2 = SpawnManager.SpawnProjectile(fakeproj.gameObject, this.gun.barrelOffset.transform.TransformPoint(vector), Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                component2.Owner = (this.Owner as PlayerController);
                component2.baseData.damage = 8f;
                component2.baseData.speed = 20f;
                component2.baseData.range = 30f;
                component2.baseData.force = 5;

                if (Alt)
                {
                    ProjectileSplitController split = component2.gameObject.GetOrAddComponent<ProjectileSplitController>();
                    split.distanceBasedSplit = true;
                    split.distanceTillSplit = 0f;
                    split.dmgMultAfterSplit = .5f;
                    split.amtToSplitTo = 2;
                    split.splitAngles = 25f;
                }
                if (UnityEngine.Random.Range(0, 5) <= 1)
                {
                    (this.Owner as PlayerController).DoPostProcessProjectile(component2);
                }
                gun.ClipShotsRemaining--;
                AkSoundEngine.PostEvent("Play_BlackBetty_fire_001", base.gameObject);
                yield return new WaitForSeconds(.15f);
                ComboNumber = 0;
                ComboDropTimer = 0;

            }
            yield return null;

        }

        public int ComboNumber = 0;
        public float ComboDropTimer = 0;
        public bool DoingCombo = false;

        private bool HasReloaded;
        public override void Update()
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

                if(ComboDropTimer > 0)
                {
                    ComboDropTimer -= Time.deltaTime;
                }
                if (DoingCombo && ComboDropTimer <= 0)// dropped Combo
                {
                    ComboNumber = 0;
                    DoingCombo = false;
                }

               

            }




        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

                ComboNumber = 0;
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

                base.OnReloadPressed(player, gun, bSOMETHING);
                StartCoroutine(reloadSoundCo());

            }

        }

        private IEnumerator reloadSoundCo()
        {
            AkSoundEngine.PostEvent("Play_Gunchuck_reload_001", base.gameObject);
            yield return new WaitForSeconds(.15f);
            AkSoundEngine.PostEvent("Play_Gunchuck_reload_001", base.gameObject);
        }
    }
}
