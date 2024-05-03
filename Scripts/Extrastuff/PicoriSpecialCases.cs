using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;

namespace Knives
{
    class PicoriSpecialCases : MonoBehaviour
    {

        public PicoriSpecialCases()
        {


        }


        private void Start()
        {

            player = base.GetComponent<PlayerController>();
            player.OnUsedPlayerItem += Player_OnUsedPlayerItem;

        }

        private void Player_OnUsedPlayerItem(PlayerController arg1, PlayerItem arg2)
        {

            if (Fourk.m_extantKages != null)
            {
                if (player.CurrentItem.PickupObjectId == 108 || player.CurrentItem.PickupObjectId == 109 || player.CurrentItem.PickupObjectId == 448)
                {
                    if (player.PlayerHasActiveSynergy("Tools of the Heros"))
                    {
                        StartCoroutine(DoSpawnExtras(player));
                    }       
                }
            }
        }

        private IEnumerator DoSpawnExtras(PlayerController player)
        {
            doingExtras = true;
            SpawnObjectPlayerItem item = player.CurrentItem.GetComponent<SpawnObjectPlayerItem>();
            GameObject bomb = item.objectToSpawn;
            bool isbomb = false;
            if (player.CurrentItem.PickupObjectId == 108 || player.CurrentItem.PickupObjectId == 109)
            {
                isbomb = true;
            }
            foreach (KageBunshinController Dude in Fourk.m_extantKages)
            {
                if (Dude != null)
                {
                    if (Dude.gameObject != null)
                    {


                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(bomb, Dude.specRigidbody.UnitCenter, Quaternion.identity);
                        Vector3 vector2 = Dude.specRigidbody.UnitCenter;
                        tk2dBaseSprite component4 = gameObject.GetComponent<tk2dBaseSprite>();
                        if (component4)
                        {
                            component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
                        }
                        if (isbomb)
                        {
                            Vector2 vector3 = player.unadjustedAimPoint - player.LockedApproximateSpriteCenter;
                            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject, Dude.specRigidbody.UnitCenter, vector3, item.tossForce, false, false, true, false);
                        }


                    }
                }
            }
            yield return new WaitForSeconds(.1f);
            doingExtras = false;
        }

        private void Update()
        {
            

        }

        public static bool doingExtras = false;


        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }


        public PlayerController player;
    }
}