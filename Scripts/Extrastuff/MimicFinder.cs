using System;
using ItemAPI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{   
	public class MimicFinderComp : MonoBehaviour
	{
		
		public MimicFinderComp()
		{

		}

		
		private void Start()
		{
			gun = base.GetComponent<Gun>();
			player = GunPlayerOwner(gun);
		}

		private void Update()
		{
			if(player.CurrentGun == gun)
            {
				if(gun.spriteAnimator.GetClipByName(MimicFoundAnimation) != null) // no animation found.. don't even bother calculating
                {
					if (gun.spriteAnimator.IsPlaying(gun.idleAnimation)) // is gun idle for delay duration? 
					{
						if (internalDelay <= 0)
						{
							if (player.CurrentRoom != null) //room exists?  
							{
								RoomHandler room = player.CurrentRoom;
								
								float dist = 15;
								IPlayerInteractable nearestInteractable = room.GetNearestInteractable(player.CenterPosition, dist, player); // any interactable Objects?
								if(nearestInteractable != null) 
                                {
									if(nearestInteractable is Chest) // is it a chest? 
                                    {
										Chest chest = nearestInteractable as Chest;
                                        if (chest.IsMimic) // Mimic Spotted!
										{
											tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName(MimicFoundAnimation);
											gun.spriteAnimator.Play(clip, 0, MimicFoundFPS, true);
											internalDelay = delay;
										}
                                    }
                                }
								
							}
						}
						else
						{
							internalDelay -= Time.deltaTime;
						}
					}
					else
					{
						internalDelay = delay;
					}
				}
            }
		}

		
		public PlayerController GunPlayerOwner(Gun bullet)
		{
			bool flag = bullet && bullet.CurrentOwner && bullet.CurrentOwner is PlayerController;
			PlayerController result;
			if (flag)
			{
				result = (bullet.CurrentOwner as PlayerController);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int MimicFoundFPS = 10;
		public string MimicFoundAnimation = null;
		public PlayerController player;
		public Gun gun;
		public float delay = .5f;
		private float internalDelay = .5f;
	}


}