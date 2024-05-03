using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;

namespace Knives
{
    class Pikannon : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Pikannon", "Pikannon");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:pikannon", "ski:pikannon");
            gun.gameObject.AddComponent<Pikannon>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Flower Power");
            gun.SetLongDescription("Command your troop of Pikis! Recall them with reload. Charge attack to focus the assault. \n\n" +
                "A strange red device with various squeaks coming from inside. The creatures it produces are loyal to a fault they will rush headlong into danger without a single hesitation. \n\n" +
                "They trust you dearly. Please take care to keep them alive." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Pikannon_idle_001", 6);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 18);
            gun.SetAnimationFPS(gun.chargeAnimation, 15);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.DefaultModule.ammoCost = 0;
            gun.DefaultModule.angleVariance = 5;
            gun.gunClass = GunClass.NONE;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 77;
            gun.DefaultModule.cooldownTime = .45f;
            gun.SetBaseMaxAmmo(77);
            gun.CanReloadNoMatterAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(5, 0);
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 2f;
            projectile.baseData.speed *= 1.2f;
            projectile.baseData.range = 11f;
            projectile.objectImpactEventName = null;
            projectile.transform.parent = gun.barrelOffset;
            projectile.SetProjectileSpriteRight("Piki", 22, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 17, 11);
            projectile.shouldRotate = true;
            projectile.hitEffects.suppressMidairDeathVfx = true;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Pikis", "Knives/Resources/piki_clipfull", "Knives/Resources/piki_clipempty");

            

            Piki = projectile.gameObject.GetOrAddComponent<PikiProjMod>();


            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(181) as Gun).DefaultModule.projectiles[0].projectile);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            
            projectile2.baseData.damage = 0f;
            projectile2.baseData.speed = 50f;
            projectile2.baseData.range = 40f;
            projectile2.gameObject.GetOrAddComponent<projectileStates>().OnionCharged = true;
            
            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .4f,
                AmmoCost = 0,

            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,

            };

            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;
        }

        public static int ID;

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool fatal)
        {
            
            if (arg2.aiActor != null)
            {
                if (arg2.aiActor.healthHaver != null)
                {
                    this.m_extantLockOnSprite = null;
                    foreach (AIActor piki in GetPikis(arg2.UnitCenter, 40))
                    {
                        piki.gameObject.GetOrAddComponent<AiactorSpecialStates>().PikiOverrideTarget = arg2.aiActor;
                    }
                    this.m_extantLockOnSprite = arg2.aiActor.PlayEffectOnActor((GameObject)BraveResources.Load("Global VFX/VFX_LockOn_Predator", ".prefab"), Vector3.zero, true, true, true);
                }

            }
        }
        private GameObject m_extantLockOnSprite;
        public static PikiProjMod Piki;

        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }



        public override void OnPostFired(PlayerController player, Gun gun)
        {



        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {

            if(projectile.gameObject.GetComponent<PikiProjMod>() != null)
            {
                AkSoundEngine.PostEvent("Play_Piki_Shot_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_Pikannon_fire_001", base.gameObject);
                gun.LoseAmmo(1);
                switch ((int)UnityEngine.Random.Range(1, 6))
                {
                    case 1:
                        projectile.HasDefaultTint = true;
                        projectile.DefaultTintColor = UnityEngine.Color.red;
                        projectile.gameObject.GetComponent<PikiProjMod>().Pikitospawn = EasyPikiDatabase.RedBoy;
                        break;
                    case 2:
                        projectile.baseData.range *= 1.5f;
                        projectile.ResetDistance();
                        projectile.gameObject.GetComponent<PikiProjMod>().Pikitospawn = EasyPikiDatabase.YellowBoy;
                        break;
                    case 3:
                        projectile.HasDefaultTint = true;
                        projectile.DefaultTintColor = UnityEngine.Color.blue;
                        projectile.baseData.range *= .7f;
                        projectile.ResetDistance();
                        projectile.gameObject.GetComponent<PikiProjMod>().Pikitospawn = EasyPikiDatabase.BlueBoy;
                        break;
                    case 4:
                        projectile.baseData.damage *= 2f;
                        projectile.HasDefaultTint = true;
                        projectile.DefaultTintColor = new Color(.23f,0,.55f);
                        projectile.baseData.range *= .5f;
                        projectile.ResetDistance();
                        projectile.gameObject.GetComponent<PikiProjMod>().Pikitospawn = EasyPikiDatabase.PurpleBoy;
                        break;
                    case 5:
                        
                        projectile.HasDefaultTint = true;
                        projectile.DefaultTintColor = UnityEngine.Color.white;
                        projectile.baseData.range *= 1.6f;
                        projectile.ResetDistance();
                        projectile.gameObject.GetComponent<PikiProjMod>().Pikitospawn = EasyPikiDatabase.WhiteBoy;
                        break;
                }
            }

            if (projectile.gameObject.GetOrAddComponent<projectileStates>().OnionCharged)
            {
                
                projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));

            }

            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            gun.spriteAnimator.StopAndResetFrameToDefault();
            gun.spriteAnimator.Play("Pikannon_reload");
            
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);
            AkSoundEngine.PostEvent("Play_Pikannon_reload_001", base.gameObject);
            bool flag2 = player.CurrentRoom != null;
            if (flag2)
            {
                
                foreach (AIActor piki in GetPikis(player.CenterPosition, 6))
                {
                    ProcessEnemy(piki);
                }
               

            }


        }

        private static List<AIActor> GetPikis(Vector2 Posi, int Radius)
        {
           
            List<AIActor> list = new List<AIActor>();
            var AllEnemies = StaticReferenceManager.AllEnemies;
            for (int i = 0; i < AllEnemies.Count; i++)
            {
                AIActor Actor = AllEnemies[i];
                if (Actor != null && Actor.gameObject.GetComponent<CompanionController>() != null)
                {

                    if (Vector2.Distance(Actor.CenterPosition, Posi) < Radius)
                    {
                        list.Add(Actor);

                    }
                }
            }

            return list;
        }

        private void ProcessEnemy(AIActor target)
        {
           
           
            bool health_check = !target.healthHaver.IsBoss;
            
            if (TargetPiki.Contains(target.aiActor.EnemyGuid) && health_check)
            {

                this.gun.GainAmmo(1);
                this.gun.MoveBulletsIntoClip(1);
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                target.GetComponent<AiactorSpecialStates>().pikiSuck = true;
                target.EraseFromExistence(true);

            }


        }

        private IEnumerator HandleEnemySuck(AIActor target)
        {
            //ETGModConsole.Log("StartSuck");
            PlayerController player = this.Owner as PlayerController;
            target.ParentRoom = player.CurrentRoom;
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag = this.gun && copySprite;
                if (flag)
                {
                    Vector3 position = this.gun.PrimaryHandAttachPoint.position;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
           
            //ETGModConsole.Log("EndSuck");
            yield break;
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


        public override void OnPostDrop(GameActor owner)
        {

        }

        private bool HasReloaded;
        public override void Update()
        {
            if (gun.CurrentOwner && GameManager.Instance.CurrentFloor != 0 && GameManager.Instance.IsLoadingLevel == false)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }



        public List<string> TargetPiki = new List<string>
        {
            "ski:RedPiki",
            "ski:YellowPiki",
            "ski:BluePiki",
            "ski:PurplePiki",
            "ski:WhitePiki"
        };
        
    }
}