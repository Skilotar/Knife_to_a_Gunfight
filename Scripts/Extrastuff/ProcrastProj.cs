using System;
using ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace Knives
{
    public class Procrast_Proj : MonoBehaviour
    {

        public Procrast_Proj()
        {

        }
        private void Start()
        {
            try
            {
                proj = base.gameObject.GetComponent<Projectile>();
                this.parentOwner = this.proj.ProjectilePlayerOwner();
                inator = this.proj.PossibleSourceGun;
                respawn = parentOwner.gameObject.GetOrAddComponent<Procrast_Respawn>();
                respawn.gun = inator;
                proj.OnHitEnemy += this.OnHitEnemy;
                AkSoundEngine.PostEvent("Play_Beew_001", base.gameObject);
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if(arg2.aiActor.healthHaver != null)
                {
                    if(arg2.aiActor.healthHaver.IsBoss != true)
                    {
                        bool bossroom = false;
                        foreach (AIActor actor in parentOwner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
                        {
                            if (actor != null)
                            {
                                if (actor.healthHaver != null)
                                {
                                    if (actor.healthHaver.IsBoss)
                                    {
                                        bossroom = true;
                                    }
                                }
                            }

                        }

                        if(bossroom != true)
                        {
                            GameObject VFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(VFX, arg2.UnitCenter, Quaternion.identity);
                            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(arg2.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                            respawn.GUIDs.Add(arg2.aiActor.EnemyGuid);
                            inator.LoseAmmo(1);
                            arg2.aiActor.EraseFromExistenceWithRewards(true);
                        }
                        
                    }
                }
            }
        }

        public Projectile proj;
        private PlayerController parentOwner;
        private Procrast_Respawn respawn;
        private Gun inator;
    }
}