using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;

namespace Knives
{
    class ToyAK: AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Toy AK", "T_ak");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:toy_ak", "ski:toy_ak");
            gun.gameObject.AddComponent<ToyAK>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Design Flaw");
            gun.SetLongDescription("Chance to jam when firing. This gun jams less the cooler you are. Press reload any time the gun is jammed to clear the mechanism. \n\n" +
                "" +
                "This particular model of toy gun fires bullets at a much increased rate to its smaller counterparts. However due to shotty spring mechanisms it is prone to jamming.\n" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "T_ak_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 5);


            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(24) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 6;
            gun.gunClass = GunClass.SHITTY;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .7f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.DefaultModule.cooldownTime = .18f;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.carryPixelOffset = new IntVector2(5, 0);
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.PreventNormalFireAudio = true;
            //gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.83f, .38f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 3.5f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range *= 1f;
            
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ToyAK.JamPrefab = SpriteBuilder.SpriteFromResource(ToyAK.JamVFX, null);
            ToyAK.JamPrefab.name = ToyAK.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(ToyAK.JamPrefab);
            FakePrefab.MarkAsFakePrefab(ToyAK.JamPrefab);
            ToyAK.JamPrefab.SetActive(false);


        }
       
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            player.GunChanged += this.OnGunChanged;
            base.OnPickedUpByPlayer(player);
        }
       
        public bool jammed = false;
        public System.Random rng = new System.Random();
       
        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            int jam = rng.Next(1, (int)(25 + (5 * player.stats.GetStatValue(PlayerStats.StatType.Coolness))));
            
            if (jam == 1 && this.gun.ClipShotsRemaining != this.gun.DefaultModule.numberOfShotsInClip && this.gun.ClipShotsRemaining != 1 && this.gun.AdjustedReloadTime > 0 && this.gun.ClipShotsRemaining < (this.gun.ClipCapacity *.5f) )
            {
                this.gun.DefaultModule.cooldownTime = 999999f;
                this.gun.reloadTime = 2.6f;

                AkSoundEngine.PostEvent("Play_t_ak_jam", base.gameObject);
                AkSoundEngine.PostEvent("Play_t_ak_jam", base.gameObject);
                AkSoundEngine.PostEvent("Play_t_ak_jam", base.gameObject);
                AkSoundEngine.PostEvent("Play_t_ak_jam", base.gameObject);
                AkSoundEngine.PostEvent("Play_t_ak_jam", base.gameObject);
                jammed = true;

                GameObject original = ToyAK.JamPrefab;
                tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                player.BloopItemAboveHead(OhGeez, "");



            }
            else
            {
                AkSoundEngine.PostEvent("Play_t_ak_shoot", base.gameObject);
            }
            gun.PreventNormalFireAudio = true;
            
            
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            

            base.OnFinishAttack(player, gun);
        }
       
        public override void PostProcessProjectile(Projectile projectile)
        {

            

            base.PostProcessProjectile(projectile);
        }
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            StartCoroutine(jamreloadProcessor());
                
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);
                
            

        }
        public IEnumerator jamreloadProcessor()
        {
            if(jammed && gun.ClipShotsRemaining == gun.ClipCapacity)
            {
               
                yield return new WaitForSecondsRealtime(.01f);
                this.gun.ClearCooldowns();
                this.gun.reloadTime = .7f;
                this.gun.DefaultModule.cooldownTime = .17f;
                jammed = false;
            }
            else
            {
                if (jammed)
                {
                    gun.spriteAnimator.Play("T_ak_critical_fire");
                    yield return new WaitForSecondsRealtime(2.6f);
                    this.gun.ClearCooldowns();
                    this.gun.reloadTime = .7f;
                    this.gun.DefaultModule.cooldownTime = .17f;
                    jammed = false;
                }

            }

        }
       
        public override void  OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            player.GunChanged -= this.OnGunChanged;
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (jammed == true)
            {
                newGun.GainAmmo(-1);
                
            }
        }
        
        private bool HasReloaded;
        public override void  Update()
        {
            if (gun.CurrentOwner && GameManager.Instance.CurrentFloor != 0 && GameManager.Instance.IsLoadingLevel == false)
            {
                
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
                if (jammed && gun.ClipShotsRemaining == gun.ClipCapacity && instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.Shoot))
                {
                    this.gun.ClearCooldowns();
                    this.gun.reloadTime = .7f;
                    this.gun.DefaultModule.cooldownTime = .17f;
                    
                    jammed = false;
                }
            }
        }

        private static string JamVFX = "Knives/Resources/Jam";
        private static GameObject JamPrefab;
        private static string vfxName = "NoHealingAllowed";
    }
}


