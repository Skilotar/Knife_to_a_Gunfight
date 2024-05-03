using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;



namespace Knives
{   
    public class Stopda : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "Stopda Rock";
            string resourcePath = "Knives/Resources/Stopda_Rock";
            GameObject gameObject = new GameObject();
            Stopda rock = gameObject.AddComponent<Stopda>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Can't Stopda Rock";
            string longDesc = "Ever since the dawn of the universe this rock has never stopped moving even if it had to bend the laws of physics to get some speed. " +
                "Even when is seems to sit still it vibrates unstoppably. This Stone has the unique ability to store all energy projected into it to use at will. " +
                "Increases speed out of combat based on how many projectiles it blocks." +
             
                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.B;
            Stopda.BuildPrefab();

            rock.OrbitalPrefab = Stopda.orbitalPrefab;
            
            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;

            ID = rock.PickupObjectId;
        }

        public static int ID;
        public static void BuildPrefab()
        {
            bool flag = Stopda.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Stopda_Rock", null);
                gameObject.name = "Stopda";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                Stopda.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                Stopda.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                Stopda.orbitalPrefab.orbitDegreesPerSecond = 40f;
                Stopda.orbitalPrefab.shouldRotate = true ;
                Stopda.orbitalPrefab.orbitRadius = 2.5f;
                

                Stopda.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
                
                
            }
        }


        public override void Pickup(PlayerController player)
        {

            player.OnEnteredCombat += this.OnEnteredCombat;
            if (this.m_extantOrbital != null)
            {
                SpeculativeRigidbody specRigidbody = this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            }
            base.Pickup(player);

        }

        private void OnEnteredCombat()
        {
            PlayerOrbital wee = this.m_extantOrbital.GetComponent<PlayerOrbital>();
            wee.orbitDegreesPerSecond = 50 + (rockpoints * 2);

        }

        
        
        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {

           
            rockpoints++;
           
            //other.projectile.DieInAir();
            //PhysicsEngine.SkipCollision = true;
       
        }
    
        public override void  Update()
        {

           


            base.Update();
        }

   


        float rockpoints;
       
        public override DebrisObject Drop(PlayerController player)
        {

           
            Stopda.speedUp = false;
            return base.Drop(player);
        }

        public override void  OnDestroy()
        {
            
            Stopda.speedUp = false;
            base.OnDestroy();
        }

        public static bool speedUp = false;
        public static PlayerOrbital orbitalPrefab;
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();
       
 
    }

}
