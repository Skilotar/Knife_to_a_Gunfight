using System;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	public class AiactorSpecialStates : MonoBehaviour
	{

		public AiactorSpecialStates()
		{

			isbeingcheckedbyOccams = false;
			LootedByBaba = false;
			hitbyovercharger = false;
			RedTaped = false;
			transmogedbyBookofMispells = false;
			Snared = false;
			Coilered = false;
			smelledChechPerf = false;
			broken = true;
			Rez = false;

		}


	private void Start()
		{
			this.aIActor = base.GetComponent<AIActor>();
			
		}


		private void Update()
		{
			
		}

		public bool ignition = false;
		public int currentHighNoonStacks;
		public bool EnemyIsCool = false;
		public int EnemyCoolnessLevel;
		public int ArcaneStacks = 0;
		public bool Rez = false;
		public bool pikiSuck = false;
		public AIActor PikiOverrideTarget;
		public bool LootedByBaba = false;
		public bool broken = false;
		public bool hitbyovercharger = false;
		public bool RedTaped = false;
		public bool isbeingcheckedbyOccams = false;
		public bool transmogedbyBookofMispells = false;
		public bool Snared = false;
		public bool Coilered = false;
		public bool smelledChechPerf = false;
		public int GlobalKillsGotten = 0;
		private AIActor aIActor;
		public AiactorSpecialStates.PikiColors PikiColor;
		public enum PikiColors
        {
			RED,
			YELLOW,
			BLUE,
			PURPLE,
			WHITE,
			
        };
	}
}
