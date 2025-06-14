using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;
using Dungeonator;
using System.Collections.ObjectModel;
using Alexandria.ItemAPI;

namespace Knives
{

    public class USB : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("USB", "USB");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:usb", "ski:usb");
            gun.gameObject.AddComponent<USB>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Universal.Space.Blaster.");
            gun.SetLongDescription("Reload to scan enemies and copy their traits until a new scan is done and they are overridden\n\n" +
                "A highly moddable space blaster with a port in the handle for a memory drive to be inserted. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "USB_idle_001", 2);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(345) as Gun, true, false);
            Gun gun2 = PickupObjectDatabase.GetById(345) as Gun;

            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(89)).muzzleFlashEffects;
            
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.6f;
            gun.DefaultModule.cooldownTime = .35f;
            baseCooldownTime = gun.DefaultModule.cooldownTime;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(450);
           
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(18/16f, 14/16f, 0f);
            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 5f;
            projectile.baseData.speed *= 1.2f;
            projectile.AdditionalScaleMultiplier = .6f;

            projectile.enemyImpactEventName = ((Gun)PickupObjectDatabase.GetById(89)).DefaultModule.projectiles[0].enemyImpactEventName;
            projectile.objectImpactEventName = ((Gun)PickupObjectDatabase.GetById(89)).DefaultModule.projectiles[0].objectImpactEventName;
            projectileStates state = projectile.gameObject.GetOrAddComponent<projectileStates>();
            state.USBproj = true;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;

