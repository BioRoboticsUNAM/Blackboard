using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

namespace Blackboard
{
	#region Delegados
	/// <summary>
	/// Represents the method that will handle the MessageReceived and MessageSent event of a Blackboard object.
	/// </summary>
	/// <param name="message">Message received/sent</param>
	public delegate void MessageIOEH(CommandBase message);
	/// <summary>
	/// Represents the method that will handle the MessageReceived and MessageSent event of a Blackboard object.
	/// </summary>
	/// <param name="origin">The module requesting redirect</param>
	/// <param name="destination">The destination module</param>
	/// <param name="message">Message received/sent</param>
	public delegate void MessageRedirectEH(ModuleClient origin, ModuleClient destination, CommandBase message);
	/// <summary>
	/// Represents the method that will handle the ClientConnected and ClientDisconnected event of a Blackboard object.
	/// </summary>
	/// <param name="ip">The ip Address of the client</param>
	public delegate void BlackboardClientCnnEH(IPAddress ip);
	#endregion

	/// <summary>
	/// Implements a blackboard
	/// </summary>
	public class Blackboard
	{
		#region Variables
		private int port;
		private SocketTcpServer server;
		private ModuleCollection modules;
		#endregion

		#region Constructores

		private Blackboard()
		{
			//Initialize();
		}

		/// <summary>
		/// Initializes a new instance of Blackboard
		/// </summary>
		/// <param name="port">The port where messajes will arrive</param>
		public Blackboard(int port)
		{
			this.port = port;
			Initialize();
		}

		#endregion

		#region Propiedades
		/// <summary>
		/// Gets the port when the blackboard accept incomming connections
		/// </summary>
		public int Port
		{
			get { return port; }
		}

		/// <summary>
		/// Gets the modules managed by the Blackboard
		/// </summary>
		public ModuleCollection Modules
		{
			get { return modules; }
		}
		#endregion

		#region Eventos
		/// <summary>
		/// Represents the method that will handle the data ModuleConnectedToBlackBoard event 
		/// </summary>
		public event BlackboardClientCnnEH ModuleConnectedToBlackBoard;
		/// <summary>
		/// Represents the method that will handle the data BlackboardConnectedToModule event 
		/// </summary>
		public event ModuleConnectionEH BlackboardConnectedToModule;
		/// <summary>
		/// Represents the method that will handle the data ModuleDisconnectedFromBlackBoard event 
		/// </summary>
		public event BlackboardClientCnnEH ModuleDisconnectedFromBlackBoard;
		/// <summary>
		/// Represents the method that will handle the data BlackboardDisconnectedFromModule event 
		/// </summary>
		public event ModuleConnectionEH BlackboardDisconnectedFromModule;
		/// <summary>
		/// Represents the method that will handle the data MessageSent event 
		/// </summary>
		public event MessageIOEH MessageSent;
		/// <summary>
		/// Represents the method that will handle the data MessageReceived event 
		/// </summary>
		public event MessageIOEH MessageReceived;
		/// <summary>
		/// Represents the method that will handle the data MessageRedirect event 
		/// </summary>
		public event MessageRedirectEH MessageRedirect;
		#endregion

		#region Metodos publicos
		/// <summary>
		/// Starts the Blackboard
		/// </summary>
		/// <returns></returns>
		public void Start()
		{
			foreach (ModuleClient module in modules)
			{
				module.Start();
			}
			server.Start();
		}

		/// <summary>
		/// Load the modules the blackboard will be connected with
		/// </summary>
		/// <param name="path">Path of file containing module information</param>
		/// <returns>True if load was successfull</returns>
		public bool LoadModules(string path)
		{
			char[] splitChars = { '\r', '\n' };
			string[] lines;
			ModuleClient m;
			int i;

			if (!File.Exists(path)) return false;
			try
			{
				lines = File.ReadAllText(path).Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
			}
			catch { return false; }
			for (i = 0; i < lines.Length; ++i)
			{
				if ((m = ModuleClient.FromString(lines[i])) != null)
				{
					m.Connected += new ModuleConnectionEH(module_Connected);
					m.Disconnected += new ModuleConnectionEH(module_Disconnected);
					modules.Add(m);
				}
			}
			if (i == 0) return false;
			return true;
		}

