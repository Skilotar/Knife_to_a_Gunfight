﻿using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class LeaveRoomResetter : MonoBehaviour
    {
        public void Start()
        {
            parentRoom = parentRoom ?? transform.position.GetAbsoluteRoom();
            if (parentRoom != null)
            {
                parentRoom.BecameInvisible += ResetParentRoom;
            }
        }

        public void ResetParentRoom()
        {
            if (parentRoom != null)
            {
                parentRoom.ResetPredefinedRoomLikeDarkSouls();
                parentRoom.BecameInvisible -= ResetParentRoom;
                parentRoom = null;
                Destroy(gameObject);
            }
        }

        public RoomHandler parentRoom;
    }
}