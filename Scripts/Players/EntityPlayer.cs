using System.Numerics;
using System.Runtime.InteropServices;

namespace ApexSharp
{
    internal sealed class EntityPlayer : Entity
    {
        private long m_BasePointer = 0;
        public EntityPlayer(int index) => Index = index;

        public int Index { get; set; }
        public override long BasePointer 
        {
            get
            {
                if (m_BasePointer == 0)
                    m_BasePointer = Memory.Read<long>(Offset.REGION + Offset.CL_ENTITYLIST + (Index << 5));
                return m_BasePointer;
            }
            set => m_BasePointer = value;
        }

        public int Glow
        {
            get => Memory.Read<int>(BasePointer + Offset.GLOW_ENABLE);
            set => Memory.Write(BasePointer + Offset.GLOW_ENABLE, value);
        }
        public int GlowThroughWall
        {
            get => Memory.Read<int>(BasePointer + Offset.GLOW_THROUGH_WALL);
            set => Memory.Write(BasePointer + Offset.GLOW_THROUGH_WALL, value);
        }

        public float LastVisibleTime => Memory.Read<float>(BasePointer + Offset.LAST_VISIBLE_TIME);
        
        public bool IsVisible(float timeBase)
        {
            // 벽 뒤로 사라졌을 경우에 0.25초 만큼 지속합니다.
            var lastVisibleTime = LastVisibleTime;
            if (lastVisibleTime < 0.0f)
                return false;
                
            const float Duration = 0.25f; 
            if (lastVisibleTime + Duration > timeBase)
                return true;
            return false;
        }
        public Vector3 GetBonePositionByIndex(int boneId)
        {
            var boneArr = Memory.Read<long>(BasePointer + Offset.BONES);
            
            var x = Memory.Read<float>(boneArr + 0xCC + (boneId * 0x30));
            var y = Memory.Read<float>(boneArr + 0xDC + (boneId * 0x30));
            var z = Memory.Read<float>(boneArr + 0xEC + (boneId * 0x30));

            var boneOrigin = new Vector3(x, y, z);
            return LocalOrigin + boneOrigin;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x4
    {
        public const int Size = sizeof(float) * 4 * 3;

        public float M00;
        public float M01;
        public float M02;
        public float M03;

        public float M10;
        public float M11;
        public float M12;
        public float M13;

        public float M20;
        public float M21;
        public float M22;
        public float M23;

        Matrix3x4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            M00 = m00;	M01 = m01; M02 = m02; M03 = m03;
            M10 = m10;	M11 = m11; M12 = m12; M13 = m13;
            M20 = m20;	M21 = m21; M22 = m22; M23 = m23;
        } 
    };
}
