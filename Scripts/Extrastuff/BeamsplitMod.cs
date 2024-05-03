﻿using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BeamSplittingModifier : MonoBehaviour
{
    public BeamSplittingModifier()
    {
        subBeams = new Dictionary<BasicBeamController, float>();
        distanceTilSplit = 7f;
        amtToSplitTo = 0;
        splitAngles = 39;
        dmgMultOnSplit = 0.66f;
    }
    private void Start()
    {
        this.projectile = base.GetComponent<Projectile>();
        this.beamController = base.GetComponent<BeamController>();
        this.basicBeamController = base.GetComponent<BasicBeamController>();
        if (this.projectile.Owner is PlayerController) this.owner = this.projectile.Owner as PlayerController;

        if (projectile.baseData.range > distanceTilSplit)
        {
            originalRange = projectile.baseData.range;
            projectile.baseData.range = distanceTilSplit;
        }
        else
        {
            distanceTilSplit = projectile.baseData.range;
        }
    }

    private void ClearExtantSubBeams()
    {
        if (subBeams.Count <= 0) { return; }
        for (int i = subBeams.Count - 1; i >= 0; i--)
        {
            if (subBeams.ElementAt(i).Key && subBeams.ElementAt(i).Key.gameObject)
            {
                subBeams.ElementAt(i).Key.CeaseAttack();
            }
        }
        subBeams.Clear();
    }
    private void CreateNewSubBeams()
    {
        ClearExtantSubBeams();
        float ProjectileInterval = splitAngles / ((float)amtToSplitTo - 1);
        float currentAngle = basicBeamController.GetFinalBoneDirection();
        float startAngle = currentAngle + (splitAngles * 0.5f);
        int iteration = 0;

        for (int i = 0; i < amtToSplitTo; i++)
        {
            LinkedList<BasicBeamController.BeamBone> bones;
            bones = basicBeamController.m_bones;
            LinkedListNode<BasicBeamController.BeamBone> linkedListNode = null;
            if (bones != null) linkedListNode = bones.Last;
            else { Debug.LogError("Bones was NULL"); return; }

            Vector2 bonePosition = basicBeamController.GetBonePosition(linkedListNode.Value);

            float finalAngle = startAngle - (ProjectileInterval * iteration);

            GameObject newSubBeamPrefab = FakePrefab.Clone(projectile.gameObject);
            if (newSubBeamPrefab == null) Debug.LogError("BeamSplitComp: Cloned Beam Prefab was NULL!");

            BeamController controllerPrefab = newSubBeamPrefab.GetComponent<BeamController>();
            if (controllerPrefab == null) { Debug.LogError("BeamSplitComp: ControllerPrefab was NULL!"); }
            if (controllerPrefab is BasicBeamController)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(controllerPrefab.gameObject);

                BasicBeamController newBasicSubBeam = gameObject.GetComponent<BasicBeamController>();
                newBasicSubBeam.State = BasicBeamController.BeamState.Firing;
                newBasicSubBeam.HitsPlayers = false;
                newBasicSubBeam.HitsEnemies = true;
                newBasicSubBeam.Origin = bonePosition;
                newBasicSubBeam.Direction = finalAngle.DegreeToVector2();
                newBasicSubBeam.usesChargeDelay = false;
                newBasicSubBeam.muzzleAnimation = string.Empty;
                newBasicSubBeam.chargeAnimation = string.Empty;
                newBasicSubBeam.beamStartAnimation = string.Empty;
                newBasicSubBeam.projectile.Owner = this.projectile.Owner;
                newBasicSubBeam.Owner = this.basicBeamController.Owner;
                newBasicSubBeam.Gun = this.basicBeamController.Gun;
                if (originalRange > 0) newBasicSubBeam.projectile.baseData.range = originalRange;
                newBasicSubBeam.projectile.baseData.damage *= dmgMultOnSplit;

                if (newBasicSubBeam.GetComponent<BeamSplittingModifier>()) Destroy(newBasicSubBeam.GetComponent<BeamSplittingModifier>());

                subBeams.Add(newBasicSubBeam, (ProjectileInterval * iteration));
            }
            else { Debug.LogError("BeamSplitComp: Controller prefab was not beam????"); }

            iteration++;
        }
    }
    private void Update()
    {
        if (projectile.baseData.range > distanceTilSplit) { originalRange = projectile.baseData.range; projectile.baseData.range = distanceTilSplit; }
        if ((basicBeamController.ApproximateDistance >= distanceTilSplit) && subBeams.Count < amtToSplitTo)
        {
            CreateNewSubBeams();
        }
        if ((basicBeamController.ApproximateDistance < distanceTilSplit) && subBeams.Count > 0)
        {
            ClearExtantSubBeams();
        }
        float currentAngle = basicBeamController.GetFinalBoneDirection();
        float startAngle = currentAngle + (splitAngles * 0.5f);
        if (subBeams.Count > 0)
        {
            for (int i = 0; i < subBeams.Count; i++)
            {
                BasicBeamController particularSubBeam = subBeams.ElementAt(i).Key;
                LinkedList<BasicBeamController.BeamBone> bones;
                bones = basicBeamController.m_bones;
                LinkedListNode<BasicBeamController.BeamBone> linkedListNode = bones.Last;
                Vector2 bonePosition = basicBeamController.GetBonePosition(linkedListNode.Value);

                float angleOffset = subBeams.ElementAt(i).Value;
                particularSubBeam.Direction = (startAngle - angleOffset).DegreeToVector2();
                particularSubBeam.Origin = bonePosition;
                particularSubBeam.LateUpdatePosition(bonePosition);
            }
        }
    }
    private void OnDestroy()
    {
        ClearExtantSubBeams();
    }
    private Dictionary<BasicBeamController, float> subBeams;
    public float distanceTilSplit;
    public int amtToSplitTo;
    public float splitAngles;
    public float dmgMultOnSplit;

    private float originalRange;
    private Projectile projectile;
    private BasicBeamController basicBeamController;
    private BeamController beamController;
    private PlayerController owner;
} //Makes the beam split into multiple weaker beams after travelling a certain distance