using System;
using System.Collections;
using Dungeonator;
using UnityEngine;
using System.Collections.Generic;

using ItemAPI;


namespace Knives
{
	public class ProjToggleBeamMod : MonoBehaviour
	{

		
		public void Start()
		{
			projectile = base.gameObject.GetComponent<Projectile>();
			Player = projectile.Owner as PlayerController;
			TargetAngle = projectile.Direction.ToAngle();
		}

		public void Update()
		{
			if (projectile.gameObject != null)
			{
				if (LunaProjCollide.Toggle == true)
				{
					if (LastKnownBeamController == null)
					{
						mainbeamgroup();

						if (this.Player.HasPassiveItem(241))
                        {
							Scatterbeamgroup();
                        }

                        if (Player.PlayerHasActiveSynergy("Space Jammin"))
                        {
							Jamminbeamgroup();
                        }
						
					}
				}

				if(this.projectile.baseData.speed < 1f)
                {
					TargetAngle = TargetAngle - (60 * Time.deltaTime);
					this.projectile.SendInDirection(TargetAngle.DegreeToVector2(), true, true);
				}

			}
		}

		public void mainbeamgroup()
        {
			DoBeamCreate(this.beam, 0);
			DoBeamCreate(this.beam, 120);
			DoBeamCreate(this.beam, -120);
		}
		public void Scatterbeamgroup()
		{
			DoBeamCreate(this.beam, -180);
			DoBeamCreate(this.beam, 60);
			DoBeamCreate(this.beam, -60);
		}

		public void Jamminbeamgroup()
		{
			DoBeamCreate(pew[UnityEngine.Random.Range(0, 4)], UnityEngine.Random.Range(0, 360));
			DoBeamCreate(pew[UnityEngine.Random.Range(0, 4)], UnityEngine.Random.Range(0, 360));
			DoBeamCreate(pew[UnityEngine.Random.Range(0, 4)], UnityEngine.Random.Range(0, 360));
			DoBeamCreate(pew[UnityEngine.Random.Range(0, 4)], UnityEngine.Random.Range(0, 360));
			StartCoroutine(DanceParty());
		}
		private IEnumerator DanceParty()
		{
			
			int RNG = UnityEngine.Random.Range(0, validColors.Count);
			RenderSettings.ambientLight = validColors[RNG];
			CameraController camera = GameManager.Instance.MainCameraController;
			camera.DoScreenShake(shake, Player.CenterPosition);
			yield return new WaitForSeconds(.5f);
			int RNG2 = UnityEngine.Random.Range(0, validColors.Count);
			RenderSettings.ambientLight = validColors[RNG2];
			camera = GameManager.Instance.MainCameraController;
			camera.DoScreenShake(shake, Player.CenterPosition);

		}

		public void DoBeamCreate(Projectile beamtofire, float Angle)
        {
			BeamController beamController1 = BeamToolbox.FreeFireBeamFromAnywhere(beamtofire, this.Player, this.projectile.gameObject, Vector2.zero, false, 0, 1f, true, true, Angle);
			Projectile component1 = beamController1.GetComponent<Projectile>();
			if(beamtofire == this.beam) component1.baseData.damage *= 1.2f;
			BasicBeamController beam = component1.gameObject.GetOrAddComponent<BasicBeamController>();
			beam.penetration = 1;
			beam.reflections = 0;
			
			LastKnownBeamController = beamController1;
		}


		float TargetAngle = 0;
		public BeamController LastKnownBeamController;
		public Projectile beam = (PickupObjectDatabase.GetById(20) as Gun).DefaultModule.projectiles[0].projectile;
		public Projectile projectile;
		public PlayerController Player;

		List<Projectile> pew = new List<Projectile>()
		{
			((Gun)PickupObjectDatabase.GetById(121)).DefaultModule.projectiles[0].projectile,
			((Gun)PickupObjectDatabase.GetById(60)).DefaultModule.projectiles[0].projectile,
			((Gun)PickupObjectDatabase.GetById(87)).DefaultModule.projectiles[0].projectile,
			((Gun)PickupObjectDatabase.GetById(40)).DefaultModule.projectiles[0].projectile
		};
		public List<Color> validColors = new List<Color>
		{
			new Color(1,.78f,0), // yellow
            new Color(.89f,0f,.54f), // pink
            new Color(.1f,.80f,.84f), // blue
            new Color(.34f,.90f,.15f), //green
            new Color(.84f,.1f,.1f), //red
            new Color(.58f,.15f,.90f), //purple
        };
		public ScreenShakeSettings shake = new ScreenShakeSettings
		{
			magnitude = .5f,
			speed = 2f,
			time = 0f,
			falloff = .1f,
			direction = new Vector2(0, 0),
			vibrationType = ScreenShakeSettings.VibrationType.Auto,
			simpleVibrationStrength = Vibration.Strength.Medium,
			simpleVibrationTime = Vibration.Time.Instant
		};
	}
}
