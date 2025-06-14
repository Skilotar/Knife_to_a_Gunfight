using System;
using ItemAPI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using HarmonyLib;

namespace Knives
{
	public class GunGradualReload : MonoBehaviour
	{

		public GunGradualReload()
		{
			
			reloadnumber = 1;
			emptyclipPunishtime = .5f;
			mincliptofire = 1;
		}


		private void Start()
		{
			gun = base.GetComponent<Gun>();
			player = GunPlayerOwner(gun);
            player.inventory.OnGunChanged += Inventory_OnGunChanged;
			
			origiReloadtime = gun.reloadTime;
			reloadrate = (gun.reloadTime / gun.ClipCapacity);
		}

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            if(previous == gun)
            {
				if (reloadSustainStopSound != null)
				{
					AkSoundEngine.PostEvent(reloadSustainStopSound, base.gameObject);
				}
			}
        }

        public bool holding = false;

		private void Update()
		{
			if(player != null)
            {
				if (Time.timeScale > 0)
				{
					if (player.CurrentGun == gun) // my gun?
					{
						if (holding == false)
						{

							if (IntroSound != null)
							{
								AkSoundEngine.PostEvent(IntroSound, base.gameObject);
							}
							holding = true;
						}

						if (gun.ClipShotsRemaining > 0 && gun.ClipShotsRemaining >= mincliptofire) // has ammo?
						{
							if (gun.IsReloading)
							{
								if (this.gun.IsReloading && Key(GungeonActions.GungeonActionType.Shoot, this.gun.CurrentOwner as PlayerController) && this.gun.ClipShotsRemaining > 0) //shot durring reload with ammo
								{
									maintaincount = gun.ClipShotsRemaining;
									this.gun.ForceImmediateReload(false); // force reload to finish and reset back to number we know was loaded beforehand.
									GameUIRoot.Instance.ForceClearReload(player.PlayerIDX);
									gun.ClipShotsRemaining = maintaincount;
								}

							}
						}
						else
						{
							if (gun.ClipShotsRemaining <= 0 && Key(GungeonActions.GungeonActionType.Shoot, this.gun.CurrentOwner as PlayerController)) // anti jank with the below clip shooting idk how you can shoot with negative ammo but you sure can :)
							{
								if (!gun.IsReloading)
								{
									this.gun.ClipShotsRemaining = 0;

									gun.reloadTime = ((this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * reloadrate) + emptyclipPunishtime;


									timer = reloadrate + emptyclipPunishtime; // adds extra time to the last bullet to punish spam shooting at the bottom of the clip.
									gun.Reload();
								}

							}
						}

						if (!this.gun.IsReloading)
						{

							if (startreload == true)
							{
								if (reloadSustainStopSound != null)
								{
									AkSoundEngine.PostEvent(reloadSustainStopSound, base.gameObject);
								}

								if (gun.ClipCapacity == gun.ClipShotsRemaining)
								{
									if (reloadEndSound != null)
									{
										AkSoundEngine.PostEvent(reloadEndSound, base.gameObject);
									}
								}


								startreload = false;
							}

							gun.reloadTime = ((this.gun.ClipCapacity - this.gun.ClipShotsRemaining) * reloadrate);
							if (gun.ClipShotsRemaining == 0)
							{
								gun.reloadTime = gun.reloadTime + emptyclipPunishtime;
							}
							//sets the guns reload time to the time it will take for all missing rounds to be loaded


						}

						if (this.gun.IsReloading)
						{
							if (startreload == false)
							{
								if (reloadStartSound != null)
								{
									AkSoundEngine.PostEvent(reloadStartSound, base.gameObject);

								}

								if (reloadSustainPlaySound != null)
								{
									AkSoundEngine.PostEvent(reloadSustainPlaySound, base.gameObject);
								}

								startreload = true;
							}

							if (gun.ClipShotsRemaining == 0)
							{
								gun.reloadTime = gun.reloadTime - emptyclipPunishtime;
							}
							reloadrate = (gun.reloadTime / gun.ClipCapacity);
							gun.reloadTime = origiReloadtime;
							if (timer > 0)
							{
								timer -= BraveTime.DeltaTime; // timer loop while gun is reloading
							}
							else
							{
								gun.MoveBulletsIntoClip(reloadnumber);
								timer = reloadrate * player.stats.GetStatModifier(PlayerStats.StatType.ReloadSpeed);


								if (hasRepeatingAnimation) gun.spriteAnimator.PlayFromFrame(gun.reloadAnimation, frameToRepeatAnimationFrom); // repeats animation

								foreach (string audio in additionalAudioEventsPerReload) // plays each audio sound if needed
								{
									AkSoundEngine.PostEvent(audio, base.gameObject);
								}


								if (gun.ClipCapacity == gun.ClipShotsRemaining)
								{
									maintaincount = gun.ClipShotsRemaining;
									this.gun.ForceImmediateReload(false); // force reload to finish and reset back to number we know was loaded beforehand.
									GameUIRoot.Instance.ForceClearReload(player.PlayerIDX);
									gun.ClipShotsRemaining = maintaincount;


								}
							}

						}
					}
					else
					{
						if (reloadSustainStopSound != null)
						{
							AkSoundEngine.PostEvent(reloadSustainStopSound, base.gameObject);
						}
						if (holding == true)
						{
							startreload = false;
							gun.reloadTime = origiReloadtime;
							holding = false;
						}
					}

				}
			}
            else
            {
				player = GunPlayerOwner(gun);
			}
		}

		public bool hasRepeatingAnimation; // for special animations that need to loop after a startup period

		public int frameToRepeatAnimationFrom; 

		public List<string> additionalAudioEventsPerReload = new List<string>(); // sounds to play as the gun is reloading ammo

		public string IntroSound; // sound for when the gun is picked up.

		public string reloadStartSound; // sound for starting reload

		public string reloadSustainPlaySound; // for looping sounds, should be used along with reloadsustainstop

		public string reloadSustainStopSound;

		public string reloadEndSound; // plays when the gun's clip is fully reloaded

		private bool startreload = false;
		public float emptyclipPunishtime;
		private int maintaincount;
		private float timer;
		private float reloadrate;
		private float origiReloadtime;
		public int reloadnumber; // number of ammo to reload per loop
		public Gun gun;
		public PlayerController player;
		public int mincliptofire;

		

		public bool Key(GungeonActions.GungeonActionType action, PlayerController user)
		{
			return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
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

		
		
		
	}


}