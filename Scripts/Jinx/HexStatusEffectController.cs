using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using SaveAPI;

namespace Knives
{
	public class HexStatusEffectController : MonoBehaviour
	{

		public HexStatusEffectController()
		{
			statused = false;
			stealPunishment = false;
		}
		public static GameObject HexVFX;
		public static GameActorDecorationEffect HexDummyEffect;
		public static void Initialise()
		{
			HexVFX = SpriteBuilder.SpriteFromResource("Knives/Resources/Hex.png", new GameObject("HexIcon"));
			HexVFX.SetActive(false);
			tk2dBaseSprite vfxSprite = HexVFX.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(HexVFX);
			UnityEngine.Object.DontDestroyOnLoad(HexVFX);

			HexDummyEffect = new GameActorDecorationEffect()
			{
				AffectsEnemies = true,
				OverheadVFX = HexVFX,
				AffectsPlayers = true,
				AppliesTint = false,
				AppliesDeathTint = false,
				AppliesOutlineTint = true,
				OutlineTintColor = new Color(.84f, .0f, .33f),
				duration = float.MaxValue,
				effectIdentifier = "HexIcon",
				
				resistanceType = EffectResistanceType.None,
				PlaysVFXOnActor = false,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
			};
		}
		public void Start()
		{
			if(base.GetComponent<AIActor>() != null)
            {
				this.aIActor = base.GetComponent<AIActor>();
				if(aIActor.bulletBank != null)
                {
					aIActor.bulletBank.OnProjectileCreated += this.OnAiTriedToShoot;
				}
				if(aIActor.aiShooter != null)
                {
					aIActor.aiShooter.PostProcessProjectile += this.OnAiTriedToShoot;
                }

			}
			
			if(base.GetComponent<PlayerController>() != null)
            {
				this.player = base.GetComponent<PlayerController>();
				player.PostProcessProjectile += this.postprocessprojectile;
			}
			
		}
		System.Random rng = new System.Random();
        public void postprocessprojectile(Projectile arg1, float arg2)
        {
            if (statused && ActiveDelay && Key(GungeonActions.GungeonActionType.Shoot, player))
            {
                if (player != null)
                {

					int VFX = UnityEngine.Random.Range(1, 5);
					GameObject ChosenVFX = EasyVFXDatabase.HexHL;
					switch (VFX)
					{
						case 1:
							ChosenVFX = EasyVFXDatabase.HexHL;
							break;

						case 2:
							ChosenVFX = EasyVFXDatabase.HexHR;
							break;

						case 3:
							ChosenVFX = EasyVFXDatabase.HexVL;
							break;

						case 4:
							ChosenVFX = EasyVFXDatabase.HexVR;
							break;
					}

					// add dodge 
					if (player.CurrentGun.PickupObjectId == HexEater.ID)
                    {
						int Haunted = rng.Next(1, 40);
						if (Haunted == 1 && doing == false)
						{

							StartCoroutine(DoHexPlayerDamageProcessing(ChosenVFX));

						}
					}
                    else
                    {
						int Haunted = rng.Next(1, 10);
						if (Haunted == 1 && doing == false && internalmintimer <=0)
						{

							StartCoroutine(DoHexPlayerDamageProcessing(ChosenVFX));

						}
					}
					
				}
				
			}

        }
		public float internalmintimer = 0;
		public bool doing = false;
        private IEnumerator DoHexPlayerDamageProcessing(GameObject ChosenVFX)
        {
			doing = true;
			//do audio 
			AkSoundEngine.PostEvent("Play_WPN_tiger_growl_01", base.gameObject);
			AkSoundEngine.PostEvent("Play_WPN_tiger_growl_01", base.gameObject);

			//do windup
			GameObject Wind = player.PlayEffectOnActor(EasyVFXDatabase.HexWind, new Vector3(0f, 0f, 0f));
			Wind.GetComponent<tk2dBaseSprite>().scale *= 1.2f;
			Wind.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
			player.sprite.AttachRenderer(Wind.GetComponent<tk2dBaseSprite>());
			Wind.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");


			yield return new WaitForSeconds(.75f);


			GameObject slash = player.PlayEffectOnActor(ChosenVFX, new Vector3(0f, 0f, 0f));
			slash.GetComponent<tk2dBaseSprite>().scale *= 2;
			slash.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
			player.sprite.AttachRenderer(slash.GetComponent<tk2dBaseSprite>());
			slash.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
			AkSoundEngine.PostEvent("Play_WPN_tiger_swipe_01", base.gameObject);
			if (player.healthHaver.IsVulnerable & player.IsDodgeRolling == false)
            {
				// swipe hit audio
				player.healthHaver.ApplyDamage(1, Vector2.zero, "Tempted The Hex", CoreDamageTypes.Magic, DamageCategory.Normal, false, null, false);
				player.GetComponent<HexStatusEffectController>().statused = false;
				
				if (player.healthHaver.IsDead && AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HEXLINDED) == false)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HEXLINDED, true);
				}


			}


			yield return new WaitForSeconds(.1f);

			if(stealPunishment == true)
            {
				internalmintimer = 4f;
            }
			doing = false;

		}

		public void OnAiTriedToShoot(Projectile obj)
        {
			if (statused)
			{
                if (aIActor != null)
                {
					int VFX = UnityEngine.Random.Range(1, 5);
					GameObject ChosenVFX = EasyVFXDatabase.HexHL;
					switch (VFX)
					{
						case 1:
							ChosenVFX = EasyVFXDatabase.HexHL;
							break;

						case 2:
							ChosenVFX = EasyVFXDatabase.HexHR;
							break;

						case 3:
							ChosenVFX = EasyVFXDatabase.HexVL;
							break;

						case 4:
							ChosenVFX = EasyVFXDatabase.HexVR;
							break;
					}

					GameObject slash = aIActor.PlayEffectOnActor(ChosenVFX, new Vector3(0f, 0f, 0f));
					slash.GetComponent<tk2dBaseSprite>().scale *= 2;
					slash.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aIActor.CenterPosition + new Vector2(0, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
					
					aIActor.sprite.AttachRenderer(slash.GetComponent<tk2dBaseSprite>());
					slash.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                    if (aIActor.healthHaver.IsBoss)
                    {
						aIActor.healthHaver.ApplyDamage(1, Vector2.zero, "hexed", CoreDamageTypes.Magic, DamageCategory.Normal, false, null, false);
                        if (SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.BOSSDAMAGEVIAHEX) < 5000) 
						{
							SaveAPIManager.RegisterStatChange(CustomTrackedStats.BOSSDAMAGEVIAHEX, SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.BOSSDAMAGEVIAHEX) + 1);
						}
						
					}
                    else
                    {
						aIActor.healthHaver.ApplyDamage(2, Vector2.zero, "hexed", CoreDamageTypes.Magic, DamageCategory.Normal, false, null, false);
					}

					if (aIActor.EnemyGuid == "7c5d5f09911e49b78ae644d2b50ff3bf")
					{
						hitlich = true;
					}
				}
			}

		}
		bool ActiveDelay = false;
		bool setup = false;
		public bool hitlich = false;
		public void Update()
		{
			if (statused)
			{
                if (!setup)
                {
					// durration timer
					setup = true;
					StartCoroutine(HexingTimer());
                }

				if(aIActor != null)
                {
					if (aIActor.healthHaver)
					{
						if (aIActor.healthHaver.IsAlive)
						{

							aIActor.ApplyEffect(HexDummyEffect);


						}
					}
				}
				
				if(player != null)
                {
					if (player.healthHaver)
					{
						if (player.healthHaver.IsAlive)
						{
							player.ApplyEffect(HexDummyEffect);
							
						}
					}
				}
				
				
			}
            else
            {
                if (setup)
                {
					setup = false;
                }
				if(aIActor != null)
                {
					aIActor.RemoveEffect(HexDummyEffect);
					aIActor.ClearOverrideOutlineColor();
				}
                if (player != null)
                {
					player.RemoveEffect(HexDummyEffect);
					
				}
				if (stealPunishment == true)
                {
					statused = true;
                }


			}
			if(internalmintimer > 0)
            {
				internalmintimer -= Time.deltaTime;
            }
		}

        public IEnumerator HexingTimer()
        {
			if(aIActor != null)
            {
				yield return new WaitForSeconds(5f);
				statused = false;
			}
			if(player != null)
            {
				yield return new WaitForSeconds(.75f);
				ActiveDelay = true;
				yield return new WaitForSeconds(3.25f);
				ActiveDelay = false;
				statused = false;
			}
           
		}
		public bool Key(GungeonActions.GungeonActionType action, PlayerController user)
		{
			return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
		}

		public bool statused = false;
		public bool stealPunishment = false;
		private AIActor aIActor;
        private PlayerController player;
    }
}
