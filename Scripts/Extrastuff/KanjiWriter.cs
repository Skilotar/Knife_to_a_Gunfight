using System;
using System.Collections;
using Dungeonator;
using UnityEngine;

using ItemAPI;
using System.Collections.Generic;

namespace Knives
{
	public class KanjiWriterProjMod : MonoBehaviour
	{

		public KanjiWriterProjMod()
		{

		}
		Color color = new Color();
		public void Start()
		{
			projectile = base.gameObject.GetComponent<Projectile>();
			projectile.OnHitEnemy += this.onHitEnemy;
			Owner = (PlayerController)projectile.Owner;


            if (Painter)
            {
				color = UnityEngine.Random.ColorHSV(0, 1, .4f, .9f, .5f, .9f);
				projectile.sprite.color = color;
				ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
				trail.shadowLifetime = .05f;
				trail.shadowTimeDelay = .01f;
				trail.dashColor = color;
				trail.spawnShadows = true;

			}

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage = 0f;
            projectile2.baseData.speed = 20f;
            projectile2.baseData.range = 1;
            projectile2.AdditionalScaleMultiplier = .01f;


			Kproj = projectile2;


        }

        private void onHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (arg3)
                {
                    if(UnityEngine.Random.Range(0, 2) < 1)
					{
						PickAndWriteSymbol(arg2.UnitCenter);
					}
					
                }
				if (Painter)
                {

					arg2.aiActor.RegisterOverrideColor(color, "painter");
				}
            }
        }
		System.Random random = new System.Random();
		
		public void PickAndWriteSymbol(Vector2 posi)
        {
			
			
            switch (TaggAr.TaggARCount)
            {
				case 1: StartCoroutine(Fire(posi));
					TaggAr.TaggARCount++;
					break;
				case 2: StartCoroutine(Poison(posi));
					TaggAr.TaggARCount++;
					break;
				case 3: StartCoroutine(Heart(posi));
					TaggAr.TaggARCount++;
					break;
				case 4: StartCoroutine(Web(posi));
					TaggAr.TaggARCount = 1;
					break;
			}	
			


        }

        private IEnumerator Fire(Vector2 posi)
        {
			Vector2 vecsave = posi;
			Projectile comp1 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(-1.8f, 1.5f),Owner,0f,0,1,false);
			applytrail(comp1, 0);
			
			Projectile comp2 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(1.8f, 1.5f), Owner, 180f, 0, 1, false);
			applytrail(comp2, 0);
			
			Projectile comp3 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0, 3), Owner, 270f, 0, 1, false);
			comp3.baseData.range = 2.75f;
			comp3.ResetDistance();
			applytrail(comp3, 0);

			Projectile comp4 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0,0), Owner, 210f, 0, 1, false);
			comp4.baseData.range = 1.5f;
			comp4.ResetDistance();
			applytrail(comp4, 0);

			Projectile comp5 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0,0), Owner, 330f, 0, 1, false);
			comp5.baseData.range = 1.5f;
			comp5.ResetDistance();
			applytrail(comp5, 0);

			yield return null;
		}

		private IEnumerator Poison(Vector2 posi)
		{
			Vector2 vecsave = posi;

			Projectile comp1 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 0f, 0, 1, false);
			comp1.baseData.range = .01f;
			comp1.ResetDistance();
			applytrail(comp1, 1, 1.5f);

			Projectile comp2 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, -1.5f), Owner, 270f, 0, 1, false);
			comp2.baseData.range = .1f;
			comp2.ResetDistance();
			applytrail(comp2, 1,.5f);

			Projectile comp3 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(-2, 2), Owner, 315f, 0, 1, false);
			comp3.baseData.range = 5.5f;
			comp3.ResetDistance();
			applytrail(comp3, 1,.3f);

			Projectile comp4 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(2, 2), Owner, 225f, 0, 1, false);
			comp4.baseData.range = 5.5f;
			comp4.ResetDistance();
			applytrail(comp4, 1,.3f);

			yield return null;


		}

		private IEnumerator Heart(Vector2 posi)
		{
			Vector2 vecsave = posi;

			Projectile comp1 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(-1.3f, 1f), Owner, 315f, 0, 1, false);
			comp1.baseData.range = 2.15f;
			comp1.ResetDistance();
			applytrail(comp1, 0, 1.25f, true, EasyGoopDefinitions.CharmGoopDef);

			Projectile comp2 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(1.3f, 1f), Owner, 225f, 0, 1, false);
			comp2.baseData.range = 2.15f;
			comp2.ResetDistance();
			applytrail(comp2, 0, 1.25f,true,EasyGoopDefinitions.CharmGoopDef);

			
			yield return null;


		}

		private IEnumerator Web(Vector2 posi)
		{
			Vector2 vecsave = posi;

			Projectile comp2 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 0f, 0, 1, false);
			comp2.baseData.range = 2.5f;
			comp2.ResetDistance();
			applytrail(comp2, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp3 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 45f, 0, 1, false);
			comp3.baseData.range = 2.5f;
			comp3.ResetDistance();
			applytrail(comp3, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp4 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 90f, 0, 1, false);
			comp4.baseData.range = 2.5f;
			comp4.ResetDistance();
			applytrail(comp4, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp5 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 135f, 0, 1, false);
			comp5.baseData.range = 2.5f;
			comp5.ResetDistance();
			applytrail(comp5, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp6 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 180f, 0, 1, false);
			comp6.baseData.range = 2.5f;
			comp6.ResetDistance();
			applytrail(comp6, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp7 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 225f, 0, 1, false);
			comp7.baseData.range = 2.5f;
			comp7.ResetDistance();
			applytrail(comp7, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp8 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 270f, 0, 1, false);
			comp8.baseData.range = 2.5f;
			comp8.ResetDistance();
			applytrail(comp8, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			Projectile comp9 = MiscToolMethods.SpawnProjAtPosi(Kproj, vecsave + new Vector2(0f, 0f), Owner, 315f, 0, 1, false);
			comp9.baseData.range = 2.5f;
			comp9.ResetDistance();
			applytrail(comp9, 0, .5f, true, EasyGoopDefinitions.PlayerFriendlyWebGoop);

			yield return null;


		}


		public void applytrail(Projectile comp, int gooptype, float Size = .35f, bool special = false, GoopDefinition goopy = null)
        {
			TrailFireModifier mirrorProjectileModifier = comp.gameObject.AddComponent<TrailFireModifier>();
			mirrorProjectileModifier.goopRadius = Size;
			mirrorProjectileModifier.goopType = gooptype;
			mirrorProjectileModifier.needsToUseGreenFire = false;
			mirrorProjectileModifier.goopy = goopy;
			mirrorProjectileModifier.useSpecialGoop = special;
		}

        

		public bool Painter = false;
		public Projectile Kproj;
		public Projectile projectile;
		public PlayerController Owner;
	}
}