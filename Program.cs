
using System.Diagnostics;

namespace ApexSharp
{
    internal class Program
    {
        public static bool DebugMode = false;
        
        static Settings Settings => Settings.Instance;
        static readonly LocalPlayer LocalPlayer = new ();
        static readonly IEnumerable<EntityPlayer> EntityPlayers = GetEntities(DebugMode);

        static void Main()
        {
            Memory.Attach();
            
            var mouse = new Input(InputType.Mouse, Settings.INPUT_MOUSE_DEVICE_PATH);
            Task.Run(mouse.Capture);

            var keyboard = new Input(InputType.Keyboard, Settings.INPUT_KEYBOARD_DEVICE_PATH);
            Task.Run(keyboard.Capture);

            var sense = new Sense();
            var aimbot = new Aimbot();
            var recoil = new Recoil(Settings.RECOIL_STRENGTH);
            var assist = new Assist();
            
            Console.WriteLine(Settings);

            if (Settings.ASSIST_ENABLED)
                Task.Run(assist.Start);
            
            while(true) 
            { 
                try
                {
                    LocalPlayer.BasePointer = 0;
                    foreach (var entityPlayer in EntityPlayers)
                        entityPlayer.BasePointer = 0;

                    if (LocalPlayer.Invalid) continue;
                    if (LocalPlayer.IsDead) continue;

                    if (Settings.SENSE_ENABLED)
                        sense.Update(LocalPlayer, EntityPlayers);

                    if (LocalPlayer.IsKnocked) continue;
                 
                    if (Settings.AIMBOT_ENABLED)
                        aimbot.Update(LocalPlayer, EntityPlayers);

                    if (Settings.RECOIL_ENABLED)
                        recoil.Update(LocalPlayer);
                    
                    if (Settings.ASSIST_ENABLED)
                        assist.Update(LocalPlayer);

                    Thread.Sleep(5);
                }
                catch
                {
                    Console.WriteLine("메모리 접근에 문제가 발생했습니다. 5초간 대기합니다.");
                    Thread.Sleep(5000);
                    Console.WriteLine("다시 루프를 실행합니다.");
                }
            }
        }

        private static List<EntityPlayer> GetEntities(bool debugMode)
        {
            var enumerable = Enumerable.Range(1, debugMode ? 0xFFFF : 60);
            var entities = from index in enumerable select new EntityPlayer(index);
            if (DebugMode)
                return new (from entity in entities where entity.Invalid is not true && entity.IsDummy select entity);
            return new (entities);
        }
    }
}
