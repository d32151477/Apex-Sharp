using System.Numerics;

namespace ApexSharp
{

    internal class Sense
    {
        public void Update(LocalPlayer player, IEnumerable<EntityPlayer> entityPlayers)
        {
            var playerTeamNum = player.TeamNum;
            foreach (var entityPlayer in entityPlayers)
            {
                if (entityPlayer.Invalid) continue;
                if (entityPlayer.TeamNum == playerTeamNum) continue;

                entityPlayer.Glow = 1;
                entityPlayer.GlowThroughWall = 2;

                if (entityPlayer.Visible)
                    entityPlayer.GlowColor = new Vector3(0.0f, 1.0f, 0.0f);
                else
                {
                    int shield = entityPlayer.ShieldHealth;
                    if (shield >= 120) entityPlayer.GlowColor = new Vector3(1.0f, 0.0f, 0.0f);
                    else if (shield >= 100) entityPlayer.GlowColor = new Vector3(0.5f, 0.0f, 1.0f);
                    else if (shield >= 75) entityPlayer.GlowColor = new Vector3(0.0f, 0.5f, 1.0f);
                    else entityPlayer.GlowColor = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }
}