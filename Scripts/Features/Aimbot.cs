using System.Numerics;

namespace ApexSharp
{
    internal class Aimbot
    {
        public EntityPlayer Target { get; set; }
        private static Settings Settings => Settings.Instance;

        public void Update(LocalPlayer player, IEnumerable<EntityPlayer> entityPlayers)
        {
            var target = Target = default;
            var targetFov = float.MaxValue;

            var cameraOrigin = player.CameraOrigin;
            var viewAngles = player.ViewAngles;
            var teamNum = player.TeamNum;
            var timeBase = player.TimeBase;

            foreach (var entityPlayer in entityPlayers)
            {
                if (entityPlayer.Invalid) continue;
                if (entityPlayer.IsKnocked) continue;
                if (entityPlayer.IsVisible(timeBase) == false) continue;
                if (entityPlayer.TeamNum == teamNum) continue;

                float fov = GetFov(cameraOrigin, viewAngles, entityPlayer.LocalOrigin);
                if (fov >= targetFov)
                    continue;

                target = entityPlayer;
                targetFov = fov;
            }
            if (target == default)
                return;

            var distance = Vector3.Distance(player.LocalOrigin, target.LocalOrigin);
            var meters = distance * 0.02535f;
            if (meters > Settings.AIMBOT_MAX_DISTANCE)
                return;

            var angles = GetPredictedViewAngles(player, target, Settings.AIMBOT_MAX_FOV);
            if (angles == Vector3.Zero)
                return;

            Target = target;
            player.ViewAngles = angles;
        }
        public Vector3 GetPredictedViewAngles(LocalPlayer player, EntityPlayer target, float maxFov)
        {
            var cameraOrigin = player.CameraOrigin;
            var targetOrigin = target.GetBonePositionByIndex(Settings.AIMBOT_BONE_ID);
            
            var weapon = new Weapon(player);

            if (weapon.ZoomFov != 0.0f && weapon.ZoomFov != 1.0f)
                maxFov *= weapon.ZoomFov / 90.0f;
            
            // 탄도 각도를 계산합니다.
            var angles = Vector3.Zero;
            if (weapon.BulletSpeed > 1.0f)
            {
                var predictor = new Predictor(cameraOrigin, targetOrigin, target.VecAbsVelocity, weapon.BulletSpeed, weapon.BulletGravity);
                if (predictor.Simulate(out var angles2d))
                    angles = new Vector3(angles2d.X, angles2d.Y, 0.0f);
            }

            // 맨 손이거나, 예측할 수 없을 때 직사 각도를 구합니다.
            if (angles == Vector3.Zero)
                angles = GetAngles(cameraOrigin, targetOrigin);

            var viewAngles = player.ViewAngles;
            var breathAngles = player.BreathAngles;

            // 화면 흔들림 제거
            angles -= breathAngles - viewAngles;
            angles = NormalizeAngles(angles);

            var fov = GetFov(breathAngles, angles);
            if (fov > maxFov)
                return Vector3.Zero;

            var delta = angles - viewAngles;
            delta = NormalizeAngles(delta);

            var smoothedAngles = viewAngles + delta / Settings.AIMBOT_SMOOTHNESS;

            return smoothedAngles;
        }
        private static float GetFov(Vector3 viewOrigin, Vector3 viewAngles, Vector3 targetOrigin)
        {
            return GetFov(viewAngles, GetAngles(viewOrigin, targetOrigin));
        }
        private static float GetFov(Vector3 viewAngles, Vector3 targetAngles)
        {
            var angles = NormalizeAngles(targetAngles - viewAngles);
            return MathF.Sqrt(MathF.Pow(angles.X, 2.0f) + MathF.Pow(angles.Y, 2.0f));
        }
        private static Vector3 GetAngles(Vector3 from, Vector3 to)
        {
            var delta = from - to;

            var hyp = MathF.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

            var x = MathF.Atan(delta.Z / hyp) * (180.0f / MathF.PI);
            var y = MathF.Atan(delta.Y / delta.X) * (180.0f / MathF.PI);
            var z = 0;
            
            if (delta.X >= 0.0) 
                y += 180.0f;

            return new Vector3(x, y, z);
        }
        private static Vector3 NormalizeAngles(Vector3 angles)
        {
            float x = angles.X;
            float y = angles.Y;
            float z = angles.Z;
            
            while (x > 89.0f)
                x -= 180.0f;

            while (x < -89.0f)
                x += 180.0f;

            while (y > 180.0f)
                y -= 360.0f;

            while (y < -180.0f)
                y += 360.0f;

            return new Vector3(x, y, z);
        }
    }
    
