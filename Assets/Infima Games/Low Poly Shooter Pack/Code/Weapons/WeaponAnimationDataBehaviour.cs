using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// WeaponAnimationDataBehaviour. Used as an abstract class to contain all definitions for the Recoil class.
    /// </summary>
    public abstract class WeaponAnimationDataBehaviour : MonoBehaviour
    {
        #region GETTERS
        
        /// <summary>
        /// This function should return the RecoilData used for the camera.
        /// </summary>
        public abstract RecoilData GetCameraRecoilData();
        /// <summary>
        /// This function should return the RecoilData used for the weapon.
        /// </summary>
        public abstract RecoilData GetWeaponRecoilData();

        /// <summary>
        /// Returns a RecoilData value according to the passed MotionType.
        /// </summary>
        /// <returns></returns>
        public abstract RecoilData GetRecoilData(MotionType motionType);

        /// <summary>
        /// Return all the data needed to set the lowered pose of a weapon.
        /// </summary>
        public abstract LowerData GetLowerData();

        #endregion
    }
}