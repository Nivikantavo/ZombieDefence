//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// MotionType.
    /// </summary>
    public enum MotionType { Camera, Weapon }
    
    /// <summary>
    /// Motion. This abstract class serves as a base class for all components that apply any sort of cool procedural
    /// motions to either the weapons, or the camera, in the asset.
    /// It has a bunch of helper things that make it easier to handle, and runs through the MotionApplier, forming
    /// a nice cycle! 
    /// </summary>
    [RequireComponent(typeof(MotionApplier))]
    public abstract class Motion : MonoBehaviour
    {
        #region PROPERTIES
        
        /// <summary>
        /// Alpha.
        /// </summary>
        public float Alpha => alpha;
        
        #endregion
        
        #region FIELDS SERIALIZED
        
        [Title(label: "Settings")]
        
        //TODO: Make private! And check the whole script for stuff.
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float alpha = 1.0f;

        [Title(label: "References")]
        
        //TODO: Tooltip.
        [SerializeField, NotNull]
        protected MotionApplier motionApplier;
        
        #endregion
        
        /// <summary>
        /// Awake.
        /// </summary>
        protected virtual void Awake()
        {
            //Try to get the applier if we haven't assigned it.
            if (motionApplier == null)
                motionApplier = GetComponent<MotionApplier>();
            
            //Subscribe.
            if(motionApplier != null)
                motionApplier.Subscribe(this);
        }

        /// <summary>
        /// Tick.
        /// </summary>
        public abstract void Tick();
        
        public abstract Vector3 GetLocation();

        public abstract Vector3 GetEulerAngles();
    }
}