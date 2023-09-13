
using System.Numerics;

namespace ApexSharp
{
    public class Region
    {
        public static bool Invalid
        {
            get
            {
                const string kIngamePrefixName = "mp_rr_";
                var levelName = Memory.ReadString(Offset.REGION + Offset.LEVEL_NAME, kIngamePrefixName.Length);

                bool ingame = levelName.StartsWith(kIngamePrefixName);
                if (ingame)
                    return false;
                return true;
            }
        }
        public static Matrix4x4 ViewMatrix
        {
            get
            {
                var pViewRenderer = Memory.Read<long>(Offset.REGION + Offset.VIEW_RENDER);
                var pViewMatrix = Memory.Read<long>(pViewRenderer + Offset.VIEW_MATRIX);
                var matrix = Memory.Read<Matrix4x4>(pViewMatrix);

                return matrix;
            }
        }
    }
}