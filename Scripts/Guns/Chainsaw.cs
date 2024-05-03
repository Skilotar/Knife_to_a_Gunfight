using System;
using ItemAPI;
using Dungeonator;
using Gungeon;
using SaveAPI;

using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Knives
{
    class Chainsaw : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Chainsaw Hand", "Chainsaw");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:chainsaw_hand", "ski:chainsaw_hand");
            gun.gameObject.AddComponent<Chainsaw>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Groovy");
            gun.SetLongDescription("Swings slow as they hit an enemy and continue to deal damage.\n\n" +
                "A motor powered blade that can chew its way through the gundead. It is designed to be revved with one hand and has a connection for a prostetic linkage. " +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Chainsaw_idle_001", 8);
            
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.introAnimation, 10);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .7f, StatModifier.ModifyMethod.ADDITIVE);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.SetBaseMaxAmmo(1100);
            gun.DefaultModule.cooldownTime = 1f;
            gun.quality = PickupObject.ItemQuality.B;

            gun.carryPixelOffset = new IntVector2(7, -3);



            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, 1f, 0f);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.barrelOffset.transform.localPosition = new Vector3(1.25f, 1f, 0f);
            projectile.baseData.damage = 4f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 4f;
            projectile.baseData.force = 5;
            ProjectileSlashingBehaviour slashy = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashDamageUsesBaseProjectileDamage = true;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            slashy.SlashRange = 0f;
            slashy.SlashDimensions = 50f;
            slashy.soundToPlay = null;
            slashy.SlashVFX = gun.muzzleFlashEffects;
            proj = projectile;
            projectile.transform.parent = gun.barrelOffset;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            StandardID = gun.PickupObjectId;
        }
      

        private void Slashy_OnSlashEnd(bool hitAnEnemy, PlayerController player)
        {
            if (hitAnEnemy == false)
            {
                gun.spriteAnimator.OverrideTimeScale = -1;
                AkSoundEngine.SetRTPCValue("SAW_Sound", 75);
            }
        }

        private void Slashy_OnSlashHitEnemy1(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            gun.spriteAnimator.OverrideTimeScale = .45f;
            AkSoundEngine.SetRTPCValue("SAW_Sound", 100);
            if(hitEnemy != null)
            {
                if(player.PlayerHasActiveSynergy("Glory Kill!"))
                {
                    if (hitEnemy.healthHaver.IsDead && hitEnemy.gameObject.GetOrAddComponent<AiactorSpecialStates>().LootedByBaba == false)
                    {

                        Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(tinyAmmo.ID).gameObject, hitEnemy.sprite.WorldCenter, vector * -1, 5f, false, false, false);
                        hitEnemy.gameObject.GetOrAddComponent<AiactorSpecialStates>().LootedByBaba = true;
                    }
                }
                
            }
            
        }

        
        public static Projectile proj;
        public static int StandardID;
        public static VFXPool swipevfx = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;

        public System.Random rng = new System.Random();
        
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            Sawing = false;
            player.primaryHand.ForceRenderersOff = true;
            gun.PreventNormalFireAudio = true;

        }

        public override void PostProcessProjectile(Projectile projectile)
        {

            


            base.PostProcessProjectile(projectile);
        }



        private bool HasReloaded;
        public bool setup = false;
        public bool Startup = false;

        public override void Update()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
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
                if (gun.spriteAnimator.IsPlaying("Chainsaw_fire"))
                {
                    if (Sawing== false)
                    {
                        StartCoroutine(DoSaw());
                    }
                    
                }
                
                if(gun.spriteAnimator.IsPlaying("Chainsaw_fire") == false)
                {
                    gun.spriteAnimator.OverrideTimeScale = -1;
                    AkSoundEngine.SetRTPCValue("SAW_Sound", 0);
                }

                if (gun.spriteAnimator.IsPlaying("Chainsaw_empty") == true)
                {
                    gun.spriteAnimator.OverrideTimeScale = -1;
                    AkSoundEngine.SetRTPCValue("SAW_Sound", 50);
                }

                if (Startup == false)
                {
                    AkSoundEngine.PostEvent("Play_Chainsaw_Sawing", base.gameObject);
                    AkSoundEngine.PostEvent("Play_Chainsaw_Idle", base.gameObject);
                    AkSoundEngine.SetRTPCValue("SAW_Sound", 0);
                    Startup = true;
                    ChainsawCleanup buzzbuzz = player.gameObject.GetOrAddComponent<ChainsawCleanup>();
                    buzzbuzz.player = player;
                    buzzbuzz.saw = this.gun;
                   
                }

            }
        }
        public bool Sawing = false;
        private IEnumerator DoSaw()
        {
            Sawing = true;
            float angle = 30;
            if (gun.spriteAnimator.CurrentFrame == 1) angle = 60f;
            if (gun.spriteAnimator.CurrentFrame == 2) angle = 30f;
            if( gun.spriteAnimator.CurrentFrame == 3) angle = 10f;
            if (gun.spriteAnimator.CurrentFrame == 4) angle = -10f;
            if( gun.spriteAnimator.CurrentFrame == 5) angle = -30;
            if (gun.spriteAnimator.CurrentFrame == 6) angle = -60f;
            if (gun.CurrentAngle > 90 || gun.CurrentAngle < -90) angle *= -1;
            Projectile Comp = MiscToolMethods.SpawnProjAtPosi(proj, gun.barrelOffset.transform.position, this.gun.CurrentOwner as PlayerController, gun.CurrentAngle + angle);
            //ETGModConsole.Log(gun.CurrentAngle + angle);
            ProjectileSlashingBehaviour slashy = Comp.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashDamageUsesBaseProjectileDamage = true;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            slashy.SlashRange = 4f;
            slashy.SlashDimensions = 45f;
            slashy.soundToPlay = null;
            AkSoundEngine.SetRTPCValue("SAW_Sound", 75);
            slashy.OnSlashHitEnemy += Slashy_OnSlashHitEnemy1;
            slashy.OnSlashEnd += Slashy_OnSlashEnd;
            
            slashy.SlashVFX = swipevfx;
            gun.LoseAmmo(1);
            
            yield return new WaitForSeconds(.1f / (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.RateOfFire));
            Sawing = false;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            if(gun.spriteAnimator.IsPlaying("Chainsaw_intro") != true)
            {
                AkSoundEngine.SetRTPCValue("SAW_Sound", 45);
                AkSoundEngine.PostEvent("Play_Chainsaw_Starter", base.gameObject);
                tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("Chainsaw_intro");
                gun.spriteAnimator.Play(clip, 0, 2 / gun.reloadTime, true);
            }
            
        }


        public override void OnDropped()
        {

            base.OnDropped();
        }
        public static bool Holding = false;

    }
}