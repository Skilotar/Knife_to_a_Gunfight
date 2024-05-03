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
    public class BackupDrone : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "B_drone";
            string resourcePath = "Knives/Resources/GunnerDrone/gunnerdrone_001";
            GameObject gameObject = new GameObject();
            BackupDrone rock = gameObject.AddComponent<BackupDrone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Beep boop";
            string longDesc = "" +

                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.EXCLUDED;
            BackupDrone.BuildPrefab();

            rock.OrbitalPrefab = BackupDrone.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;


            ID = rock.PickupObjectId;
            Drone = rock;
        }

        public static int ID;
        public static IounStoneOrbitalItem Drone;


        public static void BuildPrefab()
        {
            bool flag = BackupDrone.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/GunnerDrone/gunnerdrone_001", null);
                gameObject.name = "Backup";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;

                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                BackupDrone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                BackupDrone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                BackupDrone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                BackupDrone.orbitalPrefab.shouldRotate = false;
                BackupDrone.orbitalPrefab.orbitRadius = 2f;
                BackupDrone.orbitalPrefab.SetOrbitalTier(0);

                tk2dSpriteAnimation animation = gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
                tk2dSpriteAnimator animator = gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteCollectionData drone = SpriteBuilder.ConstructCollection(gameObject, ("B_Drone_Collection"));

                tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "Backup_idle", frames = new tk2dSpriteAnimationFrame[0], fps = 8 };
                List<tk2dSpriteAnimationFrame> frames4 = new List<tk2dSpriteAnimationFrame>();
                for (int i = 1; i <= 8; i++)
                {
                    tk2dSpriteCollectionData collection4 = drone;
                    int frameSpriteId4;
                   
                    frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/GunnerDrone/gunnerdrone_00{i}", collection4);
                   
                    tk2dSpriteDefinition frameDef4 = collection4.spriteDefinitions[frameSpriteId4];
                    frames4.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId4, spriteCollection = collection4 });
                }
                clip.frames = frames4.ToArray();
                clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
                animator.Library = animation;
                animator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
                animator.DefaultClipId = animation.GetClipIdByName("Backup_idle");
                animator.playAutomatically = true;



                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);


            }
        }


        public override void Pickup(PlayerController player)
        {




            base.Pickup(player);


        }


      
        public override void Update()
        {
            
            base.Update();
        }

       


        public override DebrisObject Drop(PlayerController player)
        {

            return base.Drop(player);
        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }

        public static bool speedUp = false;
        public static PlayerOrbital orbitalPrefab;
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();
        
    }

}