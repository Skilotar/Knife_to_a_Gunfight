using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Alexandria.ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;



namespace Knives
{   
    public class Stopda : AdvancedPlayerOrbitalItem
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
            KnownLastOwner = player;
            player.OnEnteredCombat += this.OnEnteredCombat;
            
            base.Pickup(player);

        }
        public PlayerController KnownLastOwner;

        public GameObject rememberme;
        private void OnEnteredCombat()
        {
            PlayerOrbital wee = rememberme.GetComponent<PlayerOrbital>();
            wee.orbitDegreesPerSecond = 50 + (rockpoints / 2);
            //AcquireTargetNOW(wee);
        }

        

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            rockpoints++;
           
            if (KnownLastOwner != null)
            {
                
                if (KnownLastOwner.PlayerHasActiveSynergy("Rolling Stones"))
                {
                    
                    MiscToolMethods.SpawnProjAtPosi(Callahan.rocks, myRigidbody.transform.position, this.Owner, UnityEngine.Random.Range(-180, 180));
                    MiscToolMethods.SpawnProjAtPosi(Callahan.rocks, myRigidbody.transform.position, this.Owner, UnityEngine.Random.Range(-180, 180));
                    MiscToolMethods.SpawnProjAtPosi(Callahan.rocks, myRigidbody.transform.position, this.Owner, UnityEngine.Random.Range(-180, 180));
                    
                }
                if (KnownLastOwner.PlayerHasActiveSynergy("Apollo 440"))
                {
                    
                    PhysicsEngine.SkipCollision = true;
                    
                    if(other.GetComponent<Projectile>() != null)
                    {
                        Projectile projectile = other.GetComponent<Projectile>();
                        SpawnManager.PoolManager.Remove(projectile.transform);
                        PassiveReflectItem.ReflectBullet(projectile, true, this.Owner, 2f, 1f, 4f, 0f);
                        projectile.RemoveBulletScriptControl();
                        projectile.BulletScriptSettings.preventPooling = true;
                        projectile.RemoveBulletScriptControl();
                        projectile.AdjustPlayerProjectileTint(ExtendedColours.honeyYellow, 1);

                        projectile.baseData.speed *= 0f;

                        projectile.baseData.damage *= 0.5f;
                        projectile.UpdateSpeed();


                        ChildProjCleanup cleanup = projectile.gameObject.GetOrAddComponent<ChildProjCleanup>();
                        cleanup.delay = 7f;
                        cleanup.parentProjectile = null;
                        cleanup.doColor = false;
                    }
                    
                }
            }
        }
    
        public override void  Update()
        {
            
            base.Update();
        }


        public override void OnOrbitalCreated(GameObject orbital)
        {
            if (orbital.GetComponent<SpeculativeRigidbody>())
            {
                SpeculativeRigidbody specRigidbody = orbital.GetComponent<SpeculativeRigidbody>();
                specRigidbody.OnPreRigidbodyCollision += this.OnPreCollison;
                rememberme = orbital;
            }
            base.OnOrbitalCreated(orbital);
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
