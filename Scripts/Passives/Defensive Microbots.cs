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
    public class Micro : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "Defensive Microbot";
            string resourcePath = "Knives/Resources/Defensive_microbots";
            GameObject gameObject = new GameObject();
            Micro rock = gameObject.AddComponent<Micro>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Captain Look Out!";
            string longDesc = "A singular defensively oriented microbot designed for shooting out enemy projectiles long before they reach their target. \n" +
                "This bot scales in fire rate along with you." +

                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.B;
            Micro.BuildPrefab();

            rock.OrbitalPrefab = Micro.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;


            ID = rock.PickupObjectId;
        }

        public static int ID;


        public static void BuildPrefab()
        {
            bool flag = Micro.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Defensive_microbots", null);
                gameObject.name = "Stopda";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;

                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                Micro.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                Micro.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                Micro.orbitalPrefab.orbitDegreesPerSecond = 10f;
                Micro.orbitalPrefab.shouldRotate = false;
                Micro.orbitalPrefab.orbitRadius = 1.5f;
                Micro.orbitalPrefab.SetOrbitalTier(56709); 

                tk2dSpriteAnimation animation = gameObject.GetOrAddComponent<tk2dSpriteAnimation>();
                tk2dSpriteAnimator animator = gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteCollectionData drone = SpriteBuilder.ConstructCollection(gameObject, ("Drone_Collection"));

                tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "Microbots_idle", frames = new tk2dSpriteAnimationFrame[0], fps = 5 };
                List<tk2dSpriteAnimationFrame> frames4 = new List<tk2dSpriteAnimationFrame>();
                for (int i = 1; i <= 12; i++)
                {
                    tk2dSpriteCollectionData collection4 = drone;
                    int frameSpriteId4;
                    if (i < 10)
                    {
                        frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/microbot/Defensive_microbots_00{i}", collection4);
                    }
                    else
                    {
                        frameSpriteId4 = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/microbot/Defensive_microbots_0{i}", collection4);
                    }
                    tk2dSpriteDefinition frameDef4 = collection4.spriteDefinitions[frameSpriteId4];
                    frames4.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId4, spriteCollection = collection4 });
                }
                clip.frames = frames4.ToArray();
                clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
                animator.Library = animation;
                animator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
                animator.DefaultClipId = animation.GetClipIdByName("Microbots_idle");
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

        Projectile posibleTarget = new Projectile();
        Projectile secondaryTarget = new Projectile();

        public override void Update()
        {
            bool flag = this.m_extantOrbital != null;
            if (flag)
            {
                bool flag2 = base.Owner;
                if (flag2)
                {
                    posibleTarget = null;
                    secondaryTarget = null;
                    posibleTarget = GetBulletInRange();
                    
                    if (base.Owner.PlayerHasActiveSynergy("Backup's Backup") && posibleTarget !=null)
                    {
                        secondaryTarget = GetSecondaryBulletInRange();
                    }

                    if (posibleTarget != null && doingShooty == false)
                    {

                        StartCoroutine(DoShootyTheBulletOutOfTheAir(posibleTarget));

                        if (secondaryTarget != null)
                        {
                            StartCoroutine(DoShootyTheBulletOutOfTheAir(secondaryTarget));
                        }
                    }
                    if(doingShooty == true )
                    {
                        if (elapse == .5f)
                        {
                            EmergencyDoShootyCleanup();
                        }
                        else
                        {
                            elapse += Time.deltaTime;
                        }
                    }
                    
                }
            }
            base.Update();
        }

        private void EmergencyDoShootyCleanup()
        {
            if (g_laser != null)
            {
                UnityEngine.GameObject.Destroy(g_laser);
            }
            if (g_laser2 != null)
            {
                UnityEngine.GameObject.Destroy(g_laser2);
            }
            doingShooty = false;
            elapse = 0;
        }

        bool doingShooty = false;
        float elapse = 0;
        UnityEngine.GameObject g_laser;
        UnityEngine.GameObject g_laser2;

        private IEnumerator DoShootyTheBulletOutOfTheAir(Projectile randProj)
        {
            elapse = 0;
            doingShooty = true;
            if(randProj != null)
            {
                float dist = Vector2.Distance(this.m_extantOrbital.GetOrAddComponent<tk2dBaseSprite>().WorldCenter, randProj.transform.position);
                Vector2 angprep = (Vector2)randProj.transform.position - (Vector2)this.m_extantOrbital.GetOrAddComponent<tk2dBaseSprite>().WorldCenter;
                float ang = angprep.ToAngle();
                AkSoundEngine.PostEvent("Play_WPN_zapper_shot_01", base.gameObject);
                UnityEngine.GameObject laser = RenderLaserSight(this.m_extantOrbital.GetOrAddComponent<tk2dBaseSprite>().WorldCenter, dist * 16, 2, ang, true, UnityEngine.Color.red);
                if(g_laser != null)
                {
                    g_laser2 = laser;
                }
                else
                {
                    g_laser = laser;
                }
                
                Vector2 posi = randProj.transform.position;
                SilencerInstance.DestroyBulletsInRange(posi, .2f, true, false);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(.1f / Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
                

                UnityEngine.GameObject.Destroy(laser);
            }

            yield return new WaitForSeconds(.4f / Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire));

            doingShooty = false;
        }



        private Projectile GetBulletInRange()
        {
            Projectile selected = new Projectile();
            float knownLowest = 10000;

            for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
            {
                Projectile projectile = StaticReferenceManager.AllProjectiles[i];
                bool flag = projectile;
                if (flag)
                {
                    bool flag2 = !(projectile.Owner is PlayerController);
                    if (flag2)
                    {
                        bool flag3 = projectile.collidesWithPlayer || projectile.Owner is AIActor;
                        if (flag3)
                        {
                            bool flag4 = !projectile.ImmuneToBlanks;
                            if (flag4)
                            {
                                if (Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.transform.position) <= 8)
                                {
                                    if (knownLowest > Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.LastPosition))
                                    {
                                        selected = projectile;
                                        knownLowest = Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.LastPosition);
                                    }

                                }
                            }
                        }
                    }
                }
            }



            return selected;
        }
        private Projectile GetSecondaryBulletInRange()
        {
            Projectile selected = new Projectile();
            float knownLowest = 10000;

            for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
            {
                Projectile projectile = StaticReferenceManager.AllProjectiles[i];
                bool flag = projectile;
                if (flag)
                {
                    bool flag2 = !(projectile.Owner is PlayerController);
                    if (flag2)
                    {
                        bool flag3 = projectile.collidesWithPlayer || projectile.Owner is AIActor;
                        if (flag3)
                        {
                            bool flag4 = !projectile.ImmuneToBlanks;
                            if (flag4)
                            {
                                if (Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.transform.position) <= 8)
                                {
                                    if (knownLowest > Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.LastPosition))
                                    {
                                        if(projectile != posibleTarget)
                                        {
                                            selected = projectile;
                                            knownLowest = Vector2.Distance(this.m_extantOrbital.GetComponent<SpeculativeRigidbody>().UnitCenter, projectile.LastPosition);
                                        }
                                        
                                    }

                                }
                            }
                        }
                    }
                }
            }



            return selected;
        }



        public static GameObject RenderLaserSight(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
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
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
            }
            return gameObject;
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