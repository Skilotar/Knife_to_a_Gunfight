using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class projectileStates : MonoBehaviour
	{

		public projectileStates()
		{
			boostedbyHarlight = false;
			isloneStarStar = false;
			isloneStarLone = false;
			hitbomb = false;
			boostedbyshotgate = false;
			isfocusshot = false;
			LooperLooped = false;
			isPickStarter = false;
			CarSMG = false;
			isDiceStarter = false;
			OnionCharged = false;
			collideWhammy = false;
			Gunchuck = false;
			Chuckmain = false;
			gazer = false;
			Explodia = false;
			PepperCharge = false;
			PepperUn = false;
			picorispin = false;
			HonorSwipe = false;
			HonorStab = false;
			arcane = false;
		}


		private void Start()
		{
			

		}


		private void Update()
		{

		}

		public bool arcane;
		public bool picorispin;
		public bool PepperCharge;
		public bool PepperUn;
		public bool HonorSwipe;
		public bool HonorStab;
		public bool Explodia;
		public bool gazer;
		public bool Chuckmain;
		public bool Gunchuck;
		public bool OnionCharged;
		public bool isDiceStarter;
		public bool CarSMG;
		public bool isPickStarter;
		public bool LooperLooped;
		public bool isfocusshot;
		public bool boostedbyHarlight;
		public bool boostedbyshotgate;
		public bool isloneStarStar;
		public bool isloneStarLone;
		public bool hitbomb;
		public bool isveng;
		public bool collideWhammy;

		private Projectile projectile;
	}
}

