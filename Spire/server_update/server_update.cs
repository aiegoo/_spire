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
            var group = new StringBuilder();
            var secrets = new StringBuilder();
            var parser = new Parser();
            
            if(args.Length != 2){
                Console.WriteLine("server-update [group name or all] [script name]");
                Environment.Exit(1);
            } else {
                group.Append(args[0]);
                script.Append(args[1]);
            }

            if(System.IO.File.Exists("secret.txt")){
                secrets.Append(System.IO.File.ReadAllText("secret.txt"));
            } else {
                Console.WriteLine("Could not find secret.txt file...");
                Environment.Exit(1);
            }

            var client = new HttpClient();
            var script_name = script.ToString();
            var group_name = group.ToString();
            var secret = secrets.ToString();

            if(System.IO.File.Exists("slavelist.yaml")){
                hosts = parser.Parse(System.IO.File.ReadAllText("slavelist.yaml"), group_name);
            } else {
                Console.WriteLine("Could not find slavelist.yaml file...");
                Environment.Exit(1);
            }

            var output_filename = "output.log";
            var out_file = System.IO.File.CreateText(output_filename);
            var current_dir = System.IO.Directory.GetCurrentDirectory();

            if(hosts.Length == 0){
                Console.WriteLine("\nCould not find any slaves belonging to {0}... Please check the configuration file...\n", group_name);
                Environment.Exit(1);
            }
            
            foreach(var line in hosts){
                
                var complete = script_name + secret;
                Console.WriteLine("\nRunning script {0} on host {1}", script_name, line);
                
                var data = new Dictionary<string, string>{{"fname", Obfuscate(complete)}};
                var tosend = new FormUrlEncodedContent(data);

                try{

                    var send = await client.PostAsync(line, tosend);
                    var response = await send.Content.ReadAsStringAsync();
                    Console.WriteLine("{0}\n", response);

                    var output = String.Format("[+]{0}\n{1}\n", line, response);
                    out_file.WriteLine(output);


                } catch (Exception e) {
                    
                    var err = e.ToString();
                    var error = string.Format("[!] An error has occurred [{0}]!\n{1}\n", line, err);
                    var err_file = "errors.log";
                    var err_log = String.Format("{0}/{1}", current_dir, err_file);
                    
                    if(System.IO.File.Exists(err_file)){
                        var stream = System.IO.File.AppendText(err_file);
                        stream.WriteLine(error);
                        stream.Close();
                    } else {
                        var stream = System.IO.File.CreateText(err_file);
                        stream.WriteLine(error);
                        stream.Close();
                    }

                    Console.WriteLine("Something went wrong with {0}... Check {1} for more details...\n", line, err_log);

                }
            }

            var output_log = String.Format("{0}/{1}", current_dir, output_filename);
            out_file.Close();
            Console.WriteLine("Output saved to {0}\n", output_log);
        }
    }
}
