using System.Numerics;
using ApexSharp;

internal class Recoil
{
    private readonly float m_Strength;
    private Vector3 m_PreviousPunchWeaponAngles = Vector3.Zero;
    
    public Recoil(float strength)
    {
        m_Strength = strength;
    }

    public void Update(LocalPlayer player)
    {
        var viewAngles = player.ViewAngles;
        var punchWeaponAngles = player.PunchWeaponAngles;
        
        var angles = viewAngles + (m_PreviousPunchWeaponAngles - punchWeaponAngles) * m_Strength;
        
        player.ViewAngles = new Vector3(angles.X, angles.Y, 0.0f);
        m_PreviousPunchWeaponAngles = punchWeaponAngles;
    }

}