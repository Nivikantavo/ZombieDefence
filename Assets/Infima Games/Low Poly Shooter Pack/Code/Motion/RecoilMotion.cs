//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    //TODO: Inherit this from something that doesn't give you a motionType, since we don't really care.
    public class RecoilMotion : Motion
    {
        [Tooltip("The character's InventoryBehaviour component.")]
        [SerializeField, NotNull]
        private InventoryBehaviour inventoryBehaviour;
        
        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;
        
        [Title(label: "Recoil")]

        [Tooltip("The type of motion we want this component to apply.")]
        [SerializeField]
        private MotionType motionType;
        
        /// <summary>
        /// Recoil Spring Location. Used to apply location recoil to the camera.
        /// We don't really use this one as much as the rotation one, since applying location changes to the
        /// camera feels quite bad.
        /// </summary>
        private Spring recoilSpringLocation;
        /// <summary>
        /// Recoil Spring Rotation. Used to apply rotation recoil to the camera.
        /// </summary>
        private Spring recoilSpringRotation;

        /// <summary>
        /// Current Recoil Curves. We apply these, so it is important that they are up to date.
        /// </summary>
        private ACurves recoilCurves;

        /// <summary>
        /// Awake.
        /// </summary>
        protected override void Awake()
        {
            //Base.
            base.Awake();

            //Initialize Recoil Spring.
            recoilSpringLocation = new Spring();
            //Initialize Recoil Spring.
            recoilSpringRotation = new Spring();
        }
        /// <summary>
        /// Tick.
        /// </summary>
        public override void Tick()
        {
            //Check for reference errors.
            if (inventoryBehaviour == null || characterBehaviour == null)
            {
                //ReferenceError.
                Log.ReferenceError(this, gameObject);
                
                //Return.
                return;
            }

            //Try to get a WeaponAnimationDataBehaviour from the equipped weapon.
            var recoilBehaviour = inventoryBehaviour.GetEquipped().GetComponent<WeaponAnimationDataBehaviour>();
            //If there's none, then we don't even need to run this script at all, basically.
            if (recoilBehaviour == null)
                return;

            //Grab the RecoilData value we need.
            RecoilData recoilData = recoilBehaviour.GetRecoilData(motionType);
            //If there's no RecoilData assigned, then there's no reason to bother with this either, nothing will work.
            if (recoilData == null)
                return;
            
            //Get shotsFired.
            int shotsFired = characterBehaviour.GetShotsFired();
            //Get the recoilDataMultiplier value from the actual recoilData object.
            float recoilDataMultiplier = recoilData.StandingStateMultiplier;
            
            //Recoil Location.
            Vector3 recoilLocation = default;
            //Recoil Rotation.
            Vector3 recoilRotation = default;
            
            //TODO: COMMENT
            recoilCurves = recoilData.StandingState;
            if (characterBehaviour.IsAiming())
            {
                recoilDataMultiplier = recoilData.AimingStateMultiplier;
                recoilCurves = recoilData.AimingState;
            }
            
            //We really need a recoil object to calculate recoil. If we don't have one, we'll just completely ignore
            //doing any recoil, because there's no point.
            if (recoilCurves != null)
            {
                //We need three curves for things to work properly.
                if (recoilCurves.LocationCurves.Length == 3)
                {
                    /*
                    * Calculate the final recoil location by evaluating the recoil curve at the correct time.
                    * The correct time in this case is always the amount of shots that we have just fired, so the recoil
                    * curves are built to be based on specific ammo counts. Just something to take into account.
                   */
                    recoilLocation.x = recoilCurves.LocationCurves[0].Evaluate(shotsFired);
                    recoilLocation.y = recoilCurves.LocationCurves[1].Evaluate(shotsFired);
                    recoilLocation.z = recoilCurves.LocationCurves[2].Evaluate(shotsFired);
                }
            
                //We need three curves for things to work properly.
                if(recoilCurves.RotationCurves.Length == 3)
                {
                    //Calculate the final recoil rotation by evaluating the recoil curve at the correct time.
                    recoilRotation.x = recoilCurves.RotationCurves[0].Evaluate(shotsFired);
                    recoilRotation.y = recoilCurves.RotationCurves[1].Evaluate(shotsFired);
                    recoilRotation.z = recoilCurves.RotationCurves[2].Evaluate(shotsFired);
                }
            
                //TODO: Comment.
                recoilLocation *= recoilCurves.LocationMultiplier * recoilDataMultiplier;
                recoilRotation *= recoilCurves.RotationMultiplier * recoilDataMultiplier;
            }
            
            //Update the location recoil values.
            //We do this after the null check because we want to make sure the recoil stops smoothly (spring-ly?) even
            //if we suddenly don't have a recoil object anymore.
            recoilSpringLocation.UpdateEndValue(recoilLocation);  
            //Update the rotational recoil values.
            recoilSpringRotation.UpdateEndValue(recoilRotation);
        }

        /// <summary>
        /// GetLocation.
        /// </summary>
        public override Vector3 GetLocation()
        {
            //Check Reference.
            if (recoilCurves == null)
                return default;
            
            //Return.
            return recoilSpringLocation.Evaluate(recoilCurves.LocationSpring);
        }
        /// <summary>
        /// GetEulerAngles.
        /// </summary>
        public override Vector3 GetEulerAngles()
        {           
            //Check Reference.
            if (recoilCurves == null)
                return default;
            
            //Return.
            return recoilSpringRotation.Evaluate(recoilCurves.RotationSpring);
        }
    }
}