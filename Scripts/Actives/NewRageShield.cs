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

	class New_Rage_shield : PlayerItem
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
			var item = obj.AddComponent<New_Rage_shield>();

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
			ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 200f);


			//Set the rarity of the item



			item.quality = PickupObject.ItemQuality.B;
			
			ID = item.PickupObjectId;
		}

		public static int ID;
		//controls if the hat is on or off
		public int m_boostPoints;
		public bool toggle = false;
		public bool timerToggle = false;
        public Shader m_glintShader;
        public GameObject RageVfx;

		public GameObject Shield = EasyVFXDatabase.Shield;

		public override void Pickup(PlayerController player)
		{
			this.m_glintShader = Shader.Find("Brave/ItemSpecific/LootGlintAdditivePass");
			RageVfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);

			base.Pickup(player);
		}
		public override void DoEffect(PlayerController user)
		{
            if (instanceVFX != null)
            {
				UnityEngine.GameObject.Destroy(instanceVFX);
			}
			toggle = true;
			float dura = 4f;

			this.LastOwner.IsEthereal = true;
			SpeculativeRigidbody specRigidbody = user.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions));
			user.specRigidbody.BlockBeams = true;
			user.MovementModifiers += this.NoMotionModifier;
			user.IsStationary = true;
			user.CurrentStoneGunTimer = 4f;
			user.OnPreDodgeRoll += this.HandleDodgeRollStarted;
			user.OnTriedToInitiateAttack += this.HandleTriedAttack;
			
			m_boostPoints = 0;
			StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
			levelBoost = 0;
			instanceShield = user.PlayEffectOnActor(Shield,new Vector3(0,0,0),true);
			AkSoundEngine.PostEvent("Play_ENM_gunnut_swing_01", base.gameObject);

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
		public override void DoActiveEffect(PlayerController user)
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
			user.CurrentStoneGunTimer = 0f;
			user.OnPreDodgeRoll -= this.HandleDodgeRollStarted;
			user.OnTriedToInitiateAttack -= this.HandleTriedAttack;
			if (instanceVFX != null)
			{
				UnityEngine.GameObject.Destroy(instanceVFX);
			}

			GameObject Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
			if(levelBoost != 0)
            {

				this.instanceVFX = this.LastOwner.PlayEffectOnActor(Vfx, new Vector3(0f, 1.375f, 0f), true, true, false);
				StartCoroutine(RageBoost());
			}
			
			if(instanceShield != null)
            {
				UnityEngine.GameObject.Destroy(instanceShield);
            }
			


		}

        private IEnumerator RageBoost()
        {
			AkSoundEngine.PostEvent("Play_BOSS_bulletbros_anger_01", base.gameObject);
			RemoveStat(PlayerStats.StatType.Damage);
			AddStat(PlayerStats.StatType.Damage, levelBoost);
			this.LastOwner.stats.RecalculateStats(LastOwner, true);
			
			yield return new WaitForSeconds(6f);
			UnityEngine.GameObject.Destroy(instanceVFX);
			RemoveStat(PlayerStats.StatType.Damage);
			AddStat(PlayerStats.StatType.Damage, 0);
			this.LastOwner.stats.RecalculateStats(LastOwner, true);

		}
		public float levelBoost = 0;
		private void PreventBulletCollisions(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (otherRigidbody.projectile)
			{
				if (otherRigidbody.projectile.IsBlackBullet)
				{
					m_boostPoints = m_boostPoints + 2;
					
				}
				AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Block_01", base.gameObject);
				m_boostPoints++;
				otherRigidbody.projectile.DieInAir(false, true, true, false);
				if(m_boostPoints >= 2 && m_boostPoints <7)
                {
					instanceShield.GetOrAddComponent<tk2dBaseSprite>().color = UnityEngine.Color.red;
					levelBoost = .1f;

				}
				if (m_boostPoints >= 7 && m_boostPoints < 12)
				{
					instanceShield.GetOrAddComponent<tk2dBaseSprite>().color = new Color(1.0f, .69f, .0f);
					levelBoost = .25f;
				}
				if (m_boostPoints >= 12)
				{
					instanceShield.GetOrAddComponent<tk2dBaseSprite>().color = new Color(1.0f, .87f, .20f);
					levelBoost = .5f;
				}

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

		

		public float counter = 4;

		public override void Update()
		{
			if (this.LastOwner != null)
			{
			}
			base.Update();
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

		public override void OnPreDrop(PlayerController user)
		{
			this.EndEffect(user);

			base.OnPreDrop(user);
		}

		
		public static List<int> spriteIds = new List<int>();
		
		private GameObject instanceVFX;
		private GameObject instanceShield;


	}
}