            List<string> resourcesList = new List<string>()
            {
                "Knives/Resources/USBOverheads/None_icon.png",
                "Knives/Resources/USBOverheads/Basic_icon.png",
                "Knives/Resources/USBOverheads/Basic_shotty_icon.png",
                "Knives/Resources/USBOverheads/Sniper_icon.png",
                "Knives/Resources/USBOverheads/Magic_icon.png",
                "Knives/Resources/USBOverheads/Radial_icon.png",
                "Knives/Resources/USBOverheads/Melee_icon.png",
                "Knives/Resources/USBOverheads/Explode_icon.png",
                "Knives/Resources/USBOverheads/Boss_icon.png",
                "Knives/Resources/USBOverheads/Auto_icon.png",
                "Knives/Resources/USBOverheads/Split_icon.png",
                "Knives/Resources/USBOverheads/Devo_icon.png",
                "Knives/Resources/USBOverheads/Spooky_icon.png",
                "Knives/Resources/USBOverheads/Laser_icon.png",
                "Knives/Resources/USBOverheads/Armored_icon.png",
                "Knives/Resources/USBOverheads/Cube_icon.png",
                "Knives/Resources/USBOverheads/Poison_icon.png",
                "Knives/Resources/USBOverheads/Shroom_icon.png",
                "Knives/Resources/USBOverheads/Burst_icon.png",
                "Knives/Resources/USBOverheads/Shark_icon.png",
                "Knives/Resources/USBOverheads/Bounce_icon.png",
                "Knives/Resources/USBOverheads/Heavy_Melee_icon.png",
                "Knives/Resources/USBOverheads/Maiden_icon.png",
                "Knives/Resources/USBOverheads/Mimic_icon.png",
                "Knives/Resources/USBOverheads/Fire_icon.png",
                "Knives/Resources/USBOverheads/Fairy_icon.png",
                "Knives/Resources/USBOverheads/Poison_shotty_icon.png",
                "Knives/Resources/USBOverheads/Key_icon.png",
                "Knives/Resources/USBOverheads/Cactus_icon.png",
                "Knives/Resources/USBOverheads/Cactus_shotty_icon.png",
                "Knives/Resources/USBOverheads/Astral_icon.png",
                "Knives/Resources/USBOverheads/Hungry_icon.png",
                "Knives/Resources/USBOverheads/Fish_icon.png",
                "Knives/Resources/USBOverheads/Titan_icon.png",
                "Knives/Resources/USBOverheads/Vet_icon.png",
                "Knives/Resources/USBOverheads/Vet_shotty_icon.png",
                "Knives/Resources/USBOverheads/Chain_icon.png",
                "Knives/Resources/USBOverheads/Bubble_icon.png",
                "Knives/Resources/USBOverheads/Eye_icon.png",
                "Knives/Resources/USBOverheads/Electric_icon.png",
                "Knives/Resources/USBOverheads/Null_icon.png",
            };
            List<string> namesList = new List<string>()
            {
                "NONE",
                "BASIC",
                "SHOTGUN",
                "SNIPER",
                "MAGIC",
                "RADIAL",
                "MELEE",
                "EXPLOSIVE",
                "BOSS",
                "FULLAUTO",
                "SPLIT",
                "DEVO",
                "SPOOKY",
                "LASER",
                "ARMORED",
                "CUBE",
                "POISON",
                "SHROOM",
                "BURST",
                "SHARK",
                "BOUNCYMELEE",
                "HEAVYMELEE",
                "MAIDEN",
                "MIMIC",
                "FIRE",
                "FAIRY",
                "POISONSHOTTY",
                "KEY",
                "CACTUS",
                "CACTUSSHOTTY",
                "ASTRAL",
                "HUNGRY",
                "FISH",
                "REALLYBIG",
                "VET",
                "VETSHOTTY",
                "CHAIN",
                "BUBBLE",
                "EYE",
                "ELECTRIC",
                "NULL",
            };
            overheads = MiscToolMethods.GenerateOverheadFromResourceList(resourcesList, namesList);
        }

        public static List<GameObject> overheads = new List<GameObject>();

        public static int ID;
        public static float baseCooldownTime;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if(number == (int)EnemyType.HEAVYMELEE || number == (int)EnemyType.LASER)
            {
                AkSoundEngine.PostEvent("Play_WPN_bountyhunterarm_shot_02", base.gameObject);
            }
            if(number == (int)EnemyType.REALLYBIG)
            {
                AkSoundEngine.PostEvent("Play_WPN_bountyhunterarm_shot_03", base.gameObject);
            }
            if(number != (int)EnemyType.REALLYBIG || number != (int)EnemyType.HEAVYMELEE)
            {
                AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            }

        }
        public enum EnemyType
        {
            NONE,
            BASIC,
            SHOTGUN,
            SNIPER,
            MAGIC,
            RADIAL,
            MELEE,
            EXPLOSIVE,
            BOSS,
            FULLAUTO,
            SPLIT,
            DEVO,
            SPOOKY,
            LASER,
            ARMORED,
            CUBE,
            POISON,
            SHROOM,
            BURST,
            SHARK,
            BOUNCYMELEE,
            HEAVYMELEE,
            MAIDEN,
            MIMIC,
            FIRE,
            FAIRY,
            POISONSHOTTY,
            KEY,
            CACTUS,
            CACTUSSHOTTY,
            ASTRAL,
            HUNGRY,
            FISH,
            REALLYBIG, 
            VET,
            VETSHOTTY,
            CHAIN, 
            BUBBLE,
            EYE, //
            ELECTRIC,
            NULL,
        }
        public int number;
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(projectile.GetComponent<projectileStates>() != null)
            {
                if (projectile.GetComponent<projectileStates>().USBproj == true)
                {


                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    gun.DefaultModule.cooldownTime = baseCooldownTime;
                    number = gundownloadedState;
                    if (gundownloadedState == (int)EnemyType.NULL)
                    {
                        number = UnityEngine.Random.Range(1, (int)EnemyType.NULL - 1); // randomize between basic and one below null
                    }
                    switch (number)
                    {
                        case 0: //None
                            break;

                        case 1: //basic - duplicate
                            StartCoroutine(USBbasic(projectile, player));
                            break;

                        case 2: //shotgun
                            gun.DefaultModule.cooldownTime = .5f;
                            StartCoroutine(USBshotty(projectile, player));
                            break;

                        case 3: //sniper
                            gun.DefaultModule.cooldownTime = .8f;
                            StartCoroutine(USBsnipe(projectile, player));
                            break;

                        case 4: //magic
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBmagic(projectile, player));
                            break;

                        case 5: //radial
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBpulse(projectile, player));
                            break;

                        case 6: //melee
                            StartCoroutine(USBmelee(projectile, player));
                            break;

                        case 7: //explosive
                            gun.DefaultModule.cooldownTime = .25f;
                            StartCoroutine(USBExplosive(projectile, player));
                            break;

                        case 8: //boss
                            gun.DefaultModule.cooldownTime = .3f;
                            StartCoroutine(USBHome(projectile, player));
                            break;

                        case 9: // full auto
                            gun.DefaultModule.cooldownTime = .25f;
                            StartCoroutine(USBAuto(projectile, player));
                            break;

                        case 10: //split
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBsplit(projectile, player));
                            break;

                        case 11: //Devo
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBDevolve(projectile, player));
                            break;

                        case 12: //spooky
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBSpooky(projectile, player));
                            break;

                        case 13: //laser
                            gun.DefaultModule.cooldownTime = .45f;
                            StartCoroutine(USBLaser(projectile, player));
                            break;

                        case 14:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBArmored(projectile, player));
                            break;

                        case 15:
                            gun.DefaultModule.cooldownTime = .7f;
                            StartCoroutine(USBCube(projectile, player));
                            break;

                        case 16:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBPoison(projectile, player));
                            break;

                        case 17:
                            gun.DefaultModule.cooldownTime = .5f;
                            StartCoroutine(USBShroom(projectile, player));
                            break;

                        case 18:
                            gun.DefaultModule.cooldownTime = .5f;
                            StartCoroutine(USBBurst(projectile, player));
                            break;

                        case 19:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBShark(projectile, player));
                            break;

                        case 20:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBBouncyMelee(projectile, player));
                            break;

                        case 21:
                            gun.DefaultModule.cooldownTime = 1.1f;
                            StartCoroutine(USBHeavyMelee(projectile, player));
                            break;

                        case 22:
                            gun.DefaultModule.cooldownTime = .8f;
                            StartCoroutine(USBMaiden(projectile, player));
                            break;

                        case 23:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBMimic(projectile, player));
                            break;

                        case 24:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBFire(projectile, player));
                            break;

                        case 25:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBFairy(projectile, player));
                            break;

                        case 26:
                            gun.DefaultModule.cooldownTime = .5f;
                            StartCoroutine(USBPoisonShotty(projectile, player));
                            break;

                        case 27:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBkey(projectile, player));
                            break;

                        case 28:
                            gun.DefaultModule.cooldownTime = .15f;
                            StartCoroutine(USBCactus(projectile, player));
                            break;

                        case 29:
                            gun.DefaultModule.cooldownTime = .45f;
                            StartCoroutine(USBCactusShotty(projectile, player));
                            break;

                        case 30:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBAstral(projectile, player));
                            break;

                        case 31:
                            gun.DefaultModule.cooldownTime = .4f;
                            StartCoroutine(USBhungry(projectile, player));
                            break;

                        case 32:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBFish(projectile, player));
                            break;

                        case 33:
                            gun.DefaultModule.cooldownTime = 1f;
                            StartCoroutine(USBReallyBig(projectile, player));
                            break;

                        case 34:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBVet(projectile, player));
                            break;

                        case 35:
                            gun.DefaultModule.cooldownTime = .45f;
                            StartCoroutine(USBVetShotty(projectile, player));
                            break;

                        case 36:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBChain(projectile, player));
                            break;

                        case 37:
                            gun.DefaultModule.cooldownTime = .3f;
                            StartCoroutine(USBBubble(projectile, player));
                            break;

                        case 38:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBEye(projectile, player));
                            break;

                        case 39:
                            gun.DefaultModule.cooldownTime = .35f;
                            StartCoroutine(USBElectric(projectile, player));
                            break;
                    }
                }
            }
            base.PostProcessProjectile(projectile);
        }

        private IEnumerator USBElectric(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= 1.2f;
            projectile.gameObject.GetOrAddComponent<LightningProjectileComp>();
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 1;
            projectile.Owner = player;
            projectile.Shooter = player.specRigidbody;
            projectile.baseData.damage = 5f;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = ExtendedColours.vibrantOrange;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Sp4rk_001",
                "Knives/Resources/BeamSprites/Sp4rk_002",
                "Knives/Resources/BeamSprites/Sp4rk_003",
            };
            projectile.AddTrailToProjectile(
            "Knives/Resources/BeamSprites/Sp4rk_001",
            new Vector2(3, 2),
            new Vector2(1, 1),
            BeamAnimPaths, 20,
            BeamAnimPaths, 20,
            -1,
            0.0001f,
            5,
            false
            );
            yield return null;
        }

        private IEnumerator USBEye(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .75f;

            yield return new WaitForSeconds(.07f);
            AkSoundEngine.PostEvent("Play_WPN_eyeballgun_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(((Gun)PickupObjectDatabase.GetById(90)).DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.damage *= .75f;

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBBubble(Projectile projectile, PlayerController player)
        {
            BubbleBulletMod bubble = projectile.gameObject.GetOrAddComponent<BubbleBulletMod>();
            bubble.player = player;
            yield return null;
        }

        private IEnumerator USBChain(Projectile projectile, PlayerController player)
        {
            if (UnityEngine.Random.Range(0, 4f) < 1)
            {
                TetherProjMod rope = projectile.gameObject.GetOrAddComponent<TetherProjMod>();
                rope.color = UnityEngine.Color.grey;
                PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                poke.penetration = 1;
                ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                trail.shadowLifetime = .35f;
                trail.shadowTimeDelay = .1f;
                trail.dashColor = UnityEngine.Color.grey;
                trail.spawnShadows = true;
            }
            yield return null;
        }

        private IEnumerator USBVetShotty(Projectile projectile, PlayerController player)
        {
           
            yield return new WaitForSeconds(.001f);

            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 5), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }
                
            }
            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 5), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }
            }
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            yield return new WaitForSeconds(.2f);
            if(projectile != null)
            {
                if (UnityEngine.Random.Range(0, 4f) < 1)
                {
                    VetHome(projectile);
                }
            }
            if (component != null)
            {
                if (UnityEngine.Random.Range(0, 4f) < 1)
                {
                    VetHome(component);
                }
            }
            if (component2 != null)
            {
                if (UnityEngine.Random.Range(0, 4f) < 1)
                {
                    VetHome(component2);
                }
            }

            

        }

        public void VetHome(Projectile projectile)
        {
            projectile.Speed *= 2;
            projectile.baseData.damage *= 1.2f;
            projectile.SendInDirection(OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(projectile.LastPosition), true, true);
            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .3f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = ExtendedColours.lime;
            trail.spawnShadows = true;
        }

        private IEnumerator USBVet(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.2f);
            if (projectile != null)
            {
                if (UnityEngine.Random.Range(0, 2f) < 1)
                {
                    VetHome(projectile);
                }
            }
            yield return null;
        }

        private IEnumerator USBReallyBig(Projectile projectile, PlayerController player)
        {
            projectile.AdditionalScaleMultiplier *= 4;
            projectile.baseData.damage *= 5;
            projectile.Speed *= .5f;
            projectile.baseData.force = 40;
            BounceProjModifier bounce = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounce.numberOfBounces = 4;
            PierceProjModifier stab = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetratesBreakables = true;
            stab.penetration = 99;
            Vector2 vector = player.CenterPosition - player.unadjustedAimPoint.XY();
            player.knockbackDoer.ApplyKnockback(vector, 20);

            yield return null;
        }

        private IEnumerator USBFish(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .75f;

            yield return new WaitForSeconds(.07f);
            AkSoundEngine.PostEvent("Play_Bubble_shot_001", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(((Gun)PickupObjectDatabase.GetById(7)).DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.damage *= .75f;

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBhungry(Projectile projectile, PlayerController player)
        {
            if (UnityEngine.Random.Range(0, 2f) < 1)
            {
                HungryProjectileModifier monch = projectile.gameObject.GetOrAddComponent<HungryProjectileModifier>();
                monch.HungryRadius = .2f;
            }
            yield return null;
        }

        private IEnumerator USBAstral(Projectile projectile, PlayerController player)
        {

            AkSoundEngine.PostEvent("Play_WPN_star_impact_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.damage *= .5f;

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            yield return null;
        }

        private IEnumerator USBCactusShotty(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.001f);
            Gun cac = (Gun)PickupObjectDatabase.GetById(124);
            
            GameObject gameObject = SpawnManager.SpawnProjectile(cac.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 5), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", base.gameObject);
            GameObject gameObject2 = SpawnManager.SpawnProjectile(cac.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 5), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }


            }

        }

        private IEnumerator USBCactus(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.05f);
            Gun cac = (Gun)PickupObjectDatabase.GetById(124);
            AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(cac.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 2), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            yield return new WaitForSeconds(.05f);
            AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", base.gameObject);
            GameObject gameObject2 = SpawnManager.SpawnProjectile(cac.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 2), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag22 = component2 != null;
            if (flag22)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
        }

        private IEnumerator USBkey(Projectile projectile, PlayerController player)
        {
            projectile.DefaultTintColor = ExtendedColours.silvedBulletsSilver;
            projectile.HasDefaultTint = true;
            if(UnityEngine.Random.Range(0,5f) <= 1)
            {
                Projectile Key = MiscToolMethods.SpawnProjAtPosi(((Gun)PickupObjectDatabase.GetById(95)).DefaultModule.projectiles[0], gun.barrelOffset.transform.position, player, gun);
                Key.baseData.damage *= 3;
                Key.statusEffectsToApply.Add(StaticStatusEffects.tripleCrossbowSlowEffect);
            }
            yield return null;
        }

        private IEnumerator USBPoisonShotty(Projectile proj, PlayerController player)
        {
            
            GoopModifier gloop = proj.gameObject.GetOrAddComponent<GoopModifier>();
            gloop.goopDefinition = EasyGoopDefinitions.PoisonDef;
            gloop.CollisionSpawnRadius = 2;
            gloop.SpawnGoopOnCollision = true;
            gloop.SpawnGoopInFlight = false;
            yield return new WaitForSeconds(.001f);

            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 5), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                GoopModifier gloop1 = component.gameObject.GetOrAddComponent<GoopModifier>();
                gloop1.goopDefinition = EasyGoopDefinitions.PoisonDef;
                gloop1.CollisionSpawnRadius = 2;
                gloop1.SpawnGoopOnCollision = true;
                gloop1.SpawnGoopInFlight = false;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 5), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                GoopModifier gloop2 = component2.gameObject.GetOrAddComponent<GoopModifier>();
                gloop2.goopDefinition = EasyGoopDefinitions.PoisonDef;
                gloop2.CollisionSpawnRadius = 2;
                gloop2.SpawnGoopOnCollision = true;
                gloop2.SpawnGoopInFlight = false;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }


            }

            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);

        }

        private IEnumerator USBFairy(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .8f;
            projectile.CharmApplyChance = .25f;
            projectile.charmEffect = StaticStatusEffects.charmingRoundsEffect;
            projectile.AppliesCharm = true;
            yield return null;
        }

        private IEnumerator USBFire(Projectile projectile, PlayerController player)
        {

            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = ExtendedColours.carrionRed;
            projectile.fireEffect = StaticStatusEffects.hotLeadEffect;
            projectile.FireApplyChance = 1f;
            projectile.AppliesFire = true;
            ReverseSpeedRampMod ramp = projectile.gameObject.GetOrAddComponent<ReverseSpeedRampMod>();
            ramp.doesDamageRamp = false;
            ramp.doeffects = true;
            ramp.minSpeedMult = .8f;
            ramp.speedMultRate = 1.05f;

            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .3f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = new Color(.47f, .30f, .37f);
            trail.spawnShadows = true;

            yield return null;
        }

        private IEnumerator USBMimic(Projectile projectile, PlayerController player)
        {
            
            projectile.baseData.damage *= .5f;
            yield return new WaitForSeconds(.1f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(((Gun)PickupObjectDatabase.GetById(734)).DefaultModule.projectiles[0], gun.barrelOffset.transform.position, player, gun);
            proj.baseData.damage *= 1.5f;
            proj.baseData.speed *= 1f;
            proj.HasDefaultTint = true;
            proj.DefaultTintColor = ExtendedColours.poisonGreen;
            
        }

        private IEnumerator USBMaiden(Projectile projectile, PlayerController player)
        {

            for (int i = 0; i < 6; i++)
            {

                GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 60) + startingRotation), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                    bounce.bouncesTrackEnemies = true;
                    bounce.TrackEnemyChance = 1;
                    bounce.bounceTrackRadius = 7;
                    
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component);
                    }

                }
            }
            if (startingRotation != 180)
            {
                startingRotation += 20;
            }


            yield return null;

        }

        private IEnumerator USBHeavyMelee(Projectile projectile, PlayerController player)
        {
            gun.LoseAmmo(4);
            projectile.AdditionalScaleMultiplier *= 1.5f;
            projectile.baseData.speed *= .75f;
            PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetration = 1;
            Vector2 vector = player.CenterPosition - player.unadjustedAimPoint.XY();
            player.knockbackDoer.ApplyKnockback(vector, 20);
            projectile.OnHitEnemy += OnHitHeavyMelee;
            yield return null;
        }

        private void OnHitHeavyMelee(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            ProjectileSlashingBehaviour slashy = arg1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashDamage = 30;
            slashy.SlashDamageUsesBaseProjectileDamage = false;
            slashy.SlashRange = 4f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 90;
            slashy.slashKnockback = 40;
            slashy.SlashVFX = ((Gun)PickupObjectDatabase.GetById(417)).muzzleFlashEffects;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
        }

        private IEnumerator USBBouncyMelee(Projectile projectile, PlayerController player)
        {
            PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetration = 1;
            projectile.OnHitEnemy += BouncyOnHitMelee;
            BounceProjModifier bounce = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounce.numberOfBounces = 1;
            yield return null;
        }

        private void BouncyOnHitMelee(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            ProjectileSlashingBehaviour slashy = arg1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 3.7f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 50;
            slashy.SlashVFX = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
        }
        private IEnumerator USBShark(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .8f;
            GoopModifier gloop = projectile.gameObject.GetOrAddComponent<GoopModifier>();
            gloop.goopDefinition = EasyGoopDefinitions.WaterGoop;
            gloop.InFlightSpawnRadius = .5f;
            gloop.InFlightSpawnFrequency = 1;
            gloop.SpawnGoopOnCollision = false;
            gloop.SpawnGoopInFlight = true;
            TrailProjMod trail = projectile.gameObject.GetOrAddComponent<TrailProjMod>();
            trail.Shark = true;
            trail.boom = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.chargeProjectiles[0].Projectile;
            trail.rate = .08f;
            yield return null;
        }

        private IEnumerator USBBurst(Projectile projectile, PlayerController player)
        {

            projectile.baseData.damage *= 1f;
            ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
            split.splitOnEnemy = true;
            split.splitOnDeath = true;
            split.distanceBasedSplit = false;
            split.numberofsplits = 1;
            split.amtToSplitTo = 6;
            split.dmgMultAfterSplit = .75f;
            split.sizeMultAfterSplit = .9f;
            split.splitAngles = 360f;
            split.removeComponentAfterUse = true;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = ExtendedColours.vibrantOrange;
            yield return null;
        }

        private IEnumerator USBShroom(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .2f;
            projectile.OnHitEnemy += this.ShroomyOnHit;
            yield return null;
        }

        private void ShroomyOnHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            for (int i = 0; i < 6; i++)
            {
                Projectile proj = MiscToolMethods.SpawnProjAtPosi(CloudInABottle.cloudPoof, arg1.LastPosition, this.gun.CurrentOwner as PlayerController, 20 * i);
                proj.baseData.damage = 1;
                proj.HasDefaultTint = true;
                proj.DefaultTintColor = ExtendedColours.darkGreen;

            }
        }

        private IEnumerator USBPoison(Projectile projectile, PlayerController player)
        {
            GoopModifier gloop = projectile.gameObject.GetOrAddComponent<GoopModifier>();
            gloop.goopDefinition = EasyGoopDefinitions.PoisonDef;
            gloop.CollisionSpawnRadius = 3;
            gloop.SpawnGoopOnCollision = true;
            gloop.SpawnGoopInFlight = false;
            yield return null;
        }

        private IEnumerator USBCube(Projectile projectile, PlayerController player)
        {
            projectile.OnHitEnemy += Encubeinate;
            if (UnityEngine.Random.Range(0, 2f) < 1)
            {
                projectile.baseData.damage *= .5f;
                yield return new WaitForSeconds(.05f);
                AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
                Projectile proj = MiscToolMethods.SpawnProjAtPosi(((Gun)PickupObjectDatabase.GetById(475)).DefaultModule.projectiles[0], gun.barrelOffset.transform.position, player, gun);
                proj.baseData.damage *= .4f;
                proj.baseData.speed *= 2f;
                proj.AdditionalScaleMultiplier = .5f;
                proj.OnHitEnemy += Encubeinate;
            }
        }

        private void Encubeinate(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                arg2.aiActor.sprite.renderer.material.SetFloat("_RectangleAmount", 1f);
            }
        }

        private IEnumerator USBArmored(Projectile projectile, PlayerController player)
        {
            BlockletsModifier block = projectile.gameObject.GetOrAddComponent<BlockletsModifier>();
            block.DieWithShield = true;
            yield return null;
        }

        private IEnumerator USBAuto(Projectile proj, PlayerController player)
        {
            
            yield return new WaitForSeconds(.05f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle +2), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            yield return new WaitForSeconds(.05f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle -2), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag22 = component2 != null;
            if (flag22)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
        }

        private IEnumerator USBsplit(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= 2f;
            yield return new WaitForSeconds(.001f);

            if(projectile != null)
            {
                ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
                split.distanceBasedSplit = true;
                split.amtToSplitTo = 1;
                split.distanceTillSplit = .001f;
                split.dmgMultAfterSplit = 1f;
                split.numberofsplits = 1;
                split.removeComponentAfterUse = true;
                split.splitAngles = 45f;
            }

        }
        private IEnumerator USBmagic(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .5f;
            yield return new WaitForSeconds(.1f);
            
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[145]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.ChanceToTransmogrify = .3f;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBsnipe(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .2f;
            yield return new WaitForSeconds(.1f);
            
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[25]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_m1rifle_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }
        private IEnumerator USBDevolve(Projectile projectile, PlayerController player)
        {
            projectile.OnHitEnemy += this.OnHitEnemy;
            projectile.sprite.color = UnityEngine.Color.black;
            projectile.baseData.damage *= .8f;
            yield return null;
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (UnityEngine.Random.Range(1, 4) == 1)
                {
                    arg2.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid("05891b158cd542b1a5f3df30fb67a7ff"), BlinkpoofVfx);
                }
            }
            
        }

        private IEnumerator USBSpooky(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .7f;
            yield return new WaitForSeconds(.1f);

            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[29]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBLaser(Projectile projectile, PlayerController player)
        {
            projectile.baseData.damage *= .8f;
            projectile.baseData.speed *= .5f;
            
            BeamBulletsBehaviour beam = projectile.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
            beam.beamToFire = ((Gun)PickupObjectDatabase.GetById(87)).DefaultModule.projectiles[0].projectile;
            beam.firetype = BeamBulletsBehaviour.FireType.CROSS;
            yield return null;
           
        }

        private IEnumerator USBHome(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.2f);
            if(projectile != null)
            {
                projectile.SendInDirection(OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(projectile.LastPosition), true);
                projectile.baseData.damage *= 2f;
            }
            

        }

        private IEnumerator USBExplosive(Projectile projectile, PlayerController player)
        {

            
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.blast));
            yield return null;
        }

        private void blast(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (!arg3)
                {
                    BlastBlightedStatusController blast = arg2.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                    blast.statused = true;
                }
            }
        }

        public int startingRotation = 0;
        private IEnumerator USBpulse(Projectile proj, PlayerController player)
        {
            
            yield return new WaitForSeconds(.001f);
            
            for (int i = 0; i < 6; i++)
            {

                GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 60) + startingRotation), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component);
                    }

                }
            }
            if (startingRotation != 180)
            {
                startingRotation += 20;
            }
            
        }
        public System.Random rng = new System.Random();
        private void doslashy(PlayerController player)
        {
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.AdditionalScaleMultiplier = .001f;
            component.Owner = this.gun.CurrentOwner;
            component.baseData.damage = 7;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 4f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 20;
            slashy.SlashVFX = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
        }
        private IEnumerator USBmelee(Projectile proj, PlayerController player)
        {
            Projectile projectile = proj;
            yield return new WaitForSeconds(.001f);
            doslashy(player);
            int sound = rng.Next(1, 4);
            if (sound == 1)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
            }
            if (sound == 2)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
            }
            if (sound == 3)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
            }

        }

        private IEnumerator USBshotty(Projectile proj, PlayerController player)
        {

            
            yield return new WaitForSeconds(.001f);
           
            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 5), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 5), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }


            }
            
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            
        }
        private IEnumerator USBbasic(Projectile proj,PlayerController player)
        {
            proj.baseData.damage *= .75f;
            
            yield return new WaitForSeconds(.07f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.damage *= .75f;

                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
            
        }

        private bool HasReloaded;
        public bool Startup = false;
        //This block of code allows us to change the reload sounds.
        public override void  Update()
        {
            if (gun.CurrentOwner != null)
            {

                if(Startup != true)
                {
                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    gundownloadedState = 0;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    Startup = true;
                }

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                    AkSoundEngine.PostEvent("Play_USB_reload_002", base.gameObject);

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
                AkSoundEngine.PostEvent("Play_USB_reload_001", base.gameObject);

                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));

                component.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/GlitchUnlit");
                component.sprite.usesOverrideMaterial = true;
                component.sprite.renderer.material.SetFloat("_GlitchInterval", 0.1f);
                component.sprite.renderer.material.SetFloat("_DispProbability", 0.03f);
                component.sprite.renderer.material.SetFloat("_DispIntensity", 0.03f);
                component.sprite.renderer.material.SetFloat("_ColorProbability", 0.4f);
                component.sprite.renderer.material.SetFloat("_ColorIntensity", 0.04f);

                component.pierceMinorBreakables = true;
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 0f;
                    component.baseData.speed = 30f;
                    
                }

                if(gundownloadedState == (int)EnemyType.ARMORED || gundownloadedState == (int)EnemyType.MAIDEN)
                {
                    player.StartCoroutine(BubbleMeScotty(player, gun));
                }

                if(gundownloadedState == (int)EnemyType.FAIRY)
                {
                    SpawnNavi(player,gun);
                }

                if(gundownloadedState == (int)EnemyType.CACTUS || gundownloadedState == (int)EnemyType.CACTUSSHOTTY)
                {
                    StartCoroutine(DoCactus(player, gun));
                }

                if(gundownloadedState == (int)EnemyType.HUNGRY)
                {
                    StartCoroutine(MinorRelodStoneTypeSuck(player,gun));
                    GameManager.Instance.StartCoroutine(MiscToolMethods.DoReverseDistortionWaveLocal(player.CenterPosition, .5f, 0.1f, 5f, 1f));
                }
                if(gundownloadedState == (int)EnemyType.EYE)
                {
                    if (gun.ClipShotsRemaining == 0)
                    {
                        MiscToolMethods.SpawnProjAtPosi(((Gun)PickupObjectDatabase.GetById(90)).DefaultModule.finalProjectile, gun.barrelOffset.transform.position, player, this.gun);
                    }
                }
            }
        }

        private IEnumerator MinorRelodStoneTypeSuck(PlayerController player, Gun gun)
        {
            while (gun.IsReloading)
            {
                DoMunch(player, gun);
                yield return null;
            }
            yield return null;
        }

        private void DoMunch(PlayerController player, Gun gun)
        {
            List<Projectile> bullets = GetBullets(); // I am in pain
            for(int i = bullets.Count-1; i >= 0; i--)
            {
                Projectile bullet = bullets[i];
                if(Vector2.Distance(bullet.specRigidbody.UnitCenter ,player.CenterPosition) <= 6)
                {
                    gun.GainAmmo(1);
                    StartCoroutine(HandleBulletSuck(bullet, player));
                }
            }
        }
        private IEnumerator HandleBulletSuck(Projectile bullet, PlayerController player)
        {
            //ETGModConsole.Log("StartSuck");
            Transform copySprite = this.CreateEmptySprite(bullet);
            Vector3 startPosition = copySprite.transform.position;
            bullet.DieInAir();
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag = this.gun && copySprite;
                if (flag)
                {
                    Vector3 position = player.CenterPosition;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
           
            yield break;
        }

        
        private Transform CreateEmptySprite(Projectile bullet)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = bullet.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(bullet.sprite.Collection, bullet.sprite.spriteId);
            tk2dSprite.transform.position = bullet.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            return gameObject2.transform;
        }

        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                bool flag = projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks;
                if (flag)
                {
                    bool flag2 = projectile.Owner != null;
                    if (flag2)
                    {
                        bool flag3 = projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable;
                        if (flag3)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }

        private IEnumerator DoCactus(PlayerController player, Gun gun)
        {
            
            Gun cac = (Gun)PickupObjectDatabase.GetById(124);
            for(int f = 0; f < 3; f++)
            {
                AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", base.gameObject);
                for (int i = 0; i < 6; i++)
                {

                    GameObject gameObject = SpawnManager.SpawnProjectile(cac.DefaultModule.projectiles[0].gameObject, gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 60) + startingRotation), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag = component != null;
                    if (flag)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        if (UnityEngine.Random.Range(1, 11) == 1)
                        {
                            player.DoPostProcessProjectile(component);
                        }

                    }
                }
                if (startingRotation != 180)
                {
                    startingRotation += 20;
                }
                yield return new WaitForSeconds(.5f);
            }

        }

        private void SpawnNavi(PlayerController player, Gun gun)
        {
            if (Navi != null) Navi.EraseFromExistence(true);
            RoomHandler room = player.CurrentRoom;
            CellData cell = room.GetNearestCellToPosition(player.CenterPosition);
            Vector2 vector = cell.position.ToCenterVector2();
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c182a5cb704d460d9d099a47af49c913");
            AIActor boy = AIActor.Spawn(orLoadByGuid, cell.position, room, true, AIActor.AwakenAnimationType.Awaken);
            boy.IgnoreForRoomClear = true;
            boy.HitByEnemyBullets = true;
            boy.IsNormalEnemy = false;
            boy.CanTargetPlayers = false;
            boy.IsWorthShootingAt = true;

            boy.isPassable = true;
            boy.AssignedCurrencyToDrop = 0;
            boy.PreventAutoKillOnBossDeath = true;
            boy.CompanionOwner = player;
            boy.gameObject.GetOrAddComponent<KillOnRoomClear>();
            boy.RegisterOverrideColor(ExtendedColours.MikuBlue, "IS THAT HATSUNE MIKU?!?!?!");
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(boy.specRigidbody, null, false);

            CompanionController companionController = boy.gameObject.GetOrAddComponent<CompanionController>();
            companionController.CanInterceptBullets = true;
            companionController.Initialize(player);
            companionController.aiActor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(companionController.aiActor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));

            Navi = boy;
            owner = player;
        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody)
            {
                if (otherRigidbody == owner)
                {
                    PhysicsEngine.SkipCollision = true;
                }

            }
        }

        private PlayerController owner;
        private AIActor Navi = null;
        private GameObject m_shield;
        private IEnumerator BubbleMeScotty(PlayerController player, Gun gun)
        {

            Vector3 pointInSpace = player.specRigidbody.UnitCenter;

            Gun owner = PickupObjectDatabase.GetById(380) as Gun;
            GameObject gameObject1 = owner.ObjectToInstantiateOnReload.gameObject;
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject1, pointInSpace, Quaternion.identity);
            m_shield = gameObject2;
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(pointInSpace, tk2dBaseSprite.Anchor.MiddleCenter);
            gameObject2.transform.localScale = new Vector3(1.8f, 1.8f, 1);

            MajorBreakable breakable = m_shield.GetOrAddComponent<MajorBreakable>();
            
            breakable.MaxHitPoints = 5f;
            BreakableShieldController controller = m_shield.GetOrAddComponent<BreakableShieldController>();
            controller.maxDuration = gun.reloadTime;

            while (m_shield != null)
            {

                Vector3 pointInSpace2 = player.specRigidbody.UnitCenter + new Vector2(0, .25f);
                m_shield.transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().transform.position = pointInSpace2;
                m_shield.GetComponent<SpeculativeRigidbody>().Reinitialize();

                yield return null;
            }
            m_shield = null;

        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            GetEnemyType(arg2.aiActor);
            StartCoroutine(playchime());
        }

        private IEnumerator playchime()
        {
            
            AkSoundEngine.PostEvent("Play_USB_fire_002", base.gameObject);
            yield return new WaitForSeconds(.5f);
            AkSoundEngine.PostEvent("Play_USB_fire_001", base.gameObject);
            
        }

        private void GetEnemyType(AIActor hitenemy)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            CoinBloop m_coinbloop = player.GetComponentInChildren<CoinBloop>();
            float backToThePastSamuraiJack = m_coinbloop.bloopWait;
            m_coinbloop.bloopWait = 1.2f;
            gundownloadedState = -10;
            
            foreach(string id in Basic)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BASIC;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Auto)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.FULLAUTO;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in split)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SPLIT;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in devo)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.DEVO;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in laser)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.LASER;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Spooky)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SPOOKY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Shotguns)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SHOTGUN;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Sniper)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SNIPER;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Magic)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MAGIC;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Radial)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.RADIAL;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Melee)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MELEE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Explosive)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.EXPLOSIVE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Boss)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BOSS;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Armored)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.ARMORED;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Cube)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.CUBE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in poison)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.POISON;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Shroom)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SHROOM;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Burst)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BURST;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Shark)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SHARK;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Shark)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SHARK;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in BouncyMelee)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BOUNCYMELEE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in HeavyMelee)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.HEAVYMELEE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Maiden)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MAIDEN;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Mimic)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MIMIC;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Fire)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.FIRE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in poisonShotty)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.POISONSHOTTY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Fairy)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.FAIRY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    SpawnNavi(Player,this.gun);
                    break;
                }

            }
            foreach (string id in Key)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.KEY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Cactus)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.CACTUS;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in CactusShotty)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.CACTUSSHOTTY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Astral)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.ASTRAL;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Hungry)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.HUNGRY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Fish)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.FISH;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in REALLYBIG)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.REALLYBIG;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in vet)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.VET;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in vetShotty)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.VETSHOTTY;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Chain)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.CHAIN;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Bubble)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BUBBLE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Eye)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.EYE;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            foreach (string id in Electric)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.ELECTRIC;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }


            foreach (string id in Null)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.NULL;
                    player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                    m_coinbloop.bloopWait = backToThePastSamuraiJack;
                    break;
                }

            }
            if (gundownloadedState == -10) // Undefinded or Modded
            {
                gundownloadedState = (int)EnemyType.NULL;
                player.BloopItemAboveHead(overheads[gundownloadedState].GetComponent<tk2dSprite>(), "");
                m_coinbloop.bloopWait = backToThePastSamuraiJack;
                ETGModConsole.Log(hitenemy.EnemyGuid);
                ETGModConsole.Log(hitenemy.name);
            }
            //gundownloadedState = (int)EnemyType.BUBBLE;
            
        }
        public int gundownloadedState;
        

        public List<string> Basic = new List<string> // Extra bullets
        {
            "01972dee89fc4404a5c408d50007dad5", // bulletkin
            "8bb5578fba374e8aae8e10b754e61d62", // cardinal
            
            "1a78cfb776f54641b832e92c44021cf2", // ashen bk
            "5f3abc2d561b4b9c9e72b879c6f10c7e", // fallen
            "844657ad68894a4facb1b8e1aef1abf9", // hooded 
            "906d71ccc1934c02a6f4ff2e9c07c9ec", // office kin man
            "9eba44a0ea6c4ea386ff02286dd0e6bd", // office kin woman
            "6f818f482a5c47fd8f38cce101f6566c", // pirate
            "2feb50a6a40f4f50982e89fd276f6f15", // bullat
            "57255ed50ee24794b7aac1ac3cfb8a95", // gun cultist
            "226fd90be3a64958a5b13cb0a4f43e97", // musket
            "80ab6cd15bfc46668a8844b2975c6c26", // Chameleon
            "f38686671d524feda75261e469f30e0b", // ammoconda ball
            "78a8ee40dff2477e9c2134f6990ef297", // flayer bell
            "4538456236f64ea79f483784370bc62f", // fusebot
            "be0683affb0e41bbb699cb7125fdded6", // mouser
            "a9cc6a4e9b3d46ea871e70a03c9f77d4", // mar pst Imp
            "556e9f2a10f9411cb9dbfd61e0e0f1e1", // con pst soldier
            "12a054b8a6e549dcac58a82b89e319e5", // rbt pst terminator
        };

        public List<string> Electric = new List<string>
        {
            "2b6854c0849b4b8fb98eb15519d7db1c", // bullet mech
            "d4f4405e0ff34ab483966fd177f2ece3", // grey cylinder
            "534f1159e7cf4f6aa00aeea92459065e", // red cylinder

        };
        public List<string> Eye = new List<string>
        {
            "7b0b1b6d9ce7405b86b75ce648025dd6", // beadie
        };
        public List<string> Bubble = new List<string>//                              *
        {
             "6e972cd3b11e4b429b888b488e308551", // Gunzookie
             "8a9e9bedac014a829a48735da6daf3da", // Gunzockie
        };

        public List<string> Chain = new List<string>//                                 *
        {
           "b1770e0f1c744d9d887cc16122882b4f", // executioner
           "463d16121f884984abe759de38418e48", // chaingunner
        };

        public List<string> vetShotty = new List<string>// 
        {
            "2752019b770f473193b08b4005dc781f", // vet shotty                          *
        };
        public List<string> vet = new List<string>// 
        {
            "70216cae6c1346309d86d4a0b4603045", // veterin                        * 
        };
        

        public List<string> REALLYBIG = new List<string>//                               *
        {
            "c4cf0620f71c4678bb8d77929fd4feff", // Titan Man
            "1f290ea06a4c416cabc52d6b3cf47266", // Titan Boss
            "df4e9fedb8764b5a876517431ca67b86", // Titan Girl
        };

        public List<string> Fish = new List<string> //                                 *
        {
            "06f5623a351c4f28bc8c6cda56004b80", // fish kin
            "143be8c9bbb84e3fb3ab98bcd4cf5e5b", // fish kin 2 Electric boogaloo
        };

        public List<string> Hungry = new List<string>//                               *
        {
            "475c20c1fd474dfbad54954e7cba29c1", // Tarnisher
        };
        public List<string> Astral = new List<string> //                              *
        {
            "45192ff6d6cb43ed8f1a874ab6bef316", // Misfire Beast
            "98ca70157c364750a60f5e0084f9d3e2", // Phaser Spider
            "3e98ccecf7334ff2800188c417e67c15", // Kilithid  (squidgames)
            "db97e486ef02425280129e1e27c33118", // shadow agunim
        };


        public List<string> CactusShotty = new List<string>//                        *
        {
           "ddf12a4881eb43cfba04f36dd6377abb", // western shotty
        };

        public List<string> Cactus = new List<string>//                        *
        {
           "3b0bd258b4c9432db3339665cc61c356", // western cactus
           "5861e5a077244905a8c25c2b7b4d6ebb", // west
        };

        public List<string> Key = new List<string>//                           *
        {
            "699cd24270af4cd183d671090d8323a1", // key
        };

        public List<string> Fairy = new List<string>//                          *            
        {
            "c182a5cb704d460d9d099a47af49c913", // fairy
        };

        public List<string> Mimic = new List<string> //                           *
        {
            "6450d20137994881aff0ddd13e3d40c8", // Black
            "d8fd592b184b4ac9a3be217bc70912a2", // Red
            "abfb454340294a0992f4173d6e5898a8", // Green
            "d8d651e3484f471ba8a2daa4bf535ce6", // blue
            "2ebf8ef6728648089babb507dec4edb7", // br_mimic
            "479556d05c7c44f3b6abb3b2067fc778", // wall
            "ac9d345575444c9a8d11b799e8719be0", // RatChest
            "796a7ed4ad804984859088fc91672c7f", // Pedistal
        };

        public List<string> Shark = new List<string>//                             *
        {
            "72d2f44431da43b8a3bae7d8a114a46d", // shark
            "b70cbd875fea498aa7fd14b970248920", // big shark
        };

        public List<string> Burst = new List<string>//           *
        {
            "ed37fa13e0fa4fcf8239643957c51293", // gigi
            "4b21a913e8c54056bc05cafecf9da880", // gigi parrot
        };

        public List<string> Shroom = new List<string>//           *
        {
            "f905765488874846b7ff257ff81d6d0c", //fungun
            "eed5addcc15148179f300cc0d9ee7f94", //spogre
        };

        public List<string> poison = new List<string>//        *
        {
            "e61cab252cfb435db9172adc96ded75f",
            "fe3fe59d867347839824d5d9ae87f244", // pois boys
            "b8103805af174924b578c98e95313074",
            "d4a9836f8ab14f3fadd0f597438b1f1f", // mutant
            "f020570a42164e2699dcf57cac8a495c", // K-bullet mutant
            "ff4f54ce606e4604bf8d467c1279be3e", // KaliBullet
        };

        public List<string> poisonShotty = new List<string> //            *
        {
            "7f665bd7151347e298e4d366f8818284", // mutant shotty
        };

        public List<string> Fire = new List<string>//                    *
        {
            "37de0df92697431baa47894a075ba7e9", // candle kin
            "9d50684ce2c044e880878e86dbada919", // coaler
            "eeb33c3a5a8e4eaaaaf39a743e8767bc", // candle guy
            "ffdc8680bdaa487f8f31995539f74265", // muzzlewhisp
            "d8a445ea4d944cc1b55a40f22821ae69", // muzzleflare
        };

        public List<string> Cube = new List<string>//           *
        {
            "33b212b856b74ff09252bf4f2e8b8c57", // Lead Cube
            "3f2026dc3712490289c4658a2ba4a24b", // flesh cube
            "ba928393c8ed47819c2c5f593100a5bc", // Metal cube guy ????
            "f155fd2759764f4a9217db29dd21b7eb", // mountain cube
            "864ea5a6a9324efc95a0dd2407f42810", // Cubulon
            "0b547ac6b6fc4d68876a241a88f5ca6a", // lead bulon
        };

        public List<string> Armored = new List<string>//             *
        {
            "9b4fb8a2a60a457f90dcf285d34143ac", // gat 
            "3cadf10c489b461f9fb8814abc1a09c1", // minelet
            "39e6f47a16ab4c86bec4b12984aece4c", // armor
            "19b420dec96d4e9ea4aebc3398c0ba7a", // Bombshee
            "bb73eeeb9e584fbeaf35877ec176de28", // Blockner
            "edc61b105ddd4ce18302b82efdc47178", // Blockner Rematch

        };

        public List<string> Null = new List<string>//            *
        {
            "a446c626b56d4166915a4e29869737fd", // chancekin
            "1bc2a07ef87741be90c37096910843ab", // Chancebulon
        };

        public List<string> Auto = new List<string> //*      *
        {
           "df7fb62405dc4697b7721862c7b6b3cd", // tread kin -- auto
           "db35531e66ce41cbb81d507a34366dfe", // ak kin -- auto
           "88b6b6a93d4b4234a67844ef4728382c", // bandana kin red -- auto
           "47bdfec22e8e4568a619130a267eab5b", // Treadnaught kins
        };
        public List<string> split = new List<string> //*      *
        {
           "e5cffcfabfae489da61062ea20539887", // shroomer -- special
        };
        public List<string> devo = new List<string> //*                                             *
        {
            "05891b158cd542b1a5f3df30fb67a7ff", // arrowhead - devolver rounds
        };
        public List<string> Spooky = new List<string> //*                                                 *
        {
            "4db03291a12144d69fe940d5a01de376", // hollow -- ghost
            "56f5a0f2c1fc4bc78875aea617ee31ac", // spectre
            "336190e29e8a4f75ab7486595b700d4a", // skullet
            "95ec774b5a75467a9ab05fa230c0c143", // skullmet
            "5288e86d20184fa69c91ceb642d31474", // gunmy
            "e21ac9492110493baef6df02a2682a0d", // gunmey_spent 
            "af84951206324e349e1f13f9b7b60c1a", // skusket
            "1cec0cdf383e42b19920787798353e46", // black skusk
            "c2f902b7cbe745efb3db4399927eab34", // skusk head 
            "21dd14e5ca2a4a388adab5b11b69a1e1", // shelleton
            "d5a7b95774cd41f080e517bea07bf495", // revol
            "88f037c3f93b4362a040a87b30770407", // gunreap -- ghost
            "7ec3e8146f634c559a7d58b19191cd43", // spiralatt
            "216fd3dfb9da439d9bd7ba53e1c76462", // necronomicon
            "cf27dd464a504a428d87a8b2560ad40a", // Stoner
        };
        public List<string> laser = new List<string> //*                                                  *
        {
            "ac986dabc5a24adab11d48a4bccf4cb1", // Det -- laser bullet mod
            "48d74b9c65f44b888a94f9e093554977", // X det
            "c5a0fd2774b64287bf11127ca59dd8b4", // diag x det
            "b67ffe82c66742d1985e5888fd8e6a03", // vert det
            "d9632631a18849539333a92332895ebd", // diag det
            "1898f6fe1ee0408e886aaf05c23cc216", // hor x det
            "abd816b0bcbf4035b95837ca931169df", // vert x det
            "07d06d2b23cc48fe9f95454c839cb361", // hor x det -- laser bullet mod
        };
        public List<string> Shotguns = new List<string> // shotgun of bullets                                         *
        {
            "128db2f0781141bcb505d8f00f9e4d47", // red shotty
            "b54d89f9e802455cbb2b8a96a31e8259", // blue shotty
            
            "1bd8e49f93614e76b140077ff2e33f2b", // ash shotty
            "044a9f39712f456597b9762893fbc19c", // shotgrub
            
            "86dfc13486ee4f559189de53cfb84107", // pirate shotty
            "e861e59012954ab2b9b6977da85cb83c", // western snake
            "2d4f8b5404614e7d8b235006acde427a", // shotgat
            "022d7c822bc146b58fe3b0287568aaa2", // blizz
        };
        public List<string> Sniper = new List<string> // sniper bullets                                          *
        {
            "31a3ea0c54a745e182e22ea54844a82d", // sniper
            "c5b11bfc065d417b9c4d03a5e385fe2c", // pro
        };
        public List<string> Magic = new List<string> // Witch Bullets                                          *
        {
            "206405acad4d4c33aac6717d184dc8d4", // ap gunjurer
            "c4fba8def15e47b297865b18e36cbef8", // gunjurer
            "9b2cf2949a894599917d4d391a0b7394", // high gunjurer
            "56fb939a434140308b8f257f0f447829", // lore
            "c0ff3744760c4a2eb0bb52ac162056e6", // Booklet
            "6f22935656c54ccfb89fca30ad663a64", // blue booklet
            "a400523e535f41ac80a43ff6b06dc0bf", // green booklet
            "78e0951b097b46d89356f004dda27c42", // tablet
            "cf2b7021eac44e3f95af07db9a7c442c", // gunsinger
            "c50a862d19fc4d30baeba54795e8cb93", // aged singer
            "b1540990a4f1480bbcb3bea70d67f60d", // ammomancer
            "8b4a938cdbc64e64822e841e482ba3d2", // jammomancer
            "ba657723b2904aa79f9e51bce7d23872", // jammerlengo
            "43426a2e39584871b287ac31df04b544", // wizbang
            "76bc43539fc24648bff4568c75c686d1", // chicken :)
        };
        public List<string> Radial = new List<string> //Ring of bullets                                      *
        {
            "116d09c26e624bca8cca09fc69c714b3", // poo
            "1a4872dafdb34fd29fe8ac90bd2cea67", // king bullat
            "981d358ffc69419bac918ca1bdf0c7f7", // gargoyle
            "98ea2fe181ab4323ab6e9981955a9bca", // shambling rounds
            "3f6d6b0c4a7c4690807435c7b37c35a5", // agonizer
            
            "37340393f97f41b2822bc02d14654172", // creech
        };
        public List<string> Maiden = new List<string>//                                              *
        {
            "cd4a4b7f612a4ba9a720b9f97c52f38c", // Maiden
            "9215d1a221904c7386b481a171e52859" // Fridge Maiden
        };

        public List<string> Melee = new List<string> // swipe                                             *
        {
            "0239c0680f9f467dbe5c4aab7dd1eca6", // blobulon
            "042edb1dfb614dc385d5ad1b010f2ee3", // loid
            "42be66373a3d4d89b91a35c9ff8adfec", // lin
            "ccf6d241dad64d989cbcaca2a8477f01", // T-bulon
            "062b9b64371e46e195de17b6f10e47c8", // bloodbulon
            "249db525a9464e5282d02162c88e0357", // spent
            "b5e699a0abb94666bda567ab23bd91c4", // king toadie
            "d4dd2b2bbda64cc9bcec534b4e920518", // revenge king todie
            "02a14dec58ab45fb8aacde7aacd25b01", // old todie
        };
        public List<string> HeavyMelee = new List<string>//                                           *
        {
           "22fc2c2c45fb47cf9fb5f7b043a70122", // Grip Master
           "78eca975263d4482a4bfa4c07b32e252", // Dragun Knife
           "2e6223e42e574775b56c6349921f42cb", // ADV Dragun Knife
           "ec8ea75b557d4e7b8ceeaacdf6f8238c", // gunnut
           "383175a55879441d90933b5c4e60cf6f", // spectral gunnut
        };
        public List<string> BouncyMelee = new List<string>//                                           *
        {
            "6b7ef9e5d05b4f96b04f05ef4a0d1b18", // rubber
            "98fdf153a4dd4d51bf0bafe43f3c77ff", // tazie
        };

        public List<string> Explosive = new List<string> // boom                                         *
        {
            "b4666cb6ef4f4b038ba8924fd8adf38f", // grenat
            "4d37ce3d666b4ddda8039929225b7ede", // grenade kin
            "c0260c286c8d4538a697c5bf24976ccf", // nitra
            "5f15093e6f684f4fb09d3e7e697216b4", // m80
            "566ecca5f3b04945ac6ce1f26dedbf4f", // flayer claymore
            "05cb719e0178478685dc610f8b3e8bfc", // brolett 
            "39de9bd6a863451a97906d949c103538", // tzar Bomba
        };
        public List<string> Boss = new List<string> // Auto Aim bullets                                        *
        {
            "ea40fcc863d34b0088f490f4e57f8913",
            "c00390483f394a849c36143eb878998f",
            "ec6b674e0acd4553b47ee94493d66422",
            "ffca09398635467da3b1f4a54bcfda80", 
            "1b5810fafbec445d89921a4efb4e42b7", 
            "4b992de5b4274168a8878ef9bf7ea36b", 
            "c367f00240a64d5d9f3c26484dc35833", 
            "da797878d215453abba824ff902e21b4", 
            "5729c8b5ffa7415bb3d01205663a33ef", 
            "fa76c8cfdf1c4a88b55173666b4bc7fb",
            "8b0dd96e2fe74ec7bebc1bc689c0008a", 
            "5e0af7f7d9de4755a68d2fd3bbc15df4", 
            "9189f46c47564ed588b9108965f975c9", 
            "6868795625bd46f3ae3e4377adce288b",
            "4d164ba3f62648809a4a82c90fc22cae",
            "6c43fddfd401456c916089fdd1c99b1c",
            "3f11bbbc439c4086a180eb0fb9990cb4",
            "f3b04a067a65492f8b279130323b41f0",
            "41ee1c8538e8474a82a74c4aff99c712",
            "465da2bb086a4a88a803f79fe3a27677",
            "05b8afe0b6cc4fffa9dc6036fa24c8ec", 
            "cd88c3ce60c442e9aa5b3904d31652bc",
            "68a238ed6a82467ea85474c595c49c6e", 
            "7c5d5f09911e49b78ae644d2b50ff3bf", 
        };

        public static BlinkPassiveItem m_BlinkPassive = PickupObjectDatabase.GetById(436).GetComponent<BlinkPassiveItem>();
        public GameObject BlinkpoofVfx = m_BlinkPassive.BlinkpoofVfx;
        
        
    }
}