		/// <summary>
		/// Load the script the blackboard will execute
		/// </summary>
		/// <param name="path">Path of file containing the script</param>
		/// <returns>True if load was successfull</returns>
		public bool LoadScript(string path)
		{
			return true;
		}

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		public bool FindDestinationModule(string commandName, out ModuleClient destination)
		{
			Prototype p;
			return FindDestinationModule(commandName, out destination, out p);
		}

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="prototype">When this method returns, contains the Prototype for the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		public bool FindDestinationModule(string commandName, out ModuleClient destination, out Prototype prototype)
		{
			destination = null;
			prototype = null;
			if ((commandName == null) || !Regex.IsMatch(commandName, @"[A-Za-z_]+")) return false;
			foreach (ModuleClient m in modules)
				if (m.SupportCommand(commandName, out prototype))
				{
					destination = m;
					return true;
				}
			return false;
		}
		#endregion

		#region Metodos privados

		private void Initialize()
		{
			modules = new ModuleCollection(this, 6);
			server = new SocketTcpServer(port);
			server.ClientConnected += new TcpClientConnectedEventHandler(server_ClientConnected);
			server.ClientDisconnected += new TcpClientConnectedEventHandler(server_ClientDisconnected);
			server.DataReceived += new TcpDataReceivedEventHandler(server_DataReceived);
		}

		private ModuleClient getModule(string name)
		{
			for (int i = 0; i < modules.Count; ++i)
				if (modules[i].Name == name) return modules[i];
			return null;
		}

		private ModuleClient getModule(IPAddress ip, int port)
		{
			for (int i = 0; i < modules.Count; ++i)
				if ((modules[i].ServerAddress == ip) && (modules[i].Port == port)) return modules[i];
			return null;
		}

		/// <summary>
		/// Converts the string representation of a Message to its Message. A return value indicates whether the operation succeeded
		/// </summary>
		/// <param name="s">A string containing a messsage to convert.</param>
		/// <param name="result">When this method returns, contains the Message value equivalent to the date and time contained in s, if the conversion succeeded, or Null if the conversion failed. The conversion fails if the s parameter is a null reference (Nothing in Visual Basic), or does not contain a valid string representation of a message. This parameter is passed uninitialized</param>
		/// <returns>true if the s parameter was converted successfully; otherwise, false. </returns>
		public bool ParseMessage(string s, out CommandBase result)
		{
			string rxSpace = @"[\t ]+";
			string rxOrigin = @"(?<origin>[A-Z][A-Z\-]*)";
			string rxDestination = @"((?<destination>[A-Z][A-Z\-]*)" + rxSpace + ")?";
			string rxCommand = @"(?<cmd>[a-z][a-zA-Z0-9\-_]*)";
			string rxParam = @"(?<param>(\x22[^\x00\0x22@]*\x22)|(\w(\w|\t|\.|-)*\w))";
			//string rxParam = @"(?<param>(\x22[^\x00\0x22@]*\x22)|(\w[^\x22\x00@]*\w))";
			//string rxParam = @"(?<param>\x22[a-zA-Z0-9\-_ \t\.]*\x22)";
			string rxSuccess = @"(" + rxSpace + "(?<success>1|0))?";
			string rxId = "(" + rxSpace + @"\@(?<id>\d+)[\t ]*)?";
			Match m;
			Regex rx;
			ModuleClient origin;
			ModuleClient destination;
			string command;
			string param;
			string success;
			string id;
			string msg;

			result = null;
			rx = new Regex(rxOrigin + rxSpace + rxDestination + "(?<msg>(" + rxCommand + rxSpace + rxParam + rxSuccess + "))" + rxId, RegexOptions.Compiled);
			m = rx.Match(s.Trim());
			if (!m.Success) return false;
			origin = getModule(m.Result("${origin}"));
			//m = new Message();
			destination = getModule(m.Result("${destination}"));
			command = m.Result("${cmd}");
			param = m.Result("${param}");
			success = m.Result("${success}");
			id = m.Result("${id}");
			msg = m.Result("${msg}");
			if (origin == null)
			{
				if (MessageReceived != null) MessageReceived(result);
				return false;
			}
			if (MessageReceived != null)
				MessageReceived(result);
			//if (destination != null)
			//	RedirectMsg(origin, destination, m);

			if (command != "")
				Process(origin, command, param, id);
			//else Process(origin, str, id);
			return true;
		}

