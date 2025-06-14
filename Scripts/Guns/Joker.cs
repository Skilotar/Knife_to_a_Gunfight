using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Joker : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Prank Gun", "Joker");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:prank_gun", "ski:prank_gun");
            gun.gameObject.AddComponent<Joker>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("BANG!!");
            gun.SetLongDescription("A comedy prop gun is probably not the smartest thing to bring to a firefight.\n\n" +
                "The sheer surprise alone is enough to peak the anxiety of your opposition" +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Joker_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 6;
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .6f;
            gun.DefaultModule.cooldownTime = .1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(200);
            
            gun.quality = PickupObject.ItemQuality.D;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 20f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range = 1;
            ProjectileSlashingBehaviour stabby = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashDimensions = 20;
            stabby.SlashRange = 2.5f;
            stabby.SlashVFX.type = VFXPoolType.None;
            stabby.soundToPlay = "";
            stabby.slashKnockback = 15;
            stabby.SlashDamageUsesBaseProjectileDamage = true;
            projectile.transform.parent = gun.barrelOffset;
            gun.barrelOffset.transform.position = new Vector3(14 / 16f, 10 / 16f, 0);

            MiscToolMethods.TrimAllGunSprites(gun);
            gun.gunClass = GunClass.SILLY;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
            if(player.CurrentRoom != null)
            {
                Joker.fleeData = new FleePlayerData();
                Joker.fleeData.Player = player;
                Joker.fleeData.StartDistance = 20f;
                Projectile bigfearzoneboi = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, gun.barrelOffset.transform.position, player, gun);
                ProjectileSlashingBehaviour stabby = bigfearzoneboi.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                stabby.SlashDimensions = 20;
                stabby.SlashRange = 14f;
                stabby.SlashVFX.type = VFXPoolType.None;
                stabby.soundToPlay = "";
                stabby.slashKnockback = 15;
                stabby.SlashDamage = 10;
                stabby.SlashDamageUsesBaseProjectileDamage = false;
                stabby.OnSlashHitEnemy += Stabby_OnSlashHitEnemy;
            }
            
        }

        private void Stabby_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if (hitEnemy != null)
            {
                hitEnemy.behaviorSpeculator.FleePlayerData = Joker.fleeData;
                GameManager.Instance.StartCoroutine(Joker.RemoveFear(hitEnemy));
            }

        }

        private static IEnumerator RemoveFear(AIActor aiactor)
        {
            yield return new WaitForSeconds(5f);
            aiactor.behaviorSpeculator.FleePlayerData = null;
            yield break;
        }

       
        private static FleePlayerData fleeData;

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
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



            }

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);
            }



        }

    }
}