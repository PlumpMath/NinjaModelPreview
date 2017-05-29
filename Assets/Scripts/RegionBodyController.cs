using UnityEngine;
using System.Collections;

public class RegionBodyController : MonoBehaviour {

    public Animation anim;
    /* Body parts names BodyXXXXX */
    public GameObject[] bodyParts;
    /* Bones:
     * 0 - Neck
     * 1 - Left calf
     * 2 - Right calf
     * 3 - Root
     * 4 - Spine
     * 5 - Right clavicle
     * 6 - Right upper arm
     * 7 - Right forearm
     **/
    public Transform[] locomotionBones;

    public float blendSpeed = 10.0f;
    public float smoothBlendSpeed = 3.0f;
    public float leaningSpeed = 5.0f;
    public float leaningAngle = 25.0f;

    public Vector3 desiredDirection = Vector3.forward;
    public Vector3 direction = Vector3.forward;
    public Vector3 hookDirection = Vector3.forward;
    public float speed = 0.0f;
    public bool searching = false;
    public bool leaving = false;
    public bool dancing = false;
    public bool pickingup = false;
    public bool block = false;
    public bool hookThrowing = false;
    public bool hookRollback = false;
    public bool hookVisible = false;
    public bool pulling = false;

    private Vector3 lastDirection = Vector3.forward;
    private float animIdleWeight = 0.0f;
    private float animWalkingWeight = 0.0f;
    private float animRunningWeight = 0.0f;
    private float animSearchingWeight = 0.0f;
    private float animLeavingWeight = 0.0f;
    private float animDancingWeight = 0.0f;
    private float animPickingUpWeight = 0.0f;
    private float animBlockWeight = 0.0f;
    private float animSpearWeight = 0.0f;
    private float animSpearInWeight = 0.0f;
    private float animSpearIdleWeight = 0.0f;
    private float animSpearOutWeight = 0.0f;
    private float animSpearDamageAttackerWeight = 0.0f;
    private float animSpearPullAttackerWeight = 0.0f;
    private float animSpearDamageVictimWeight = 0.0f;
    private float animSpearPullVictimWeight = 0.0f;
    private bool hookAnimSetupMoving = false;
    private float hookThrowingTime = 0.0f;
    private float pullingTime = 0.0f;

    private float smoothLean = 0.0f;
    private float animHookLocoAngle = 0.0f;

    private AnimationState animIdle = null;
    private AnimationState animWalk = null;
    private AnimationState animRun = null;
    private AnimationState animSearching = null;
    private AnimationState animLeaving = null;
    private AnimationState animDancing = null;
    private AnimationState animPickingUp = null;
    private AnimationState animBlock = null;
    private AnimationState animSpearIn = null;
    private AnimationState animSpearIdle = null;
    private AnimationState animSpearOut = null;
    private AnimationState animSpearDamageAttacker = null;
    private AnimationState animSpearPullAttacker = null;
    private AnimationState animSpearDamageVictim = null;
    private AnimationState animSpearPullVictim = null;

    void Start () {

        animIdle = anim["Idle"];
        animIdle.enabled = true;
        animIdle.layer = 1;
        animWalk = anim["Walk"];
        animWalk.enabled = false;
        animWalk.layer = 1;
        animRun = anim["Run"];
        animRun.enabled = false;
        animRun.layer = 1;
        animRun.speed = 0.66f;
        animSearching = anim["Searching"];
        animSearching.enabled = false;
        animSearching.layer = 1;
        //animLeaving = anim["Leaving"];
        //animLeaving.enabled = false;
        //animLeaving.layer = 1;
        //animDancing = anim["Dancing"];
        //animDancing.enabled = false;
        //animDancing.layer = 1;
        animPickingUp = anim["Pickup"];
        animPickingUp.enabled = false;
        animPickingUp.layer = 1;
        animBlock = anim["Block"];
        animBlock.enabled = false;
        animBlock.layer = 1;
        animSpearIn = anim["Spear_In"];
        animSpearIn.enabled = false;
        animSpearIn.layer = 1;
        animSpearIdle = anim["Spear_Idle"];
        animSpearIdle.enabled = false;
        animSpearIdle.layer = 1;
        animSpearOut = anim["Spear_Out"];
        animSpearOut.enabled = false;
        animSpearOut.layer = 1;
        animSpearDamageAttacker = anim["Spear_Damage_Attacker"];
        animSpearDamageAttacker.enabled = false;
        animSpearDamageAttacker.layer = 2;
        animSpearPullAttacker = anim["Spear_DamagePull_Attacker"];
        animSpearPullAttacker.enabled = false;
        animSpearPullAttacker.layer = 2;
        animSpearDamageVictim = anim["Spear_Damage_Victim"];
        animSpearDamageVictim.enabled = false;
        animSpearDamageVictim.layer = 2;
        animSpearPullVictim = anim["Spear_DamagePull_Victim"];
        animSpearPullVictim.enabled = false;
        animSpearPullVictim.layer = 2;

        animSpearIn.AddMixingTransform(locomotionBones[3]);
        animSpearIdle.AddMixingTransform(locomotionBones[3]);
        animSpearOut.AddMixingTransform(locomotionBones[3]);
        animBlock.AddMixingTransform(locomotionBones[3]);


    }

