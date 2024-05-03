using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class TiledSpriteConnector : MonoBehaviour
    {
        private void Start()
        {
            tiledSprite = base.GetComponent<tk2dTiledSprite>();
        }
        private void Update()
        {
            if (sourceRigidbody)
            {
                Vector2 unitCenter = sourceRigidbody.UnitCenter;
                Vector2 unitCenter2 = Vector2.zero;
                if (usesVector && targetVector != Vector2.zero) unitCenter2 = targetVector;
                else if (targetRigidbody) unitCenter2 = targetRigidbody.UnitCenter;
                if (unitCenter2 != Vector2.zero)
                {
                    tiledSprite.transform.position = unitCenter;
                    Vector2 vector = unitCenter2 - unitCenter;
                    float num = BraveMathCollege.Atan2Degrees(vector.normalized);
                    int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
                    tiledSprite.dimensions = new Vector2((float)num2, tiledSprite.dimensions.y);
                    tiledSprite.transform.rotation = Quaternion.Euler(0f, 0f, num);
                    tiledSprite.UpdateZDepth();

                }
                else
                {
                    if (eraseSpriteIfTargetOrSourceNull) UnityEngine.Object.Destroy(tiledSprite.gameObject);
                    else if (eraseComponentIfTargetOrSourceNull) UnityEngine.Object.Destroy(this);
                }
            }
            else
            {
                if (eraseSpriteIfTargetOrSourceNull) UnityEngine.Object.Destroy(tiledSprite.gameObject);
                else if (eraseComponentIfTargetOrSourceNull) UnityEngine.Object.Destroy(this);
            }
        }
       
        public SpeculativeRigidbody sourceRigidbody;
        public SpeculativeRigidbody targetRigidbody;
        public Vector2 targetVector;
        public bool usesVector;
        public bool eraseSpriteIfTargetOrSourceNull;
        public bool eraseComponentIfTargetOrSourceNull;
        private tk2dTiledSprite tiledSprite;
    }
}
