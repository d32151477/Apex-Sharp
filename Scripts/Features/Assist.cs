
using System.Diagnostics;

namespace ApexSharp
{
    internal class Assist
    {
        public static Settings Settings => Settings.Instance;

        private int m_LastFrame;
        private int m_SuperglideTime;
        private bool m_SuperglideBegin;

        public void Start()
        {
            while (true)
            {
                // 오토 버니합
                if (Input.IsDown(Settings.ASSIST_AUTO_BHOP_KEY))
                {
                    // 마우스 휠 위로 버튼 입니다.
                    Process.Start("xdotool", "click 4");
                    Thread.Sleep(15);
                }
            }
        }
        public void Update(LocalPlayer player)
        {
            // 오토 슈퍼 글라이드
            int currentFrame = Memory.Read<int>(Offset.REGION + Offset.CURRENT_FRAME);
            if (currentFrame == m_LastFrame)
                return;
                
            var traversalProgress = player.TraversalProgress;
            var cantSuperglide = traversalProgress <= 0.85f || traversalProgress >= 1.0f;
            if (cantSuperglide)
                return;
            
            if (m_SuperglideBegin)
            {
                if (m_SuperglideTime == 4)
                    Memory.Write(Offset.REGION + Offset.FORCE_JUMP, 5);
                else if (m_SuperglideTime == 5)
                    Memory.Write(Offset.REGION + Offset.FORCE_TOGGLE_DUCK, 6);
                else if (m_SuperglideTime >= 13)
                {
                    Memory.Write(Offset.REGION + Offset.FORCE_JUMP, 4);
                    Memory.Write(Offset.REGION + Offset.FORCE_TOGGLE_DUCK, 4);
                    m_SuperglideBegin = false;
                }
                m_SuperglideTime++;
            }
            else if (Input.IsDown(Settings.ASSIST_AUTO_SUPERGLID_KEY))
            {
                m_SuperglideBegin = true;
                m_SuperglideTime = 0;
            }
            m_LastFrame = currentFrame;
        }
    }
}