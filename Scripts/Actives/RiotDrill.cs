using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;
using System.Collections.Generic;

namespace Knives
{
    class Riot_Drill : PlayerItem
    {
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/RiotDrill",
            "Knives/Resources/RiotDrill_cooldown",
           
        };
        public static void Register()
        {
            string itemName = "Riot Drill";

            string resourceName = "Knives/Resources/RiotDrill";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Riot_Drill>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "You Know The Drill";
            string longDesc = "A launchable thermite grinder designed for pushing enemies out from behind cover. \n\n" +
                "The beloved tool of Miffed Marge, A fearsom freedom fighter against the Hegemony's oppression"

                 +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 23f);


           
            item.quality = PickupObject.ItemQuality.C;

            Riot_Drill.spriteIDs = new int[Riot_Drill.spritePaths.Length];
            Riot_Drill.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(Riot_Drill.spritePaths[0], item.sprite.Collection);
            Riot_Drill.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(Riot_Drill.spritePaths[1], item.sprite.Collection);
            item.sprite.SetSprite(spriteIDs[0]);

            item.UsesNumberOfUsesBeforeCooldown = false;
            item.numberOfUses = 2;


            Gun gun = (Gun)ETGMod.Databases.Items[229];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);

            projectile2.baseData.damage = 10f;
            projectile2.baseData.speed = 30f;
            projectile2.baseData.range = 15f;

            projectile2.SetProjectileSpriteRight("RiotDrill_proj", 12, 8, false, tk2dBaseSprite.Anchor.MiddleCenter, 10, 6);

            Drill = projectile2;
            ID = item.PickupObjectId;

        }

        public static int ID;
        public static Projectile Drill;
        private static int[] spriteIDs;

        public override void Pickup(PlayerController player)
        {

          
            base.Pickup(player);
        }

       
        public override void  OnPreDrop(PlayerController player)
        {

            base.OnPreDrop(player);
        }
        public override void  DoEffect(PlayerController player)
        {
            player.DidUnstealthyAction();
            AkSoundEngine.PostEvent("Play_WPN_sawgun_shot_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(Drill.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                
                
                component.AdditionalScaleMultiplier = 1.6f;
                DrillSticksInWall drill = component.gameObject.GetOrAddComponent<DrillSticksInWall>();
                Gun gun = (Gun)PickupObjectDatabase.GetById(122);

                
            }


        }

        public override void Update()
        {
            if(this.LastOwner != null)
            {
                if(this.IsOnCooldown == true)
                {
                    this.sprite.SetSprite(spriteIDs[1]);
                }
                else
                {
                    this.sprite.SetSprite(spriteIDs[0]);
                }

                if(this.LastOwner.PlayerHasActiveSynergy("Freedom For Salvo"))
                {
                    this.UsesNumberOfUsesBeforeCooldown = true;

                }
                else
                {
                    this.UsesNumberOfUsesBeforeCooldown = false;

                }
            }
            base.Update();
        }


    }
    public class DrillSticksInWall : MonoBehaviour
    {
        public DrillSticksInWall()
        {
           
        }
        public Projectile Drill;
        private void Start()
        {
            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            SpeculativeRigidbody specRigidBody = this.m_projectile.specRigidbody;
            this.m_projectile.BulletScriptSettings.surviveTileCollisions = true;
            this.m_projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
            this.m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            m_projectile.OnDestruction += M_projectile_OnDestruction;
        }

        private void M_projectile_OnDestruction(Projectile obj)
        {
            AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", m_projectile.gameObject);
        }

        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            DoStop();
        }

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)//Non Wall Collision
        {
            if (otherRigidbody.aiActor)
            {

            }
            else
            {
                if (!otherRigidbody.minorBreakable)
                {
                    DoStop();
                }
            }
            
        }

       
        public void DoStop()
        {

            AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", base.gameObject);
            this.m_projectile.baseData.speed *= 0f;
            this.m_projectile.UpdateSpeed();
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Remove(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));

            StartCoroutine(DeathTimer());
            StartCoroutine(FireDrill());
        }

        private IEnumerator FireDrill()
        {
            AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_loop_01", m_projectile.gameObject);
            yield return new WaitForSeconds(.15f);
            while (m_projectile != null)
            {
                yield return new WaitForSeconds(.08f);

                float spread = UnityEngine.Random.Range(-20, 20);

                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[748]).DefaultModule.chargeProjectiles[0].Projectile;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, m_projectile.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, m_projectile.Direction.ToAngle() + spread), true); ;
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = this.m_projectile.Owner;
                    component.Shooter = this.m_projectile.Owner.specRigidbody;
                    component.baseData.damage = 4f;
                    component.baseData.range = 7f - (Math.Abs(spread)/4);
                    component.baseData.speed *= .4f;
                    component.AdditionalScaleMultiplier = .45f;
                    component.AppliesFire = true;
                    component.FireApplyChance = .50f;
                    component.damagesWalls = true;
                    //component.DefaultTintColor = Color.red;
                    //component.HasDefaultTint = true;
                    component.hitEffects.suppressMidairDeathVfx = true;
                    component.SuppressHitEffects = true;
                    component.baseData.force = 0;
                    component.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile2));
                    component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison2));
                    GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RANDOM, 10, .30f, m_projectile.specRigidbody.UnitCenter, m_projectile.specRigidbody.UnitCenter, m_projectile.Direction, 25, -10, null, null, Color.yellow, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                    

                }



            }
            yield return null;
        }

        private void OnPreCollison2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.aiActor)
            {

            }
            else
            {
                if (!otherRigidbody.minorBreakable)
                {
                    PhysicsEngine.SkipCollision = true;
                    myRigidbody.projectile.ResetDistance();
                }
               
            }
        }

        private void OnPreCollisontile2(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            myRigidbody.projectile.ResetDistance();
        }

        private IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(8f);
            this.m_projectile.DieInAir();
            AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", m_projectile.gameObject);
          
        }

        private Projectile m_projectile;
        private float m_hitNormal;
        private PlayerController projOwner;
        
    }
}