		//public bool ParseMessage(string s, out Module m)
		//private bool ParseMessage(string s, out Module m)
		//{
		//    string rxSpace = @"[\t ]+";
		//    string rxOrigin = @"(?<origin>[A-Z][A-Z\-]*)";
		//    string rxDestination = @"((?<destination>[A-Z][A-Z\-]*)" + rxSpace + ")?";
		//    string rxCommand = @"(?<cmd>[a-z][a-zA-Z0-9\-_]*)";
		//    string rxParam = @"(?<param>\x22[^\x00\0x22]*\x22)";
		//    //string rxParam = @"(?<param>\x22[a-zA-Z0-9\-_ \t\.]*\x22)";
		//    string rxSuccess = @"[\t ]*";
		//    string rxStr = @"(?<str>[\w \t\.]+[\w\.])";
		//    string rxId = "(" + rxSpace + @"\@(?<id>\d+)[\t ]*)?";
		//    Match result;
		//    Regex rx;
		//    Module origin;
		//    Module destination;
		//    string command;
		//    string param;
		//    string str;
		//    string msg;
		//    string id;

		//    m = null;
		//    rx = new Regex(rxOrigin + rxSpace + rxDestination + "(?<msg>(" + rxCommand + rxSpace + rxParam + "|" + rxStr + "))" + rxId, RegexOptions.Compiled);
		//    result = rx.Match(s.Trim());
		//    if (!result.Success) return false;
		//    origin = getModule(result.Result("${origin}"));
		//    if (origin == null)
		//    {
		//        if(MessageReceived!= null) MessageReceived(null, s);
		//        return false;
		//    }

		//    destination = getModule(result.Result("${destination}"));
		//    command = result.Result("${cmd}");
		//    param = result.Result("${param}");
		//    str = result.Result("${str}");
		//    id = result.Result("${id}");
		//    msg = result.Result("${msg}");
		//    if(MessageReceived!= null)
		//        MessageReceived(origin, msg + (id != "" ? " @" + id : ""));
		//    if (destination != null)
		//        RedirectMsg(origin, destination, msg + (id != "" ? " @" + id : ""));

		//    if(command != "")
		//        Process(origin, command, param, id);
		//    else Process(origin, str, id);
		//    return true;
		//}

		private void Process(ModuleClient origin, string command, string param, string id)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		private void Process(ModuleClient origin, string str, string id)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void RedirectMsg(ModuleClient origin, ModuleClient destination, string msg)
		{
			// destination.SendMessage(msg);
			//if (MessageRedirect != null)
			//	MessageRedirect(origin, destination, msg);
		}

		#endregion

		#region Metodos de Clase (Estáticos)

		/// <summary>
		/// Creates a blackboard from a XML configuration file
		/// </summary>
		/// <param name="path">The path of the XML configuration file</param>
		/// <returns>The configured blackboard</returns>
		public static Blackboard FromXML(string path)
		{
			TextWriter w = TextWriter.Null; 
			return FromXML(path, w);
		}

