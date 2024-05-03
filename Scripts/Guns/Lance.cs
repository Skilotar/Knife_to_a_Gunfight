using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Lance : AdvancedGunBehaviour
    {

    
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Sir lot o lance", "lnc");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:sir_lot_o_lance", "ski:sir_lot_o_lance");
            gun.gameObject.AddComponent<Lance>();
            
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Now With 100% Less Horse");
            gun.SetLongDescription("\nThis is a terribly worn lance made from metal and dragun peices. It seems to have been forged as an attempt to relive adventures of a diffrent time. However, since this is the gungeon, this lance has been equipt with rocket boosters to avoid Kaliber's curse. \n\n" +
                "By far the most effective way to use this weapon is to slightly tilt it toward your adversary whilst you charge them." +
                "\n\n\n - Knife_to_a_Gunfight");
                
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "lnc_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 2);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 5);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("camera_gun", true, true);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .5f, StatModifier.ModifyMethod.ADDITIVE);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.CanReloadNoMatterAmmo = true;
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 5000;
            gun.DefaultModule.cooldownTime = .01f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            
            gun.quality = PickupObject.ItemQuality.C;
            //gun.encounterTrackable.EncounterGuid = "Big Pokey STICK!";
            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, .5f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.gameObject.GetOrAddComponent<GunSpecialStates>().DoesCountBlocks = false;

            //gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            //gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Lance", "Knives/Resources/lnc_clipfull", "Knives/Resources/lnc_clipempty");

            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= .20f;
            projectile.baseData.range = .001f;
            projectile.PlayerKnockbackForce = -7f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.baseData.force = 45;
            projectile.AppliesStun = true;
            projectile.AppliedStunDuration = .3f;
            projectile.StunApplyChance = 1;
            projectile.objectImpactEventName = null;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage *= 1f;
            projectile2.baseData.force *= 0.1f;
            projectile2.baseData.range = 20f;
            projectile2.AdditionalScaleMultiplier = 1.5f;
            projectile2.baseData.speed *= .8f;
            projectile2.AppliesFire = true;
            projectile2.FireApplyChance = 100;

            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            projectile3.baseData.damage = 35f;
            projectile3.baseData.force *= 0.1f;
            projectile3.baseData.range = 20f;
            projectile3.baseData.speed *= .8f;
            projectile3.shouldFlipVertically = true;
            projectile3.shouldRotate = true;
            projectile3.SetProjectileSpriteRight("Poke", 28, 11, false, tk2dBaseSprite.Anchor.MiddleCenter, 30, 11);
            poke = projectile3;
            Lance.BuildPrefab();

            projectile.transform.parent = gun.barrelOffset;

            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            Lance.lnc = gun.PickupObjectId;
        }

        public static Projectile poke;
        public static int lnc;
        
        public string playerName;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            if (player.IsPrimaryPlayer)
            {
                playerName = "primaryplayer";
            }
            else
            {
                playerName = "secondaryplayer";
            }
            base.OnPickedUpByPlayer(player);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            PlayerController owner = this.gun.CurrentOwner as PlayerController;
            Projectile projectile = ((PickupObjectDatabase.GetById(15) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            ProjectileSlashingBehaviour slash = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slash.SlashDimensions = 60;
            slash.SlashRange = 3f;
            slash.playerKnockback = 0;
            slash.SlashDamage = 0;
            slash.SlashVFX.type = VFXPoolType.None;
            slash.DestroyBaseAfterFirstSlash = true;
            slash.soundToPlay = null;
            component.Owner = player;
            component.baseData.damage *= 0;
            component.Shooter = player.specRigidbody;


            bool Hunter = owner.PlayerHasActiveSynergy("Dragun Slayer");
            if (Hunter)
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                float zRotation = BraveMathCollege.Atan2Degrees(vector);
                player.StartCoroutine(this.HandleSwing(player, vector, (.5f * player.stats.GetStatValue(PlayerStats.StatType.Damage)), 2.5f));
            }
            else
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                float zRotation = BraveMathCollege.Atan2Degrees(vector);
                player.StartCoroutine(this.HandleSwing(player, vector, (.3f * player.stats.GetStatValue(PlayerStats.StatType.Damage)), 2.5f));
            }

            
            bool engine = player.PlayerHasActiveSynergy("Upgraded");
            if (engine)
            {

                Projectile projectile1 = ((PickupObjectDatabase.GetById(15) as Gun)).DefaultModule.projectiles[0];
                GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 180), true);
                Projectile component1 = gameObject1.GetComponent<Projectile>();
                ProjectileSlashingBehaviour slash1 = component1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                slash1.SlashDimensions = 20;
                slash1.SlashRange = 4;
                slash1.playerKnockback = -7;
                slash1.SlashDamage = 1;
                slash1.SlashVFX = ((PickupObjectDatabase.GetById(223) as Gun)).muzzleFlashEffects;
                slash1.DestroyBaseAfterFirstSlash = true;
                slash1.soundToPlay = null;
                
                component1.Owner = player;
                component1.baseData.damage *= 1;

                component1.Shooter = player.specRigidbody;

            }

            gun.PreventNormalFireAudio = true;
           
        }
        
        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                {

                    if (user.IsPrimaryPlayer)
                    {
                       
                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "primaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 5);
                        

                    }
                    else
                    {

                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "secondaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 5);
                        
                    }
                }
                SpeculativeRigidbody hitRigidbody2 = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody2 && hitRigidbody2.projectile && hitRigidbody2.projectile.Owner != this.gun.CurrentOwner)
                {
                    PassiveReflectItem.ReflectBullet(hitRigidbody2.projectile, true, this.gun.CurrentOwner, 15f, 1f, 1f, 0f);
                }


                yield return null;
            }
            yield break;
        }

     
        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }

       

        private bool HasReloaded;
        public bool setup = false;
        public bool SynSpawn = false;
        public float HitCooldown = 0;
        public float attackinterval = .1f;
        
        //This block of code allows us to change the reload sounds.
        public override void  Update()
        {
            if (gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;

               
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                    this.gun.ClipShotsRemaining--;
                }
                
                if(setup == false)
                {
                    PassiveItem.IncrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));
                    LanceCleanup clean = player.gameObject.GetOrAddComponent<LanceCleanup>();
                    clean.player = player;
                    this.gun.ClipShotsRemaining--;
                    setup = true;
                }

                if (player.PlayerHasActiveSynergy("200% More Horse"))
                {
                    if (HitCooldown > 0)
                    {
                        HitCooldown -= Time.deltaTime;
                    }
                    if(attackinterval > 0)
                    {
                        attackinterval -= Time.deltaTime;
                    }
                    if (this.gun.IsFiring)
                    {
                        if (SynSpawn == false)
                        {

                            Projectile wallfinder = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, player.sprite.WorldCenter, player, gun, 0, 25, false);
                            wallfinder.sprite.renderer.enabled = false;
                            wallfinder.hitEffects.suppressMidairDeathVfx = true;
                            wallfinder.objectImpactEventName = null;

                            wallfinder.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(wallfinder.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
                            wallfinder.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(wallfinder.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));

                            SynSpawn = true;
                        }
                        if (m_extantKage != null && player.Velocity.magnitude > 7.4f)
                        {
                            if (Vector2.Distance(m_extantKage.transform.position, player.specRigidbody.UnitCenter) < 2f && HitCooldown <= 0)
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
                                player.ForceBlank(3.5f, .8f, false, true, null, true, 10);
                                DoSafeExplosion(player.specRigidbody.UnitCenter);
                                HitCooldown = .6f;
                            }
                            if(attackinterval <= 0)
                            {
                                Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, m_extantKage.transform.position + new Vector3(.75f, .75f), player, this.gun, 0, 1, false);
                                ProjectileSlashingBehaviour stabby = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                                stabby.SlashDamageUsesBaseProjectileDamage = false;
                                stabby.SlashDamage = 3;
                                stabby.SlashDimensions = 20;
                                stabby.SlashRange = 4.5f;
                                stabby.SlashVFX = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
                                stabby.soundToPlay = null;
                                attackinterval = .1f / (player.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
                            }

                        }
                    }
                    else
                    {
                        if (SynSpawn == true)
                        {
                            SynSpawn = false;
                        }
                    }

                    

                    if (m_extantMagicLance != null)
                    {
                        m_extantMagicLance.transform.rotation = Quaternion.Euler(0f, 0f, (gun.CurrentOwner as PlayerController).CurrentGun.CurrentAngle + 180);
                        if (m_extantKage != null)
                        {
                            m_extantMagicLance.transform.position = m_extantKage.transform.position + new Vector3(.75f, .75f, 0);
                            m_extantMagicLance.gameObject.GetComponent<tk2dSprite>().sprite.FlipY = this.gun.sprite.FlipY;
                        }
                    }
                    
                }

                if (CanBeUsed(player))
                {
                    if (!OnHopCooldown && this.gun.IsFiring)
                    {
                        AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_mushroom_charge_01", base.gameObject);
                        StartCoroutine(Hop(player));
                    }
                }

                if (Kill)
                {
                    if (m_extantKage != null) UnityEngine.GameObject.Destroy(m_extantKage);
                    if (m_extantMagicLance != null) UnityEngine.GameObject.Destroy(m_extantMagicLance);
                    Lance.Kill = false;
                }

                if (this.gun.GetComponent<GunSpecialStates>().successfullBlocks > 0 && Stopit == false)
                {

                    Stopit = true;
                    StartCoroutine(CounterThrust(player));
                    
                    
                }
            }
           
        }

        private IEnumerator CounterThrust(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Block_01", base.gameObject);
            yield return new WaitForSeconds(.2f);
            AkSoundEngine.PostEvent("Play_Parry_Success", base.gameObject);
            
            gun.GetComponent<GunSpecialStates>().successfullBlocks = 0;
            gun.spriteAnimator.Play("lnc_critical_fire");

            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, player.CenterPosition, player, this.gun);
            ProjectileSlashingBehaviour stabby = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashRange = 6f;
            stabby.SlashDamageUsesBaseProjectileDamage = false;
            stabby.SlashDamage = 30;
            stabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            stabby.SlashDimensions = 60;
            stabby.SlashVFX.type = VFXPoolType.None;

            MiscToolMethods.SpawnProjAtPosi(poke, player.CenterPosition, player, this.gun);

        }

        public void DoSafeExplosion(Vector3 position)
        {
            ExplosionData defaultSmallExplosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = null;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
        }

        // Token: 0x04000275 RID: 629
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 8f,
            doDestroyProjectiles = false,
            doForce = false,
            debrisForce = 10f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false
        };


        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/dupelnc", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            Lance.storeLance = gameObject;

        }

        public static bool Kill;

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
        }

        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            DungeonData data = GameManager.Instance.Dungeon.data;
            if (m_extantKage != null) UnityEngine.GameObject.Destroy(m_extantKage);
            if (m_extantMagicLance != null) UnityEngine.GameObject.Destroy(m_extantMagicLance);
            
            Vector2 wallpoint = myRigidbody.UnitCenter;
            RoomHandler room = player.CenterPosition.GetAbsoluteRoom();

            IntVector2 spot = (IntVector2)room.GetNearestAvailableCell(wallpoint, new IntVector2(2, 2), new CellTypes?(CellTypes.FLOOR), true);
            CellData cell = room.GetNearestCellToPosition(spot.ToVector2());
            if (cell.isOccludedByTopWall)
            {
                //ETGModConsole.Log("wall adjust");
                spot = spot + new IntVector2(0, 1);
            }
            Vector2 point = spot.ToVector2();

            GameObject shadowclone = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(820) as SpawnObjectPlayerItem).objectToSpawn, point, Quaternion.identity);
            KageBunshinController kageBunshin = shadowclone.GetOrAddComponent<KageBunshinController>();
            
            kageBunshin.UsesRotationInsteadOfInversion = true;
            kageBunshin.RotationAngle = 180;

            kageBunshin.Duration = 99f;
            OverrideInit(player, kageBunshin);

            kageBunshin.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.LowObstacle));
            shadowclone.GetComponentInChildren<Renderer>().material.SetColor("_FlatColor", UnityEngine.Color.yellow);

            ImprovedAfterImage trail = shadowclone.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .1f;
            trail.shadowTimeDelay = .01f;
            trail.dashColor = UnityEngine.Color.yellow;
            trail.spawnShadows = true;


            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Lance.storeLance, shadowclone.transform.position, Quaternion.identity);
            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(shadowclone.transform.position, tk2dBaseSprite.Anchor.LowerLeft);


            m_extantMagicLance = gameObject;
            m_extantKage = shadowclone;
            
        }

        private void OverrideInit(PlayerController user, KageBunshinController kage)
        {
            kage.Owner = user;
            kage.sprite = kage.GetComponentInChildren<tk2dSprite>();
            kage.sprite.usesOverrideMaterial = true;
            if (kage.Duration > 0f)
            {
                UnityEngine.Object.Destroy(kage.gameObject, kage.Duration);
            }
        }

        public bool CanBeUsed(PlayerController user)
        {

            //if not in or near wall
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(user.CenterPosition, VectorConversions.Round));
            CellData cellaim = room.GetNearestCellToPosition(user.CenterPosition);
            CellData cellaimmunis = room.GetNearestCellToPosition(user.CenterPosition - new Vector2(0, 1f));
            if (cellaim.HasPitNeighbor(GameManager.Instance.Dungeon.data) == true && cellaimmunis.HasPitNeighbor(GameManager.Instance.Dungeon.data) == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        bool OnHopCooldown = false;
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if(m_extantKage != null)
            {
                Lance.Kill = true;
            }
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            this.HasReloaded = false;
            if (doingcounter == false)
            {
                StartCoroutine(BlockCounter(player, gun));
            }

        }
        bool doingcounter = false;
        bool Stopit = false;
        private IEnumerator BlockCounter(PlayerController player, Gun gun)
        {
            doingcounter = true;
            Stopit = false;
            player.MovementModifiers += this.NoMotionModifier;
            player.inventory.GunLocked.SetOverride("Lance Counter", true, null);
            gun.CanBeDropped = false;
            gun.carryPixelOffset = new IntVector2(-6, 0);
            yield return new WaitForSeconds(.3f);
            AkSoundEngine.PostEvent("Play_ENM_gunnut_swing_01", base.gameObject);
            while (gun.IsReloading && !Stopit)
            {
                
                gun.gameObject.GetOrAddComponent<GunSpecialStates>().DoesCountBlocks = true;
                
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f, .5f, .0f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.AdditionalScaleMultiplier = .001f;
                component.Owner = this.gun.CurrentOwner;
                component.baseData.damage = 0;
                ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                slashy.SlashRange = 2.3f;
                slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                slashy.SlashDimensions = 60;
                slashy.SlashVFX.type = VFXPoolType.None;
                slashy.soundToPlay = "";
                yield return new WaitForSeconds(.1f);

            }
            gun.gameObject.GetOrAddComponent<GunSpecialStates>().DoesCountBlocks = false;
            player.MovementModifiers -= this.NoMotionModifier;
            gun.CanBeDropped = true;
            gun.carryPixelOffset = new IntVector2(0, 0);
            player.inventory.GunLocked.SetOverride("Lance Counter", false, null);
            gun.GetComponent<GunSpecialStates>().successfullBlocks = 0;
            doingcounter = false;
        }

        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public IEnumerator Hop(PlayerController player)
        {
            player.SetIsFlying(true, "hop");
            OnHopCooldown = true;
            yield return new WaitForSecondsRealtime(.45f);
            player.SetIsFlying(false, "hop");
            yield return new WaitForSecondsRealtime(.6f);
            OnHopCooldown = false;
            yield break;
        }
        
        public GameObject m_extantKage = new GameObject();
        public GameObject m_extantMagicLance = new GameObject();
        public static GameObject storeLance;

    }
}

