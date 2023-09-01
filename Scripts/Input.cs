using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace ApexSharp
{
    public class Input
    {
        public static readonly Dictionary<InputType, Input> Instances = new();

        [DllImport("libc", SetLastError = true)]
        private static extern int open(string filename, int flags);

        [DllImport("libc", SetLastError = true)]
        private static extern int ioctl(int fd, int request, int data);

        [DllImport("libc", SetLastError = true)]
        private static extern int read(int fd, out InputEvent ev, int size);

        private const int O_RDONLY = 0x0000;
        private const int O_NONBLOCK = 0x0400;

        private int m_Handle = -1;
        private InputType m_Type;
        private string m_Path;
        private readonly ConcurrentDictionary<ushort, InputEvent> m_States = new();
        
        public Input(InputType type, string path)
        {
            m_Type = type;
            m_Path = path;
            m_Handle = open(path, O_RDONLY | O_NONBLOCK);
            if (m_Handle == -1)
                throw new Exception($"해당 입력 디바이스({m_Path})를 불러올 수 없습니다.");
            
            Instances.Add(type, this);
        }
        public void Capture()
        {
            Console.WriteLine($"{m_Type} 입력 디바이스({m_Path}) 캡처를 시작합니다.");

            var size = Marshal.SizeOf<InputEvent>();
            while (true)
            {
                if (read(m_Handle, out var @event, size) == -1)
                    continue;
                m_States[@event.Code] = @event;
                //Console.WriteLine($"event: type {@event.Type}, code {@event.Code}, value {@event.Value}");
            }
        }
        public bool IsDown(ushort code)
        {
            if (m_States.TryGetValue(code, out var @event))
            {
                if (@event.Value > 0)
                    return true;
            }
            return false;

        }
        public static bool IsDown(InputKey key) => Instances[key.Type].IsDown(key.Code);
        public static bool IsDown(InputType type, ushort code) => Instances[type].IsDown(code);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct InputEvent
    {
        public long TimeSeconds;
        public long TimeMicroseconds;
        public ushort Type;
        public ushort Code;
        public int Value;
    }
    public enum InputType { Keyboard, Mouse }
    public struct InputKey
    {
        public InputType Type;
        public ushort Code;

        public InputKey(InputType type, ushort code)
        {
            Type = type;
            Code = code;
        }
        public override string ToString()
        {
            return $"{Code} ({Type})";
        }
    }
}