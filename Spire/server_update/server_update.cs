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
            var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            
            for(int i = 0; i < bytes.Length; i++){
                result.Append(bytes[i].ToString());
            }

            return(result.ToString());
        }

        public static async Task Main(string[] args)
        {
            
            var hosts = new string[0];
            var script = new StringBuilder();
            var secrets = new StringBuilder();
            
            if(args.Length != 1){
                Console.WriteLine("Script name needed as an argument...");
                Environment.Exit(1);
            } else {
                script.Append(args[0]);
            }

            if(System.IO.File.Exists("slavelist.txt")){
                hosts = System.IO.File.ReadAllLines("slavelist.txt");
            } else {
                Console.WriteLine("Could not find slavelist.txt file...");
                Environment.Exit(1);
            }

            if(System.IO.File.Exists("secret.txt")){
                secrets.Append(System.IO.File.ReadAllText("secret.txt"));
            } else {
                Console.WriteLine("Could not find secret.txt file...");
                Environment.Exit(1);
            }

            var client = new HttpClient();
            var script_name = script.ToString();
            var secret = secrets.ToString();
            
            foreach(var line in hosts){
                
                var complete = script_name + secret;
                Console.WriteLine("\nRunning script {0} on host {1}", script_name, line);
                
                var data = new Dictionary<string, string>{{"fname", Obfuscate(complete)}};
                var tosend = new FormUrlEncodedContent(data);

                try{

                    var send = await client.PostAsync(line, tosend);
                    var response = await send.Content.ReadAsStringAsync();
                    Console.WriteLine("{0}\n", response);

                } catch (Exception e) {
                    
                    var err = e.ToString();
                    var error = string.Format("[!] Error on {0}\n\n{1}\n", line, err);
                    var err_file = "slaves.log";
                    
                    if(System.IO.File.Exists(err_file)){
                        var stream = System.IO.File.AppendText(err_file);
                        stream.WriteLine(error);
                        stream.Close();
                    } else {
                        var stream = System.IO.File.CreateText(err_file);
                        stream.WriteLine(error);
                        stream.Close();
                    }

                    Console.WriteLine("Something is wrong with {0}... Check slaves.log for more details...\n", line);

                }
            }
        }
    }
}
