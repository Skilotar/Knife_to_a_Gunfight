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

    public class Phalanx : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Phalanx", "Phalanx_gun");
            Game.Items.Rename("outdated_gun_mods:phalanx", "ski:phalanx");
            gun.gameObject.AddComponent<Phalanx>();
            gun.SetShortDescription("Hold Formation!");
            gun.SetLongDescription("Shoots Fragile Blocklets. \n\n" +
                "The perfect miliary formation perfected by the Great Bromans millennia ago. " +
                "The stance was so inspiring that the fearsome Iron Legion Swears by it; integrating it into their weapons and ships. " +
                "Good ol Phalanx! Nothing beats that! " +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Phalanx_gun_idle_001", 8);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(51) as Gun).gunSwitchGroup;
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.idleAnimation, 5);
            
            for (int i = 0; i < 12; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            }
            int increment = 0;
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {

                mod.ammoCost = 1;
                mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = .5f;
                mod.angleVariance = 0f;
                mod.numberOfShotsInClip = 8;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 2f;
                projectile.AdditionalScaleMultiplier *= .8f;
                BlockletsModifier block = projectile.gameObject.GetOrAddComponent<BlockletsModifier>();
                block.DieWithShield = true;
                float speedboost = 0;
                if (increment >= 3) speedboost = 0;
                if (increment <= 4 && increment >= 6) speedboost = .1f;
                if (increment <= 7 && increment >= 9) speedboost = .2f;
                if (increment <= 10 && increment >= 12) speedboost = .3f;
                projectile.baseData.speed *= .55f + speedboost;
                projectile.shouldRotate = true;
                mod.positionOffset = offsets[increment];
                mod.angleFromAim = offsetAngles[increment];
                
                if (mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
                increment++;
                projectile.transform.parent = gun.barrelOffset;
            }

            gun.reloadTime = 1.2f;
            gun.barrelOffset.transform.position = new Vector3(1.5f, .7f, 0f);
            gun.SetBaseMaxAmmo(250);
            gun.gunClass = GunClass.SHOTGUN;
            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
            
            ID = gun.PickupObjectId;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }

        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                
            }

            base.Update();
        }

        public static int ID;

        public static Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>
        {
            { 0,new Vector2(0,0)},
            { 1,new Vector2(0,0.3f)},
            { 2,new Vector2(0,-0.3f)},
            { 3,new Vector2(0.5f,0)},
            { 4,new Vector2(0.5f,0.3f)},
            { 5,new Vector2(0.5f,-0.3f)},
            { 6,new Vector2(1f,0)},
            { 7,new Vector2(1f,0.3f)},
            { 8,new Vector2(1f,-0.3f)},
            { 9,new Vector2(1.5f,0)},
            { 10,new Vector2(1.5f,0.3f)},
            { 11,new Vector2(1.5f,-0.3f)},
        };

        public static Dictionary<int, float> offsetAngles = new Dictionary<int, float>
        {
            { 0,0},
            { 1,2},
            { 2,-2},
            { 3,0},
            { 4,2},
            { 5,-2},
            { 6,0},
            { 7,2},
            { 8,-2},
            { 9,0},
            { 10,2},
            { 11,-2},
        };

        
    }

    public class BlockletsModifier : MonoBehaviour
    {
        // Token: 0x0600086B RID: 2155 RVA: 0x0006760C File Offset: 0x0006580C
        public BlockletsModifier()
        {

        }

        // Token: 0x0600086C RID: 2156 RVA: 0x0006765B File Offset: 0x0006585B
        private void Start()
        {
            parentProjectile = base.GetComponent<Projectile>();

            Vector3 pointInSpace = parentProjectile.LastPosition;
            Gun owner = PickupObjectDatabase.GetById(380) as Gun;
            GameObject gameObject1 = owner.ObjectToInstantiateOnReload.gameObject;
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject1, pointInSpace, Quaternion.identity);
            m_shield = gameObject2;
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(pointInSpace, tk2dBaseSprite.Anchor.MiddleCenter);
            m_shield.transform.localScale = new Vector3(.5f, .5f, 1);
            MajorBreakable breakable = m_shield.GetOrAddComponent<MajorBreakable>();
            breakable.MaxHitPoints = 2f;
            breakable.HitPoints = 2;
            breakable.breakVfx = null;
            breakable.OnDamaged += this.TaskManagerKillHim;
            parentProjectile.OnDestruction += ParentProjectile_OnDestruction;
        }

        private void TaskManagerKillHim(float obj)
        {
            if (m_shield.GetOrAddComponent<MajorBreakable>().HitPoints <= 0f)
            {
                UnityEngine.GameObject.Destroy(m_shield);
            }
        }

        private void ParentProjectile_OnDestruction(Projectile obj)
        {
            UnityEngine.GameObject.Destroy(m_shield);
        }

        private void Update()
        {
            bool flag = this.parentProjectile != null;
            if (flag)
            {
                Vector3 pointInSpace = parentProjectile.LastPosition;
                if(m_shield != null)
                {
                    
                    m_shield.transform.position = pointInSpace;
                    m_shield.GetComponent<SpeculativeRigidbody>().transform.position = pointInSpace;
                    m_shield.GetComponent<SpeculativeRigidbody>().Reinitialize();
                    
                }
                else
                {
                    if (DieWithShield)
                    {
                        parentProjectile.hitEffects.overrideMidairDeathVFX = null;
                        parentProjectile.DieInAir();
                    }
                }
                
            }
        }
        
        public Projectile parentProjectile;
        public GameObject gameObject;
        public GameObject m_shield;
        public bool DieWithShield = false;
    }
}