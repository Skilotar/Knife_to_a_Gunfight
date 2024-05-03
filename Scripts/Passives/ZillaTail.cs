using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Reflection;
using System.Collections.Generic;



namespace Knives
{
    public class GunZilla_Tail : PassiveItem
    {
        public static void Register()
        {
            try
            {
                string itemName = "Gunzilla Tail";

                string resourceName = "Knives/Resources/Gunzilla_tail";

                GameObject obj = new GameObject(itemName);

                var item = obj.AddComponent<GunZilla_Tail>();

                ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

                //Ammonomicon entry variables
                string shortDesc = "Turn Tail";
                string longDesc = "An ancient strategy shared by lizards of old. It is best to lose a limb that to die altogether." +

                    "\n\n\n -Knife_to_a_Gunfight";

                //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
                //Do this after ItemBuilder.AddSpriteToObject!
                ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

                BuildPrefab();
                ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);


                item.quality = PickupObject.ItemQuality.D;


                ID = item.PickupObjectId;
            }
            catch (Exception E)
            {
                ETGModConsole.Log(E.ToString());
            }



        }
        public static GameObject tail = new GameObject();
        public static PlayerController LastKnownOwner;
        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/tail/tail_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/tail/tail_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 5; i++)
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                if (i <= 9)
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/tail/tail_00{i}", spriteForObject);
                }
                else
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/tail/tail_00{i}", spriteForObject);
                }

                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;

            SpeculativeRigidbody body = gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            pixelCollider.ManualWidth = 6;
            pixelCollider.ManualHeight = 13;
            pixelCollider.ManualOffsetX = 2;
            pixelCollider.ManualOffsetY = 0;
            body.PixelColliders = new List<PixelCollider>()
            {
                pixelCollider
            };
            HealthHaver heal = gameObject.GetOrAddComponent<HealthHaver>();
            heal.SetHealthMaximum(3f);
            


            tail = gameObject;
        }

        public static int ID;
        
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            LastKnownOwner = player;
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.OnDamaged += HealthHaver_OnDamaged;
            base.Pickup(player);
        }

        public GameObject m_tail;
        public int startingRotation = 0;
        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (this.Owner.healthHaver.IsDead != true)
            {
                if(m_tail != null)
                {
                    UnityEngine.GameObject.Destroy(m_tail);
                }

                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(tail, this.Owner.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(this.Owner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.TeleporterPrototypeTelefragVFX, LastKnownOwner.sprite.WorldCenter, Quaternion.identity);
                
               
                RoomHandler room = this.Owner.CurrentRoom;
                if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
                foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (actor.healthHaver.IsDead == false)
                    {
                        actor.OverrideTarget = gameObject.GetComponent<SpeculativeRigidbody>();

                    }
                }

                gameObject.GetComponent<HealthHaver>().OnDeath += GunZilla_Tail_OnDeath;
                m_tail = gameObject;

                if (gameObject != null)
                {
                    if (this.Owner.PlayerHasActiveSynergy("Mecha Gunzilla"))
                    {

                        gameObject.GetComponent<tk2dBaseSprite>().renderer.material.shader = Shader.Find("Brave/ItemSpecific/LootGlintAdditivePass");
                        gameObject.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
                        Gun owner = PickupObjectDatabase.GetById(380) as Gun;
                        GameObject gameObject1 = owner.ObjectToInstantiateOnReload.gameObject;
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject1, gameObject.GetComponent<tk2dBaseSprite>().WorldCenter, Quaternion.identity);
                        gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(gameObject.GetComponent<tk2dBaseSprite>().WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                        gameObject2.transform.localScale = new Vector3(.5f, .5f, 1);
                        
                    }

                

                    if (this.Owner.PlayerHasActiveSynergy("Irradiated Blood"))
                    {

                        Projectile projectile = ((Gun)PickupObjectDatabase.GetById(329)).DefaultModule.chargeProjectiles[1].Projectile;
                        
                        for (int i = 0; i < 6; i++)
                        {

                            GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile.gameObject, gameObject.GetComponent<tk2dBaseSprite>().WorldCenter, Quaternion.Euler(0f, 0f, (i * 60) + startingRotation), true);
                            Projectile component = gameObject3.GetComponent<Projectile>();
                            bool flag = component != null;
                            if (flag)
                            {
                                component.Owner = this.Owner;
                                component.Shooter = this.Owner.specRigidbody;
                                component.PoisonApplyChance = 100;
                                component.AppliesPoison = true;
                                BounceProjModifier bnc = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                                bnc.numberOfBounces = 1;
                                bnc.bouncesTrackEnemies = true;
                                bnc.TrackEnemyChance = 100;

                            }
                        }
                        if (startingRotation != 180)
                        {
                            startingRotation += 20;
                        }



                    }
                }
                

            }
        }

      
        public override DebrisObject Drop(PlayerController player)
        {
            
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.OnDamaged -= HealthHaver_OnDamaged;

            return base.Drop(player);

        }

       

        public void GunZilla_Tail_OnDeath(Vector2 obj)
        {
            RoomHandler room = LastKnownOwner.CurrentRoom;
            if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
            foreach (AIActor actor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if (actor.healthHaver.IsDead == false)
                {
                    actor.OverrideTarget = LastKnownOwner.specRigidbody;
                }
            }

            
        }

       
    }
}