using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using Brave;
using Dungeonator;
using ItemAPI;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Knives
{
    class Dullahan_curse : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Dullahan's Curse";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Dullahan_curse";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Dullahan_curse>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Just Dead Cels";
            string longDesc = "Your body can now stay alive through sheer power of hatred.\n\n" +
                "Spawns a head that explodes into poison gas when it dies. Shoot or Active use to die." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 30f);


            //Set the rarity of the item
            item.CanBeDropped = false;
            
            
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.B;
        }


        public AIActor host;
        public bool CamToggle = false;
        public bool modifierController = false;
        public override void  DoEffect(PlayerController player)
        {
            
            if (host == null)
            {
                player.StartCoroutine(ItemBuilder.HandleDuration(this, 10, player, DoActiveEffect));
                //head
                IntVector2 aim = Vector2Extensions.ToIntVector2(player.CenterPosition, VectorConversions.Round);
                RoomHandler room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(aim);
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c2f902b7cbe745efb3db4399927eab34");
                //AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("cf27dd464a504a428d87a8b2560ad40a");

                AIActor aiActor = AIActor.Spawn(orLoadByGuid.aiActor, player.CenterPosition, room, true, AIActor.AwakenAnimationType.Spawn, true);
                //PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiActor.specRigidbody, null, false);
                aiActor.specRigidbody.OnPreRigidbodyCollision += this.Oncollison;
                aiActor.CanTargetEnemies = false;
                aiActor.CanTargetPlayers = false;
                aiActor.CustomPitDeathHandling += this.custompitdeathhandle;
                aiActor.IgnoreForRoomClear = true;
                aiActor.MovementSpeed = 10f;
                aiActor.CompanionOwner = player;
                aiActor.IsWorthShootingAt = true;
                aiActor.HitByEnemyBullets = true;
                
                aiActor.reinforceType = AIActor.ReinforceType.Instant;
                aiActor.HandleReinforcementFallIntoRoom(.1f);
                aiActor.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;

                host = aiActor;
                vangaurd = true;
                MindControlEffect mindControl = aiActor.gameObject.GetOrAddComponent<MindControlEffect>();
                mindControl.owner = player;


                if (CheckForMonger(room) != true)
                {
                    CameraOverrideTarget cam = aiActor.gameObject.GetOrAddComponent<CameraOverrideTarget>();
                    cam.startOverrideOnStart = true;
                }
                if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
                foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (actor.healthHaver.IsDead == false)
                    {
                        actor.OverrideTarget = host.specRigidbody;
                        SetPrivateType<AIActor>(actor, "m_canTargetEnemies", true);
                        if (actor.bulletBank != null)
                        {
                            actor.bulletBank.OnProjectileCreated += this.OnAiTriedToShoot;
                        }
                        if (actor.aiShooter != null)
                        {
                            actor.aiShooter.PostProcessProjectile += this.OnAiTriedToShoot;
                        }

                    }
                }

                player.IsEthereal = true;
                player.SetIsStealthed(true, "cause they just are");
                player.healthHaver.IsVulnerable = false;
                if (modifierController != true)
                {
                    player.MovementModifiers += this.NoMotionModifier;
                    player.IsStationary = true;
                    modifierController = true;
                }
                player.CurrentStoneGunTimer = 9999999f;
                this.LastOwner.stats.RecalculateStats(LastOwner, true);

                CamToggle = true;
              

            }
           
        }

        private void OnAiTriedToShoot(Projectile obj)
        {
            obj.specRigidbody.OnPreRigidbodyCollision += this.EnemyProjectile;
        }

        private void EnemyProjectile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody.aiActor != null)
            {
                if(otherRigidbody.aiActor.EnemyGuid != "c2f902b7cbe745efb3db4399927eab34")
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
            
        }

        private bool CheckForMonger(RoomHandler room)
        {
            if(room != null)
            {
                List<AIActor> boi =  room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                foreach(AIActor actor in boi)
                {
                    if (actor.EnemyGuid == "f3b04a067a65492f8b279130323b41f0")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Oncollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody == this.LastOwner.specRigidbody)
            {
                PhysicsEngine.SkipCollision = true;
            }
        }

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            RoomHandler room = this.LastOwner.CurrentRoom;
            DoSafeExplosion(host.sprite.WorldCenter);
            for (int i = 0; i < 12; i++)
            {
                Projectile poof = MiscToolMethods.SpawnProjAtPosi(CloudInABottle.cloudPoof, host.sprite.WorldCenter, this.LastOwner, 20 * i);
                poof.PoisonApplyChance = 1;
                poof.AppliesPoison = true;
                poof.DefaultTintColor = new Color(0.46f, 0.59f, 0.13f);
                poof.sprite.color.WithAlpha(0.75f);
                poof.HasDefaultTint = true;
            }

            if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
            foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if(actor != null)
                {
                    if (actor.healthHaver.IsDead == false)
                    {
                        actor.OverrideTarget = this.LastOwner.specRigidbody;
                        SetPrivateType<AIActor>(actor, "m_canTargetEnemies", false);
                    }
                }
               
            }

        }

        private void custompitdeathhandle(AIActor actor, ref bool suppressDamage)
        {
            PlayerController player = this.LastOwner;
            player.CurrentStoneGunTimer = 1f;

            player.MovementModifiers -= this.NoMotionModifier;
            player.IsStationary = false;
            modifierController = false;
            player.IsEthereal = false;

            if (host != null)
            {
                host.healthHaver.ApplyDamage(99999, Vector2.zero, "unbound", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, false);
                host = null;
            }

            player.SetIsStealthed(false, "cause they just are");


            CamToggle = false;
            //GameManager.Instance.MainCameraController.StartTrackingPlayer();
            //GameManager.Instance.MainCameraController.SetManualControl(false, true);



            throw new NotImplementedException();
        }

        public override void  DoActiveEffect(PlayerController player)
        {

            if(host != null)
            {
                player.CurrentStoneGunTimer = 1f;
                player.MovementModifiers -= this.NoMotionModifier;
                player.IsStationary = false;
                modifierController = false;
                player.IsEthereal = false;
                player.healthHaver.IsVulnerable = true;
                CamToggle = false;
                player.SetIsStealthed(false, "cause they just are");
                if (host != null)
                {
                    host.healthHaver.ApplyDamage(99999, Vector2.zero, "unbound", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, false);
                    host = null;
                }
                //GameManager.Instance.MainCameraController.StartTrackingPlayer();
                //GameManager.Instance.MainCameraController.SetManualControl(false, true);
            }



            base.DoActiveEffect(player);
        }

       
        public bool vangaurd = false;
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public override void Update()
        {   // deathcontroller
            if (this.LastOwner != null)
            {


                if (this.LastOwner != null && vangaurd)
                {
                    if (host != null && host.healthHaver.GetCurrentHealth() == 0)
                    {
                        this.DoActiveEffect(this.LastOwner);
                    }

                    if (host != null && CamToggle)
                    {
                        // in head state
                        //CameraController m_camera = GameManager.Instance.MainCameraController;
                        //m_camera.StopTrackingPlayer();
                        //m_camera.SetManualControl(true, false);
                        //m_camera.OverridePosition = host.CenterPosition;
                        RoomHandler room = this.LastOwner.CurrentRoom;
                        if (host != null)
                        {
                            if (KeyDown(GungeonActions.GungeonActionType.Shoot, (PlayerController)this.LastOwner))
                            {
                                host.healthHaver.ApplyDamage(9999f,Vector2.zero,"you should explode yourself NOW");
                            }

                           

                        }


                        //Keep superHot watch from breaking everything

                        StartCoroutine(stophotwatch());
                        
                        

                    }

                }

                CanBeUsed(this.LastOwner);
            }
                base.Update();
        }



        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
           

        }

        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 15f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = true,
            secretWallsRadius = 3,
            force = 2,
            forceUseThisRadius = true,


        };


        public bool KeyDown(GungeonActions.GungeonActionType action, PlayerController user)
        {
            if (!GameManager.Instance.IsLoadingLevel && user != null)
            {
                return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }
        public IEnumerator stophotwatch()
        {
            yield return new WaitForSecondsRealtime(.01f);
            BraveTime.ClearAllMultipliers();
            BraveTime.SetTimeScaleMultiplier(1, this.gameObject);
            BraveTime.SetTimeScaleMultiplier(1, this.gameObject);
            BraveTime.SetTimeScaleMultiplier(1, this.gameObject);
            BraveTime.SetTimeScaleMultiplier(1, this.gameObject);
            BraveTime.SetTimeScaleMultiplier(1, this.gameObject);

        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.IsInCombat)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }


        public Vector2 standing;
        public Vector2 bullet;
        public void proximity(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            standing = host.CenterPosition;
            List<Projectile> bullets = GetBullets(); // I am in pain
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Projectile projectile = bullets[i];
                float health = host.healthHaver.GetCurrentHealth();
                if (health > 0.0)
                {
                    bullet = (Vector2)projectile.LastPosition;

                    float radius = .5f;
                    if (Vector2.Distance(bullet, standing) < radius)
                    {
                        host.healthHaver.ApplyDamage(1f, Vector2.zero, "YES");

                    }


                }
            }



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

    }



}

