using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Blk.Engine.Remote;
using Blk.Engine;

namespace Blk.Gui
{
	[Serializable]
	public class GUISettings
	{
		#region Variables

		private bool autoLog;

		private ModuleStartupMethod mStartupSequenceMode;

		private string conigurationFileName;

		private List<InjectedString> injectedStrings;

		private int verbosityLevel;

		#endregion

		#region Constructors

		public GUISettings()
		{
			this.verbosityLevel = 0;
			this.conigurationFileName = "blackboard.xml";
			injectedStrings = new List<InjectedString>(100);
		}

		public GUISettings(Blackboard blackboard, string bbConfigFile): this()
		{
			if(blackboard == null)
				throw new ArgumentNullException();
			this.verbosityLevel = blackboard.VerbosityLevel;
			this.conigurationFileName = bbConfigFile;
		}

		#endregion

		#region Properties

		[XmlElement("autoLog")]
		public bool AutoLog
		{
			get { return this.autoLog; }
			set { this.autoLog = value; }
		}

		[XmlElement("xmlConfigurationFile")]
		public string ConigurationFileName
		{
			get { return this.conigurationFileName; }
			set
			{
				if ((value == null) || !File.Exists(value))
					throw new FieldAccessException();
				this.conigurationFileName = value;
			}
		}

		[XmlElement("moduleStartupSequenceMethod")]
		public ModuleStartupMethod ModuleStartupSequenceMode
		{
			get { return this.mStartupSequenceMode; }
			set { this.mStartupSequenceMode = value; }
		}

		[XmlElement("verbosityLevel")]
		public int VerbosityLevel
		{
			get { return this.verbosityLevel; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException();
				this.verbosityLevel = value;
			}
		}

		[XmlArrayItem("injectedStrings")]
		public InjectedString[] InjectedStringsArray
		{
			get { return this.InjectedStrings.ToArray(); }
			set
			{
				this.InjectedStrings.Clear();
				this.InjectedStrings.AddRange(value);
			}
		}

		[XmlIgnore]
		public List<InjectedString> InjectedStrings
		{
			get { return this.injectedStrings; }
		}

		#endregion

		#region Methods

		public static string GetSettingsFileName(string xmlConfigurationFilePath)
		{
			FileInfo fiConfigFile;
			string fileName;
			try
			{
				fiConfigFile = new FileInfo(xmlConfigurationFilePath);
				fileName = fiConfigFile.Name.Substring(0, fiConfigFile.Name.Length - fiConfigFile.Extension.Length) + ".bbsf";
			}
			catch { return "settings.bbsf"; }
			return fileName;
		}

		public static bool LoadFromFile(string fileName, out GUISettings settings)
		{
			XmlSerializer serializer;
			settings = null;

			try
			{
				if (!File.Exists(fileName))
					return false;
				serializer = new XmlSerializer(typeof(GUISettings));
				using (FileStream fstream = File.Open(fileName, FileMode.Open, FileAccess.Read))
				{
					settings = (GUISettings)serializer.Deserialize(fstream);
				}
			}
			catch { return false; }
			return true;
		}

		public static bool SaveToFile(GUISettings settings)
		{
			if (settings == null)
				return false;

			XmlSerializer serializer;
			string fileName;

			try
			{
				fileName = GetSettingsFileName(settings.ConigurationFileName);
				serializer = new XmlSerializer(typeof(GUISettings));
				using (FileStream fstream = File.Open(fileName, FileMode.Create, FileAccess.Write))
				{
					serializer.Serialize(fstream, settings);
				}
			}
			catch { return false; }
			return true;
		}

		#endregion
	}
}
