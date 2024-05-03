using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ItemAPI;

namespace Knives
{

    class Rage_shield : PlayerItem
    {
		
        public static void Register()
        {
            //The name of the item
            string itemName = "Anchor Rage";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/RageShield";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Rage_shield>();
			
            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Seething Patience";
            string longDesc = "When holding this shield aloft the wielder will be invulnerable to all attacks. " +
                "This shield binds the user down into a extremely defensive position and allows them to build up anger for their opponents. " +
                "The stronger or more plentiful the blocked attacks the more rage you will build" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

			//Adds the actual passive effect to the item

			item.ForcedPositionInAmmonomicon = 512;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 18f);


            //Set the rarity of the item



            item.quality = PickupObject.ItemQuality.A;
			Rage_shield.BuildPrefab();

			ID = item.PickupObjectId;
		}

		public static int ID;
		//controls if the hat is on or off
		public int m_boostPoints;
		public bool toggle = false;
		public bool timerToggle = false;
		public GameObject Vfx;
		
		public override void Pickup(PlayerController player)
        {
			
			Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
			
			base.Pickup(player);
        }
        public override void  DoEffect(PlayerController user)
        {

			toggle = true;
			float dura = 5f;
			
			this.LastOwner.IsEthereal = true;
			SpeculativeRigidbody specRigidbody = user.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions));
			user.specRigidbody.BlockBeams = true;
			user.MovementModifiers += this.NoMotionModifier;
			user.IsStationary = true;
			user.IsGunLocked = true;
			user.OnPreDodgeRoll += this.HandleDodgeRollStarted;
			user.OnTriedToInitiateAttack += this.HandleTriedAttack;
			this.CanBeDropped = false;
			m_boostPoints = -2;
			StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
			
		}
		private void HandleTriedAttack(PlayerController obj)
		{
			this.DoActiveEffect(obj);
		}
		private void HandleDodgeRollStarted(PlayerController obj)
		{
			this.DoActiveEffect(obj);
		}
		private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
		{
			voluntaryVel = Vector2.zero;
		}
		public override void  DoActiveEffect(PlayerController user)
		{
			this.EndEffect(user);
		}


		
		
		public void EndEffect(PlayerController user)
        {
			toggle = false;
			this.LastOwner.IsEthereal = false;
			base.IsCurrentlyActive = false;
			user.MovementModifiers -= this.NoMotionModifier;
			SpeculativeRigidbody specRigidbody = user.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions));
			user.specRigidbody.BlockBeams = false;
			user.IsStationary = false;
			user.IsGunLocked = false;
			user.OnPreDodgeRoll -= this.HandleDodgeRollStarted;
            user.OnTriedToInitiateAttack -= this.HandleTriedAttack;
			this.CanBeDropped = true;

			GameObject Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
			if (Vfx && !this.instanceVFX && m_boostPoints >= 3)
			{
				this.instanceVFX = this.LastOwner.PlayEffectOnActor(Vfx, new Vector3(0f, 1.375f, 0f), true, true, false);
			}

		}

	

		private void PreventBulletCollisions(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (otherRigidbody.projectile)
			{
                if (otherRigidbody.projectile.IsBlackBullet)
                {
					m_boostPoints = m_boostPoints + 2;
                }
				m_boostPoints++;
				otherRigidbody.projectile.DieInAir(false, true, true, false);
				
				PhysicsEngine.SkipCollision = true;
			}
			if (otherRigidbody.aiActor)
			{
				if (otherRigidbody.knockbackDoer)
				{
					otherRigidbody.knockbackDoer.ApplyKnockback(otherRigidbody.UnitCenter - myRigidbody.UnitCenter, 50f, false);
				}
				PhysicsEngine.SkipCollision = true;
			}
		}

		public void RageBoost(int last_boost)
        {
			RemoveStat(PlayerStats.StatType.Damage);
			AddStat(PlayerStats.StatType.Damage, last_boost * .20f);
			this.LastOwner.stats.RecalculateStats(LastOwner, true);

			
		}

		public int counter = 0;
		
		public override void Update()
        {
			if(this.LastOwner != null)
            {
				bool flag = base.LastOwner && !this.HatObject && base.LastOwner.CurrentGun.sprite;
				if (flag)
				{
					this.SpawnVFXAttached();
				}
				bool flag2 = !base.LastOwner.CurrentGun.sprite && this.HatObject != null;
				if (flag2)
				{
					UnityEngine.Object.Destroy(this.HatObject);
				}
				if (this.IsCurrentlyActive)
				{
					RageBoost(m_boostPoints);
					timerToggle = true;
				}
				if (timerToggle && Time.timeScale > 0f)
				{
					counter++;
				}
				if (counter >= 1750)
				{
					m_boostPoints = 0;
					counter = 0;
					timerToggle = false;
					RageBoost(m_boostPoints);
					this.instanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
					this.instanceVFX = null;

				}

			}

			base.Update();
        }
		public static void BuildPrefab()
		{   //hatdoer
			GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/RageShield", null);
			gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameObject gameObject2 = new GameObject("kinda a hat");
			tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
			tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);
			Rage_shield.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/RageShield", tk2dSprite.Collection));
			Rage_shield.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/RageShield", tk2dSprite.Collection));

			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			Rage_shield.spriteIds.Add(tk2dSprite.spriteId);
			gameObject2.SetActive(false);
			tk2dSprite.SetSprite(Rage_shield.spriteIds[0]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(Rage_shield.spriteIds[1]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(Rage_shield.spriteIds[2]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");

			FakePrefab.MarkAsFakePrefab(gameObject2);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			Rage_shield.shieldprefab = gameObject2;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E20 File Offset: 0x00003020
		private void SpawnVFXAttached()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Rage_shield.shieldprefab, base.LastOwner.transform.position + new Vector3(0.6f, 1.05f, -5f), Quaternion.identity);
			gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.LastOwner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
			GameManager.Instance.StartCoroutine(this.HandleSprite(gameObject));
			this.HatObject = gameObject;
		}

		public GameObject shield;
		public IEnumerator HandleSprite(GameObject prefab)
		{
			shield = prefab;
			while (prefab != null && base.LastOwner != null)
			{
				prefab.transform.position = base.LastOwner.transform.position + new Vector3(0.1f, -0.1f, -5f);
				
				if (toggle == false)
				{
					prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
				}
				else
				{
					prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = true;
				}
				bool flag = base.LastOwner.IsBackfacing();
				if (flag)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(Rage_shield.spriteIds[1]);
				}
				bool flag2 = !base.LastOwner.IsBackfacing() && this.LastOwner.CurrentGun.sprite.WorldCenter.x - this.LastOwner.specRigidbody.UnitCenter.x < 0f;
				if (flag2)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(Rage_shield.spriteIds[0]);
				}
				bool flag3 = !base.LastOwner.IsBackfacing() && this.LastOwner.CurrentGun.sprite.WorldCenter.x - this.LastOwner.specRigidbody.UnitCenter.x > 0f;
				if (flag3)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(Rage_shield.spriteIds[2]);
				}


				yield return null;
			}
		UnityEngine.Object.Destroy(prefab.gameObject);
		yield break;
		}
		private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
		{
			StatModifier modifier = new StatModifier();
			modifier.amount = amount;
			modifier.statToBoost = statType;
			modifier.modifyType = method;

			foreach (var m in passiveStatModifiers)
			{
				if (m.statToBoost == statType) return; //don't add duplicates
			}

			if (this.passiveStatModifiers == null)
				this.passiveStatModifiers = new StatModifier[] { modifier };
			else
				this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
		}


		//Removes a stat
		private void RemoveStat(PlayerStats.StatType statType)
		{
			var newModifiers = new List<StatModifier>();
			for (int i = 0; i < passiveStatModifiers.Length; i++)
			{
				if (passiveStatModifiers[i].statToBoost != statType)
					newModifiers.Add(passiveStatModifiers[i]);
			}
			this.passiveStatModifiers = newModifiers.ToArray();
		}

        public override void  OnPreDrop(PlayerController user)
        {
			toggle = false;
			this.LastOwner.IsEthereal = false;
			base.IsCurrentlyActive = false;
			user.MovementModifiers -= this.NoMotionModifier;
			SpeculativeRigidbody specRigidbody = user.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions));
			user.specRigidbody.BlockBeams = false;
			user.IsStationary = false;
			user.IsGunLocked = false;
			user.OnPreDodgeRoll -= this.HandleDodgeRollStarted;
			user.OnTriedToInitiateAttack -= this.HandleTriedAttack;

			base.OnPreDrop(user);
        }

        private static GameObject shieldprefab;
		public static List<int> spriteIds = new List<int>();
		private GameObject HatObject;
		private GameObject instanceVFX;

	}
}
