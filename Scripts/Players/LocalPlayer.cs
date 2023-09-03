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

        private const long kLobbyNamePtr = 8746661657006010477;
        private static long LevelNamePtr => Memory.Read<long>(Offset.REGION + Offset.LEVEL_NAME);
        public static bool IsLobby => LevelNamePtr is kLobbyNamePtr;
    }
}
