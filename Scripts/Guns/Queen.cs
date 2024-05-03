using System;
using System.Collections;
using Gungeon;
using System.Collections.Generic;
using System.Linq;
using MonoMod;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using Brave.BulletScript;
using Pathfinding;

namespace Knives
{

    public class Queen : AdvancedGunBehaviour
    {
        
        // heh heh DNspy people cant read this..  lol
        // congrats on using my actual source code  ;)
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Killer Queen", "queen");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:killer_queen", "ski:killer_queen");
            gun.gameObject.AddComponent<Queen>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Dynamite With A Laserbeam");
            gun.SetLongDescription("Guaranteed to blow your mind!\n\n" +
                "The Great Queen Elizabeth the LXI, kaliber rest her soul, was a brutal tyrant of the collonies of the new new new great england space provence. \n\n" +
                "She grew so fond of executions that she devised a gun that, with new shrinking technology, could turn a nitra into a projectile to then be used as a execution device. \n\n" +
                "The thought of killing two creatures to satisfy the execution of one was just marvelous to her. \n\n" +
                "Because of this ruthlessness the gun itself was nicknamed the Killer Queen." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "queen_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("dl45", true, true);

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.DefaultModule.cooldownTime = .2f;
            
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.SetBaseMaxAmmo(75);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Tiny nytra", "Knives/Resources/queen_clipfull", "Knives/Resources/queen_clipempty");
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "Killah queen bite za dusto!";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //Setting static values for a custom gun's projectile stats prevents them from scaling with player stats and bullet modifiers (damage, shotspeed, knockback)
            //You have to multiply the value of the original projectile you're using instead so they scale accordingly. For example if the projectile you're using as a base has 10 damage and you want it to be 6 you use this
            //In our case, our projectile has a base damage of 5.5, so we multiply it by 1.1 so it does 10% more damage from the ak-47.
            projectile.baseData.damage *= 1.2f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range = 20;
            projectile.transform.parent = gun.barrelOffset;
            projectile.PenetratesInternalWalls = true;
            projectile.projectileHitHealth = 100;
            

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = false;

            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
            if (instanceForPlayer.IsKeyboardAndMouse() == false)
            {
                // special controls for controller
                nytraPlacerCtrl(player);
                

            }
            else
            {
                // standard controls for keyboard
                nytraPlacer(player);

            }

            AkSoundEngine.PostEvent("Play_BOSS_FuseBomb_Attack_Toys_01", base.gameObject);


        }
        private bool HasReloaded;
        
        protected void Update()
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
            else
            {
               
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

        public void nytraPlacer(PlayerController owner)
        {   
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c0260c286c8d4538a697c5bf24976ccf");
            AIActor aiactor;
            IntVector2 aim = Vector2Extensions.ToIntVector2(owner.unadjustedAimPoint, VectorConversions.Round);
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(aim);
           
            if (room != null && room == owner.CurrentRoom && owner.IsInCombat && owner.unadjustedAimPoint.GetAbsoluteRoom() == owner.CurrentRoom)
            {
                aiactor = AIActor.Spawn(orLoadByGuid.aiActor, owner.unadjustedAimPoint, room, true, AIActor.AwakenAnimationType.Default, true);
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                aiactor.IsHarmlessEnemy = true;
                aiactor.IgnoreForRoomClear = true;
                aiactor.MovementSpeed = 0;
                aiactor.healthHaver.ForceSetCurrentHealth(1);
                aiactor.gameObject.AddComponent<KillOnRoomClear>();
                aiactor.reinforceType = AIActor.ReinforceType.Instant;
                aiactor.HandleReinforcementFallIntoRoom(.1f);
            }
            
        }
        public void nytraPlacerCtrl(PlayerController Owner)
        {
            Vector2 spot = this.gun.barrelOffset.transform.TransformPoint(6, 0, 0);

            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c0260c286c8d4538a697c5bf24976ccf");
            AIActor aiactor;
            IntVector2 aim = Vector2Extensions.ToIntVector2(spot, VectorConversions.Round);
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(aim);

            if (room != null && room == Owner.CurrentRoom && Owner.IsInCombat && Owner.unadjustedAimPoint.GetAbsoluteRoom() == Owner.CurrentRoom)
            {
                aiactor = AIActor.Spawn(orLoadByGuid.aiActor, spot, room, true, AIActor.AwakenAnimationType.Default, true);
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                aiactor.IsHarmlessEnemy = true;
                aiactor.IgnoreForRoomClear = true;
                aiactor.MovementSpeed = 0;
                aiactor.gameObject.AddComponent<KillOnRoomClear>();
                aiactor.reinforceType = AIActor.ReinforceType.Instant;
                aiactor.HandleReinforcementFallIntoRoom(.1f);
                aiactor.healthHaver.ForceSetCurrentHealth(1);
            }

            

        }

    }
}
