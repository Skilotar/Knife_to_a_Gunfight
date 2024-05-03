using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using GungeonAPI;

public class EmmisiveBeams : MonoBehaviour
{
    public EmmisiveBeams()
    {
        this.EmissivePower = 100;
        this.EmissiveColorPower = 1.55f;
    }
    public void Start()
    {
        Transform trna = base.transform.Find("beam impact vfx");
        tk2dSprite sproot = trna.GetComponent<tk2dSprite>();
        if (sproot != null)
        {
            sproot.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            sproot.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            sproot.renderer.material.SetFloat("_EmissivePower", EmissivePower);
            sproot.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);
        }
        this.beamcont = base.GetComponent<BasicBeamController>();
        BasicBeamController beam = this.beamcont;
        beam.sprite.usesOverrideMaterial = true;
        BasicBeamController component = beam.gameObject.GetComponent<BasicBeamController>();
        bool flag = component != null;
        bool flag2 = flag;
        if (flag2)
        {
            component.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            component.sprite.renderer.material.SetFloat("_EmissivePower", EmissivePower);
            component.sprite.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);

        }
    }
    private BasicBeamController beamcont;
    public float EmissivePower;
    public float EmissiveColorPower;
}