using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ApexSharp
{
    internal class Memory
    {
        [DllImport("libc")]
        private static extern unsafe int process_vm_readv(long pid, IOVEC* local_iov, ulong liovcnt, IOVEC* remote_iov, ulong riovcnt, ulong flags);
        
        [DllImport("libc", SetLastError = true)]
        private static extern unsafe int process_vm_writev(long pid, IOVEC* local_iov, ulong liovcnt, IOVEC* remote_iov, ulong riovcnt, IntPtr flags);

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct IOVEC
        {
            public long iov_base;
            public int iov_len;

            public IOVEC(long iov_base, int iov_len)
            {
                this.iov_base = iov_base;
                this.iov_len = iov_len;
            }
        }
        
        public static long PID = 0;
        public static void Attach() 
        {
            var info = new ProcessStartInfo { 
                FileName = "pidof", 
                Arguments = "-s r5apex.exe", 
                UseShellExecute = false, 
                RedirectStandardOutput = true,
            };
            
            using var process = Process.Start(info) ?? throw new Exception("pidof를 통해 프로세스 ID를 가져올 수 없습니다.");

            var output = process.StandardOutput.ReadLine();
            if (!long.TryParse(output, out PID))
                throw new Exception("에이펙스 프로세스를 찾을 수 없습니다.");

            Console.WriteLine($"에이펙스 프로세스 ID ({PID})를 확인했습니다.");
        }

        public static unsafe T Read<T>(long address)
        {
            var size = Unsafe.SizeOf<T>();
            var pointer = stackalloc byte[size];

            var local_iov = new IOVEC((long)pointer, size);
            var remote_iov = new IOVEC(address, size);

            if (process_vm_readv(PID, &local_iov, 1, &remote_iov, 1, 0) < 0)
                throw new AccessViolationException("잘못된 메모리 접근입니다.");

            return *(T*)pointer;
        }
        public static unsafe string ReadString(long address, int size)
        {
            var pointer = stackalloc byte[size];
            var local_iov = new IOVEC((long)pointer, size);
            var remote_iov = new IOVEC(address, size);

            if (process_vm_readv(PID, &local_iov, 1, &remote_iov, 1, 0) < 0)
                throw new AccessViolationException("잘못된 메모리 접근입니다.");
                
            var bytes = new byte[size];
            Marshal.Copy((IntPtr)pointer, bytes, 0, size);
            
            return Encoding.UTF8.GetString(bytes);;
        }

        public static unsafe void Write<T>(long address, T value)
        {
            int size = Unsafe.SizeOf<T>();
            var local_iov = new IOVEC((long)&value, size);
            var remote_iov = new IOVEC(address, size);
            
            if (process_vm_writev(PID, &local_iov, 1, &remote_iov, 1, IntPtr.Zero) < 0)
                throw new AccessViolationException("잘못된 메모리 접근입니다.");
        }

    }
}