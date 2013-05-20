using System;
using System.Collections.Generic;
using Blk.Engine;


namespace ConfigUtil
{
	public class ConfigurationHelper
	{

		private string filePath;

		private Blackboard blackboard;

		public Blackboard Blackboard
		{
			get { return this.blackboard; }
			set
			{
				blackboard = value;
			}
		}

		public string FilePath
		{
			get { return this.filePath; }
			set { this.filePath = value; }
		}



		public void SerializeToXml()
		{
		}

	}
}
