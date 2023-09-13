using System.Numerics;

namespace ApexSharp
{
    internal class LocalPlayer : Entity
    {
        private long m_BasePointer = 0;

        public override long BasePointer 
        {
            get
            {
                if (m_BasePointer == 0)
                    m_BasePointer = Memory.Read<long>(Offset.REGION + Offset.LOCAL_PLAYER);
                return m_BasePointer;
            }
            set => m_BasePointer = value;
        }

        public Vector3 PunchWeaponAngles => Memory.Read<Vector3>(BasePointer + Offset.VEC_PUNCH_WEAPON_ANGLE);
        public Vector3 BreathAngles => Memory.Read<Vector3>(BasePointer + Offset.BREATH_ANGLES);
        public float TraversalStartTime => Memory.Read<float>(BasePointer + Offset.TRAVERSAL_START_TIME);
        public float TraversalProgress => Memory.Read<float>(BasePointer + Offset.TRAVERSAL_PROGRESS);
        public bool IsSemiAuto => Memory.Read<bool>(BasePointer + Offset.IS_SEMI_AUTO);
        public float TimeBase => Memory.Read<float>(BasePointer + Offset.TIME_BASE);
        
        public Vector3 ViewAngles
        {
            get => Memory.Read<Vector3>(BasePointer + Offset.VIEW_ANGLES);
            set => Memory.Write(BasePointer + Offset.VIEW_ANGLES, value);
        }
        public bool Attacking 
        {
            get 
            {
                const int Pressed = 5;

                var button = Memory.Read<SDK.KButtonT>(Offset.REGION + Offset.IN_ATTACK);
                if (button.State == Pressed)
                    return true;
                return false;
            }
        }
    }
    namespace SDK
    {
        public struct KButtonT
        {
            public int[] Down;
            public int State;
        }
    }
}