		/// <summary>
		/// Creates a blackboard from a XML configuration file
		/// </summary>
		/// <param name="path">The path of the XML configuration file</param>
		/// <param name="log">Output log</param>
		/// <returns>The configured blackboard</returns>
		public static Blackboard FromXML(string path, TextWriter log)
		{
			Blackboard blackboard;
			XmlDocument doc;
			XmlDocument tmpDoc;
			XmlNodeList modules;
			XmlNodeList commands;
			int i, j, cmdCount;

			string moduleName;
			string programPath;
			string programArgs;
			string cmdName;
			bool cmdParams;
			bool cmdAnswer;
			bool cmdPriority;
			int cmdTimeOut;
			Command startupCommand;
			Prototype msg;
			List<Prototype> msgList;
			ModuleClient mod;
			IPAddress ip;
			int port;

			log.Flush();
			if (!File.Exists(path))
			{
				log.WriteLine("File does not exist");
				throw new FileLoadException("File does not exist");
			}
			doc = new XmlDocument();
			doc.Load(path);

			if (
				(doc.GetElementsByTagName("blackboard").Count != 1) ||
				(doc.GetElementsByTagName("configuration").Count != 1) ||
				(doc.GetElementsByTagName("modules").Count < 1))
			{
				log.WriteLine("Incorrect format");
				throw new FileLoadException("Incorrect format");
			}

			blackboard = new Blackboard();
			log.WriteLine("Loading configuration...");
			tmpDoc = new XmlDocument();
			tmpDoc.LoadXml(doc.GetElementsByTagName("configuration")[0].OuterXml);
			// Leo puerto
			if ((tmpDoc.GetElementsByTagName("port").Count != 1) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("port")[0].InnerText, out blackboard.port))
			{
				blackboard.port = 2300;
				log.WriteLine("No Blackboard port specified, using default: " + blackboard.Port);
			}
			else log.WriteLine("Blackboard port: " + blackboard.Port);

