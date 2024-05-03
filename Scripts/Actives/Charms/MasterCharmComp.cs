using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;

namespace Knives
{
	public class MasterCharmComp : MonoBehaviour
	{

		public MasterCharmComp()
		{

		}


		private void Start()
		{
			attachedGun = base.GetComponent<Gun>();
			player = attachedGun.GunPlayerOwner();
			InitIcons();
			Attach();
		}

		bool notify = false;
        private void Update()
        {
			if(attachedGun != null)
            {
				extantCharm.GetComponent<tk2dSprite>().renderer.enabled = attachedGun.sprite.renderer.enabled;
            }
			if(attachedGun == null)
            {
				if(extantCharm != null)
                {
					UnityEngine.GameObject.Destroy(extantCharm);
				}
				
            }
        }


		int originalPrice;
		private void Attach()
		{
			if (extantCharm == null)
			{
				if (baseitem != null)
				{
					if (baseitem.sprite)
					{
						GameObject gameObject = SpriteBuilder.SpriteFromResource(resource, null);
						gameObject.SetActive(false);
						ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
						if (attachedGun.sprite.FlipY != true)
						{
							attachedGun.HandleAimRotation(player.transform.TransformPoint(40, 0, 0));
						}
						else
						{
							attachedGun.HandleAimRotation(player.transform.TransformPoint(-40, 0, 0));
						}

						GameObject charmObject = UnityEngine.Object.Instantiate<GameObject>(gameObject, player.transform.position, Quaternion.identity);
						charmObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(attachedGun.PrimaryHandAttachPoint.transform.position, tk2dBaseSprite.Anchor.UpperCenter);
						charmObject.transform.parent = attachedGun.PrimaryHandAttachPoint;



						extantCharm = charmObject;
					}
				}
			}

			if (upgraded == false)
			{

				if (type == CharmType.Gnome) //
				{
					if(attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam)) // I hate beams 
					{
						DoBeamConsolationPrize(.15f); // Post Process doesn't work.
					}
                    else
                    {
						attachedGun.PostProcessProjectile += postprocessGnome;
					}
					
					upgraded = true;
				}
				if (type == CharmType.Knife) // 
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Knife;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postProcessKnife;
					}
					
