using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace slave.Controllers
{
	public class HomeController : Controller
	{

		private string Search(string data, string secret){
			
			var result = new StringBuilder();
			var dir = new DirectoryInfo("scripts");
			var files = dir.GetFiles("*");

			foreach(FileInfo file in files){
				var complete = file.Name + secret;
				var filename = Obfuscate(complete);
				if(Equals(data, filename)){
					result.Append(file.Name);
				}
			}

			return(result.ToString());
		}

        private static string Obfuscate(string data){
            
			var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            
			for(int i = 0; i < bytes.Length; i++){
                result.Append(bytes[i].ToString());
            }

            return(result.ToString());
        }

		[Route("/")]
		public ActionResult Index (){
			return NotFound();
		}

		[Route("/")]
		[STAThread]
		[HttpPost]
		public ActionResult PostIndex()
		{

			try{
				
				var filename_obfuscated = HttpContext.Request.Form["fname"];
				var secret = System.IO.File.ReadAllText("secret.txt");

				var filename = Search(filename_obfuscated, secret);

				if(String.IsNullOrEmpty(filename)){
					return Content("Could not find the filename in question...");
				}
				
				var path = String.Format("scripts/{0}", filename);
				Process.Start(path);
				var response = String.Format("Successfully executed {0}!", filename);
				return Content(response);

			} catch (Exception e){

				var err = e.ToString();
				var error = string.Format("[!] An error has occurred!\n\n{0}\n", err);
                var err_file = "error_logs.log";
                    
                if(System.IO.File.Exists(err_file)){
                    var stream = System.IO.File.AppendText(err_file);
                    stream.WriteLine(error);
                    stream.Close();
                } else {
                    var stream = System.IO.File.CreateText(err_file);
                    stream.WriteLine(error);
                    stream.Close();
                }

				var output = String.Format("Something went wrong... Check {0} for more details...", err_file);
				return Content(output);

			}

		}
	}
}
