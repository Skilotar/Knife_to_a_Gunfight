using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections.Generic;
using Alexandria.ItemAPI;

namespace Knives
{
    public class CharmBeamJankComp : MonoBehaviour
    {

        public CharmBeamJankComp()
        {
            // I hate Beams
            // Hate Beams
        }
        private void Start()
        {
            try
            {
                m_projectile = base.GetComponent<Projectile>();
                BasebeamComp = base.GetComponent<BasicBeamController>();
                beamcomp = base.GetComponent<BeamController>();

                //GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        private void Instance_OnNewLevelFullyLoaded()
        {
            if(GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                type = CharmType.None;
            }
        }

        public void Update()
        {
           
           
            if (player != null)
            {
                if(m_projectile != null)
                {
                    if (set == false)
                    {
                        m_projectile.OnHitEnemy += this.onhitenemy;
                        set = true;

                        if (type == CharmBeamJankComp.CharmType.Target)
                        {
                            HomingModifier Home = m_projectile.gameObject.GetOrAddComponent<HomingModifier>();
                            Home.HomingRadius = 5;
                            Home.AngularVelocity = 200;
                        }

                        if (type == CharmBeamJankComp.CharmType.Gnome)
                        {
                           // Lol Nope
                        }

                        if (type == CharmBeamJankComp.CharmType.Clock)
                        {
                           
                            ReverseSpeedRampMod ramp = m_projectile.gameObject.GetOrAddComponent<ReverseSpeedRampMod>();
                            ramp.isCharm = true;
                            ramp.doeffects = false;
                        }
                    }
                }
            }
 
            if( internalCooldown > 0)
            {
                internalCooldown -= Time.deltaTime;
            }
            

            if(type == CharmType.None) // You should UnityEngine.GameObject.Destroy(); yourself. NOW!!!!
            {
                set = false;
                m_projectile.OnHitEnemy -= this.onhitenemy;
                //ETGModConsole.Log("LightningStrike");
                UnityEngine.GameObject.Destroy(this);
            }
            
        }
        private float internalCooldown = 0f;
        private int hitsToProc = 50;
        public bool set = false;
        public AIActor effected;
        private void onhitenemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3) // Beams not random instead doing a buildup to proc.  This is to keep it from generating a random number every beamtick
        {
            if(arg2.aiActor != null)
            {
                if (internalCooldown <= 0)
                {
                    if (type == CharmBeamJankComp.CharmType.Knife)
                    {
                        if (hitsToProc <= 0)
                        {
                            Projectile swipe = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, arg2.UnitCenter, player, player.CurrentGun.CurrentAngle);
                            ProjectileSlashingBehaviour stabby = swipe.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                            stabby.playerKnockback = 0;
                            stabby.DestroyBaseAfterFirstSlash = true;
                            stabby.SlashDamageUsesBaseProjectileDamage = false;
                            stabby.SlashDamage = Mathf.CeilToInt(arg1.baseData.damage * .2f);
                            stabby.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                            stabby.SlashVFX = (PickupObjectDatabase.GetById(335) as Gun).muzzleFlashEffects;
                            internalCooldown = 1;
                            hitsToProc = 50;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Poma)
                    {

                        if (hitsToProc <= 0)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Projectile pop1 = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, arg2.UnitCenter, player, player.CurrentGun.CurrentAngle + (i * 60));
                                pop1.HasDefaultTint = true;
                                pop1.DefaultTintColor = ExtendedColours.vibrantOrange;
                                PierceProjModifier poke = pop1.gameObject.GetOrAddComponent<PierceProjModifier>();
                                poke.penetration++;
                                pop1.OnHitEnemy += popcollide;
                                effected = arg2.aiActor;
                            }


                            internalCooldown = .5f;
                            hitsToProc = 50;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Mouse)
                    {

                        if (arg3 == true)
                        {
                            RatCrown.SpawnRat(this.player, arg2.aiActor.sprite.WorldCenter);
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Pepper)
                    {
                        if (hitsToProc <= 0)
                        {
                            arg2.specRigidbody.aiActor.ApplyEffect(StaticStatusEffects.hotLeadEffect);

                            internalCooldown = 1f;
                            hitsToProc = 70;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Ice)
                    {
                        if (hitsToProc <= 0)
                        {
                            arg2.specRigidbody.aiActor.ApplyEffect(StaticStatusEffects.frostBulletsEffect);

                            internalCooldown = .5f;
                            hitsToProc = 25;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Shroom)
                    {
                        if (hitsToProc <= 0)
                        {
                            arg2.specRigidbody.aiActor.ApplyEffect(StaticStatusEffects.irradiatedLeadEffect);

                            internalCooldown = 1f;
                            hitsToProc = 70;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Whammy)
                    {
                        if (hitsToProc <= 0)
                        {
                            Projectile yari = ((Gun)PickupObjectDatabase.GetById(16)).DefaultModule.projectiles[0].projectile;
                            Projectile whammy = MiscToolMethods.SpawnProjAtPosi(yari, this.player.CurrentGun.barrelOffset.transform.position, player, this.player.CurrentGun, 40);
                            whammy.baseData.damage *= .5f;
                            whammy.AdditionalScaleMultiplier *= .8f;
                            whammy.DefaultTintColor = ExtendedColours.lime;
                            whammy.HasDefaultTint = true;

                            internalCooldown = .25f;
                            hitsToProc = 25;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Whammy)
                    {
                        if (hitsToProc <= 0)
                        {
                            Projectile yari = ((Gun)PickupObjectDatabase.GetById(16)).DefaultModule.projectiles[0].projectile;
                            Projectile whammy = MiscToolMethods.SpawnProjAtPosi(yari, this.player.CurrentGun.barrelOffset.transform.position, player, this.player.CurrentGun, 40);
                            whammy.baseData.damage *= .5f;
                            whammy.AdditionalScaleMultiplier *= .8f;
                            whammy.DefaultTintColor = ExtendedColours.lime;
                            whammy.HasDefaultTint = true;

                            internalCooldown = .5f;
                            hitsToProc = 25;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Meat)
                    {
                        if (arg3 == true)
                        {
                            if (UnityEngine.Random.Range(1, 10) <= 1)
                            {
                                LootEngine.SpawnItem(PickupObjectDatabase.GetById(MeatPickup.ID).gameObject, arg2.aiActor.sprite.WorldCenter, Vector2.zero, 1, true, true);
                                hitsToProc = 5;
                            }
                        }

                    }

                    if (type == CharmBeamJankComp.CharmType.Slime)
                    {
                        if (hitsToProc <= 0)
                        {
                            arg2.aiActor.ApplyEffect(StaticStatusEffects.tripleCrossbowSlowEffect);

                            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.BlobulonGoopDef);
                            goopManagerForGoopType.TimedAddGoopCircle(arg2.aiActor.sprite.WorldCenter, 3f, 1f, false);

                           
                            hitsToProc = 15;
                        }
                        else
                        {
                            hitsToProc--;
                        }


                    }

                    if (type == CharmBeamJankComp.CharmType.Radioactive)
                    {
                        if (hitsToProc <= 0)
                        {
                            arg2.aiActor.ApplyEffect(StaticStatusEffects.irradiatedLeadEffect);
                            arg2.aiActor.ApplyEffect(new GameActorillnessEffect());



                            internalCooldown = 1f;
                            hitsToProc = 50;
                        }
                        else
                        {
                            hitsToProc--;
                        }

                        if (arg3 == true) // Kill detection is so bad it basically works as a random chance   lol
                        {

                            DoSafeExplosion(arg2.UnitCenter);
                            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
                            GameObject Nuke = assetBundle.LoadAsset<GameObject>("assets/data/vfx prefabs/impact vfx/vfx_explosion_nuke.prefab");
                            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Nuke);

                            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(arg2.UnitCenter, tk2dBaseSprite.Anchor.LowerCenter);

                            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                            {
                                float FlashHoldtime = 0.1f;
                                float FlashFadetime = 0.3f;
                                Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashHoldtime);
                                StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false);
                            }

                            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef);
                            goopManagerForGoopType.TimedAddGoopCircle(arg2.UnitCenter, 5f, 2f, false);

                        }

                    }

                    if (type == CharmBeamJankComp.CharmType.Target || type == CharmBeamJankComp.CharmType.Lucky)
                    {
                        if (hitsToProc <= 0)
                        {
                            if(type == CharmBeamJankComp.CharmType.Target)
                            {
                                arg2.aiActor.healthHaver.ApplyDamage(20, Vector2.zero, "Lucky?");
                            }
                            else
                            {
                                arg2.aiActor.healthHaver.ApplyDamage(30, Vector2.zero, "Lucky?");
                            }
                            
                            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                            arg2.aiActor.PlayEffectOnActor(EasyVFXDatabase.SmallMagicPuffVFX, new Vector3(0, 0, 0));
                            internalCooldown = 1f;
                            hitsToProc = 1000;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Rubber)
                    {
                        if (hitsToProc <= 0)
                        {
                            AkSoundEngine.PostEvent("Play_Ducky", base.gameObject);
                            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.WaterGoop);
                            goopManagerForGoopType.TimedAddGoopCircle(arg2.UnitCenter, 1f, 2f, false);

                            Projectile ducky = MiscToolMethods.SpawnProjAtPosi(RubberCharm.ducky, arg2.specRigidbody.UnitCenter, player, UnityEngine.Random.Range(0, 360));
                            BounceProjModifier bnc = ducky.gameObject.GetOrAddComponent<BounceProjModifier>();
                            bnc.bouncesTrackEnemies = true;
                            bnc.bounceTrackRadius = 40;
                            bnc.TrackEnemyChance = 100;

                            internalCooldown = 1f;
                            hitsToProc = 60;
                        }
                        else
                        {
                            hitsToProc--;
                        }
                    }

                    if (type == CharmBeamJankComp.CharmType.Clock)
                    {

                    }
                }
            }
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
            damageRadius = 20,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 80f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 40f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,

        };

        private void popcollide(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if(arg2.aiActor == effected)
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }

        public static bool Toggle = false;
        
        public Projectile m_projectile;
        private BasicBeamController BasebeamComp;
        public BeamController beamcomp;
        public Projectile m_orb;
        public PlayerController player;

        public CharmBeamJankComp.CharmType type = CharmType.None;
        public enum CharmType
        {
            Gnome,
            Knife,
            Poma,
            Mouse,
            Pepper,
            Shroom,
            Ice,
            Target,
            Whammy,
            Dango,
            Clock,
            Lucky,
            Rubber,
            Battery,
            Piggy,
            Coffee,
            Metronome,
            Feather,
            Meat,
            Slime,
            Radioactive,
            None,
        }
    }
}