 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using System.Collections;
using System.Reflection;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using MonoMod;
using System.Runtime.CompilerServices;

namespace Knives
{
    public class MindControlHeadband: PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension
      
        public static void Register()
        {
            //The name of the item
            string itemName = "Mind Swap Headband";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/MindSwapHeadband";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MindControlHeadband>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "CHANGE NOW!";
            string longDesc =
                "Use once to control enemy, use again to release them\n\n" +

                "This Technique of a imfamous space captain has been condensed and printed on the inside of this headband. Now for the low low price of 20 payments of 19.99$ you too can swap your brain into someone elses body! \n\n" +
                "This is not legal in 4 galaxies and will easily get you hunted by the space police. But this isnt space! This is the Gungeon! There ARE NO RULES HERE! AH HAHAHAHA. " +
                
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);


            //Set the rarity of the item
            item.CanBeDropped = false;
            
            
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;

            
        }

       
        public AIActor host;
        public int CamToggle = 0;
        public int modifierController = 0;
        
        
        public override void  DoEffect(PlayerController player)
        {
            player.StartCoroutine(ItemBuilder.HandleDuration(this, 10, player, DoActiveEffect));
            
            Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
            float zRotation = BraveMathCollege.Atan2Degrees(vector);
            player.StartCoroutine(this.HandleSwing(player, vector,0f, 20f));

          

            Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];

            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.LastOwner.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.LastOwner.CurrentGun == null) ? 0f : this.LastOwner.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.baseData.range = 20;
            component.baseData.damage = 0;
            component.baseData.speed *= 3f;
               
             
            component.Owner = this.LastOwner;

        }

        public override void  DoActiveEffect(PlayerController player)
        {
            if(host != null)
            {


                player.CurrentStoneGunTimer = 1f;

                player.MovementModifiers -= this.NoMotionModifier;
                player.IsStationary = false;
                modifierController = 0;
                player.IsEthereal = false;

                if (host != null)
                {
                    host.healthHaver.ApplyDamage(99999, Vector2.zero, "unbound", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, false);

                }

                player.SetIsStealthed(false, "cause they just are");
                PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
                this.LastOwner.stats.RecalculateStats(LastOwner, true);
                CamToggle = 0;
                GameManager.Instance.MainCameraController.StartTrackingPlayer();
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
                
            }


            base.DoActiveEffect(player);
        }

        public bool vangaurd = false;
        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < .1)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && !hitRigidbody.aiActor.healthHaver.IsBoss && hitRigidbody.aiActor.IsNormalEnemy)
                {
                    hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "Hero's Sword", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

                    user.IsEthereal = true;
                    user.SetIsStealthed(true, "cause they just are" );
                    PassiveItem.IncrementFlag(user, typeof(LiveAmmoItem));
                    if (modifierController == 0)
                    {
                        user.MovementModifiers += this.NoMotionModifier;
                        user.IsStationary = true;
                        modifierController++;
                    }
                    user.CurrentStoneGunTimer = 9999999f;
                    this.LastOwner.stats.RecalculateStats(LastOwner, true);
                    
                    host = hitRigidbody.aiActor;
                    vangaurd = true;
                    MindControlEffect orAddComponent = host.gameObject.GetOrAddComponent<MindControlEffect>();
                    orAddComponent.owner = user;
                    host.CompanionOwner = user;
                    host.CustomPitDeathHandling += this.custompitdeathhandle;
                    host.aiActor.SetAIMovementContribution(Vector2.zero);
                    host.BaseMovementSpeed = 15;
                    host.IsWorthShootingAt = true;
                    host.IgnoreForRoomClear = true;
                    host.HitByEnemyBullets = true;
                    host.UniquePlayerTargetFlag = true;
                    CompanionisedEnemyBulletModifiers friend = host.gameObject.GetOrAddComponent<CompanionisedEnemyBulletModifiers>();
                    friend.Start();
                    
                    
                    host.Update();
                    TargetNumber1(host,user);
                    CamToggle = 1;

                   

                }
                 
                yield return null;
            }
            yield break;
        }

        private void custompitdeathhandle(AIActor actor, ref bool suppressDamage)
        {
            PlayerController player = this.LastOwner;
            player.CurrentStoneGunTimer = 1f;

            player.MovementModifiers -= this.NoMotionModifier;
            player.IsStationary = false;
            modifierController = 0;
            player.IsEthereal = false;

            if (host != null)
            {
                host.healthHaver.ApplyDamage(99999, Vector2.zero, "unbound", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, false);

            }

            player.SetIsStealthed(false, "cause they just are");
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            CamToggle = 0;
            GameManager.Instance.MainCameraController.StartTrackingPlayer();
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            throw new NotImplementedException();
        }

        public void TargetNumber1(AIActor host, PlayerController user)
        { if (host != null && user.IsInCombat)
            {
                RoomHandler room = user.CurrentRoom;
                foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if(enemy != host && host != null)
                    {
                        enemy.OverrideTarget = host.specRigidbody;
                        
                    }
 
                }
            }
        }
         protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
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


        
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public override void Update()
        {
            if (this.LastOwner != null && vangaurd)
            {
                if (host != null && host.healthHaver.GetCurrentHealth() == 0)
                {
                    this.DoActiveEffect(this.LastOwner);
                }

                if (host != null && CamToggle == 1)
                {
                    CameraController m_camera = GameManager.Instance.MainCameraController;
                    m_camera.StopTrackingPlayer();
                    m_camera.SetManualControl(true, false);
                    m_camera.OverridePosition = host.CenterPosition;
                    TargetNumber1(host, this.LastOwner);
                    RoomHandler room = this.LastOwner.CurrentRoom;
                    proximity(this.LastOwner);
                    foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        System.Random rng = new System.Random();
                        int chanceToFire = rng.Next(1, 120);

                        if (chanceToFire == 1 && enemy != host)
                        {
                            enemy.aiShooter.ShootAtTarget();
                        }
                    }

                    StartCoroutine(stophotwatch());

                }


            }


            base.Update();
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
        public Vector2 standing;
        public Vector2 bullet;
        public void proximity(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            standing = host.CenterPosition;
            foreach (var projectile in GetBullets())
            {
                float health = host.healthHaver.GetCurrentHealth();
                if (health > 0.0)
                {
                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1f;
                    if (Vector2.Distance(bullet, standing) < radius)
                    {
                        host.healthHaver.ApplyDamage(1f, Vector2.zero, "YES");
                        
                    }
                   
                        
                }
            }

      

        }
        public  List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null && projectile.Owner != host)
                    {
                        
                       
                            list.Add(projectile);
                     
                    }
                }
            }
            return list;
        }
    }


    


}
