using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using Dungeonator;
using NpcApi;
using System.Collections;

using HutongGames.PlayMaker;

namespace Knives
{
	public class JinxItemDisplay : MonoBehaviour
	{

		public JinxItemDisplay()
		{


		}


		private void Start()
		{

			player = base.GetComponent<PlayerController>();
			//ETGModConsole.Log("JinxDisplayAdded");

		}

		public int pickupID = -1;
		public CustomShopController m_shop;
		private GameObject m_morgun;
		private bool doingtalk = false;
		private int last_spoken_pickup = -1;
		public bool IsInRoom = false;
		
		public void Update()
		{
			if (player != null && GameManager.Instance.IsLoadingLevel == false && Lockout == false)
			{
				RoomHandler room = player.GetAbsoluteParentRoom();
				

				if (room != null && room.GetRoomName() == "Knives/Resources/JinxShop/JinxShop_bigger")
				{
					IsInRoom = true;
					IPlayerInteractable nearestInteractable = room.GetNearestInteractable(player.sprite.WorldCenter, 1f, player);
					if (nearestInteractable != null)
					{
						PickupObject foundItem = null;


						if (nearestInteractable is CustomShopItemController)
						{
							//ETGModConsole.Log("getting interact");
							pickupID = (nearestInteractable as CustomShopItemController).item.PickupObjectId; // THIS WORKS DONT TOUCH IT!!!!!!!
							m_shop = (nearestInteractable as CustomShopItemController).GetShopController();
							m_morgun = m_shop.shopkeepFSM.Fsm.GameObject;
							foundItem = PickupObjectDatabase.GetById(pickupID);
							

							m_shop.OnSteal = this.PissedOffWitch;
						}


						if (foundItem != null )
						{
							if (last_spoken_pickup != foundItem.PickupObjectId)
							{
								if (foundItem.gameObject.GetComponent<JinxItemDisplayStorageClass>() != null) // the found item nearest item in 1 meters has the jinx storage class
								{

									if (m_morgun != null)
									{

										if (last_spoken_pickup != -1 && last_spoken_pickup != foundItem.PickupObjectId)//currently standing on NEW item 
										{
											killandCool();
										}

										TalkDoerLite speaker = m_morgun.GetComponent<TalkDoerLite>();
										if (speaker != null)
										{
											if (doingtalk != true)
											{
												last_spoken_pickup = foundItem.PickupObjectId;
												doingtalk = true;
												speaker.PreventInteraction = true;
												speaker.CloseTextBox(true);
												speaker.ForceTimedSpeech(foundItem.gameObject.GetComponent<JinxItemDisplayStorageClass>().jinxItemDisplayText, .1f, -1f, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT);
												
											}
										}
										else
										{
											ETGModConsole.Log("Talker is null?");
										}



									}
									else
									{
										ETGModConsole.Log("NPC is null??????");
									}
								}


							}
							else
							{

							}
						}



					}
					else
					{
						if (last_spoken_pickup != -1)
						{
							killandCool();
						}
					}
				}
                else
                {
					if (room.GetRoomName() != "Knives/Resources/JinxShop/JinxShop_bigger" && IsInRoom == true)
                    {
						IsInRoom = false;
						killandCool();
						
                    }
                }
			}
			

		}

		private void killandCool()
        {
			if(m_morgun != null)
            {
				TalkDoerLite speaker = m_morgun.GetComponent<TalkDoerLite>();
				speaker.CloseTextBox(true);
				speaker.PreventInteraction = false;
				last_spoken_pickup = -1;
				pickupID = -1;
				StartCoroutine(TalkCooldown());
			}
			

		}

		private IEnumerator TalkCooldown()
        {
			yield return new WaitForSeconds(.25f);
			doingtalk = false;
			
        }

        public GameObject nearItem;
		public PlayerController player;
		public bool Lockout = false;
		public bool PissedOffWitch(PlayerController arg1, PickupObject arg2, int arg3)
		{

			HexStatusEffectController hex = arg1.gameObject.GetOrAddComponent<HexStatusEffectController>();
			hex.statused = true;
			hex.stealPunishment = true; //     >:)
			StartCoroutine(Hexthem());
			
			return false;
		}

        private IEnumerator Hexthem()
        {
			Lockout = true;
			m_shop.NotifyStealFailed();

			HexStatusEffectController hex = this.player.gameObject.GetOrAddComponent<HexStatusEffectController>();
			hex.statused = true;
			hex.stealPunishment = true;

			AkSoundEngine.PostEvent("Play_Hex_laugh_001", base.gameObject);
			tk2dSpriteAnimationClip clip = m_morgun.GetOrAddComponent<tk2dSpriteAnimator>().GetClipByName("StealHex");
			m_morgun.GetOrAddComponent<AIAnimator>().PlayUntilFinished(clip.name, true, null, 1f);
			yield return new WaitForSeconds(1); 
			Lockout = false;

		}
	}
}
