// 定义要执行的命令和参数
using System.Diagnostics;
using System.Security.Cryptography;

string command = "taskkill"; // 替换为要执行的命令
string arguments = $"/f /im StudentMain.exe"; // 替换为命令的参数

// 创建一个新的进程
ProcessStartInfo processStartInfo = new ProcessStartInfo
{
    FileName = "cmd.exe", // 使用 cmd.exe 执行命令
    Arguments = $"/c {command} {arguments}", // /c 参数表示在执行完命令后关闭命令窗口
    UseShellExecute = true, // 使用系统外壳程序启动
    Verb = "runas", // 以管理员身份运行
    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory // 设置当前工作目录为程序运行目录
};

try
{
    // 启动进程
    Process process = Process.Start(processStartInfo);
    process.WaitForExit(); // 等待命令执行完成
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
}