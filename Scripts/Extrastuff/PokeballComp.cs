using System;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using System.Reflection;

namespace Knives
{
	public class PokeBallComp : MonoBehaviour
	{

		public PokeBallComp()
		{

		}


		private void Start()
		{
			ball = base.gameObject.GetComponent<Projectile>();
			ball.OnHitEnemy += this.onHitEnemy;
            player = ball.Owner as PlayerController;
            PKMList = player.gameObject.GetOrAddComponent<PlayerDestroyPKMRoomClear>();
            PierceProjModifier poke = ball.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetration = 10;
            
		}

        private void onHitEnemy(Projectile pokeball, SpeculativeRigidbody hitbody, bool arg3)
        {
            if(hitbody.aiActor != null)
            {
                if (hitbody.aiActor.healthHaver.IsAlive)
                {
					StartCoroutine(TryDoCatch(pokeball,hitbody.aiActor));
                    boy = hitbody.aiActor;
                }
            }
        }

        public void Update()
        {
            if(base.gameObject != null)
            {
                if(m_debris != null)
                {
                    if (GetPrivateType<DebrisObject, bool>(m_debris, "isFalling") && absorbed == true)
                    {
                        ETGModConsole.Log("-PokeBall Emergency!! :debris Pitfallen: Destroying all involved-",true);
                        boy.EraseFromExistenceWithRewards();
                        UnityEngine.Object.Destroy(m_debris.gameObject);
                    }
                }
                if(absorbed == true && m_debris == null) // this shouldnt be possible but ya know how it goes...
                {
                    ETGModConsole.Log("-PokeBall Emergency!! :debris Gone: Destroying all involved-",true);
                    boy.EraseFromExistenceWithRewards();
                }
            }
            
        }


        public AIActor boy;
        public DebrisObject m_debris;
        public bool absorbed = false;
        private IEnumerator TryDoCatch(Projectile pokeball, AIActor boy)
        {
            // Do fall
            DebrisObject debris = pokeball.BecomeDebris(pokeball.Direction * -5, .75f);
            debris.ClearVelocity();
            m_debris = debris;
            pokeball.specRigidbody.enabled = false;
            if(pokeball != null && boy != null) 
            {
                if (boy.healthHaver.GetCurrentHealthPercentage() < .6 && boy.healthHaver.IsBoss != true && boy.gameObject.GetOrAddComponent<AiactorSpecialStates>().caught == false)   //above 75% fail to absorb Or if they are already caught  // sorry no legendaries.. too buggy
                {
                    float catchThreshold = .4f;
                    float CatchMult = 1;
                    if (player.PlayerHasActiveSynergy("UltraBall"))
                    {
                        catchThreshold = .6f;
                        CatchMult = 2;
                    }
                    // Do absorb
                    if (boy != null)
                    {
                       
                        boy.behaviorSpeculator.Stun(.1f);
                        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(boy, debris));
                        boy.knockbackDoer.SetImmobile(true, "Pokeball");
                        boy.specRigidbody.enabled = false;
                        boy.healthHaver.IsVulnerable = false;
                        boy.ToggleRenderers(false);
                        if (boy.aiShooter != null) boy.aiShooter.ToggleGunAndHandRenderers(false, "PokeBall");
                        boy.ToggleShadowVisiblity(false);
                        boy.aiActor.enabled = false;
                        absorbed = true;
                    }
                   
                    if (pokeball != null)
                    {
                        // Do wobbly
                        if (boy.healthHaver.GetCurrentHealthPercentage() > catchThreshold)   // One Wobble for above half
                        {
                            yield return new WaitForSeconds(1f);
                            if (pokeball != null) randomBounce(pokeball);
                            yield return new WaitForSeconds(.5f);
                        }
                        else                                                     // 2 Wobbles for below half
                        {
                            if (pokeball != null) randomBounce(pokeball);
                            yield return new WaitForSeconds(1f);
                            if (pokeball != null) SetBounce(pokeball);
                            yield return new WaitForSeconds(1f);
                            if (pokeball != null) SetBounce(pokeball);
                            yield return new WaitForSeconds(.5f);

                        }
                    }
                    // Determine break or catch
                    bool CatchBoy = false;
                    if(boy != null)
                    {
                        if (boy.healthHaver.GetCurrentHealthPercentage() < catchThreshold) // above 40% chance no shot
                        {
                            if ((UnityEngine.Random.Range(0, .5f) * CatchMult) >= boy.healthHaver.GetCurrentHealthPercentage()) // chance gets better with less health
                            {
                                CatchBoy = true;
                            }
                        }

                    }

                    // Catch or break
                    if (CatchBoy)
                    {
                        if(boy != null)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_OBJ_chain_switch_01", base.gameObject);
                            //boy.EraseFromExistenceWithRewards(true);
                            debris.sprite.color = ExtendedColours.darkGreen;
                            boy.ApplyEffect(StaticStatusEffects.YellowChamberCharm);
                            PKMList.PKM.Add(boy);
                            boy.healthHaver.IsVulnerable = true;
                            boy.gameObject.GetOrAddComponent<AiactorSpecialStates>().caught = true; // can't catch your own pokemon Duhhh
                            yield return new WaitForSeconds(.25f);
                            StartCoroutine(FreeHim(boy, debris));
                            boy.IgnoreForRoomClear = true;

                            boy.CanTargetEnemies = true;
                            boy.CanTargetPlayers = false;

                        }

                    }
                    else // Failed catch Break free
                    {
                        if (boy != null)
                        {
                            AkSoundEngine.PostEvent("Play_WPN_shell_impact_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_CHR_fall_respawn_01", base.gameObject);
                            StartCoroutine(FreeHim(boy, debris));
                        }
                    }


                }
            }
           


