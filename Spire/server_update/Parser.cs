using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace server_update
{

	public class Groups{
		public List<Group> groups {get; set;}
	}

	public class Group{
		public string name {get; set;}
		public List<Slave> slaves {get; set;}
	}

	public class Slave{
		public string ip {get; set;}
	}

	public class Parser
	{

		public string[] Parse(string yaml, string group_name){

			var result = new List<string>();

			var port_no = "10000";

			var content = new StringReader(yaml);
			var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
			var Groups = deserializer.Deserialize<Groups>(content);

			if(Equals(group_name, "all")){
				foreach(var group in Groups.groups){
					foreach(var slave in group.slaves){
						var complete = String.Format("{0}:{1}", slave.ip, port_no);
						result.Add(complete);
					}
				}
			} else {
				foreach(var group in Groups.groups){
					if(Equals(group.name, group_name)){
						foreach(var slave in group.slaves){
							var complete = String.Format("{0}:{1}", slave.ip, port_no);
							result.Add(complete);
						}
					}
				}
			}

			content.Close();

			return(result.ToArray());

		}

	}
}