using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ApexSharp
{
    internal class Sense
    {
        public void Update(LocalPlayer player, IEnumerable<EntityPlayer> entityPlayers, EntityPlayer Target = null)
        {
            var timeBase = player.TimeBase;
            var teamNum = player.TeamNum;

            const int HiglightSettingTypeSize = 0x28;
            var settings = Memory.Read<long>(Offset.REGION + Offset.HIGHLIGHT_SETTINGS);
            foreach (var context in s_CustomHighlightContexts.Values)
            {
                Memory.Write(settings + HiglightSettingTypeSize * context.Index + 4, context.FunctionBits);
                Memory.Write(settings + HiglightSettingTypeSize * context.Index + 8, context.Color);
            }

            foreach (var entityPlayer in entityPlayers)
            {
                if (entityPlayer.Invalid) continue;
                if (entityPlayer.IsDead) continue;
                if (entityPlayer.TeamNum == teamNum) continue;

                HighlightContext context;
                if (entityPlayer == Target)
                {
                    context = s_CustomHighlightContexts[Color.Yellow];
                }
                else if (entityPlayer.IsVisible(timeBase))
                    context = s_CustomHighlightContexts[Color.Green];
                else
                {
                    int shield = entityPlayer.ShieldHealth;
                    if (shield >= 120) context = s_CustomHighlightContexts[Color.Red];
                    else if (shield >= 100) context = s_CustomHighlightContexts[Color.Purple];
                    else if (shield >= 75) context = s_CustomHighlightContexts[Color.Blue];
                    else context = s_CustomHighlightContexts[Color.White];
                }
                entityPlayer.Glow = 1;
                entityPlayer.GlowThroughWall = 2;
                
                const int CurrentContextId = 1;
                Memory.Write(entityPlayer.BasePointer + Offset.HIGHLIGHT_SERVER_ACTIVE_STATES + CurrentContextId, context.Index);
            }

        }
        public struct HighlightContext
        {
            private static int s_CustomIndex = 65;
            private static readonly FunctionBit s_CustomFunctionBits = new(0, 125, 64, 64);

            public int Index;
            public Vector3 Color;
            public FunctionBit FunctionBits;

            public HighlightContext(Vector3 color)
            {
                Index = s_CustomIndex++;
                FunctionBits = s_CustomFunctionBits;
                Color = color;
            }
            
            [StructLayout(LayoutKind.Sequential)]
            public struct FunctionBit
            {
                public byte GeneralGlowMode;
                public byte BorderGlowMode;
                public byte BorderSize;
                public byte TransparentLevel;

                public FunctionBit(byte generalGlowMode, byte borderGlowMode, byte borderSize, byte transparentLevel)
                {
                    GeneralGlowMode = generalGlowMode; 
                    BorderGlowMode = borderGlowMode; 
                    BorderSize = borderSize; 
                    TransparentLevel = transparentLevel;
                }
            }
        }
        private static readonly Dictionary<Color, HighlightContext> s_CustomHighlightContexts = new()
        {
            [Color.Red]     = new (new (1.0f, 0.0f, 0.0f)),
            [Color.Purple]  = new (new (0.5f, 0.0f, 1.0f)),
            [Color.Blue]    = new (new (0.0f, 0.0f, 1.0f)),
            [Color.White]   = new (new (1.0f, 1.0f, 1.0f)),
            [Color.Green]   = new (new (0.0f, 1.0f, 0.0f)),
            [Color.Yellow]     = new (new (1.0f, 1.0f, 0.0f)),
        };

    }
}