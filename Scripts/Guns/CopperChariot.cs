using System;
using System.Collections.Generic;
using Gungeon;
using UnityEngine;
using ItemAPI;
using Dungeonator;


namespace Knives
{

    class CopperChariot :AdvancedGunBehaviour
    {  
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Copper Chariot", "chariot");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:copper_chariot", "ski:copper_chariot");
            gun.gameObject.AddComponent<CopperChariot>();

            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Blinding Speed!");
            gun.SetLongDescription("The ghost of a gun nut gaurds the weilder of this blade. While the blade itself is too weak to be used your new astral companion will be happy to provide assistance.\n\n" +
                "__________________________________\n\n" +
                "Copper Chariot's bullets cant hurt you but the initial swing can, even then i have made it so you will not sustain damage from the hit. Reloading will draw him back and firing your first shot while in combat will push him out.");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "chariot_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.

            GunExt.AddProjectileModuleFrom(gun, "AK-47", true, true);

            gun.muzzleFlashEffects = null;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.gunClass = GunClass.SILLY;
            gun.reloadTime = .5f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 40;
            gun.InfiniteAmmo = true;
            gun.SetBaseMaxAmmo(1000);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "hora hora hora hora!";
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
            projectile.baseData.damage = 1f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range = .00000001f;
            projectile.baseData.force = 0f;
            
            projectile.pierceMinorBreakables = true;

            projectile.transform.parent = gun.barrelOffset;


            ETGMod.Databases.Items.Add(gun, null, "ANY");
            UnityEngine.Object.Destroy(projectile.GetAnySprite());
        }

        

        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                this.RemoveNutOnGunSwitchOut(newGun);
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }


        private void RemoveNutOnGunSwitchOut(Gun current)
        {
            try
            {
                bool flag2 = (current != this.gun && this.canSpawn == 1); // to remove the nut on weapon switch
                if (flag2)
                {
                    this.BuddyKill();
                }
            }
            catch (Exception e)
            {
                Tools.Print("Copper RemoveNutOnGunSwitchOut", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = false;

            if (combat == 1 && canSpawn == 0)
            {
                canSpawn = 1;
                knightPlacer(player);
                player.GunChanged += this.OnGunChanged;
            }
            if (nut.CenterPosition != null && HasSynergy)
            {
                Gun gunnut = PickupObjectDatabase.GetById(380) as Gun;
                Gun currentGun = nut.CurrentGun;
                GameObject gameObject = gunnut.ObjectToInstantiateOnReload.gameObject;
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, nut.sprite.WorldCenter, Quaternion.identity);
                SingleSpawnableGunPlacedObject @interface = gameObject2.GetInterface<SingleSpawnableGunPlacedObject>();
                BreakableShieldController component = gameObject2.GetComponent<BreakableShieldController>();
                bool flag3 = gameObject2;
                if (flag3)
                {

                    component.maxDuration = 1.7f;

                    @interface.Initialize(currentGun);
                    component.Initialize(currentGun);


                }
            }
        }
        

        private bool HasSynergy;
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
                combatchecker();
            }
            if(this.gun.CurrentOwner != true)
            {
                BuddyKill();
            }
           
            if(nut.HasDamagedPlayer)
            {
               
                   this.gun.CurrentOwner.healthHaver.ApplyHealing(.5f);
                
                nut.HasDamagedPlayer = false;
            }
            PlayerController player = (PlayerController)gun.CurrentOwner;
            bool flag2 = player.HasPickupID(545);
            if (flag2)
            {
                HasSynergy = true;
                nut.MovementSpeed = 6.4f;
            }
            else
            {
                HasSynergy = false;
                nut.MovementSpeed = 6.1f;
            }
           

            safebullets();
            safebullets();
        }
        public int canSpawn = 0;
        public int combat = 0;
         
        public void safebullets()
        {
            foreach (var projectile in GetbuddyBullets())
            {
                projectile.collidesWithPlayer = false;
                projectile.UpdateCollisionMask();
                projectile.HasDefaultTint = true;
                projectile.DefaultTintColor = UnityEngine.Color.yellow;
            }
        }
        public void BuddyKill()
        {
            nut.healthHaver.ApplyDamage(200f, Vector2.zero, "killed", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
            canSpawn = 0;
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            player.GunChanged -= this.OnGunChanged;
        }
        public void combatchecker()
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            if (player.IsInCombat)
            {
                combat = 1;
            }
            else
            {
                
                combat = 0;
                canSpawn = 0;
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                
                BuddyKill();
                
            }
        }
        AIActor nut;
        
        public void knightPlacer(PlayerController owner)
        {
            

            IntVector2 aim = Vector2Extensions.ToIntVector2(owner.unadjustedAimPoint, VectorConversions.Round);
            RoomHandler room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(aim);
            if (room != null && room == owner.CurrentRoom && owner.IsInCombat)
            {
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("ec8ea75b557d4e7b8ceeaacdf6f8238c");
               
               
                AIActor aiActor = AIActor.Spawn(orLoadByGuid.aiActor, owner.CenterPosition, room, true, AIActor.AwakenAnimationType.Spawn, true);
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiActor.specRigidbody, null, false);
                aiActor.CanTargetEnemies = true;
                aiActor.CanTargetPlayers = false;
                aiActor.IsHarmlessEnemy = true;
                aiActor.CanDropCurrency = false;
                aiActor.IgnoreForRoomClear = true;
                aiActor.MovementSpeed = 6.1f;
                aiActor.CompanionOwner = owner;
                aiActor.IsBuffEnemy = true;
                aiActor.isPassable = true;
                
                aiActor.gameObject.AddComponent<KillOnRoomClear>();
                aiActor.reinforceType = AIActor.ReinforceType.Instant;
                aiActor.HandleReinforcementFallIntoRoom(.1f);
                nut = aiActor;

                MindControlEffect mindControl = aiActor.gameObject.GetOrAddComponent<MindControlEffect>();
                mindControl.owner = (this.gun.CurrentOwner as PlayerController);
               
                aiActor.aiShooter.IsReallyBigBoy = true;
                
                aiActor.aiShooter.customShootCooldownPeriod = .25f;
                aiActor.Update();
                
                if (nut.bulletBank != null)
                {
                    AIBulletBank bulletBank = nut.bulletBank;
                    bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(CopperChariot.OnPostProcessProjectile));
                }
                if (nut.aiShooter != null)
                {
                    AIShooter aiShooter = nut.aiShooter;
                    aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(CopperChariot.OnPostProcessProjectile));
                }
            }
            
        }

        public static void OnPostProcessProjectile(Projectile proj)
        {
            try
            {
                proj.baseData.damage = 3f;
                proj.collidesWithPlayer = false;
                proj.UpdateCollisionMask();
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPostProcessProjectile", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public List<Projectile> GetbuddyBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
               
                if (projectile.Owner == nut)
                {
                    list.Add(projectile);
                }
               
            }
            return list;
        }
        
    }
}

