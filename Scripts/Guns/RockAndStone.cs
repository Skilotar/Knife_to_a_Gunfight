using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class RockAndStone : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Dwarves Friend", "RockAndStone");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:dwarves_friend", "ski:dwarves_friend");
            gun.gameObject.AddComponent<RockAndStone>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("ROCK AND STONE!!");
            gun.SetLongDescription("All dwarves know you can never mine too deep. The depths are your home.\n\n" +
                "Increases damage the lower in the gungeon you are.\n" +
                "Small Chance to critical reload." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "RockAndStone_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 5);
            gun.SetAnimationFPS(gun.reloadAnimation, 2);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 6);
            gun.SetAnimationFPS(gun.introAnimation, 10);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = .8f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.SetBaseMaxAmmo(200);
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 1f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= .6f;
            projectile.PenetratesInternalWalls = true;
            
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 20;
            projectileStates proj = projectile.gameObject.GetOrAddComponent<projectileStates>();
            proj.isPickStarter = true;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.SHOTGUN;
            gun.PreventOutlines = true;
            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }
        System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", base.gameObject);
            DepthCalc();
            if (projectile.gameObject.GetOrAddComponent<projectileStates>().isPickStarter == true) 
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;

                float angle = projectile.transform.eulerAngles.magnitude;


                // ETGModConsole.Log(angle);
                int vary = -16;
                for (int i = 0; i < 9; i++)
                {
                    
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[550]).DefaultModule.projectiles[0];
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : angle + vary), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag = component != null;
                    if (flag)
                    {

                        component.Owner = this.gun.CurrentOwner;
                        component.Shooter = this.gun.CurrentOwner.specRigidbody;
                        component.baseData.damage = 1.2f * DepthMult ;
                        component.baseData.range = 10f;
                        component.baseData.speed *= (float)(.6 - .004 * Math.Abs(vary));
                        component.PenetratesInternalWalls = true;
                        PierceProjModifier stabby = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                        stabby.penetration = 20;
                        
                        if(UnityEngine.Random.Range(1,11) == 1)
                        {
                            player.DoPostProcessProjectile(component);
                        }
                      
                    }
                    vary++;
                    vary++;
                    vary++;
                    vary++;
                }
                for (int i = 0; i < 5; i++)
                {
                    
                    Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.gun.barrelOffset.transform.position, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : angle), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag = component != null;
                    if (flag)
                    {

                        component.Owner = this.gun.CurrentOwner;
                        component.Shooter = this.gun.CurrentOwner.specRigidbody;
                        component.baseData.damage = 1.2f * DepthMult;
                        component.baseData.range = 10f;
                        component.baseData.speed *= (float)(.6 - .06 * i);
                        component.PenetratesInternalWalls = true;
                        PierceProjModifier stabby = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                        stabby.penetration = 20;
                        component.HasDefaultTint = true;

                        if (UnityEngine.Random.Range(1, 11) == 1)
                        {
                            player.DoPostProcessProjectile(component);
                        }

                        
                    }
                }
            }
            
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

        public AIActor Steve = null;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

                TalkDoerLite talk = player.gameObject.GetOrAddComponent<TalkDoerLite>();
                int DoCritDrink = UnityEngine.Random.Range(1, 31);

                if (player.IsInCombat == false)
                {

                    if(DoCritDrink == 1)
                    {
                        string quote = Getcheers();
                        if (quote == "") quote = "Cheers!";
                        talk.ShowText(player.transform.position + new Vector3(.8f, 1.6f, 0), player.transform, gun.reloadTime, quote);
                    }
                    else
                    {
                        string quote = Getquote();
                        if (quote == "") quote = "Rock and Stone!";
                        talk.ShowText(player.transform.position + new Vector3(.8f, 1.6f, 0), player.transform, gun.reloadTime, quote);
                    }


                }
                else
                {

                    if (player.PlayerHasActiveSynergy("BeastMaster"))
                    {
                        if (Steve == null )
                        {
                            if (player.CurrentRoom != null)
                            {
                                float distance = 5f;
                                AIActor actor = player.CurrentRoom.GetNearestEnemy(player.specRigidbody.UnitCenter, out distance, false, true);
                                Steve = actor;
                                actor.CompanionOwner = player;
                                actor.ApplyEffect(StaticStatusEffects.charmingRoundsEffect);
                            }
                        }
                    }

                }

                if (DoCritDrink == 1)
                {
                    
                    tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("RockAndStone_critical_fire");
                    gun.spriteAnimator.Play(clip,0,8/gun.reloadTime,true);
                    StartCoroutine(DrinkingOnTheJob());
                }
                
                base.OnReloadPressed(player, gun, bSOMETHING);
            }

        }

        private IEnumerator DrinkingOnTheJob()
        {
            
            gun.carryPixelOffset = new IntVector2(-6, -5);
            yield return new WaitForSeconds(gun.reloadTime*.4f);
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if(player.healthHaver.GetCurrentHealthPercentage() == 1)//full health
            {
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
                player.healthHaver.Armor += 1;
                
            }
            else
            {
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
                (this.gun.CurrentOwner as PlayerController).healthHaver.ApplyHealing(.5f);
            }
           
            yield return new WaitForSeconds(gun.reloadTime*.6f);
            gun.carryPixelOffset = new IntVector2(0, 0);

        }
        private string Getquote()
        {
            int rng = UnityEngine.Random.Range(0, Quotelist.Count - 1);
            
            return Quotelist[rng];
        }

        private string Getcheers()
        {
            int rng = UnityEngine.Random.Range(0, Cheerslist.Count -1);
            return Cheerslist[rng];
        }

        public List<string> Quotelist = new List<string>
        {
            "Rock On!",
            "Rock and Stone... YEAHHH!",
            "Rock and Stone forever!",
            "Rock And STONE!!",
            "For Rock and Stone!!",
            "Rock and roll!",
            "Rock and roll and Stone!",
            "Rock Solid!",
            "We fight for Rock and Stone!",
            "Did I hear a rock and stone?!",
            "Rock and Stone to the Bone!",
            "For Karl!!",
            "Last one down pays for drinks!",
            "This Rocks!",
            "Salute!",
            "Rock me like a Stone!"

        };
        public List<string> Cheerslist = new List<string>
        {
            "To The Fallen!",
            "Skal!",
            "To The Empires Of Old!",
            "To Danger",
            "Fortune And Glory!",
            "Long Live the Dwarves!",
            "For Karl!",
            "Cheers!",
            "To A Long Life!",
            "To Our continued Survival!",
            "To those we lost...",
            "Rock And Stone!",
            "To mates, To Darkness, And Making it back alive!",
            "To the pasts, And a new Future!",
            "To Kyle..",
            "To our comrades.. May they rest well!",
            "For Rock And Stone!"

        };

        public float DepthMult; 
        public void DepthCalc()
        {
            bool hit = false;
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)//0
            {
                DepthMult = 1f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)//.5
            {
                DepthMult = 1.1f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON)//1
            {
                DepthMult = 1.1f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.BELLYGEON)//1.5
            {
                DepthMult = 1.2f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)//2
            {
                DepthMult = 1.2f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)//2.5
            {
                DepthMult = 1.3f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON)//3
            {
                DepthMult = 1.3f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)//3.5
            {
                DepthMult = 1.4f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)//4
            {
                DepthMult = 1.5f;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)//past
            {
                DepthMult = 1.5f;
                hit = true;
            }
            if (hit == false) // floor not found Default to 1.2 X
            {
                DepthMult = 1.2f;
            }

        }

    }
}