            yield return null;
        }
        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
        private void SetBounce(Projectile pokeball)
        {
            if (left == false)
            {
                pokeball.spriteAnimator.Play("Bounce_left");
                left = true;
            }
            else
            {
                pokeball.spriteAnimator.Play("Bounce_right");
                left = false;
            }
            AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", base.gameObject);

        }

        public void randomBounce(Projectile pokeball)
        {
            if(UnityEngine.Random.Range(0,2f) > 1)
            {
                pokeball.spriteAnimator.Play("Bounce_left");
                left = true;
            }
            else
            {
                pokeball.spriteAnimator.Play("Bounce_right");
                left = false;
            }
            AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", base.gameObject);

        }
        public bool left = false;

        private IEnumerator FreeHim(AIActor boy, DebrisObject debris)
        {
            if(boy != null && debris != null)
            {
                boy.healthHaver.IsVulnerable = true;
                boy.RegisterOverrideColor(UnityEngine.Color.white, "freed");
                boy.ToggleRenderers(true);
                boy.ToggleShadowVisiblity(true);
                yield return new WaitForSeconds(.5f);
                if (boy != null && debris != null)
                {
                    if (boy.aiShooter != null) boy.aiShooter.ToggleGunAndHandRenderers(true, "PokeBall");
                    boy.DeregisterOverrideColor("freed");
                    boy.aiActor.enabled = true;
                    boy.knockbackDoer.SetImmobile(false, "Pokeball");
                    boy.specRigidbody.enabled = true;
                    UnityEngine.Object.Destroy(debris.gameObject);
                }
            }
            
        }



        private IEnumerator HandleEnemySuck(AIActor target,DebrisObject debris)
        {
            
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = .75f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag = debris && copySprite;
                if (flag)
                {
                    Vector3 position = debris.UnadjustedDebrisPosition;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }

            yield break;
        }


        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        public PlayerDestroyPKMRoomClear PKMList;
        public PlayerController player;
        public Projectile ball;
		public Gun basegun;
	}


}