    void Update () {

        float f;
        float sign;
        float animBlockingActionWeight = 0.0f;
        float animPriorActionWeight = 0.0f;
        Vector3 v3;
        Quaternion q1;
        Quaternion q2;

        /* Throwing weight setup */

        if (hookThrowing || hookRollback)
        {
            hookThrowingTime += Time.deltaTime;
            SetupThrowingMixing();
        }
        else
        {
            hookThrowingTime = 0.0f;
        }

        if (pulling)
        {
            pullingTime += Time.deltaTime;
        }
        else
        {
            pullingTime = 0.0f;
        }

        BlendBooleanAnimation(ref animSpearInWeight, hookThrowing && hookThrowingTime < 0.433f);
        BlendBooleanAnimation(ref animSpearIdleWeight, hookThrowing && hookThrowingTime >= 0.433f);
        BlendBooleanAnimation(ref animSpearOutWeight, hookRollback);

        /* Blocking action weights setup */

        BlendBooleanAnimation(ref animPickingUpWeight, pickingup);

        /* Pulling weight setup */

        BlendBooleanAnimation(ref animSpearDamageAttackerWeight, pulling && pullingTime < 0.667f && (hookThrowing || hookRollback));
        BlendBooleanAnimation(ref animSpearPullAttackerWeight, pulling && pullingTime >= 0.667f && (hookThrowing || hookRollback));
        BlendBooleanAnimation(ref animSpearDamageVictimWeight, pulling && pullingTime < 1.0f && !(hookThrowing || hookRollback));
        BlendBooleanAnimation(ref animSpearPullVictimWeight, pulling && pullingTime >= 1.0f && !(hookThrowing || hookRollback));

        /* Moving weight setup */

        animSpearWeight = Mathf.Min(1.0f, animSpearInWeight + animSpearIdleWeight + animSpearOutWeight);

        animPriorActionWeight = animSpearWeight;

        BlendBooleanAnimation(ref animSearchingWeight, searching && speed < 0.05f && animSpearWeight <= 0.0f);
        BlendBooleanAnimation(ref animLeavingWeight, leaving);
        BlendBooleanAnimation(ref animBlockWeight, block && animSpearWeight <= 0.0f);

        animBlockingActionWeight = Mathf.Max(0.0f, Mathf.Min(1.0f, Mathf.Max(animDancingWeight, Mathf.Max(animPickingUpWeight, Mathf.Max(animSearchingWeight, animLeavingWeight)))));

        BlendBooleanAnimation(ref animRunningWeight, speed >= 0.05f, true);

        animIdleWeight = Mathf.Max(0.0f, 1.0f - animWalkingWeight - animRunningWeight - animPriorActionWeight - animBlockingActionWeight);

        /* Set animation states */

        SetAnimationWeight(animIdle, animIdleWeight);
        SetAnimationWeight(animRun, Mathf.Max(0.0f, animRunningWeight - animBlockingActionWeight));
        SetAnimationWeight(animSearching, animSearchingWeight);
        SetAnimationWeight(animBlock, animBlockWeight * 5.0f);
        //SetAnimationWeight(animLeaving, animLeavingWeight);
        //SetAnimationWeight(animDancing, animDancingWeight);
        SetAnimationWeight(animPickingUp, animPickingUpWeight * 5.0f);
        //SetAnimationWeight(animBlock, animBlockWeight);
        SetAnimationWeight(animSpearIn, animSpearInWeight);
        SetAnimationWeight(animSpearIdle, animSpearIdleWeight);
        SetAnimationWeight(animSpearOut, animSpearOutWeight);
        SetAnimationWeight(animSpearDamageAttacker, animSpearDamageAttackerWeight);
        SetAnimationWeight(animSpearPullAttacker, animSpearPullAttackerWeight);
        SetAnimationWeight(animSpearDamageVictim, animSpearDamageVictimWeight);
        SetAnimationWeight(animSpearPullVictim, animSpearPullVictimWeight);

        /* Leaning */

        if (speed > 0.05f)
        {
            q1 = Quaternion.LookRotation(desiredDirection, Vector3.up);
            q2 = Quaternion.LookRotation(direction, Vector3.up);

            float eulerYn = q1.eulerAngles.y;
            float eulerYc = q2.eulerAngles.y;

            if (eulerYc - eulerYn > 180.0f)
            {
                eulerYc -= 360.0f;
            }
            else if (eulerYn - eulerYc > 180.0f)
            {
                eulerYc += 360.0f;
            }

            sign = 1.0f;
            if (eulerYc > eulerYn)
            {
                sign = -1.0f;
            }
            f = Mathf.Min(1.0f, Time.deltaTime * leaningSpeed);
            smoothLean = smoothLean * (1.0f - f) + Mathf.Min(leaningAngle, Mathf.Max(-leaningAngle, Vector3.Angle(direction, desiredDirection) * sign)) * f;
        }
        else
        {
            if (smoothLean != 0.0f)
            {
                sign = smoothLean / Mathf.Abs(smoothLean);
                smoothLean -= Mathf.Min(Mathf.Abs(smoothLean), Time.deltaTime * 30.0f) * sign;
            }
        }

        /* Body transformations */

        transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);

    }

    /*********************************
     *
     *  After animation skeletal modifications
     *
     *********************************/

    public void LateUpdate()
    {
        int i;
        float a;
        float f;
        locomotionBones[0].Rotate(0.0f, -smoothLean * 0.5f, 0.0f);
        locomotionBones[1].Rotate(0.0f, smoothLean, 0.0f);
        locomotionBones[2].Rotate(0.0f, smoothLean, 0.0f);
        locomotionBones[8].Rotate(smoothLean, 0.0f, 0.0f);
        if (hookThrowing || hookRollback)
        {
            a = Vector3.Angle(direction, hookDirection.normalized);
            if (Vector3.Cross(direction, hookDirection.normalized).y > 0.0f)
            {
                a *= -1.0f;
            }
            f = Mathf.Min(1.0f, Time.deltaTime * 5.0f);
            animHookLocoAngle = animHookLocoAngle * (1.0f - f) + a * f;
            a = animHookLocoAngle;
            if (a < 0.0f)
            {
                locomotionBones[4].Rotate(0.0f, 0.0f, a * 0.25f);
                locomotionBones[5].Rotate(0.0f, 0.0f, a * 0.25f);
                locomotionBones[6].Rotate(0.0f, 0.0f, a * 0.3f);
                locomotionBones[7].Rotate(0.0f, 0.0f, a * 0.6f);
            }
            else
            {
                locomotionBones[4].Rotate(0.0f, 0.0f, a * 0.25f);
                locomotionBones[5].Rotate(0.0f, 0.0f, a * 0.4f);
                locomotionBones[6].Rotate(0.0f, 0.0f, a * 0.4f);
                locomotionBones[7].Rotate(0.0f, 0.0f, a * 0.3f);
            }
        }
    }

    /*********************************
     *
     *  Public operations
     *
     *********************************/

    public void SetCloth(string id)
    {
        int i;
        bool b = false;
        string clothId = "Body" + id;
        for (i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i].name == clothId)
            {
                b = true;
            }
        }
        if (!b)
        {
            clothId = "Body10001";
        }
        for (i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i].name.Substring(0, clothId.Length) != clothId)
            {
                Destroy(bodyParts[i]);
            }
        }
    }

    /*********************************
     *
     *  Local utility
     *
     *********************************/

    private void SetAnimationWeight(AnimationState state, float weight)
    {
        if(state.weight == weight)
        {
            return;
        }
        state.weight = weight;
        if (state.enabled && weight <= 0.0f)
        {
            state.enabled = false;
        }
        else if (!state.enabled && weight > 0.0f)
        {
            state.enabled = true;
            state.time = 0.0f;
        }
    }

    private void BlendBooleanAnimation(ref float value, bool flag)
    {
        BlendBooleanAnimation(ref value, flag, false);
    }

    private void BlendBooleanAnimation(ref float value, bool flag, bool smooth)
    {
        float speed = blendSpeed;
        if(smooth)
        {
            speed = smoothBlendSpeed;
        }
        if(flag && value < 1.0f)
        {
            value += Time.deltaTime * speed;
            if (value > 1.0f)
            {
                value = 1.0f;
            }
        }
        else if(!flag && value > 0.0f)
        {
            value -= Time.deltaTime * speed;
            if(value < 0.0f)
            {
                value = 0.0f;
            }
        }
    }

    private void SetupThrowingMixing()
    {
        if (!hookAnimSetupMoving && speed > 0.05f)
        {
            hookAnimSetupMoving = true;
            animSpearIn.RemoveMixingTransform(locomotionBones[3]);
            animSpearIn.AddMixingTransform(locomotionBones[4]);
            animSpearIdle.RemoveMixingTransform(locomotionBones[3]);
            animSpearIdle.AddMixingTransform(locomotionBones[4]);
            animSpearOut.RemoveMixingTransform(locomotionBones[3]);
            animSpearOut.AddMixingTransform(locomotionBones[4]);
        }
        else if (hookAnimSetupMoving && speed <= 0.05f)
        {
            hookAnimSetupMoving = false;
            animSpearIn.RemoveMixingTransform(locomotionBones[4]);
            animSpearIn.AddMixingTransform(locomotionBones[3]);
            animSpearIdle.RemoveMixingTransform(locomotionBones[4]);
            animSpearIdle.AddMixingTransform(locomotionBones[3]);
            animSpearOut.RemoveMixingTransform(locomotionBones[4]);
            animSpearOut.AddMixingTransform(locomotionBones[3]);
        }
    }

}
