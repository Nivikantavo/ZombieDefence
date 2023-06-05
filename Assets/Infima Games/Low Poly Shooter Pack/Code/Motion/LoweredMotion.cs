using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// LoweredMotion. This class drives the procedural offsets that lower a weapon.
    /// </summary>
    public class LoweredMotion : Motion
    {
        #region FIELDS SERIALIZED
        
        [Tooltip("The LowerWeapon component that determines whether the character is lowering their " +
                 "weapon, or not at any given time.")]
        [SerializeField, NotNull]
        private LowerWeapon lowerWeapon;
        
        [Title(label: "References Character")]
        
        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;
        
        [Tooltip("The character's InventoryBehaviour component.")]
        [SerializeField, NotNull]
        private InventoryBehaviour inventoryBehaviour;

        #endregion
        
        /// <summary>
        /// Lowered Spring Location. Used to get the GameObject into a changed lowered
        /// pose.
        /// </summary>
        private Spring loweredSpringLocation;
        /// <summary>
        /// Recoil Spring Rotation. Used to get the GameObject into a changed lowered
        /// pose.
        /// </summary>
        private Spring loweredSpringRotation;

        /// <summary>
        /// LowerData for the current equipped weapon. If there's none, then there's no lowering, I guess.
        /// </summary>
        private LowerData lowerData;

        /// <summary>
        /// Awake.
        /// </summary>
        protected override void Awake()
        {
            //Base.
            base.Awake();

            //Initialize Lowered Spring.
            loweredSpringLocation = new Spring();
            //Initialize Lowered Spring.
            loweredSpringRotation = new Spring();
        }
        /// <summary>
        /// Tick.
        /// </summary>
        public override void Tick()
        {
            //Check References.
            if (lowerWeapon == null || characterBehaviour == null || inventoryBehaviour == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, gameObject);

                //Return.
                return;
            }

            //Get WeaponAnimationDataBehaviour.
            var animationData = inventoryBehaviour.GetEquipped().GetComponent<WeaponAnimationDataBehaviour>();
            if (animationData == null)
                return;
            
            //Get LowerData.
            lowerData = animationData.GetLowerData();
            if (lowerData == null)
                return;
            
            //TODO
            bool lowered = lowerWeapon.IsLowered() && !characterBehaviour.IsAiming();
            
            loweredSpringLocation.UpdateEndValue(lowered ? lowerData.LocationOffset : default);
            loweredSpringRotation.UpdateEndValue(lowered ? lowerData.RotationOffset : default);
        }

        /// <summary>
        /// GetLocation.
        /// </summary>
        public override Vector3 GetLocation()
        {
            //Check References.
            if (lowerData == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, gameObject);

                //Return;
                return default;
            }
            
            //Return.
            return loweredSpringLocation.Evaluate(lowerData.Interpolation);
        }
        /// <summary>
        /// GetEulerAngles.
        /// </summary>
        public override Vector3 GetEulerAngles()
        {
            //Check References.
            if (lowerData == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, gameObject);

                //Return;
                return default;
            }
            
            //Return.
            return loweredSpringRotation.Evaluate(lowerData.Interpolation);
        }
    }
}