using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace server_update
{
    class Program
    {

        private static string Obfuscate(string data){
            var result = new StringBuilder();
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            for(int i = 0; i < bytes.Length; i++){
                result.Append(bytes[i].ToString());
            }
            return(result.ToString());
        }

        public static async Task Main(string[] args)
        {
            var script_name = "";
            if(args.Length != 1){
                Console.WriteLine("Script name needed as an argument");
                Environment.Exit(1);
            } else {
                script_name = args[0];
            }
            using var client = new HttpClient();
            var hosts = System.IO.File.ReadLines("slavelist.txt");
            foreach(var line in hosts){
                var secret = System.IO.File.ReadAllText("secret.txt");
                var complete = script_name + secret;
                Console.WriteLine("\nRunning script {0} on host {1}", script_name, line);
                var data = new Dictionary<string, string>{{"fname", Obfuscate(complete)}};
                var tosend = new FormUrlEncodedContent(data);
                var send = await client.PostAsync(line, tosend);
                var response = await send.Content.ReadAsStringAsync();
                Console.WriteLine("{0}\n", response);
            }
        }
    }
}