    internal struct Weapon
    {
        public float BulletSpeed;
        public float BulletGravity;
        private float BulletGravityScale;
        public float ZoomFov;

        public Weapon(LocalPlayer player)
        {
            var entitylist = Offset.REGION + Offset.CL_ENTITYLIST;
            var handle = Memory.Read<long>(player.BasePointer + Offset.LATEST_PRIMARY_WEAPONS);
            handle &= 0xffff;

            var entity = Memory.Read<long>(entitylist + (handle << 5));
            BulletSpeed = Memory.Read<float>(entity + Offset.PROJECTILE_LAUNCH_SPEED);
            BulletGravityScale = Memory.Read<float>(entity + Offset.PROJECTILE_GRAVITY_SCALE);
            ZoomFov = Memory.Read<float>(entity + Offset.ZOOM_FOV);

            BulletGravity = 750.0f * BulletGravityScale;
        }
    }

    public struct Predictor
    {
        public Vector3 StartOrigin;
        public Vector3 TargetOrigin;
        public Vector3 TargetVelocity;
        public float BulletSpeed;
        public float BulletGravity;

        public Predictor(Vector3 startOrigin, Vector3 targetOrigin, Vector3 targetVelocity, float bulletSpeed, float bulletGravity)
        {
            StartOrigin = startOrigin;
            TargetOrigin = targetOrigin;
            TargetVelocity = targetVelocity;
            BulletSpeed = bulletSpeed;
            BulletGravity = bulletGravity;
        }

        // 상대방의 위치를 계산합니다.
        public Vector3 GetExtrapolatedTargetOrigin(float time)
        {
            return TargetOrigin + (TargetVelocity * time);
        }

        // 해당 방향으로의 총알의 발사 각도를 계산합니다.
        public bool GetPitchFromDirection(Vector2 direction, out float pitch)
        {
            pitch = default;
            float v = BulletSpeed;
            float g = BulletGravity;
            float dx = direction.X;
            float dy = direction.Y;

            float r = v * v * v * v - g * (g * dx * dx + 2.0f * dy * v * v);
            if (r >= 0.0f)
            { 
                pitch = MathF.Atan((v * v - MathF.Sqrt(r)) / (g * dx)); 
                return true;
            }
            return false;
        }
        public bool CalculateTrajectory(Vector3 to, out float travelTime, out Vector2 angles)
        {
            angles = default;
            travelTime = default;

            // 총알의 이동 방향과 거리
            var dir3 = to - StartOrigin;
            var dir2 = new Vector2(x: MathF.Sqrt(dir3.X * dir3.X + dir3.Y * dir3.Y), dir3.Z);

            if (GetPitchFromDirection(dir2, out var pitch))
            {
                // 해당 지점까지 총알의 이동 시간을 계산합니다.
                travelTime = dir2.X / (MathF.Cos(pitch) * BulletSpeed);
                
                // x는 발사 각도, y는 목표 위치를 향하는 각도
                angles = new Vector2(x: pitch, y: MathF.Atan2(dir3.Y, dir3.X));

                return true;
            }
            return false;
        }

        public bool Simulate(out Vector2 angles)
        {
            angles = default;
            float MAX_TIME = 1.0f;
            float TIME_STEP = 1.0f / 256.0f;

            for (float time = 0.0f; time <= MAX_TIME; time += TIME_STEP)
            {
                var targetOrigin = GetExtrapolatedTargetOrigin(time);
                if (!CalculateTrajectory(targetOrigin, out var travelTime, out angles))
                    return false;
                    
                if (travelTime < time)
                {
                    const float Rad2Deg = 360 / (MathF.PI * 2);
                    angles = new Vector2(-angles.X * Rad2Deg, angles.Y * Rad2Deg);
                    return true;
                }
            }
            return false;
        }
    };
}
