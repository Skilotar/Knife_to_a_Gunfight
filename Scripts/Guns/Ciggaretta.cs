using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;


namespace Knives
{

    public class Cigaretta : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Cigaretta", "cigaretter");
            Game.Items.Rename("outdated_gun_mods:cigaretta", "ski:cigaretta");
            gun.gameObject.AddComponent<Cigaretta>();
            gun.SetShortDescription("Second Hand");
            gun.SetLongDescription("Makes Enemies Cooler. Enemies may be rewarded items on a successful hit or on death.\n" +
                "They can't use these items, but the gungeon doesn't know that.." +
                "" +
                "A novelty cigarette launcher of earth circa 1923. The cardboard frame has seen better days, but it still works like a charm." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "cigaretter_idle_001", 8);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(335) as Gun).gunSwitchGroup;
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.SetAnimationFPS(gun.idleAnimation, 5);
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            for (int i = 0; i < 2; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            }

            //GUN STATS
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {

                mod.ammoCost = 2;
                mod.shootStyle = ProjectileModule.ShootStyle.Burst;
                mod.burstCooldownTime = .1f;
                mod.burstShotCount = 1;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = .25f;
                mod.angleVariance = 4f;
                mod.numberOfShotsInClip = 6;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 3f;
                projectile.baseData.speed *= 1f;
                projectile.shouldRotate = true;
                projectile.shouldFlipVertically = true;
                projectile.objectImpactEventName = "";
                projectile.SetProjectileSpriteRight("Ciggy", 7, 4, false, tk2dBaseSprite.Anchor.MiddleCenter, 7, 4);
                projectile.gameObject.GetOrAddComponent<EnemyCoolnessStatusApply>();

                mod.angleFromAim = 2;
                if (mod != gun.DefaultModule)
                {
                    mod.positionOffset = new Vector2(0, -.1f);
                    mod.ammoCost = 0;
                    mod.angleFromAim = -2;
                }
                projectile.transform.parent = gun.barrelOffset;
            }

            gun.reloadTime = 1f;
            gun.barrelOffset.transform.position = new Vector3(27 / 16f, 12 / 16f, 0 / 16f);
            gun.SetBaseMaxAmmo(600);
            gun.ammo = 600;
            gun.gunClass = GunClass.PISTOL;
            gun.quality = PickupObject.ItemQuality.C;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Don't Smoke Kids :)", "Knives/Resources/Ciggy_clipfull", "Knives/Resources/Ciggy_clipempty");

            MiscToolMethods.TrimAllGunSprites(gun);


            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);

            projectile2.baseData.damage = 1;
            projectile2.baseData.speed *= 1f;
            projectile2.baseData.range = 20f;
            projectile2.SuppressHitEffects = true;
            projectile2.HasDefaultTint = true;
            projectile2.DefaultTintColor = UnityEngine.Color.grey;
            projectile2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            projectile2.sprite.renderer.material.SetFloat("_VertexColor", 1f);
            projectile2.sprite.color = projectile2.sprite.color.WithAlpha(0.75f);
            projectile2.sprite.usesOverrideMaterial = true;
            projectile2.SetProjectileSpriteRight("steam", 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 12, 12);
            projectile2.SuppressHitEffects = true;
            projectile2.hitEffects.suppressHitEffectsIfOffscreen = true;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.objectImpactEventName = null;


            PierceProjModifier stabby = projectile2.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetratesBreakables = true;
            stabby.penetration = 5;
            BounceProjModifier bnc = projectile2.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;

            smonk = projectile2;
            ID = gun.PickupObjectId;
        }
        public static Projectile smonk;
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }
        public static int ID;


        public override void Update()
        {
            if(this.gun.CurrentOwner != null)
            {
                if (((PlayerController)this.gun.CurrentOwner).PlayerHasActiveSynergy("Chain Smoker"))
                {
                    this.gun.DefaultModule.burstShotCount = 1 + (int)((PlayerController)this.gun.CurrentOwner).stats.GetStatValue(PlayerStats.StatType.Coolness);
                }
                else
                {
                    this.gun.DefaultModule.burstShotCount = 1;
                }
            }

            base.Update();
        }

        public class EnemyCoolnessStatusApply : MonoBehaviour
        {
            public EnemyCoolnessStatusApply()
            {

            }

            private void Start()
            {
                
                this.m_projectile = base.GetComponent<Projectile>();
                if (this.m_projectile.Owner is PlayerController)
                {
                    this.projOwner = this.m_projectile.Owner as PlayerController;
                }
                m_projectile.OnHitEnemy += this.CheckAndApplyCool;
            }

            private void CheckAndApplyCool(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
            {
                if(arg2.aiActor != null)
                {
                    AIActor hit = arg2.aiActor;
                    if (hit.gameObject.GetComponent<AiactorSpecialStates>() == null)
                    {
                        hit.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                    }
                    AiactorSpecialStates state = hit.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                    if (state.EnemyIsCool == false)
                    {
                        if (hit.IsBlackPhantom)
                        {
                            state.EnemyIsCool = true;
                            state.EnemyCoolnessLevel = -6;
                        }
                        else
                        {
                            state.EnemyIsCool = true;
                            state.EnemyCoolnessLevel = 0;
                        }
                    }

                    state.EnemyCoolnessLevel++;

                    if(hit.IsBlackPhantom && state.EnemyCoolnessLevel >= 1)
                    {
                        hit.UnbecomeBlackPhantom();
                    }

                    if(state.EnemyCoolnessLevel >= 6)
                    {
                        CoolEnemiesStatusController boom = hit.gameObject.GetOrAddComponent<CoolEnemiesStatusController>();
                        boom.statused = true;
                    }

                    if (state.EnemyCoolnessLevel >= 30)
                    {
                        state.EnemyCoolnessLevel -= 10;
                        hit.healthHaver.ApplyDamage(20, Vector2.zero, "Maximum Lung Cancer", CoreDamageTypes.Poison, DamageCategory.Unstoppable, true);

                        GlobalSparksDoer.DoRadialParticleBurst((int)(state.EnemyCoolnessLevel), hit.specRigidbody.UnitBottomLeft, hit.specRigidbody.UnitTopRight, 360, 10, 0, null, null, UnityEngine.Color.grey, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                    }
                }
            }

            private void Update()
            {
                
            }
           
            
            private PlayerController projOwner;
            private Projectile m_projectile;
        }
    }

}
