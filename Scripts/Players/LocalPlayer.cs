using System.Numerics;

namespace ApexSharp
{
    internal class LocalPlayer : Entity
    {
        private long m_BasePointer;
        public override long BasePointer
        {
            get 
            {
                if (m_BasePointer == 0)
                    return m_BasePointer = Memory.Read<long>(Offset.REGION + Offset.LOCAL_PLAYER);
                return m_BasePointer;
            }
            set => m_BasePointer = value;
        }
        public Vector3 PunchWeaponAngles => Memory.Read<Vector3>(BasePointer + Offset.VEC_PUNCH_WEAPON_ANGLE);
        public Vector3 BreathAngles => Memory.Read<Vector3>(BasePointer + Offset.BREATH_ANGLES);
        public float TraversalStartTime => Memory.Read<float>(BasePointer + Offset.TRAVERSAL_START_TIME);
        public float TraversalProgress => Memory.Read<float>(BasePointer + Offset.TRAVERSAL_PROGRESS);

        public Vector3 ViewAngles
        {
            get => Memory.Read<Vector3>(BasePointer + Offset.VIEW_ANGLES);
            set => Memory.Write(BasePointer + Offset.VIEW_ANGLES, value);
        }

        public static bool CantPlay
        {
            get
            {
                const string kIngamePrefixName = "mp_rr_";
                var levelName = Memory.ReadString(Offset.REGION + Offset.LEVEL_NAME, kIngamePrefixName.Length);

                if (levelName.StartsWith(kIngamePrefixName))
                    return false;
                    
                return true;
            }
        }
    }
}
