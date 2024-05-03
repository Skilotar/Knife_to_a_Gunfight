using System;
using System.Collections;
using System.Collections.Generic;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class CarvingKnife : AdvancedGunBehaviour
    {
        //fix visual/ jsons
        //fix slash
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Carving Knife", "mhknife");
            Game.Items.Rename("outdated_gun_mods:carving_knife", "mhi:carving_knife");
            gun.gameObject.AddComponent<CarvingKnife>();
            gun.SetShortDescription("You get 3 carves.");
            gun.SetLongDescription("\nA simple carving knife used by hunters to gather resources from slain monsters. Not recommended for combat." + "\n\n- Monster Hunter Items");
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.SetupSprite(null, "mhknife_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            
            tk2dSpriteAnimationClip animationclip = gun.sprite.spriteAnimator.GetClipByName(gun.idleAnimation);
            float[] offsetsX = new float[] { -0.3750f };
            float[] offsetsY = new float[] { -0.3125f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < animationclip.frames.Length; i++)
            {
                int id = animationclip.frames[i].spriteId;
                tk2dSpriteDefinition def = animationclip.frames[i].spriteCollection.spriteDefinitions[id];
                Vector3 offset = new Vector2(offsetsX[i], offsetsY[i]);
                def.position0 += offset;
                def.position1 += offset;
                def.position2 += offset;
                def.position3 += offset;
            }

            animationclip = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            offsetsX = new float[] { -0.4375f, -0.4375f, -0.4375f, -0.4375f, -0.5000f, -0.8125f, -0.7500f, -0.7500f, -0.8750f, -0.8750f, -0.8125f, -0.7500f, -0.7500f, -0.8125f };
            offsetsY = new float[] { -1.1875f, -1.2500f, -0.9375f, -0.9375f, -1.0000f, -1.0000f, -1.1250f, -1.1250f, -1.1250f, -1.1250f, -1.2500f, -1.3750f, -1.3125f, -1.3125f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < animationclip.frames.Length; i++)
            {
                int id = animationclip.frames[i].spriteId;
                tk2dSpriteDefinition def = animationclip.frames[i].spriteCollection.spriteDefinitions[id];
                Vector3 offset = new Vector2(offsetsX[i], offsetsY[i]);
                def.position0 += offset;
                def.position1 += offset;
                def.position2 += offset;
                def.position3 += offset;
            }

            animationclip = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
            offsetsX = new float[] { -0.5000f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3750f, -0.4375f, -0.5000f, -0.5000f, -0.5000f };
            offsetsY = new float[] { -0.3750f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3125f, -0.3750f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < animationclip.frames.Length; i++)
            {
                int id = animationclip.frames[i].spriteId;
                tk2dSpriteDefinition def = animationclip.frames[i].spriteCollection.spriteDefinitions[id];
                Vector3 offset = new Vector2(offsetsX[i], offsetsY[i]);
                def.position0 += offset;
                def.position1 += offset;
                def.position2 += offset;
                def.position3 += offset;
            }
            gun.PreventNormalFireAudio = true;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.cooldownTime = 1.9f;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.reloadTime = 1.4f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.PreventNormalFireAudio = true;
            gun.quality = PickupObject.ItemQuality.D;
            gun.encounterTrackable.EncounterGuid = "three carves";
            ETGMod.Databases.Items.Add(gun, null, "ANY");



        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.SuppressHitEffects = true;
            projectile.sprite.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.DieInAir();
            StartCoroutine(slightslashdelay());
           
            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
        }
       
        public IEnumerator slightslashdelay()
        {
            yield return new WaitForSeconds(.1f);

            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f, .5f, 0), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.baseData.damage = 5;
            component.Owner = this.gun.CurrentOwner;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();

            slashy.SlashDamageUsesBaseProjectileDamage = true;
            slashy.SlashRange = 3.5f;
            slashy.SlashDimensions = 80;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            slashy.SlashVFX.type = VFXPoolType.None;
            
            AkSoundEngine.PostEvent("Play_mhw_carve", gameObject);
        }

        public override void  Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                

            }

            base.Update();
        }
    }
}