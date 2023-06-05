using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// WeaponAnimationData. Stores all information related to the weapon-specific procedural data.
    /// </summary>
    public class WeaponAnimationData : WeaponAnimationDataBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "Lowered Data")]

        [Tooltip("This object contains all the data needed for us to set the lowered pose of this weapon.")]
        [SerializeField, InLineEditor]
        private LowerData lowerData;
        
        [Title(label: "Camera Recoil Data")]

        [Tooltip("Weapon Recoil Data Asset. Used to get some camera recoil values, usually for weapons.")]
        [SerializeField, InLineEditor]
        private RecoilData cameraRecoilData;
        
        [Title(label: "Weapon Recoil Data")]

        [Tooltip("Weapon Recoil Data Asset. Used to get some recoil values, usually for weapons.")]
        [SerializeField, InLineEditor]
        private RecoilData weaponRecoilData;

        #endregion
        
        #region GETTERS

        /// <summary>
        /// GetCameraRecoilData.
        /// </summary>
        public override RecoilData GetCameraRecoilData() => cameraRecoilData;
        /// <summary>
        /// GetWeaponRecoilData.
        /// </summary>
        public override RecoilData GetWeaponRecoilData() => weaponRecoilData;

        /// <summary>
        /// GetRecoilData.
        /// </summary>
        public override RecoilData GetRecoilData(MotionType motionType) =>
            motionType == MotionType.Weapon ? GetWeaponRecoilData() : GetCameraRecoilData();

        /// <summary>
        /// GetLowerData.
        /// </summary>
        public override LowerData GetLowerData() => lowerData;

        #endregion
    }   
}