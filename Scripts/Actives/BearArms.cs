using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Knives
{
	class BearArms : PlayerItem
	{
		public static void Register()
		{
			string text = "Bear Arms";
			string resourcePath = "Knives/Resources/BearArms";
			GameObject gameObject = new GameObject(text);
			BearArms item = gameObject.AddComponent<BearArms>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Second Ammendment";
			string longDesc = "Dual Wield Current and Last Held Guns for a short time.\n\n" +
				"Highly Specialized Bear Arms, Strong enough to carry any two weapons at the same time.\n" +
				"These Genetic modification were developed shortly after a test unit of bears assisted the frontier wars.\n" +
				"\n\n\n - Knife_to_a_Gunfight";
			item.SetupItem(shortDesc, longDesc, "ski");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 600);
			item.quality = ItemQuality.B;
			ID = item.PickupObjectId;

		}

		public static int ID;

		public GameObject Vfx;

		public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
			Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
			player.inventory.OnGunChanged += Inventory_OnGunChanged;
			
		}

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
			PriorCurrentGun = previous;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return !user.inventory.DualWielding && user.CurrentGun && !user.CurrentGun.IsEmpty && user.inventory.AllGuns != null && (user.inventory.AllGuns.Count > 1) && user.inventory.GunLocked.BaseValue == false;
		}

		public override void DoEffect(PlayerController user)
		{
			wasUsed = true;
			if (user)
			{
				SetDualWield(user);
				//user.inventory.GunLocked.SetOverride("BearArms", true, null);
				
				AddStat(PlayerStats.StatType.Accuracy, .5f);
				user.stats.RecalculateStats(user, true);

				GameObject Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
                if (!this.instanceVFX)
                {
					this.instanceVFX = this.LastOwner.PlayEffectOnActor(Vfx, new Vector3(0f, 1.375f, 0f), true, true, false);
				}
			}
			user.StartCoroutine(VfxHandle());
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

        private IEnumerator VfxHandle()
        {
			yield return new WaitForSeconds(2);
			this.instanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
			this.instanceVFX = null;
		}

        public Gun PriorCurrentGun;
		
		


        public void EndEffect(PlayerController user)
		{
			if (wasUsed && user)
			{
				foreach (Gun gun in user.inventory.AllGuns)
				{
					if (gun.gameObject.GetComponent<DualWieldForcer>() != null)
					{
						Destroy(gun.gameObject.GetComponent<DualWieldForcer>());
					}
				}

				//user.inventory.GunLocked.SetOverride("BearArms", false, null);
				RemoveStat(PlayerStats.StatType.Accuracy);
				user.stats.RecalculateStats(user, true);


			}
			wasUsed = false;
		}

		public override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
			user.inventory.OnGunChanged -= Inventory_OnGunChanged;
			base.OnPreDrop(user);
		}

		private void SetDualWield(PlayerController user)
		{
			
			int currentGunIndex = user.inventory.AllGuns.IndexOf(user.CurrentGun);

			int partnerID = 0;
			Gun partnerGun = user.inventory.AllGuns[0];
			if (user.inventory.AllGuns.Count == 2)
			{
				if (currentGunIndex == 0)
				{
					partnerGun = user.inventory.AllGuns[1];
				}
				else
				{
					partnerGun = user.inventory.AllGuns[0];
				}
			}
			else
			{
				if(PriorCurrentGun == null)
                {
					partnerGun = user.inventory.AllGuns[Random.Range(currentGunIndex + 1, user.inventory.AllGuns.Count)];
				}
                else
                {
					partnerGun = PriorCurrentGun;
				}
				
			}
			partnerID = partnerGun.PickupObjectId;
			
			if ((Gun)SecGunzFieldInfo.GetValue(user.inventory) != null)
			{
				SecGunzFieldInfo.SetValue(user.inventory, user.CurrentGun);
			}

			DualWieldForcer dualWieldForcer = user.CurrentGun.gameObject.AddComponent<DualWieldForcer>();
			dualWieldForcer.PartnerGunID = partnerID;
			dualWieldForcer.TargetPlayer = user;
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

		
		private static FieldInfo SecGunzFieldInfo = typeof(GunInventory).GetField("m_currentSecondaryGun", BindingFlags.NonPublic | BindingFlags.Instance);

		private float duration = 8f;
		private bool wasUsed = false;
        private GameObject instanceVFX;
    }


	public class DualWieldForcer : MonoBehaviour
	{
		public void Activate()
		{
			try
			{
				if (this.EffectValid(this.TargetPlayer))
				{
					this.m_isCurrentlyActive = true;
					
					this.TargetPlayer.inventory.SetDualWielding(true, "synergy");

					
					if (TargetPlayer.inventory.CurrentGun && TargetPlayer.inventory.CurrentSecondaryGun && TargetPlayer.inventory.CurrentGun != TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print("Dual Gun Error", "ffffff", true);
					}

					int indexForGun = this.GetIndexForGun(this.TargetPlayer, this.gun.PickupObjectId);
					int indexForGun2 = this.GetIndexForGun(this.TargetPlayer, this.PartnerGunID);
					this.TargetPlayer.inventory.SwapDualGuns();

					if (indexForGun >= 0 && indexForGun2 >= 0)
					{
						while (this.TargetPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
						{
							this.TargetPlayer.inventory.ChangeGun(1, false, false);
						}
					}
					this.TargetPlayer.inventory.SwapDualGuns();
					if (this.TargetPlayer.CurrentGun && !this.TargetPlayer.CurrentGun.gameObject.activeSelf)
					{
						this.TargetPlayer.CurrentGun.gameObject.SetActive(true);
					}
					if (this.TargetPlayer.CurrentSecondaryGun && !this.TargetPlayer.CurrentSecondaryGun.gameObject.activeSelf)
					{
						this.TargetPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
					}
					this.TargetPlayer.GunChanged += this.HandleGunChanged;
				}
			}
			catch (Exception e)
			{
				Tools.PrintException(e);
			}
		}

		public void Awake()
		{
			this.gun = base.GetComponent<Gun>();
		}

		private void CheckStatus()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				if (!this.PlayerUsingCorrectGuns() || !this.EffectValid(this.TargetPlayer))
				{
					Console.WriteLine("DISABLING EFFECT");
					this.DisableEffect();
				}
			}
			else
			{
				if (this.gun && this.gun.CurrentOwner is PlayerController)
				{
					PlayerController playerController = this.gun.CurrentOwner as PlayerController;
					if (playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == this.gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == this.PartnerGunID)
					{
						this.m_isCurrentlyActive = true;
						this.TargetPlayer = playerController;
					}
					else
					{
						this.Activate();
					}
				}
			}
		}

		private void DisableEffect(bool forceDisable = false)
		{
			if (this.m_isCurrentlyActive || forceDisable)
			{
				this.m_isCurrentlyActive = false;
				//this.TargetPlayer.inventory.GunLocked.RemoveOverride("gunzerking");
				this.TargetPlayer.inventory.SetDualWielding(false, "synergy");
				this.TargetPlayer.GunChanged -= this.HandleGunChanged;
				this.TargetPlayer.stats.RecalculateStats(this.TargetPlayer, false, false);
				this.TargetPlayer = null;
			}
		}

		private bool EffectValid(PlayerController p)
		{
			bool result;
			if (!p)
			{
				Console.WriteLine("NULL PLAYER");
				result = false;
			}
			else
			{
				bool flag3 = this.gun.CurrentAmmo == 0;
				bool flag4 = flag3;
				if (flag4)
				{
					Console.WriteLine("CURAMMO 0");
					result = false;
				}
				else
				{
					bool flag5 = !this.m_isCurrentlyActive;
					bool flag6 = flag5;
					if (flag6)
					{
						int indexForGun = this.GetIndexForGun(p, this.PartnerGunID);
						bool flag7 = indexForGun < 0;
						bool flag8 = flag7;
						if (flag8)
						{
							Console.WriteLine("IDX4GUN <0");
							return false;
						}
						bool flag9 = p.inventory.AllGuns[indexForGun].CurrentAmmo == 0;
						bool flag10 = flag9;
						if (flag10)
						{
							Console.WriteLine("PARTNERAMMO 0");
							return false;
						}
					}
					else
					{
						bool flag11 = p.CurrentSecondaryGun != null && p.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0;
						bool flag12 = flag11;
						if (flag12)
						{
							Console.WriteLine("SECONDARYAMMO 0");
							return false;
						}
					}
					//Console.WriteLine("EFFECT VALID");
					result = true;
				}
			}
			return result;
		}

		private int GetIndexForGun(PlayerController p, int gunID)
		{
			for (int i = 0; i < p.inventory.AllGuns.Count; i++)
			{
				bool flag = p.inventory.AllGuns[i].PickupObjectId == gunID;
				bool flag2 = flag;
				if (flag2)
				{
					return i;
				}
			}
			return -1;
		}

		private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3)
		{
			this.CheckStatus();
		}

		private bool PlayerUsingCorrectGuns()
		{
			return this.gun && this.gun.CurrentOwner && this.TargetPlayer && this.TargetPlayer.inventory.DualWielding && (!(this.TargetPlayer.CurrentGun != this.gun) || this.TargetPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!(this.TargetPlayer.CurrentSecondaryGun != this.gun) || this.TargetPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
		}

		private void Update()
		{
			this.CheckStatus();
		}

		private void OnDestroy()
		{
			DisableEffect();
		}

		public int PartnerGunID;

		public PlayerController TargetPlayer;

		public Gun gun;

		private bool m_isCurrentlyActive;
	}
}