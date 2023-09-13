using System.Diagnostics;
using System.Numerics;

namespace ApexSharp
{
    internal class Recoil
    {
        private readonly float m_PitchStrength;
        private readonly float m_YawStrength;

        private Vector3 m_PreviousPunchWeaponAngles = Vector3.Zero;
        
        public Recoil(float pitchStrength, float yawStrength)
        {
            m_PitchStrength = pitchStrength;
            m_YawStrength = yawStrength;
        }

        public void Update(LocalPlayer player)
        {
            if (player.IsSemiAuto)
                return;
                
            var viewAngles = player.ViewAngles;
            var punchWeaponAngles = player.PunchWeaponAngles;
            
            var delta = m_PreviousPunchWeaponAngles - punchWeaponAngles;
            
            var pitch = delta.X * m_PitchStrength;
            var yaw = delta.Y * m_YawStrength;
            
            player.ViewAngles = viewAngles + new Vector3(pitch, yaw, 0.0f);
            m_PreviousPunchWeaponAngles = punchWeaponAngles;
        }
    }
}