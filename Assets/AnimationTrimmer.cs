using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationTrimmer : MonoBehaviour
{
    public AnimationClip animationClip;
    public List<AnimationClip> trimmedAnimations;
    public List<Animation> animations;
    public Animator animator;
    public Animation anim;
    private int counter;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.AddComponent<Animator>();
        counter = 1;
        anim = GetComponent<Animation>();
        animationClip = anim.clip;
        animationClip.legacy = true;
        trimmedAnimations = new List<AnimationClip>();
        var maxFrameCount = Mathf.RoundToInt(animationClip.frameRate * animationClip.length);
        TrimAnimation(0,maxFrameCount/2,animationClip);
        TrimAnimation(maxFrameCount/2,maxFrameCount,animationClip);
    }

    // Update is called once per frame
    private void TrimAnimation(int startFrame, int endFrame, AnimationClip sourceClip)
    {
        var subClip = new AnimationClip();
        foreach (var binding in AnimationUtility.GetCurveBindings(sourceClip))
        {
            // Get the animation curve for the current binding
            var curve = AnimationUtility.GetEditorCurve(sourceClip, binding);

            // Create a new curve with only the keyframes within the desired frame range
            var newCurve = new AnimationCurve();
            for (var i = 0; i < curve.length; i++)
            {
                if (curve[i].time >= startFrame / sourceClip.frameRate &&
                    curve[i].time <= endFrame / sourceClip.frameRate)
                {
                    newCurve.AddKey(curve[i]);
                }
            }
            // Replace the curve in the clip with the trimmed curve
            subClip.SetCurve(binding.path, binding.type, binding.propertyName, newCurve);
        }

        // Set the new clip length based on the start and end frames
        subClip.frameRate = Mathf.RoundToInt(sourceClip.frameRate);
        subClip.wrapMode = WrapMode.Loop;
        subClip.SetCurve("", typeof(AnimationClip), "m_AnimationClipSettings.m_StopTime",
            new AnimationCurve(new Keyframe[] { new Keyframe(0, endFrame / sourceClip.frameRate) }));
        subClip.name = "Clip_" + counter++;
        subClip.legacy = true;
        trimmedAnimations.Add(subClip);
        anim.AddClip(subClip,subClip.name);
    }

    public void PlayClip1()
    {
        anim.clip = trimmedAnimations[0];
        anim.Play();
        // anim.Play(trimmedAnimations[0].name);
    }
    
    public void PlayClip2()
    {
        anim.clip = trimmedAnimations[1];
        anim.Play();
        // anim.Play(trimmedAnimations[1].name);
    }
}