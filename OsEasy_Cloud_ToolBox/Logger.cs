using System;
using System.IO;
using System.Text;
using System.Threading;

namespace OsEasy_Cloud_ToolBox
{
    // 简单的文件日志工具，输出到 %temp%\OsEasy_Cloud_ToolBox.log
    internal static class Logger
    {
        private static readonly object log_lock = new object();
        private static readonly string log_file_path = Path.Combine(Path.GetTempPath(), "OsEasy_Cloud_ToolBox.log");

        // 单个日志文件的最大体积，超过则在会话开始时清空
        private const long max_log_file_size = 2 * 1024 * 1024;

        public static string LogPath
        {
            get { return log_file_path; }
        }

        public static void StartSession()
        {
            try
            {
                if (File.Exists(log_file_path) && new FileInfo(log_file_path).Length > max_log_file_size)
                {
                    File.Delete(log_file_path);
                }
            }
            catch
            {
                // 忽略清理失败
            }

            Info("========== 会话开始 ==========");
        }

        public static void Info(string message)
        {
            write("INFO", message);
        }

        public static void Warn(string message)
        {
            write("WARN", message);
        }

        public static void Error(string message)
        {
            write("ERROR", message);
        }

        public static void Error(string message, Exception ex)
        {
            write("ERROR", message + Environment.NewLine + (ex == null ? "" : ex.ToString()));
        }

        // 记录一次外部程序调用：命令行、退出码与目标程序输出
        public static void LogProcessResult(string file_name, string arguments, int exit_code, string output)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("外部程序调用: ").Append(file_name);
            if (!string.IsNullOrEmpty(arguments))
            {
                builder.Append(" ").Append(arguments);
            }
            builder.Append(" | 退出码=").Append(exit_code);

            if (!string.IsNullOrEmpty(output))
            {
                builder.Append(Environment.NewLine).Append("输出:").Append(Environment.NewLine).Append(output);
            }

            write(exit_code == 0 ? "INFO" : "WARN", builder.ToString());
        }

        private static void write(string level, string message)
        {
            try
            {
                string line = string.Format(
                    "[{0:yyyy-MM-dd HH:mm:ss.fff}] [T{1}] [{2}] {3}{4}",
                    DateTime.Now,
                    Thread.CurrentThread.ManagedThreadId,
                    level,
                    message,
                    Environment.NewLine);

                lock (log_lock)
                {
                    File.AppendAllText(log_file_path, line, Encoding.UTF8);
                }
            }
            catch
            {
                // 日志写入失败时不影响主流程
            }
        }
    }
}