					upgraded = true;
				}
				if (type == CharmType.Poma) // 
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Poma;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postprocessPoma;
					}
					upgraded = true;
				}
				if (type == CharmType.Mouse) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Mouse;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postprocessMouse;
					}
					upgraded = true;
				}
				if (type == CharmType.Pepper) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Pepper;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postprocessPepper;
					}
					
					upgraded = true;
				}
				if (type == CharmType.Ice) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam)) // I hate beams 
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Ice;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postprocessIce;
					}
					upgraded = true;
				}
				if (type == CharmType.Shroom) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Shroom;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += this.postprocessShroom;
					}
					upgraded = true;
				}
				if (type == CharmType.Target) //
				{

					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam)) // I hate beams 
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Target;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessTarget;
					}
					upgraded = true;
				}
				if (type == CharmType.Whammy) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Whammy;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessWhammy;
					}
					upgraded = true;
				}
				if (type == CharmType.Dango) // Ily Dango <3
				{
					
					StartCoroutine(RandomBuff(3));
					
				}
				if (type == CharmType.Clock) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Clock;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessClock;
					}
					upgraded = true;
				}
				if (type == CharmType.Lucky) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam)) // I hate beams 
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Lucky;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessLucky;
					}
					upgraded = true;
				}
				if (type == CharmType.Rubber) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Rubber;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessDucky;
					}
					upgraded = true;
				}
				if (type == CharmType.Battery)
				{
					AmmoRegenComp regen = attachedGun.gameObject.GetOrAddComponent<AmmoRegenComp>();
					upgraded = true;
				}
				if (type == CharmType.Piggy)
				{
					attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.MoneyMultiplierFromEnemies, .5f, StatModifier.ModifyMethod.ADDITIVE);
					player.stats.RecalculateStats(player);
					upgraded = true;
				}
				if (type == CharmType.Coffee)
				{
					attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);
					attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, .3f, StatModifier.ModifyMethod.ADDITIVE);
					player.stats.RecalculateStats(player);
					upgraded = true;
				}
				if (type == CharmType.Metronome) 
				{
					OneGunMetronomeModifier combo = attachedGun.gameObject.GetOrAddComponent<OneGunMetronomeModifier>(); // fix me
					upgraded = true;
				}
				if (type == CharmType.Feather)
				{
					GunApplyFlyMod fly = attachedGun.gameObject.GetOrAddComponent<GunApplyFlyMod>();
					upgraded = true;
				}
				if (type == CharmType.Meat) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Meat;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessHunter;
					}
					upgraded = true;
				}
				if (type == CharmType.Slime) //
				{
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Slime;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.PostProcessProjectile += postprocessSlime;
					}
					upgraded = true;
				}
				if (type == CharmType.Radioactive) 
				{
					
					if (attachedGun.HasShootStyle(ProjectileModule.ShootStyle.Beam))
					{
						Projectile pain = attachedGun.DefaultModule.projectiles[0].projectile;
						CharmBeamJankComp agonyEven = pain.gameObject.GetOrAddComponent<CharmBeamJankComp>();
						agonyEven.type = CharmBeamJankComp.CharmType.Radioactive;
						agonyEven.player = player;
					}
					else
					{
						attachedGun.sprite.color = ExtendedColours.lime;
						attachedGun.PostProcessProjectile += postprocessNuke;
					}

					upgraded = true;
				}
			}
		}

    

		private void DoBeamConsolationPrize(float percent)
        {
			
			attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, percent, StatModifier.ModifyMethod.ADDITIVE);
			player.stats.RecalculateStats(player);
			Notify("Charm Non-Functional On Beam weapons", "Adding" + percent + "% damage increase");

		}

        private void postprocessNuke(Projectile obj)
        {
			
			obj.OnHitEnemy += NukeOnHit;
			
		}

        private void NukeOnHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            
            if (arg2.aiActor)
            {
				arg2.aiActor.ApplyEffect(StaticStatusEffects.irradiatedLeadEffect);
				arg2.aiActor.ApplyEffect(new GameActorillnessEffect());

                if (arg3)
                {
					if (UnityEngine.Random.Range(1, 18) <= 1 || arg2.aiActor.GetComponent<ExplodeOnDeath>() != null)
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

		private void postprocessSlime(Projectile obj)
        {
			obj.baseData.speed *= .8f;
			GoopModifier goopy = obj.gameObject.GetOrAddComponent<GoopModifier>();
			goopy.goopDefinition = EasyGoopDefinitions.BlobulonGoopDef;
			goopy.InFlightSpawnRadius = 1f;
			goopy.CollisionSpawnRadius = 3f;
			goopy.SpawnGoopOnCollision = true;
			goopy.SpawnGoopInFlight = true;
			obj.OnHitEnemy += onSlimeEnemy;
        }

        private void onSlimeEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
				arg2.aiActor.ApplyEffect(StaticStatusEffects.tripleCrossbowSlowEffect);
            }
        }

        private void postprocessHunter(Projectile obj)
        {
			obj.OnHitEnemy += onHuntenemy;
        }

        private void onHuntenemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
				if (arg3)// fatal
				{
					if (UnityEngine.Random.Range(1, 10) <= 1)
					{
						LootEngine.SpawnItem(PickupObjectDatabase.GetById(MeatPickup.ID).gameObject, arg2.aiActor.sprite.WorldCenter, Vector2.zero, 1, true, true);
					}
				}
            }
        }

        private void postprocessDucky(Projectile obj)
        {
			BounceProjModifier bounce = obj.gameObject.GetOrAddComponent<BounceProjModifier>();
			bounce.numberOfBounces += 3;
			bounce.damageMultiplierOnBounce *= 1.1f;
			bounce.OnBounceContext += this.onBounce;
			
			GoopModifier goop = obj.gameObject.GetOrAddComponent<GoopModifier>();
			goop.SpawnGoopInFlight = false;
			goop.SpawnGoopOnCollision = true;
			goop.goopDefinition = EasyGoopDefinitions.WaterGoop;

			obj.OnHitEnemy += this.DuckyHit;
		}

        private void DuckyHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if (UnityEngine.Random.Range(1, 5) <= 1)
			{

				MiscToolMethods.SpawnProjAtPosi(RubberCharm.ducky, arg2.specRigidbody.UnitCenter, player, UnityEngine.Random.Range(0, 360));
			}
		}

		private void onBounce(BounceProjModifier arg1, SpeculativeRigidbody arg2)
        {
			AkSoundEngine.PostEvent("Play_Ducky", base.gameObject);
		}

       
        private void postprocessLucky(Projectile obj)
        {
			
			if (UnityEngine.Random.Range(1, 20) <= 1)
			{
				obj.OnHitEnemy = this.LuckyCrit;
				obj.DefaultTintColor = ExtendedColours.gildedBulletsGold;
				obj.HasDefaultTint = true;
			}
		}

        private void LuckyCrit(Projectile proj, SpeculativeRigidbody arg2, bool arg3)
        {
			if (arg2.aiActor != null)
			{
				proj.baseData.damage *= 3;
				AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
				AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
				arg2.aiActor.PlayEffectOnActor(EasyVFXDatabase.SmallMagicPuffVFX, new Vector3(0, 0, 0));
			}
		}


        private void postprocessClock(Projectile proj)
        {
			proj.baseData.range *= 1.2f;
			ReverseSpeedRampMod ramp = proj.gameObject.GetOrAddComponent<ReverseSpeedRampMod>();
			ramp.isCharm = true;
        }

        private IEnumerator RandomBuff(int numberof)
        {
			string upgrades = null;

			List<int> rolledbuffs = new List<int>();
			for (int x = numberof; x >= 1; x--)
			{
				rolledbuffs.Add(UnityEngine.Random.Range(0, BuffList.Count));
			}
			//ETGModConsole.Log("");
			attachedGun.CanBeDropped = false;
			player.inventory.GunLocked.SetOverride("Upgrading", true, null);

			//ETGModConsole.Log("Dango Buffs:");
			for (int x = rolledbuffs.Count; x >= 1; x--)
			{
				switch (rolledbuffs[(x-1)])
				{
					case 0: // damage up
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, .2f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "Damage,";
						this.player.BloopItemAboveHead(MasterCharmComp.AttackIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 1: // accuracy up
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.Accuracy, -.2f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "Spread,";
						this.player.BloopItemAboveHead(MasterCharmComp.AccuracyIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 2: // movespeed up
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, .6f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "MoveSpeed,";
						this.player.BloopItemAboveHead(MasterCharmComp.MovespeedIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 3: // clipsize up
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, .4f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "Clipsize,";
						this.player.BloopItemAboveHead(MasterCharmComp.ClipsizeIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 4: // firerate up
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, .2f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "FireRate,";
						this.player.BloopItemAboveHead(MasterCharmComp.FirerateIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 5: // reload speed
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.ReloadSpeed, -.2f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "Reload,";
						this.player.BloopItemAboveHead(MasterCharmComp.ReloadIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 6:  // knock
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.KnockbackMultiplier, .5f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "Knockback,";
						this.player.BloopItemAboveHead(MasterCharmComp.KnockbackIconPrefab.GetComponent<tk2dSprite>(), "");

						break;
					case 7: // Money
						attachedGun.AddCurrentGunStatModifier(PlayerStats.StatType.MoneyMultiplierFromEnemies, .25f, StatModifier.ModifyMethod.ADDITIVE);
						upgrades = upgrades + "MoneyMult,";
						this.player.BloopItemAboveHead(MasterCharmComp.MoneyIconPrefab.GetComponent<tk2dSprite>(), "");

						break;

				}
				AkSoundEngine.PostEvent("Play_OBJ_key_pickup_01", base.gameObject);
				yield return new WaitForSeconds(.4f);

			}
			player.inventory.GunLocked.SetOverride("Upgrading", false, null);
			attachedGun.CanBeDropped = true;
			ETGModConsole.Log("");
			yield return new WaitForSeconds(.2f);
			string[] SplitUpGang = upgrades.Split(',');
			regroup = SplitUpGang[0] + ", " + SplitUpGang[1] + ", & " + SplitUpGang[2] ;
			Notify("Dango Buffs Selected:", regroup);
			player.stats.RecalculateStats(player);
			upgraded = true;
		}
		public string regroup;
		public bool upgraded = false;

        private void postprocessWhammy(Projectile obj)
        {
			if (UnityEngine.Random.Range(1, 4) == 1)
			{
				Projectile yari = ((Gun)PickupObjectDatabase.GetById(16)).DefaultModule.projectiles[0].projectile;
				Projectile whammy = MiscToolMethods.SpawnProjAtPosi(yari, attachedGun.barrelOffset.transform.position, player, attachedGun, 40);
				whammy.baseData.damage *= .5f;
				whammy.AdditionalScaleMultiplier *= .8f;
				whammy.DefaultTintColor = ExtendedColours.lime;
				whammy.HasDefaultTint = true;
			}
		}

        private void postprocessTarget(Projectile obj)
        {
			
			HomingModifier home = obj.gameObject.GetOrAddComponent<HomingModifier>();
			home.AngularVelocity = 75;
			home.HomingRadius = 20;
			if (UnityEngine.Random.Range(1, 10) == 1)
			{
				obj.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(obj.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemyTarget));
				obj.DefaultTintColor = ExtendedColours.carrionRed;
				obj.HasDefaultTint = true;
			}
			

		}

        private void OnHitEnemyTarget(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if (arg2.aiActor != null)
            {
				arg2.healthHaver.ApplyDamage(arg1.baseData.damage * .5f, Vector2.zero, "Owie");
				AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
				AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
			}
        }

        private void postprocessShroom(Projectile obj)
        {
			if (UnityEngine.Random.Range(1, 3) == 1)
			{
				obj.AppliesPoison = true;
				obj.PoisonApplyChance = 100;

				obj.DefaultTintColor = UnityEngine.Color.green;
				obj.HasDefaultTint = true;
			}
		}

        private void postprocessIce(Projectile obj)
        {
			if (UnityEngine.Random.Range(1, 5) == 1)
			{
				obj.AppliesFreeze = true;
				obj.FreezeApplyChance = 100;
				obj.freezeEffect = StaticStatusEffects.frostBulletsEffect;
				obj.DefaultTintColor = ExtendedColours.freezeBlue;
				obj.HasDefaultTint = true;
			}
		}

        private void postprocessPepper(Projectile obj)
        {
            if(UnityEngine.Random.Range(1,3) == 1)
            {
				obj.AppliesFire = true;
				obj.FireApplyChance = 100;
				obj.fireEffect = StaticStatusEffects.hotLeadEffect;
				obj.DefaultTintColor = UnityEngine.Color.red;
				obj.HasDefaultTint = true;
            }
        }

        private void postprocessMouse(Projectile obj)
		{
			obj.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(obj.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemyMouse));

		}

        private void OnHitEnemyMouse(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if (arg3 == true)
			{
				if (arg2.aiActor != null)
				{
					RatCrown.SpawnRat(this.player, arg2.aiActor.sprite.WorldCenter);
					
				}
			}
        }

        private void postprocessGnome(Projectile obj)
        {
			ProjectileSplitController split = obj.gameObject.GetOrAddComponent<ProjectileSplitController>();
			split.distanceTillSplit = 0;
			split.distanceBasedSplit = true;
			split.numberofsplits = 1;
			split.amtToSplitTo = 2;
			split.dmgMultAfterSplit = .5f;
			split.splitAngles = 16f;
			
		}

		

		public GameObject extantCharm;

		private void postprocessPoma(Projectile obj)
		{
			if (UnityEngine.Random.Range(1, 4) == 1)
			{
				ProjectileSplitController split = obj.gameObject.GetOrAddComponent<ProjectileSplitController>();
				split.splitOnEnemy = true;
				split.distanceBasedSplit = false;
				split.numberofsplits = 1;
				split.amtToSplitTo = 6;
				split.dmgMultAfterSplit = .13f;
				split.sizeMultAfterSplit = .8f;
				split.splitAngles = 360f;
				split.removeComponentAfterUse = true;
				obj.HasDefaultTint = true;
				obj.DefaultTintColor = ExtendedColours.vibrantOrange;
				
			}
		}

		private void postProcessKnife(Projectile obj)
        {
			if (UnityEngine.Random.Range(1, 4) == 1)
			{

				obj.OnHitEnemy += this.OnhitEnemyKnife;
				obj.DefaultTintColor = ExtendedColours.silvedBulletsSilver;
				obj.HasDefaultTint = true;
			}
		}

        private void OnhitEnemyKnife(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			
			Projectile proj = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0].projectile;
			Projectile swipe = MiscToolMethods.SpawnProjAtPosi(proj, arg1.LastPosition, player, arg1.Direction.ToAngle());
			ProjectileSlashingBehaviour stabby = swipe.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
			stabby.playerKnockback = 0;
			stabby.DestroyBaseAfterFirstSlash = true;
			stabby.SlashDamageUsesBaseProjectileDamage = false;
			stabby.SlashDamage = Mathf.CeilToInt(arg1.baseData.damage * .2f);
			stabby.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
			stabby.SlashVFX = (PickupObjectDatabase.GetById(335) as Gun).muzzleFlashEffects;
		}



        public bool frenzyToggle = true;

		public Gun attachedGun;
		public PlayerItem baseitem;
		public PlayerController player;
		public string resource;
		public MasterCharmComp.CharmType type = CharmType.Gnome;
        

        public ProjectileModule ModuleToAdd { get; private set; }

		public void Notify(string header, string text)
		{
			var sprite = extantCharm.GetComponent<tk2dSprite>();
			GameUIRoot.Instance.notificationController.DoCustomNotification(
				header,
				text,
				sprite.Collection,
				sprite.spriteId,
				UINotificationController.NotificationColor.PURPLE,
				false,
				false);
		}




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
        }

		List<int> BuffList = new List<int>
		{
			0, // damage up
			1, // accuracy up
			2, // movespeed up
			3, // clipsize up 
			4, // firerate up
			5, // reload speed
			6, // knockback
			7, // Money
		};

		private static GameObject AttackIconPrefab;
		private static GameObject AccuracyIconPrefab;
		private static GameObject MovespeedIconPrefab;
		private static GameObject ClipsizeIconPrefab;
		private static GameObject FirerateIconPrefab;
		private static GameObject ReloadIconPrefab;
		private static GameObject KnockbackIconPrefab;
		private static GameObject MoneyIconPrefab;

		private void InitIcons()
		{
			MasterCharmComp.AttackIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/damage_icon", null);
			MasterCharmComp.AttackIconPrefab.name = "AttackUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.AttackIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.AttackIconPrefab);
			MasterCharmComp.AttackIconPrefab.SetActive(false);

			MasterCharmComp.AccuracyIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/Aim_icon", null);
			MasterCharmComp.AccuracyIconPrefab.name = "AimUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.AccuracyIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.AccuracyIconPrefab);
			MasterCharmComp.AccuracyIconPrefab.SetActive(false);

			MasterCharmComp.MovespeedIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/Run_icon", null);
			MasterCharmComp.MovespeedIconPrefab.name = "MoveUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.MovespeedIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.MovespeedIconPrefab);
			MasterCharmComp.MovespeedIconPrefab.SetActive(false);

			MasterCharmComp.ClipsizeIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/clip_icon", null);
			MasterCharmComp.ClipsizeIconPrefab.name = "ClipUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.ClipsizeIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.ClipsizeIconPrefab);
			MasterCharmComp.ClipsizeIconPrefab.SetActive(false);

			MasterCharmComp.FirerateIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/ROF_icon", null);
			MasterCharmComp.FirerateIconPrefab.name = "ROFUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.FirerateIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.FirerateIconPrefab);
			MasterCharmComp.FirerateIconPrefab.SetActive(false);

			MasterCharmComp.ReloadIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/reload_icon", null);
			MasterCharmComp.ReloadIconPrefab.name = "ReloadUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.ReloadIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.ReloadIconPrefab);
			MasterCharmComp.ReloadIconPrefab.SetActive(false);

			MasterCharmComp.KnockbackIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/Knockback_icon", null);
			MasterCharmComp.KnockbackIconPrefab.name = "KBUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.KnockbackIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.KnockbackIconPrefab);
			MasterCharmComp.KnockbackIconPrefab.SetActive(false);

			MasterCharmComp.MoneyIconPrefab = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/icons/damage_icon", null);
			MasterCharmComp.MoneyIconPrefab.name = "MoneyUp";
			UnityEngine.Object.DontDestroyOnLoad(MasterCharmComp.MoneyIconPrefab);
			FakePrefab.MarkAsFakePrefab(MasterCharmComp.MoneyIconPrefab);
			MasterCharmComp.MoneyIconPrefab.SetActive(false);
		}



	}
}