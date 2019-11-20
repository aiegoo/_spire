using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace slave.Controllers
{
	public class HomeController : Controller
	{

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
				var filename = HttpContext.Request.Form["fname"];
				var path = String.Format("scripts/{0}", filename);
				Process.Start(path);
				return Content("Script successfully executed!");
			} catch (Exception e){
				var err = String.Format("An error has occured: {0}", e);
				return Content(err);
			}
		}
	}
}
