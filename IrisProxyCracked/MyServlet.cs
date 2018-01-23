using System;
using System.Net;
using System.Text;
using IrisProxyCracked.Properties;

namespace IrisProxyCracked
{
    internal class MyServlet : Servlet
    {
        public override void onCreate()
        {
            base.onCreate();
        }

        public override void onGet(HttpListenerRequest request, HttpListenerResponse response)
        {
            Console.WriteLine("GET:" + request.Url);
            var buffer = Encoding.UTF8.GetBytes(Settings.Default.responseStr);

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            //listener.Stop();
        }

        public override void onPost(HttpListenerRequest request, HttpListenerResponse response)
        {
            Console.WriteLine("POST:" + request.Url);
            var res = Encoding.UTF8.GetBytes("OK");
            response.OutputStream.Write(res, 0, res.Length);
        }
    }
}