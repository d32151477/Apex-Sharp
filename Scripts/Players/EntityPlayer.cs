
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using ApexSharp;

namespace ApexSharp
{
    internal sealed class EntityPlayer : Entity
    {
        public int Index;

        private long m_BasePointer;
        private float m_LastVisibleTime;
        
        public EntityPlayer(int index) => Index = index;

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

        public float LastVisibleTime => Memory.Read<float>(BasePointer + Offset.LAST_VISIBLE_TIME);
        
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
        public Vector3 GlowColor
        {
            get => Memory.Read<Vector3>(BasePointer + Offset.GLOW_COLOR);
            set => Memory.Write(BasePointer + Offset.GLOW_COLOR, value);
        }

        public bool Invisible => !Visible;
        public bool Visible
        {
            get
            {
                var lastVisibleTime = LastVisibleTime;
                if (lastVisibleTime > m_LastVisibleTime)
                {
                    m_LastVisibleTime = lastVisibleTime;
                    return true;
                }
                m_LastVisibleTime = lastVisibleTime;
                return false;
            }
        }

        public Vector3 GetBonePositionByHitbox(int index)
        {
            var localOrigin = LocalOrigin;
            
            long model = Memory.Read<long>(BasePointer + Offset.STUDIO_HDR);
            long studioHDR = Memory.Read<long>(model + 0x8);

            long hitboxCache = Memory.Read<long>(studioHDR + 0x34);
            long hitboxsArray = studioHDR + ((hitboxCache & 0xFFFE) << (int)(4 * (hitboxCache & 1)));
            
            long indexCache = Memory.Read<long>(hitboxsArray + 0x4);
	        int hitboxIndex = ((int)indexCache & 0xFFFE) << (4 * ((int)indexCache & 1));

	        var bone = Memory.Read<byte>(hitboxsArray + hitboxIndex + (index * 0x20));
            if(bone < 0 || bone > 255)
                throw new Exception($"해당 히트박스 ID({index})를 찾을 수 없습니다.");

            var bone_array = Memory.Read<long>(BasePointer + Offset.BONES);
            var matrix = Memory.Read<Matrix3x4>(bone_array + bone * Matrix3x4.Size);
        
            var BoneOrigin = new Vector3(matrix.M03, matrix.M13, matrix.M23); 
            return localOrigin + BoneOrigin;
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