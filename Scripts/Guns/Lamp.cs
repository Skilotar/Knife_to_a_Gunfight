using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;


namespace Knives
{

    public class Lamp : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Lava Launcher", "Lamp");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:lava_launcher", "ski:lava_launcher");
            gun.gameObject.AddComponent<Lamp>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Caution Hot");
            gun.SetLongDescription("An rpg modified to fire canisters of hot wax. These Lava lamp launchers were used by hegemony forces to combat creatures of pure elements./n/n" +
                "The heated wax was well suited for reducing the threat levels of Ice and Plasma based lifeforms." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Lamp_idle_001", 3);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 5);
            gun.SetAnimationFPS(gun.reloadAnimation, 3);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(39) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_RPG_shot_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_rpg_reload_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.75f;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(50);
            
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(3f, .5f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 17f;
            projectile.baseData.speed = 40;
            projectile.shouldRotate = true;
            projectile.SetProjectileSpriteRight("Lamp_proj", 20, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 7);
            LampBoom = new ExplosionData()
            {
                effect = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<ProximityMine>().explosionData.effect,


                damageRadius = 3.5f,
                damageToPlayer = 0f,
                doDamage = true,
                damage = 15,
                doDestroyProjectiles = false,
                doForce = true,
                debrisForce = 20f,
                preventPlayerForce = true,
                explosionDelay = 0.1f,
                usesComprehensiveDelay = false,
                doScreenShake = true,
                playDefaultSFX = false,
                force = 20,
                breakSecretWalls = false,

            };
            ExplosiveModifier boom = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
            boom.explosionData = LampBoom;
            boom.doExplosion = true;

            BeamBulletsBehaviour Lava = projectile.gameObject.AddComponent<BeamBulletsBehaviour>();
            Lava.firetype = BeamBulletsBehaviour.FireType.TRAIL_V;
            Lava.vTrailAngle = 20f;
            LavaBeam = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(658) as Gun).DefaultModule.projectiles[0]);
            LavaBeam.baseData.speed *= 2;
            LavaBeam.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(LavaBeam.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(LavaBeam);
            Lava.beamToFire = LavaBeam;
            GoopModifier gooper = projectile.gameObject.AddComponent<GoopModifier>();
            gooper.SpawnGoopInFlight = false;
            gooper.SpawnGoopOnCollision = true;
            gooper.CollisionSpawnRadius = 4;
            gooper.InFlightSpawnRadius = 1;
            gooper.InFlightSpawnFrequency = 0.05f;
            gooper.goopDefinition = EasyGoopDefinitions.FireDef;
            projectile.onDestroyEventName = "Play_obj_glass_break_01";
            projectile.objectImpactEventName = "Play_obj_glass_break_01";
            projectile.enemyImpactEventName = "Play_obj_glass_break_01";
            HomingModifier home = projectile.gameObject.GetOrAddComponent<HomingModifier>();
            home.HomingRadius = 40;
            home.AngularVelocity = 20;
            gun.gunClass = GunClass.EXPLOSIVE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public static Projectile LavaBeam;
        public static ExplosionData LampBoom;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }

        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }

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

            }



        }

    }
}