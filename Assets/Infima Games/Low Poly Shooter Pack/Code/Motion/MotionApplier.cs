//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections.Generic;

namespace InfimaGames.LowPolyShooterPack
{
    public enum ApplyMode { Override, Add }
    
    public class MotionApplier : MonoBehaviour
    {
        [Title(label: "References")]

        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;

        [SerializeField, NotNull]
        private MovementBehaviour movementBehaviour;
        
        [Title(label: "Setup")]

        [SerializeField]
        private ApplyMode applyMode;
        
        private List<Motion> motions = new List<Motion>();

        private void LateUpdate()
        {
            Vector3 finalLocation = default;
            Vector3 finaEulerAngles = default;
            foreach (Motion motion in motions)
            {
                motion.Tick();
                finalLocation += motion.GetLocation() * motion.Alpha;
                finaEulerAngles += motion.GetEulerAngles() * motion.Alpha;
            }

            if(applyMode == ApplyMode.Override)
            {
                transform.localPosition = finalLocation;
                transform.localEulerAngles = finaEulerAngles;
            }
            else if (applyMode == ApplyMode.Add)
            {
                transform.localPosition += finalLocation;
                transform.localEulerAngles += finaEulerAngles;
            }
        }

        //TODO: TOOLTIP/Comment
        public CharacterBehaviour GetCharacterBehaviour() => characterBehaviour;
        public MovementBehaviour GetMovementBehaviour() => movementBehaviour;

        public void Subscribe(Motion motion) => motions.Add(motion);
    }
}