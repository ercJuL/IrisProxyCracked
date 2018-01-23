using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using IrisProxyCracked.Properties;

namespace IrisProxyCracked
{
    public class SocketServer
    {
        /// <summary>
        ///     应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Console.Title = "IrisProxyCracked by ercjul from www.52pojie.cn";
            if (!IsAdministrator())
            {
                Console.WriteLine("请以管理员方式运行本程序！");
                return;
            }
            Console.WriteLine("本软件仅限用于学习和研究目的");
            Console.WriteLine("首发www.52pojie.cn");
            Console.WriteLine("在同目录下IrisProxyCracked.exe.config中修改配置");
            Console.WriteLine("按任意键继续");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("=======初始化=======");
            init();
            Console.WriteLine("====启动本地服务====");
            HttpServerStart();
            Console.WriteLine("本次返回标识>>>>>" + Settings.Default.responseStr);
            Console.WriteLine("====服务启动成功====");
            Console.WriteLine("1.在Iris中输入任意激活码激活");
            Console.WriteLine("2.激活成功后关闭本窗口");
        }
        /// <summary>
        /// 判断是否拥有管理员权限
        /// </summary>
        /// <returns></returns>
        private static bool IsAdministrator()
        {
            var current = WindowsIdentity.GetCurrent();
            var windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// 初始化，检测hosts屏蔽，若无则屏蔽
        /// </summary>
        private static void init()
        {
            Console.Write("检测iristech.co地址>>>>>");
            try
            {
                var ipAddress = Dns.GetHostAddresses("iristech.co"); //获取https://iristech.co/解析地址
                if (ipAddress[0].ToString() == "127.0.0.1")
                {
                    Console.WriteLine(ipAddress[0] + "  已屏蔽");
                }
                else
                {
                    Console.WriteLine(ipAddress[0] + "  未屏蔽");
                    writeHosts();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// 写入hosts 127.0.0.1 iristech.co
        /// </summary>
        private static void writeHosts()
        {
            Console.WriteLine("修改Hosts屏蔽iristech.co");
            var hostsStreamWriter = new StreamWriter(Settings.Default.HostsWhere, true, Encoding.ASCII);
            hostsStreamWriter.WriteLine("127.0.0.1 iristech.co");
            hostsStreamWriter.Close();
            Console.WriteLine("修改Hosts完成");
        }
        /// <summary>
        /// 本地激活服务
        /// </summary>
        private static void HttpServerStart()
        {
            var httpListenner = new HttpListener();
            httpListenner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListenner.Prefixes.Add("https://iristech.co/");
            httpListenner.Prefixes.Add("http://iristech.co/");
            httpListenner.Start();

            new Thread(new ThreadStart(delegate
            {
                try
                {
                    loop(httpListenner);
                }
                catch (Exception)
                {
                    httpListenner.Stop();
                }
            })).Start();
        }
        /// <summary>
        /// 监听循环
        /// </summary>
        /// <param name="httpListenner"></param>
        private static void loop(HttpListener httpListenner)
        {
            while (true)
            {
                var context = httpListenner.GetContext();
                var request = context.Request;
                var response = context.Response;
                Servlet servlet = new MyServlet();
                servlet.onCreate();
                if (request.HttpMethod == "POST")
                    servlet.onPost(request, response);
                else if (request.HttpMethod == "GET")
                    servlet.onGet(request, response);
                response.Close();
            }
        }
    }
}