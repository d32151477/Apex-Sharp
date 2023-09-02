using System.Numerics;
using System.Runtime.InteropServices;

namespace ApexSharp
{
    internal abstract class Entity
    {
        public abstract long BasePointer { get; set; }
        public bool Invalid => BasePointer == 0;
        
        public Vector3 LocalOrigin => Memory.Read<Vector3>(BasePointer + Offset.LOCAL_ORIGIN);
        public Vector3 VecAbsVelocity => Memory.Read<Vector3>(BasePointer + Offset.VEC_ABS_VELOCITY);
        public int TeamNum => Memory.Read<int>(BasePointer + Offset.TEAM_NUM);
        public int ShieldHealth => Memory.Read<int>(BasePointer + Offset.SHIELD_HEALTH);
        public bool IsDead => Memory.Read<short>(BasePointer + Offset.LIFE_STATE) > 0;
        public bool IsKnocked => Memory.Read<short>(BasePointer + Offset.BLEEDOUT_STATE) > 0;
        public Vector3 CameraOrigin => Memory.Read<Vector3>(BasePointer + Offset.CAMERA_ORIGIN);

        public bool IsDummy 
        {
            get 
            {
                var className = ClassName;
                if (className.StartsWith("CAI_BaseNPC"))
                    return true;
                return false;
            }
        }
        public string ClassName
        {
            get
            {
                var client_networkable_vtable = Memory.Read<long>(BasePointer + 8 * 3);
                var get_client_class = Memory.Read<long>(client_networkable_vtable + 8 * 3);

                var disp = Memory.Read<int>(get_client_class + 3);
                var client_class_ptr = get_client_class + disp + 7;

                var client_class = Memory.Read<ClientClass>(client_class_ptr);

                return Memory.ReadString(client_class.pNetworkName, size: 32);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ClientClass 
        {
            public long pCreateFn;
            public long pCreateEventFn;
            public long pNetworkName;
            public long pRecvTable;
            public long pNext;
            public int ClassID;
            public int ClassSize;
        };
    }
}
