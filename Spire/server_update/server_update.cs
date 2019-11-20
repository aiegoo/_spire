using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace server_update
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var script_name = "";

            if(args.Length != 1){
                Console.WriteLine("Script name needed as an argument");
                Environment.Exit(1);
            } else {
                script_name = args[0];
            }

            using var client = new HttpClient();

            var hosts_file = "slavelist.txt";
            var hosts = File.ReadLines(hosts_file);

            foreach(var line in hosts){
                try{
                    Console.WriteLine("\nRunning script {0} on host {1}", script_name, line);
                    var data = new Dictionary<string, string>{{"fname", script_name}};
                    var tosend = new FormUrlEncodedContent(data);
                    var send = await client.PostAsync(line, tosend);
                    var response = await send.Content.ReadAsStringAsync();
                    Console.WriteLine("{0}\n", response);
                } catch (Exception e) {
                    Console.WriteLine("An error has occured: {0}", e);
                }
            }
        }
    }
}