			if (doc.GetElementsByTagName("modules").Count < 1)
			{
				log.WriteLine("No modules to load");
				throw new Exception("No modules to load");
			}
			modules = doc.GetElementsByTagName("modules")[0].ChildNodes;
			blackboard.Initialize();
			#region Extraccion de Modulos
			for (i = 0; i < modules.Count; ++i)
			{
				try
				{
					// Verifico que sea un modulo
					if ((modules[i].Name != "module") ||
						(modules[i].Attributes.Count < 1) ||
						(modules[i].Attributes["name"].Value.Length < 1))
						continue;

					#region Extraccion de info del modulo
					moduleName = modules[i].Attributes["name"].Value.ToUpper();
					log.WriteLine("Loading module " + moduleName);
					// Creo un subdocumento XML
					tmpDoc.LoadXml(modules[i].OuterXml);
					// Leo el Path de la aplicacion
					if (tmpDoc.GetElementsByTagName("programPath").Count != 0)
						programPath = tmpDoc.GetElementsByTagName("programPath")[0].InnerText;
					// Leo los argumentos con que se inicia la aplicacion
					if (tmpDoc.GetElementsByTagName("programArgs").Count != 0)
						programArgs = tmpDoc.GetElementsByTagName("programArgs")[0].InnerText;
					// Leo el comando de inicio
					//if (tmpDoc.GetElementsByTagName("startupCommand").Count != 0)
					//	startupMessage = Command.Parse(tmpDoc.GetElementsByTagName("startupCommand")[0].InnerText);
					// Leo la IP de la maquina donde esta el modulo
					if (
						(tmpDoc.GetElementsByTagName("ip").Count == 0) ||
						!IPAddress.TryParse(tmpDoc.GetElementsByTagName("ip")[0].InnerText, out ip))
					{
						log.WriteLine("\tInvalid IP Address");
						log.WriteLine("Module skipped");
						continue;
					}
					// Leo el puerto de conexion del modulo
					if (
						(tmpDoc.GetElementsByTagName("port").Count == 0) ||
						!Int32.TryParse(tmpDoc.GetElementsByTagName("port")[0].InnerText, out port) ||
						(port <= 1024))
					{
						log.WriteLine("\tInvalid port");
						log.WriteLine("Module skipped");
						continue;
					}
				#endregion

					// Creo el modulo
					mod = new ModuleClient(moduleName, ip, port);

					// Leo lista de comandos.
					log.WriteLine("\tLoading module commands...");
					if (doc.GetElementsByTagName("commands").Count < 1)
					{
						log.WriteLine("\tNo commands to load");
						log.WriteLine("Module skipped");
						continue;
					}
					commands = tmpDoc.GetElementsByTagName("commands")[0].ChildNodes;
					msgList = new List<Prototype>(commands.Count);

					#region Extraccion de Comandos de modulo

					for (j = 0; j < commands.Count; ++j)
					{
						// Verifico que sea un comando
						if ((commands[j].Name == "command") &&
						(commands[j].Attributes.Count >= 3) &&
						(commands[j].Attributes["name"].Value.Length > 1) &&
						Boolean.TryParse(commands[j].Attributes["answer"].Value, out cmdAnswer) &&
						Int32.TryParse(commands[j].Attributes["timeout"].Value, out cmdTimeOut) &&
						(cmdTimeOut >= 0))
						{
							// Leo nombre de comando
							cmdName = commands[j].Attributes["name"].Value;
							log.WriteLine("\t\tAdded command " + cmdName);
							// Verifico si requiere parametros
							if(!Boolean.TryParse(commands[j].Attributes["parameters"].Value, out cmdParams))
								cmdParams = true;
							// Verifico si tiene prioridad
							if (!Boolean.TryParse(commands[j].Attributes["priority"].Value, out cmdPriority))
								cmdPriority = false;
							// Creo el prototipo
							msg = new Prototype(cmdName, cmdParams, cmdAnswer, cmdTimeOut);
							// Agrego el prototipo al modulo
							mod.Prototypes.Add(msg);
							msgList.Add(msg);
						}
						else log.WriteLine("\t\tInvalid Command ");
					}
					#endregion
					// Si no hay comandos soportados por el modulo, salto el modulo
					if (msgList.Count < 1)
					{
						log.WriteLine("\tAll commands rejected.");
						log.WriteLine("Module skipped");
						continue;
					}
					// Agrego el modulo al blackboard
					blackboard.modules.Add(mod);
					log.WriteLine("Loading module complete!");
				}
				catch
				{
					// Error al cargar el modulo
					log.WriteLine("Invalid module");
					// Continuo con el siguiente
					continue;
				}
			#endregion
			}
			return blackboard;

			/*
<module name="">
<programPath></programPath>
<programArgs></programArgs>
<startupCommand></startupCommand>
<ip>192.168.1.x</ip>
<port>2000</port>
<commands>
	<command name="" answer="True" timeout="100" />
	<command name="" answer="True" timeout="100" />
	<command name="" answer="True" timeout="100" />
</commands>
</module>
			*/
		}

		#endregion

		#region Event Handler Functions

		private void module_Disconnected(ModuleClient m)
		{
			if (BlackboardDisconnectedFromModule != null)
				BlackboardDisconnectedFromModule(m);
		}

		private void module_Connected(ModuleClient m)
		{
			if (BlackboardConnectedToModule != null)
				BlackboardConnectedToModule(m);
		}

		private void server_DataReceived(TcpPacket p)
		{
			CommandBase m;
			ParseMessage(p.DataString, out m);
			if (MessageReceived != null)
				MessageReceived(m);
		}

		private void server_ClientDisconnected(Socket s)
		{
			IPAddress ip = null;
			try{
				ip = ((IPEndPoint)s.RemoteEndPoint).Address;
			}catch{}
			if (ModuleDisconnectedFromBlackBoard != null)
				ModuleDisconnectedFromBlackBoard(ip);
		}

		private void server_ClientConnected(Socket s)
		{
			IPAddress ip = ((IPEndPoint)s.RemoteEndPoint).Address;
			if (ModuleConnectedToBlackBoard != null)
				ModuleConnectedToBlackBoard(ip);
		}

		#endregion
	}
}
