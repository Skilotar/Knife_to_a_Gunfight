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
    public class MindControlHeadband : PlayerItem
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
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 40f);


            //Set the rarity of the item
            item.CanBeDropped = false;
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;


        }


        public AIActor host;
        public bool CamToggle = false;
        public bool modifierController = false;

        public override void DoEffect(PlayerController player)
        {
            player.StartCoroutine(ItemBuilder.HandleDuration(this, 10, player, DoActiveEffect));

            Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
            float zRotation = BraveMathCollege.Atan2Degrees(vector);
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, player.sprite.WorldCenter, player, player.CurrentGun);
            ProjectileSlashingBehaviour poke = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            poke.SlashRange = 25;
            poke.SlashDamage = 1f;
            poke.SlashDimensions = 20f;
            poke.SlashDamageUsesBaseProjectileDamage = false;
            poke.OnFirstEnemyHit += Poke_OnFirstEnemyHit;
            hit = false;
            StartCoroutine(resetIfMissed());
        }
        bool hit = false;
        private IEnumerator resetIfMissed()
        {
            yield return new WaitForSeconds(.1f);
            if (hit == false)
            {
                
                this.CurrentTimeCooldown = this.CurrentTimeCooldown - (this.timeCooldown * .75f);
                this.DidDamage(this.LastOwner, 1);
            }
            yield return null;
        }

        bool resetOnAttack;
        private void Poke_OnFirstEnemyHit(PlayerController player, AIActor hitEnemy)
        {
            if (hitEnemy.aiActor != null)
            {
                if (hitEnemy.aiActor.healthHaver != null)
                {
                    if (hitEnemy.healthHaver.IsAlive)
                    {
                        try
                        {
                            hit = true;
                            resetOnAttack = false;
                            IntVector2 aim = Vector2Extensions.ToIntVector2(player.CenterPosition, VectorConversions.Round);
                            RoomHandler room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(aim);
                            if (room != null)
                            {
                                hitEnemy.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
                                if(hitEnemy.EnemyGuid == "4d37ce3d666b4ddda8039929225b7ede" || hitEnemy.EnemyGuid == "c0260c286c8d4538a697c5bf24976ccf" || hitEnemy.EnemyGuid == "19b420dec96d4e9ea4aebc3398c0ba7a" || hitEnemy.EnemyGuid == "b4666cb6ef4f4b038ba8924fd8adf38f" || hitEnemy.EnemyGuid == "2d4f8b5404614e7d8b235006acde427a" || hitEnemy.EnemyGuid == "2feb50a6a40f4f50982e89fd276f6f15" || hitEnemy.EnemyGuid == "7ec3e8146f634c559a7d58b19191cd43" || hitEnemy.EnemyGuid == "5f15093e6f684f4fb09d3e7e697216b4") // things that kill themselves on attack
                                {
                                    resetOnAttack = true;
                                } 
                                host = hitEnemy;
                                vangaurd = true;
                                StartCoroutine(HandleSwapBody(host));

                                hitEnemy.CompanionOwner = player;
                                hitEnemy.CustomPitDeathHandling += this.custompitdeathhandle;
                                hitEnemy.BaseMovementSpeed = 15;
                                hitEnemy.IsWorthShootingAt = true;
                                hitEnemy.IgnoreForRoomClear = true;
                                hitEnemy.HitByEnemyBullets = true;
                                hitEnemy.UniquePlayerTargetFlag = true;
                                hitEnemy.CanTargetPlayers = false;
                                hitEnemy.SetAIMovementContribution(Vector2.zero);
                                MindControlEffect mindControl = hitEnemy.gameObject.GetOrAddComponent<MindControlEffect>();
                                mindControl.owner = player;

                                SetPrivateType<AIActor>(hitEnemy, "m_canTargetEnemies", true);

                                hitEnemy.Update();
                                TargetNumber1(hitEnemy, player);

                                if (CheckForMonger(room) != true)
                                {
                                    CameraOverrideTarget cam = hitEnemy.gameObject.GetOrAddComponent<CameraOverrideTarget>();
                                    cam.startOverrideOnStart = true;
                                }
                                if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
                                foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                                {
                                    
                                    if (actor.healthHaver.IsDead == false)
                                    {
                                        actor.OverrideTarget = hitEnemy.specRigidbody;
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

                                if (hitEnemy.healthHaver.IsDead == false)
                                {
                                   
                                    SetPrivateType<AIActor>(hitEnemy, "m_canTargetEnemies", true);
                                    if (hitEnemy.bulletBank != null)
                                    {
                                        hitEnemy.bulletBank.OnProjectileCreated -= this.OnAiTriedToShoot;
                                        hitEnemy.bulletBank.OnProjectileCreated += this.OnAiTriedToShoot2;
                                    }
                                    if (hitEnemy.aiShooter != null)
                                    {
                                        hitEnemy.aiShooter.PostProcessProjectile -= this.OnAiTriedToShoot;
                                        hitEnemy.aiShooter.PostProcessProjectile += this.OnAiTriedToShoot2;
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
                        catch (Exception e)
                        {
                            ETGModConsole.Log(e.ToString());
                        }
                    }
                    
                }
            }
        }

        private IEnumerator HandleSwapBody(AIActor target)
        {
            Color color = UnityEngine.Color.white;
            float ang = (target.CenterPosition - this.LastOwner.CenterPosition).ToAngle();
            UnityEngine.GameObject laser = RenderTether(this.LastOwner.CenterPosition, Vector2.Distance(target.CenterPosition, this.LastOwner.CenterPosition) * 16, 1, ang, true, color);
            Transform copySprite = this.CreateEmptySprite(target);
            Transform PlayerSprite = this.CreateEmptyPlayerSprite(this.LastOwner);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 1f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if(host!= null)
                {
                    bool flag = this.LastOwner && copySprite;
                    if (flag)
                    {
                        Vector3 position = this.LastOwner.CenterPosition;
                        float t = elapsed / duration * (elapsed / duration);
                        copySprite.position = Vector3.Lerp(startPosition, position, t);
                        copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                        copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);



                        PlayerSprite.position = Vector3.Lerp(position, startPosition, t);
                        PlayerSprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * PlayerSprite.rotation;
                        PlayerSprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                        position = default(Vector3);
                    }
                    if (laser != null)
                    {
                        laser.transform.rotation = Quaternion.Euler(0, 0, (target.CenterPosition - this.LastOwner.CenterPosition).ToAngle());
                        tk2dTiledSprite sprite = laser.GetComponent<tk2dTiledSprite>();
                        sprite.dimensions = new Vector2(Vector2.Distance(target.CenterPosition, this.LastOwner.CenterPosition) * 16, 1);
                        laser.transform.position = this.LastOwner.CenterPosition;
                    }
                    yield return null;
                }
                else
                {
                    UnityEngine.Object.Destroy(copySprite.gameObject);
                    UnityEngine.Object.Destroy(PlayerSprite.gameObject);
                    UnityEngine.Object.Destroy(laser);
                }
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
                UnityEngine.Object.Destroy(PlayerSprite.gameObject);
                UnityEngine.Object.Destroy(laser);
            }

            //ETGModConsole.Log("EndSuck");
            yield break;
        }

        public static GameObject RenderTether(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
        {
            GameObject laserSightPrefab = LoadHelper.LoadAssetFromAnywhere("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

            GameObject gameObject = SpawnManager.SpawnVFX(laserSightPrefab, position, Quaternion.Euler(0, 0, angle));

            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            float newWidth = 1f;
            if (width != -1) newWidth = width;
            component2.dimensions = new Vector2(length, newWidth);
            if (alterColour && colour != null)
            {
                component2.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.SetColor("_OverrideColor", (Color)colour);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", (Color)colour);
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
            }
            return gameObject;
        }


        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }
        private Transform CreateEmptyPlayerSprite(PlayerController target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            
            return gameObject2.transform;
        }



        private void OnAiTriedToShoot(Projectile obj)
        {
            obj.specRigidbody.OnPreRigidbodyCollision += this.EnemyProjectile;
        }
        private void OnAiTriedToShoot2(Projectile obj)
        {
            obj.specRigidbody.OnPreRigidbodyCollision += this.EnemyProjectile2;
        }

        private void EnemyProjectile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.aiActor != null)
            {
                if (otherRigidbody.aiActor != host)
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }

        private void EnemyProjectile2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.aiActor != null)
            {
                if (otherRigidbody.aiActor == host)
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }
        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            RoomHandler room = this.LastOwner.CurrentRoom;
            if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
            foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if (actor.healthHaver.IsDead == false)
                {
                    actor.OverrideTarget = this.LastOwner.specRigidbody;
                    SetPrivateType<AIActor>(actor, "m_canTargetEnemies", false);
                }
            }

        }

        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }




        private bool CheckForMonger(RoomHandler room)
        {
            if (room != null)
            {
                List<AIActor> boi = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                foreach (AIActor actor in boi)
                {
                    if (actor.EnemyGuid == "f3b04a067a65492f8b279130323b41f0")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void DoActiveEffect(PlayerController player)
        {
          
            
            player.MovementModifiers -= this.NoMotionModifier;
            player.IsStationary = false;
            modifierController = false;
            player.IsEthereal = false;
            player.healthHaver.IsVulnerable = true;
            CamToggle = false;
            player.SetIsStealthed(false, "cause they just are");
            if (host != null)
            {
                host.EraseFromExistence();
                host = null;
                player.CurrentStoneGunTimer = 1f;
            }
            vangaurd = false;
            //GameManager.Instance.MainCameraController.StartTrackingPlayer();
            //GameManager.Instance.MainCameraController.SetManualControl(false, true);

            SilencerInstance.DestroyBulletsInRange(player.specRigidbody.UnitCenter, 3, true, false);
            

            base.DoActiveEffect(player);
        }

        public bool vangaurd = false;
      
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
                host.EraseFromExistence();

            }

            player.SetIsStealthed(false, "cause they just are");
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            CamToggle = false;
            vangaurd = false;
            //GameManager.Instance.MainCameraController.StartTrackingPlayer();
            //GameManager.Instance.MainCameraController.SetManualControl(false, true);
           
        }

        public void TargetNumber1(AIActor host, PlayerController user)
        {   if (host != null && user.IsInCombat)
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
       
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        public override void Update()
        {
            if (this.LastOwner != null && vangaurd)
            {
                if (host == null || host.healthHaver.IsDead)
                {
                    this.DoActiveEffect(this.LastOwner);
                }

                if (host != null && CamToggle == true)
                {
                    
                    StartCoroutine(stophotwatch());

                }

                if (this.IsOnCooldown)
                {

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
     
    }

}
