
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApexSharp
{
    public class Settings
    {
        private const string Path = "./Settings.ini";
        private static readonly JsonSerializerOptions s_SerializerOptions = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
        public static readonly Settings Instance;

        public string INPUT_MOUSE_DEVICE_PATH = "/dev/input/event2";
        public string INPUT_KEYBOARD_DEVICE_PATH = "/dev/input/event4";
        
        public bool SENSE_ENABLED = true;
        public bool RECOIL_ENABLED = true;
        public bool AIMBOT_ENABLED = true;
        public bool ASSIST_ENABLED = true;

        public float RECOIL_PITCH_STRENGTH = 0.5f;
        public float RECOIL_YAW_STRENGTH = 0.5f;

        public int AIMBOT_BONE_ID = 8;
        public float AIMBOT_MAX_FOV = 15.0f;
        public float AIMBOT_MAX_DISTANCE = 30.0f;
        public float AIMBOT_SMOOTHNESS = 150.0f;

        public InputKey ASSIST_AUTO_BHOP_KEY = new (InputType.Keyboard, 56);
        public InputKey ASSIST_AUTO_SUPERGLID_KEY = new (InputType.Mouse, 275);

        static Settings()
        {
            if (File.Exists(Path))
            {
                string json = File.ReadAllText(Path);
                Instance = JsonSerializer.Deserialize<Settings>(json, s_SerializerOptions);
            }
            else
            {
                Instance = new Settings();
                string json = JsonSerializer.Serialize(Instance, s_SerializerOptions);
                File.WriteAllText(Path, json);
            }
        }
        public override string ToString()
        {
            var fields = typeof(Settings).GetFields(BindingFlags.Instance | BindingFlags.Public);

            using var writer = new StringWriter();
            writer.WriteLine("----------------------------------------------------------");
            foreach (var field in fields)
            {
                var key = field.Name;
                var value = field.GetValue(this);
                writer.WriteLine($"{key}: {value}");
            }
            writer.WriteLine("----------------------------------------------------------");
            return writer.ToString();
        }
    }
}