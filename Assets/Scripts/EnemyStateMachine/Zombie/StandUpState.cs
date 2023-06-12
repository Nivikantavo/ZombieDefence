using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StandUpState : State
{
    public bool BonesIsReset { get; private set; }

    [SerializeField] private ZombieAnimation _animation;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _timeToResetBones;
    [SerializeField] private ZombieMovment _movment;

    private Transform _hipsBone;
    private BoneTransform[] _standUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransform;
    private Transform[] _bones;
    private float _elapsedResetTime;

    public event UnityAction BonesReset;

    private void Awake()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);

        _bones = _hipsBone.GetComponentsInChildren<Transform>();
        _standUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransform = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _standUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransform[boneIndex] = new BoneTransform();
        }

        GetStartAnimationPosition(_standUpBoneTransforms);
    }

    private void OnEnable()
    {
        AlignPositionToHips();
        PopulateBoneTransform(_ragdollBoneTransform);
        BonesIsReset = false;
        _movment.Stop();
    }

    private void Update()
    {
        if(BonesIsReset == false)
        {
            ResetBones();
        }
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPositin = _hipsBone.position;
        transform.position = _hipsBone.position;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPositin;
    }

    private void ResetBones()
    {
        _elapsedResetTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetTime / _timeToResetBones;

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransform[boneIndex].Position,
                _ragdollBoneTransform[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransform[boneIndex].Rotation,
                _ragdollBoneTransform[boneIndex].Rotation,
                elapsedPercentage);
        }

        if(elapsedPercentage >= 1)
        {
            BonesReset?.Invoke(); 
            _animation.SetStandUp();
            _elapsedResetTime = 0;
            BonesIsReset = true;
        }
    }

    private void PopulateBoneTransform(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

    private void GetStartAnimationPosition(BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == _animation.StandUpStateName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransform(_standUpBoneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }
}
