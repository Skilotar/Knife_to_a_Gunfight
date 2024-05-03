using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{ 

	class SCP_323 : PassiveItem
	{
		public static void Register()
		{
			
			string itemName = "Wendigo Skull";

			
			string resourceName = "Knives/Resources/wendigo_skull";

			
			GameObject obj = new GameObject(itemName);

			
			var item = obj.AddComponent<SCP_323>();

			
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

			//Ammonomicon entry variables
			string shortDesc = "Cold and Hungry";
			string longDesc = "Humanoid class creatures who don this skull are rapidly altered into a creature of emense might and fortitude but also of great hunger. \n\n" +
				"These 'Wendigo' more formaly known as SCP-323-1, require nearly constant food intake or else face starvation over time. Unfortunatly for you, if you can read this description you are likely already a SCP-323-1. \n" +
				"Your healing capabilities are greatly increased but you will require much more food to sustain your new form. Do not delay! Go! Hunt! KILL! and Live just a little bit longer.\n\n" +
				"____________________________________\n" +
				"You are on a starvation timer that will damage you after a while, kill enemies to gain health but don't delay as loitering will lead to death!\n" +
				"starvation damage will not cost boss flawlesses." +
				"\n\n\n - Knife_to_a_Gunfight";

			//Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
			//Do this after ItemBuilder.AddSpriteToObject!
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

			//Adds the actual passive effect to the item
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .50f, StatModifier.ModifyMethod.ADDITIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 2f, StatModifier.ModifyMethod.ADDITIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, .5f, StatModifier.ModifyMethod.ADDITIVE);


			
			item.quality = PickupObject.ItemQuality.C;
			item.CanBeDropped = false;
			SCP_323.BuildPrefab();
		}
		public int counter = 0;
		public int kills = 0;
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.OnKilledEnemy += this.OnKilledEnemy;
			player.OnReceivedDamage += this.OnReceivedDamage;
			player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.Onenteredcombat));
		}
		
		public override void  Update()
		{
			//hunger and eating system w/ special cases for robot
			if(this.Owner != false)
            {
				if (Time.timeScale <= 0f)
				{

				}
				else
				{
					counter++;
				}

				if (counter == 7000)
				{

					AkSoundEngine.PostEvent("Play_ENM_creecher_peel_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_ENM_creecher_peel_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_ENM_creecher_peel_01", base.gameObject);

					if (this.Owner.characterIdentity == PlayableCharacters.Robot)
					{
						this.Owner.healthHaver.ApplyDamage(1f, Vector2.zero, "Debug.LogError(\"StarvedAsRobot?\");", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
						GameManager.Instance.StartCoroutine(this.SaveFlawless());
					}
					else
					{
						this.Owner.healthHaver.ApplyDamage(1f, Vector2.zero, "Starvation", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
						GameManager.Instance.StartCoroutine(this.SaveFlawless());
					}

					counter = 0;
				}
				if (kills >= 12)
				{
					counter = counter - 2000;
					kills = 0;
					AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Munch_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Munch_01", base.gameObject);
					if (this.Owner.characterIdentity == PlayableCharacters.Robot)
					{
						this.Owner.GiveItem("armor");
					}
					else
					{
						this.Owner.healthHaver.ApplyHealing(1.5f);
					}

				}
				bool flag = base.Owner && !this.HatObject && base.Owner.CurrentGun.sprite;
				if (flag)
				{
					this.SpawnVFXAttached();



				}
				bool flag2 = !base.Owner.CurrentGun.sprite && this.HatObject;
				if (flag2)
				{
					UnityEngine.Object.Destroy(this.HatObject);
				}
			}
			
			base.Update();
			
		}
		public bool Dosaveflaw = true;
		private IEnumerator SaveFlawless()
		{
			yield return new WaitForSeconds(0.1f);
			PlayerController player = base.Owner;
			bool flag = player.CurrentRoom != null;
			if (flag && Dosaveflaw)
			{	
				if(player.healthHaver.lastIncurredDamageSource == "Starvation" || player.healthHaver.lastIncurredDamageSource == "Debug.LogError(\"StarvedAsRobot?\");")
                {
					player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
				}
				
			}
			yield break;
		}

		public void OnReceivedDamage(PlayerController player)
        {
			if (player.healthHaver.lastIncurredDamageSource != "Starvation" && player.healthHaver.lastIncurredDamageSource != "Debug.LogError(\"StarvedAsRobot?\");")
			{
				Dosaveflaw = false;
				ETGModConsole.Log("false");
			}
		}

		public void Onenteredcombat()
        {
			Dosaveflaw = true;
			ETGModConsole.Log("true");
		}

		public void OnKilledEnemy(PlayerController user)
		{
			kills = kills + 1;
			AkSoundEngine.PostEvent("Play_ITM_CheeseWheel_Munch_01", base.gameObject);
			AkSoundEngine.PostEvent("Play_ITM_CheeseWheel_Munch_01", base.gameObject);
		}
		
		public static void BuildPrefab()
		{	//hatdoer
			GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/SCP_323_collection/wendigo_skull_right", null);
			gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameObject gameObject2 = new GameObject("Bone Hat");
			tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
			tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);
			SCP_323.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/SCP_323_collection/wendigo_skull_left", tk2dSprite.Collection));
			SCP_323.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/SCP_323_collection/wendigo_skull_up", tk2dSprite.Collection));

			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			SCP_323.spriteIds.Add(tk2dSprite.spriteId);
			gameObject2.SetActive(false);
			tk2dSprite.SetSprite(SCP_323.spriteIds[0]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(SCP_323.spriteIds[1]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(SCP_323.spriteIds[2]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");

			FakePrefab.MarkAsFakePrefab(gameObject2);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			SCP_323.skullprefab = gameObject2;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E20 File Offset: 0x00003020
		private void SpawnVFXAttached()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SCP_323.skullprefab, base.Owner.transform.position + new Vector3(0.6f, 1.05f, -5f), Quaternion.identity);
			gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.Owner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
			GameManager.Instance.StartCoroutine(this.HandleSprite(gameObject));
			this.HatObject = gameObject;
		}


		public IEnumerator HandleSprite(GameObject prefab)
		{
			while (prefab != null && base.Owner != null)
			{
				prefab.transform.position = base.Owner.transform.position + new Vector3(0.0f, 1.05f, -5f);
				bool isFalling = base.Owner.IsFalling;
				if (isFalling)
				{
					prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
				}
				else
				{
					prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = true;
				}
				bool flag = base.Owner.IsBackfacing();
				if (flag)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(SCP_323.spriteIds[1]);
				}
				bool flag2 = !base.Owner.IsBackfacing() && this.m_owner.CurrentGun.sprite.WorldCenter.x - this.m_owner.specRigidbody.UnitCenter.x < 0f;
				if (flag2)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(SCP_323.spriteIds[0]);
				}
				bool flag3 = !base.Owner.IsBackfacing() && this.m_owner.CurrentGun.sprite.WorldCenter.x - this.m_owner.specRigidbody.UnitCenter.x > 0f;
				if (flag3)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(SCP_323.spriteIds[2]);
				}


				yield return null;
			}
			UnityEngine.Object.Destroy(prefab.gameObject);
			yield break;
		}


		private static GameObject skullprefab;

		private GameObject HatObject;


		public static List<int> spriteIds = new List<int>();



		public static PauseMenuController manager = new PauseMenuController();
		public static AmmonomiconController book = new AmmonomiconController();
	}

}
    