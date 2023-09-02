using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ApexSharp
{
    internal class Offset
    {
        private static readonly Dictionary<string, long> s_Dump = new();

        static Offset()
        {
            LoadFile();
            LoadAddress();
        }
        private static void LoadFile()
        {
            var directory = string.Empty;
            foreach (var line in File.ReadAllLines("./Dump.bin"))
            {
                int equals = line.IndexOf('=');
                if (equals > -1)
                {
                    var key = line[..equals]; 
                    var value = line[(equals + 1)..];

                    // 0x로 시작하는 16진수 문자열인지 확인합니다.
                    var match = Regex.Match(value, @"^0x([0-9A-Fa-f]+)$");
                    if (match.Success)
                    {
                        var hex = match.Groups[1].Value;
                        long dec = Convert.ToInt64(hex, 16);
                        s_Dump[$@"{directory}\{key}"] = dec;
                    }
                }
                else
                {
                    var match = Regex.Match(line, @"\[(.*?)\]");
                    if (match.Success)
                        directory = match.Groups[1].Value;
                }
            }
        } 
        private static void LoadAddress()
        {
            // [Miscellaneous]
            GLOBALVARS = Get(@"Miscellaneous\GlobalVars");
            LOCAL_PLAYER = Get(@"Miscellaneous\LocalPlayer") + 0x8;
            
            CL_ENTITYLIST = Get(@"Miscellaneous\cl_entitylist");
            CAMERA_ORIGIN = Get(@"Miscellaneous\CPlayer!camera_origin");
            LAST_VISIBLE_TIME = Get(@"Miscellaneous\CPlayer!lastVisibleTime");
            STUDIO_HDR = Get(@"Miscellaneous\CBaseAnimating!m_pStudioHdr");
            
            CURRENT_FRAME = GLOBALVARS + 0x8;

            // [Buttons]
            IN_ATTACK = Get(@"Buttons\in_attack");
            IN_JUMP = Get(@"Buttons\in_jump");
            IN_DUCK = Get(@"Buttons\in_duck");
            IN_TOGGLE_DUCK = Get(@"Buttons\in_toggle_duck");

            FORCE_ATTACK = IN_ATTACK + 0x8;
            FORCE_JUMP = IN_JUMP + 0x8;
            FORCE_DUCK = IN_DUCK + 0x8;
            FORCE_TOGGLE_DUCK = IN_TOGGLE_DUCK + 0x8;
            
            // [DataMap.CBaseViewModel]
            LOCAL_ORIGIN = Get(@"DataMap.CBaseViewModel\m_localOrigin");

            // [RecvTable.DT_BaseEntity]
            NAME = Get(@"RecvTable.DT_BaseEntity\m_iName");
            TEAM_NUM = Get(@"RecvTable.DT_BaseEntity\m_iTeamNum");
            SHIELD_HEALTH = Get(@"RecvTable.DT_BaseEntity\m_shieldHealth");
            

            // [RecvTable.DT_Player]
            LIFE_STATE = Get(@"RecvTable.DT_Player\m_lifeState");
            BLEEDOUT_STATE = Get(@"RecvTable.DT_Player\m_bleedoutState");
            ZOOMING = Get(@"RecvTable.DT_Player\m_bZooming");

            // [DataMap.C_Player]
            VEC_ABS_VELOCITY = Get(@"DataMap.C_Player\m_vecAbsVelocity");
            TRAVERSAL_PROGRESS = Get(@"DataMap.C_Player\m_traversalProgress");
            TRAVERSAL_START_TIME = Get(@"DataMap.C_Player\m_traversalStartTime");
            AMMO_POOL_CAPACITY = Get(@"DataMap.C_Player\m_ammoPoolCapacity");
            VEC_PUNCH_WEAPON_ANGLE = Get(@"DataMap.C_Player\m_currentFrameLocalPlayer.m_vecPunchWeapon_Angle");

            VIEW_ANGLES = AMMO_POOL_CAPACITY - 0x14;
            BREATH_ANGLES = VIEW_ANGLES - 0x10;

            // [DataMap.CWeaponX]
            PLAYER_DATA = Get(@"DataMap.CWeaponX\m_playerData");
            ZOOM_FOV = PLAYER_DATA + CUR_ZOOM_FOV; 

            // [RecvTable.DT_BaseAnimating]
            FORCE_BONE = Get(@"RecvTable.DT_BaseAnimating\m_nForceBone");

            BONES = FORCE_BONE + 0x48;

            // [RecvTable.DT_WeaponPlayerData]
            CUR_ZOOM_FOV = Get(@"RecvTable.DT_WeaponPlayerData\m_curZoomFOV");

            // [RecvTable.DT_BaseCombatCharacter]
            LATEST_PRIMARY_WEAPONS = Get(@"RecvTable.DT_BaseCombatCharacter\m_latestPrimaryWeapons");

            // [RecvTable.DT_HighlightSettings]
            HIGHLIGHT_SERVER_CONTEXT_ID = Get(@"RecvTable.DT_HighlightSettings\m_highlightServerContextID");
            HIGHLIGHT_FUNCTION_BITS = Get(@"RecvTable.DT_HighlightSettings\m_highlightFunctionBits");
            HIGHLIGHT_PARAMS = Get(@"RecvTable.DT_HighlightSettings\m_highlightParams");

            GLOW_ENABLE = HIGHLIGHT_SERVER_CONTEXT_ID + 0x8;
            GLOW_THROUGH_WALL = HIGHLIGHT_SERVER_CONTEXT_ID + 0x10;
            GLOW_COLOR = HIGHLIGHT_PARAMS + 0x18;
            GLOW_MODE = HIGHLIGHT_FUNCTION_BITS + 0x4;

            // (https://www.unknowncheats.me/forum/apex-legends/319804-apex-legends-reversal-structs-offsets-628.html)
            
            // [WeaponSettingsMeta]
            WEAPON_SETTINGS_META_BASE = Get(@"WeaponSettingsMeta\base");

            // [WeaponSettings]
            PROJECTILE_LAUNCH_SPEED = WEAPON_SETTINGS_META_BASE + Get(@"WeaponSettings\projectile_launch_speed");
            PROJECTILE_GRAVITY_SCALE = WEAPON_SETTINGS_META_BASE + Get(@"WeaponSettings\projectile_gravity_scale");

            IS_SEMI_AUTO = WEAPON_SETTINGS_META_BASE + Get(@"WeaponSettings\is_semi_auto");
        }

        private static long Get(string key)
        {
            if (s_Dump.TryGetValue(key, out long value))
                return value;
            throw new Exception($"해당 키({key})의 오프셋을 찾을 수 없습니다.");
        }

        
        public const long REGION = 0x140000000; // cat /proc/PID/maps 로 모듈 베이스 주소를 구하세요.
        public static long GLOBALVARS; // [Miscellaneous]
        public static long LOCAL_PLAYER;
        public static long CL_ENTITYLIST;
        public static long CAMERA_ORIGIN;
        public static long LAST_VISIBLE_TIME;
        public static long STUDIO_HDR;
        public static long CURRENT_FRAME;
        public static long IN_ATTACK; // [Buttons]
        public static long IN_JUMP;
        public static long IN_DUCK;
        public static long IN_TOGGLE_DUCK;
        public static long FORCE_ATTACK;
        public static long FORCE_JUMP;
        public static long FORCE_DUCK;
        public static long FORCE_TOGGLE_DUCK;
        public static long LOCAL_ORIGIN; // [DataMap.CBaseViewModel]
        public static long NAME; // [RecvTable.DT_BaseEntity]
        public static long TEAM_NUM;
        public static long SHIELD_HEALTH;
        public static long LIFE_STATE; // [RecvTable.DT_Player]
        public static long BLEEDOUT_STATE;
        public static long ZOOMING;
        public static long TRAVERSAL_PROGRESS; // [DataMap.C_Player]
        public static long TRAVERSAL_START_TIME; 
        public static long AMMO_POOL_CAPACITY;
        public static long VEC_ABS_VELOCITY;
        public static long VEC_PUNCH_WEAPON_ANGLE;
        public static long VIEW_ANGLES;
        public static long BREATH_ANGLES;
        public static long PLAYER_DATA; // [DataMap.CWeaponX]
        public static long ZOOM_FOV; 
        public static long FORCE_BONE; // [RecvTable.DT_BaseAnimating]
        public static long BONES;
        public static long CUR_ZOOM_FOV; // [RecvTable.DT_WeaponPlayerData]
        public static long LATEST_PRIMARY_WEAPONS; // [RecvTable.DT_BaseCombatCharacter]
        public static long HIGHLIGHT_SERVER_CONTEXT_ID;
        public static long HIGHLIGHT_FUNCTION_BITS;
        public static long HIGHLIGHT_PARAMS;
        public static long GLOW_ENABLE;
        public static long GLOW_THROUGH_WALL;
        public static long GLOW_COLOR;
        public static long GLOW_MODE;
        public static long WEAPON_SETTINGS_META_BASE; // [WeaponSettingsMeta]
        public static long PROJECTILE_LAUNCH_SPEED; // [WeaponSettings]
        public static long PROJECTILE_GRAVITY_SCALE;
        public static long IS_SEMI_AUTO;
    }
}
