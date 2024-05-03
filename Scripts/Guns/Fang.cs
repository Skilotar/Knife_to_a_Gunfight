using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Fang : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Frumious Fang", "FrumiousFang");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:frumious_fang", "ski:frumious_fang");
            gun.gameObject.AddComponent<Fang>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Bite Worse Than Its Bark");
            gun.SetLongDescription("This blade deals Critical damage to beasts.\n\n" +

                "Spines that Rip and Jaws that tear!\n" +
                "Garumphing at its catch,\n" +
                "The Snarbolax you aught beware!\n" +
                "Or else you may be next.\n\n" +
                "" +
                "But to those a worthy foe\n" +
                "Who wrestle and banharb\n" +
                "A tooth to those he will bestow\n" +
                "A Frumious Fang, this Gnarled Barb!\n" +

                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "FrumiousFang_idle_001", 8);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 10);
            gun.SetAnimationFPS(gun.chargeAnimation, 10);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(126) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = .9f;
            gun.DefaultModule.numberOfShotsInClip = 400;
            gun.DefaultModule.cooldownTime = .6f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;
            
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1f, .5f, 0f);
            


            gun.muzzleFlashEffects = null;



            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 11f;
            projectile.baseData.speed = 10f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);




            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 15f;
            projectile2.baseData.speed = 50f;
            projectile2.baseData.range = 10f;


            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, 1f, 0f);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);

            projectile3.baseData.damage = 4f;
            projectile3.baseData.speed = 10f;
            projectile3.baseData.range = 6f;
            projectile3.baseData.force = 5;
            projectile3.SuppressHitEffects = true;
            projectile3.hitEffects.suppressMidairDeathVfx = true;
            
            projectile3.SetProjectileSpriteRight("Fang", 20, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 7);
            gun.muzzleFlashEffects = null;
            specialproj = projectile3;



            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .8f,


            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,

            };

            tk2dSpriteAnimationClip fireClip3 = gun.sprite.spriteAnimator.GetClipByName("FrumiousFang_charge");
            float[] offsetsX3 = new float[] { -0.7f, -0.8f, -0.8f };
            float[] offsetsY3 = new float[] { 0.0f, 0.0f, 0.0f };

            for (int i = 0; i < offsetsX3.Length && i < offsetsY3.Length && i < fireClip3.frames.Length; i++)
            {
                int id = fireClip3.frames[i].spriteId;
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY3[i];
            }

            VFXPool HorizVFXOBj = VFXToolbox.CreateVFXPool("Fang Tilemap VFX Horiz",
            new List<string>()
            {
                "Knives/Resources/Fang/Fang_001",
                "Knives/Resources/Fang/Fang_002",
                "Knives/Resources/Fang/Fang_003",
                "Knives/Resources/Fang/Fang_004",
            },
            10,
            new IntVector2(14, 8),
            tk2dBaseSprite.Anchor.MiddleLeft,
            false,
            0,
            true);

            projectile3.hitEffects.deathTileMapHorizontal = HorizVFXOBj;
            projectile3.hitEffects.tileMapHorizontal = HorizVFXOBj;

            projectile3.hitEffects.deathTileMapVertical = HorizVFXOBj;
            projectile3.hitEffects.tileMapVertical = HorizVFXOBj;

            //MiscToolMethods.TrimAllGunSprites(gun);
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public static Projectile specialproj;



        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            gun.PreventNormalFireAudio = true;

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            
            if (projectile.GetCachedBaseDamage == 11f)
            {
                //slash
                ComboManager(projectile);


            }
            if (projectile.GetCachedBaseDamage == 15f)
            {

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashRange = 3.6f;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 40;
                slasher.playerKnockback = 0;
                slasher.OnSlashHitEnemy += Slasher_OnSlashHitEnemy;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                StartCoroutine(SpikeBurst(player));
                StartCoroutine(comboCountdown());
                ComboNumber = 1;
                gun.DefaultModule.cooldownTime = .6f;
                AkSoundEngine.PostEvent("Play_PET_wolf_bite_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_PET_wolf_bite_01", base.gameObject);
            }

            base.PostProcessProjectile(projectile);
        }

        private void Slasher_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if (player != null)
            {
                if (hitEnemy != null)
                {

                    for (int i = 0; i < this.Beasts.Count; i++)
                    {
                        bool isin = hitEnemy.EnemyGuid == this.Beasts[i];

                        if (isin)
                        {
                            OhNoIveBeenImpaled(hitEnemy);
                        }
                    }
                }
            }
        }

        private void OhNoIveBeenImpaled(AIActor hitEnemy)
        {
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
            AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
            if (hitEnemy.healthHaver.IsAlive)
            {
                
                hitEnemy.healthHaver.ApplyDamage(30, Vector2.zero, "Gnarled_Barb");
            }
        }

        public int degree;
        private IEnumerator SpikeBurst(PlayerController player)
        {
            yield return new WaitForSeconds(.001f);
            degree = -16;
            for(int i = 1; i <= 9; i++)
            {
                GameObject gameObject = SpawnManager.SpawnProjectile(specialproj.gameObject, player.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle + degree));
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = (this.Owner as PlayerController);
                component.baseData.range = 6f;
                
                degree = degree + 4;

            }
           

        }

        public bool Animswap = false;
        public void ComboManager(Projectile projectile)
        {
            if (Animswap) // 2,3
            {
                gun.spriteAnimator.StopAndResetFrameToDefault();
                gun.spriteAnimator.Play("FrumiousFang_critical_fire");

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 45;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 30;
                slasher.SlashRange = 5.6f;
                slasher.slashKnockback = 7f;
                slasher.playerKnockback = 0;
                slasher.OnSlashHitEnemy += Slasher_OnSlashHitEnemy;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                slasher.SlashVFX.type = VFXPoolType.None;
                StartCoroutine(HandleDash(player));

                ComboNumber++;
                if(ComboNumber == 2 || ComboNumber == 3)
                {
                    StartCoroutine(slashInvuln(player));
                }
                if(ComboNumber >= 4)
                {
                    Animswap = false;
                    ComboNumber = 1;
                }
            }
            else // 1
            {
                gun.DefaultModule.cooldownTime = .6f;
                StartCoroutine(comboCountdown());
                ComboNumber = 1;
                extend = true;

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 65;
                slasher.SlashDamageUsesBaseProjectileDamage = false;
                slasher.SlashDamage = 20;
                slasher.SlashRange = 3.6f;
                slasher.playerKnockback = 0;
                slasher.slashKnockback = 7f;
                slasher.OnSlashHitEnemy += Slasher_OnSlashHitEnemy;
                slasher.SlashVFX.type = VFXPoolType.None;
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
               
            }

        }

        private IEnumerator slashInvuln(PlayerController player)
        {
            PassiveItem.IncrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));
            yield return new WaitForSeconds(.6f);
            PassiveItem.DecrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));
        }

        public IEnumerator HandleDash(PlayerController user)
        {

            float duration = .12f;
            float adjSpeed = 23;
            float elapsed = -BraveTime.DeltaTime;
            float angle = user.CurrentGun.CurrentAngle;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                user.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle).normalized * adjSpeed;
                yield return null;
            }

        }
        public int ComboNumber = 1;
        public bool extend = false;
        private IEnumerator comboCountdown()
        {
            Animswap = true;
            yield return new WaitForSeconds(.7f);
            if(ComboNumber == 1)
            {
                Animswap = false;
            }
            if(ComboNumber == 2)
            {
                if (extend)
                {
                    StartCoroutine(comboCountdown());
                    extend = false;
                }
                else
                {
                    Animswap = false;
                    ComboNumber = 1;
                }
                
            }
            if(ComboNumber == 3)
            {
                Animswap = false;
                ComboNumber = 1;

                gun.DefaultModule.cooldownTime = 1f;
            }

        }

        private bool HasReloaded;

        
        public override void  Update()
        {

            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

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

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {


                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                
                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }


        public List<string> Beasts = new List<string> // For the sake of consistancy Beast list includes humanoid and animalistic creatures
        {

            "044a9f39712f456597b9762893fbc19c",//grub
            "37340393f97f41b2822bc02d14654172",//creech
            "2feb50a6a40f4f50982e89fd276f6f15",//bullat
            "2d4f8b5404614e7d8b235006acde427a",//shotgat
            "b4666cb6ef4f4b038ba8924fd8adf38f",//grenat
            "7ec3e8146f634c559a7d58b19191cd43",//poofbat
            "1a4872dafdb34fd29fe8ac90bd2cea67",//kingbat
            "2ebf8ef6728648089babb507dec4edb7",//mimic
            "d8d651e3484f471ba8a2daa4bf535ce6",
            "abfb454340294a0992f4173d6e5898a8",
            "d8fd592b184b4ac9a3be217bc70912a2",
            "6450d20137994881aff0ddd13e3d40c8",
            "ac9d345575444c9a8d11b799e8719be0",
            "796a7ed4ad804984859088fc91672c7f",
            "479556d05c7c44f3b6abb3b2067fc778",//mimic
            "206405acad4d4c33aac6717d184dc8d4",//A_gunjur
            "c4fba8def15e47b297865b18e36cbef8",//gunjur
            "9b2cf2949a894599917d4d391a0b7394",//h_gunjur
            "56fb939a434140308b8f257f0f447829",//l_gunjur
            "ec8ea75b557d4e7b8ceeaacdf6f8238c",//nut
            "463d16121f884984abe759de38418e48",//chain
            "cf2b7021eac44e3f95af07db9a7c442c",//singer
            "c50a862d19fc4d30baeba54795e8cb93",
            "b1540990a4f1480bbcb3bea70d67f60d",
            "8b4a938cdbc64e64822e841e482ba3d2",
            "ba657723b2904aa79f9e51bce7d23872",
            "57255ed50ee24794b7aac1ac3cfb8a95",//singer
            "ed37fa13e0fa4fcf8239643957c51293",//bird
            "45192ff6d6cb43ed8f1a874ab6bef316",//beast
            "98ca70157c364750a60f5e0084f9d3e2",//spider
            "6e972cd3b11e4b429b888b488e308551",//frog
            "8a9e9bedac014a829a48735da6daf3da",//redfrog
            "80ab6cd15bfc46668a8844b2975c6c26",//cameleon
            "4b21a913e8c54056bc05cafecf9da880",//fancy bird
            "e861e59012954ab2b9b6977da85cb83c",//snek
            "3e98ccecf7334ff2800188c417e67c15",//squidgames
            

            //bosses
            "6868795625bd46f3ae3e4377adce288b",//rat
            "ec6b674e0acd4553b47ee94493d66422",//gull
            "8b0dd96e2fe74ec7bebc1bc689c0008a",//Squidgames 2
            "465da2bb086a4a88a803f79fe3a27677",//dragun
            "05b8afe0b6cc4fffa9dc6036fa24c8ec",


        };

    }
}