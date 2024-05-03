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

	class ten_gallon : PassiveItem
	{
		public static void Register()
		{

			string itemName = "Tall-tale Hat";


			string resourceName = "Knives/Resources/10_ammono";


			GameObject obj = new GameObject(itemName);


			var item = obj.AddComponent<ten_gallon>();


			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

			//Ammonomicon entry variables
			string shortDesc = "This Town Aint Big Enough..";
			string longDesc = "For My Hat!\n\n" +
				"In the wild fronteer of the gundrominian region it was a well established tradition that the scariest fellah wore the tallest hat.\n\n" +
				"This particular modle of 10,000 gallon hat can store all sorts of neat tricks inside!." +
				"\n\n\n - Knife_to_a_Gunfight";

			//Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
			//Do this after ItemBuilder.AddSpriteToObject!
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

			//Adds the actual passive effect to the item
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .15f, StatModifier.ModifyMethod.ADDITIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, .05f, StatModifier.ModifyMethod.ADDITIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, -1.5f, StatModifier.ModifyMethod.ADDITIVE);
			



			item.quality = PickupObject.ItemQuality.C;
			
			ten_gallon.BuildPrefab();

			//You should probably fix this to not be really stupid at some point. Just saying...
		}
		
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			


		}

		public override void  Update()
		{
			//hunger and eating system w/ special cases for robot
			if(this.Owner != null)
            {
				bool flag = base.Owner && !this.HatObject && base.Owner.CurrentGun.sprite;
				if (flag)
				{
					this.SpawnVFXAttached();
					this.SpawnVFXAttached();


				}
				bool flag2 = !base.Owner.CurrentGun.sprite && this.HatObject;
				if (flag2)
				{
					UnityEngine.Object.Destroy(this.HatObject);
					UnityEngine.Object.Destroy(this.HatObject);
				}
			}
			
			
			base.Update();

		}

	
		public static void BuildPrefab()
		{   //hatdoer
			GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/hat/10_right", null);
			gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameObject gameObject2 = new GameObject("tall hat");
			tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
			tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);
			ten_gallon.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/hat/10_left", tk2dSprite.Collection));
			ten_gallon.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Knives/Resources/hat/10_center", tk2dSprite.Collection));

			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			ten_gallon.spriteIds.Add(tk2dSprite.spriteId);
			gameObject2.SetActive(false);
			tk2dSprite.SetSprite(ten_gallon.spriteIds[0]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(ten_gallon.spriteIds[1]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			tk2dSprite.SetSprite(ten_gallon.spriteIds[2]);
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");

			FakePrefab.MarkAsFakePrefab(gameObject2);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			ten_gallon.hat = gameObject2;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E20 File Offset: 0x00003020
		private void SpawnVFXAttached()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ten_gallon.hat, base.Owner.transform.position + new Vector3(0.6f, 1.05f, -5f), Quaternion.identity);
			gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.Owner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
			GameManager.Instance.StartCoroutine(this.HandleSprite(gameObject));
			this.HatObject = gameObject;
		}


		public IEnumerator HandleSprite(GameObject prefab)
		{
			while (prefab != null && base.Owner != null)
			{
				prefab.transform.position = base.Owner.transform.position + new Vector3(0.2f, 1.05f, -5f);
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
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(ten_gallon.spriteIds[1]);//up
				}
				bool flag2 = !base.Owner.IsBackfacing() && this.m_owner.CurrentGun.sprite.WorldCenter.x - this.m_owner.specRigidbody.UnitCenter.x < 0f;
				if (flag2)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(ten_gallon.spriteIds[0]);//left
				}
				bool flag3 = !base.Owner.IsBackfacing() && this.m_owner.CurrentGun.sprite.WorldCenter.x - this.m_owner.specRigidbody.UnitCenter.x > 0f;
				if (flag3)
				{
					prefab.GetComponent<tk2dBaseSprite>().SetSprite(ten_gallon.spriteIds[2]);//right
				}


				yield return null;
			}
			UnityEngine.Object.Destroy(prefab.gameObject);
			yield break;
		}


		private static GameObject hat;

		private GameObject HatObject;


		public static List<int> spriteIds = new List<int>();



		public static PauseMenuController manager = new PauseMenuController();
		public static AmmonomiconController book = new AmmonomiconController();
	}

}