using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private BoneTransform[] _faceUpStandUpBoneTransforms;
    private BoneTransform[] _faceDownStandUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransform;
    private Transform[] _bones;
    private float _elapsedResetTime;
    private bool _hipsIsAlign = false;
    private bool _facingUp;

    public event UnityAction BonesReset;

    private void Awake()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);

        _bones = _hipsBone.GetComponentsInChildren<Transform>(false);
        _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransform = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransform[boneIndex] = new BoneTransform();
        }

        PopulateStartAnimationPosition(_faceUpStandUpBoneTransforms, true);
        PopulateStartAnimationPosition(_faceDownStandUpBoneTransforms, false);
    }

    private void OnEnable()
    {
        _facingUp = _hipsBone.up.y > 0;
        _hipsIsAlign = false;
        BonesIsReset = false;
        _movment.Stop();
    }

    private void Update()
    {
        if (_hipsIsAlign == false)
        {
            AlignRotationToHips();
            AlignPositionToHips(); 
            PopulateBoneTransform(_ragdollBoneTransform);
            _elapsedResetTime = 0;
            _hipsIsAlign = true;
        }
        else
        {
            if (BonesIsReset == false)
            {
                ResetBones();
            }
        }
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPositin = _hipsBone.position;
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransform()[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPositin;
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPositin = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.up;
        if (_facingUp)
        {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPositin;
        _hipsBone.rotation = originalHipsRotation;
    }

    private void ResetBones()
    {
        _elapsedResetTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetTime / _timeToResetBones;

        BoneTransform[] standUpBoneTransform = GetStandUpBoneTransform();

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransform[boneIndex].Position,
                standUpBoneTransform[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransform[boneIndex].Rotation,
                standUpBoneTransform[boneIndex].Rotation,
                elapsedPercentage);
        }

        if(elapsedPercentage >= 1)
        {
            BonesReset?.Invoke(); 
            _animation.SetStandUp(_facingUp);
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

    private void PopulateStartAnimationPosition(BoneTransform[] boneTransforms, bool facingUp)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;
        string clipName = facingUp ? _animation.FaceUpStateName : _animation.FaceDownStateName;
        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransform(boneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    private BoneTransform[] GetStandUpBoneTransform()
    {
        return _facingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }
}
