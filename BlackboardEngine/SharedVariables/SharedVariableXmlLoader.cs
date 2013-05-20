using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Loads a set of shared variables from an XML
	/// </summary>
	public class SharedVariableXmlLoader : ISharedVariableLoader
	{
		#region Variables

		/// <summary>
		/// List of shared variables
		/// </summary>
		private SortedList<string, SharedVariable> variables;

		/// <summary>
		/// List of structures
		/// </summary>
		private SortedList<string, Structure> structures;

		/// <summary>
		/// List of reserved types
		/// </summary>
		private List<string> reserved;

        #endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableXmlLoader
		/// </summary>
		public SharedVariableXmlLoader()
		{
			structures = new SortedList<string, Structure>();
			variables = new SortedList<string, SharedVariable>();
			reserved = new List<string>(new string[]
			{
				"bool",
				"byte",
				"char",
				"double",
				//"float",
				"int",
				"long",
				//"short",
				"string",
				//"uint",
				//"ulong",
				//"ushort"
			});
		}

        #endregion

        #region Properties
        #endregion

        #region Events
        #endregion

		#region Methods

		/// <summary>
		/// Creates a set of the variable and its fields with empty values
		/// </summary>
		/// <param name="varType">The type of the variable</param>
		/// <param name="varName">The name of the variable</param>
		private SortedList<string, SharedVariable> CreateEmptyVar(string varType, string varName)
		{
			SortedList<string, SharedVariable> sharedVariables;
			Structure structure;

			sharedVariables = new SortedList<string, SharedVariable>(100);

			// 1. Check if variable type is a primitive
			if (reserved.Contains(varType))
				sharedVariables.Add(varName, new SharedVariable(null, varType, varName, false));
			else if (structures.ContainsKey(varType))
			{
				structure = structures[varType];
				CreateVariableFields(sharedVariables, structure, varName);
			}

			// 2. Return the shared variable set
			return sharedVariables;
		}

		/// <summary>
		/// Creates the set of fields with empty values for the specified variable
		/// </summary>
		/// <param name="sharedVariables">List of shared variables where the fields will be added</param>
		/// <param name="type">The type of the variable</param>
		/// <param name="parentName">The name of the parent variable or field</param>
		private void CreateVariableFields(SortedList<string, SharedVariable> sharedVariables, Structure type, string parentName)
		{
			string varName;
			foreach (Field field in type.Fields)
			{
				varName = parentName + "." + field.Name;
				if (reserved.Contains(field.Type))
					sharedVariables.Add(varName, new SharedVariable(null, field.Type, varName, field.IsArray));
				else if (structures.ContainsKey(field.Type))
					CreateVariableFields(sharedVariables, structures[field.Type], varName);
				else
				{
					sharedVariables.Clear();
					return;
				}
			}
		}

		private void FillVarData(XmlNodeList fieldNodes, SortedList<string, SharedVariable> sharedVariables, string parentName)
		{
			if (fieldNodes == null)
				return;
			
			string varName;
			foreach (XmlNode fieldNode in fieldNodes)
			{
				varName = parentName + "." + fieldNode.Name;
				if ((fieldNode == null) || String.IsNullOrEmpty(fieldNode.Name))
					continue;
				if (fieldNode.HasChildNodes)
				{
				}
				else if (sharedVariables.ContainsKey(varName))
				{
					sharedVariables[varName].WriteStringData(null, sharedVariables[varName].Type, -1, fieldNode.InnerText);
				}
			}
		}

		/// <summary>
		/// Loads one structure from an xml node
		/// </summary>
		/// <param name="node">The XML node which contains the structure</param>
		protected void LoadStructure(XmlNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (node.Name != "struct") return;
			if ((node.Attributes["name"] == null) || !SharedVariable.RxVarNameValidator.IsMatch(node.Attributes["name"].InnerText))
				return;
			if(!node.HasChildNodes)
				return;

			XmlNodeList fieldNodes;
			Structure structure;
			Field field;
			string sName;
			string fType;
			string fName;

			sName = node.Attributes["name"].InnerText;
			if (reserved.Contains(sName) || structures.ContainsKey(sName))
				return;
			fieldNodes = node.ChildNodes;
			structure = new Structure(sName);
			foreach (XmlNode fieldNode in fieldNodes)
			{
				if (fieldNode == null)
					continue;
				if ((fieldNode.Name != "field"))
					continue;
				if ((fieldNode.Attributes["type"] == null) || !SharedVariable.RxVarTypeValidator.IsMatch(fieldNode.Attributes["type"].InnerText))
					continue;
				if ((fieldNode.Attributes["name"] == null) || !SharedVariable.RxVarNameValidator.IsMatch(fieldNode.Attributes["name"].InnerText))
					continue;
				fType = fieldNode.Attributes["type"].InnerText;
				fName = fieldNode.Attributes["name"].InnerText;
				field = new Field(fType, fName);
				if (structure.Fields.Contains(fName))
					return;
				structure.Fields.Add(field);
			}
			if (structure.Fields.Count < 1)
				return;
			structures.Add(sName, structure);
		}

		/// <summary>
		/// Loads the structures from the xml document
		/// </summary>
		/// <param name="doc">The XML document which contains the structures data</param>
		protected void LoadStructures(XmlDocument doc)
		{
			XmlNodeList structNodes = doc.GetElementsByTagName("struct");

			structures.Clear();
			foreach (XmlNode node in structNodes)
				LoadStructure(node);
		}
		
		/// <summary>
		/// Loads a shared variable from an xml node
		/// </summary>
		/// <param name="node">The XML node which contains the variable</param>
		protected void LoadVariable(XmlNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (node.Name != "variable") return;
			if ((node.Attributes["type"] == null) || !SharedVariable.RxVarTypeValidator.IsMatch(node.Attributes["type"].InnerText))
				return;
			if ((node.Attributes["name"] == null) || !SharedVariable.RxVarNameValidator.IsMatch(node.Attributes["name"].InnerText))
				return;
			
			SortedList<string, SharedVariable> sharedVariables;
			string varName;
			string varType;

			// 1. Get variable name and type.
			varName = node.Attributes["name"].InnerText;
			varType = node.Attributes["type"].InnerText;
			
			// 2. Validate variable name and type.
			if (reserved.Contains(varName) || structures.ContainsKey(varName))
				return;
			if (!reserved.Contains(varType) && !structures.ContainsKey(varType))
				return;

			// 3. Create empty variable
			sharedVariables = CreateEmptyVar(varType, varName);

			// 4. Fill variable data
			FillVarData(node.ChildNodes, sharedVariables, varName);
		}

		/// <summary>
		/// Loads the variables from the xml document
		/// </summary>
		/// <param name="doc">The XML document which contains the variable data</param>
		private void LoadVariables(XmlDocument doc)
		{
			XmlNodeList varNodes = doc.GetElementsByTagName("variable");

			variables.Clear();
			foreach (XmlNode node in varNodes)
				LoadVariable(node);
		}

		/// <summary>
		/// Loads a set of shared variables from an XmlDocument
		/// </summary>
		/// <param name="doc">The XML document which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		public void FromDocument(XmlDocument doc, Blackboard blackboard)
		{
			
			if (blackboard == null)
				throw new ArgumentNullException("blackboard");
			if (doc == null)
				throw new ArgumentNullException("doc");

			// First extract structures
			LoadStructures(doc);
			// Check all field types are known 
			if(!ValidateNames())
				throw new ArgumentException("Unknown types defined");
			LoadVariables(doc);
			//blackboard.VirtualModule.SharedVariables.Add();
		}

		/// <summary>
		/// Loads a set of shared variables from an XML file
		/// </summary>
		/// <param name="filePath">The path of the XML file which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		public void FromFile(string filePath, Blackboard blackboard)
		{
			if (blackboard == null)
				throw new ArgumentNullException("blackboard");
			if (!File.Exists(filePath))
				throw new FileLoadException("File does not exist", filePath);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			FromDocument(doc, blackboard);
		}

		/// <summary>
		/// Loads a set of shared variables from an XML stream
		/// </summary>
		/// <param name="stream">The XML stream which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		public void FromStream(Stream stream, Blackboard blackboard)
		{
			if (blackboard == null)
				throw new ArgumentNullException("blackboard");
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			FromDocument(doc, blackboard);
		}

		/// <summary>
		/// Loads a set of shared variables from an XML string
		/// </summary>
		/// <param name="xml">The XML string which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		public void FromString(string xml, Blackboard blackboard)
		{
			if (blackboard == null)
				throw new ArgumentNullException("blackboard");
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			FromDocument(doc, blackboard);
		}

		/// <summary>
		/// Validate the types of each field of all loaded structures
		/// </summary>
		private bool ValidateNames()
		{
			List<string> typeNames;
			Structure structure;
			Field field;

			// First step: Fill a list of type names
			typeNames = new List<string>(reserved);
			typeNames.AddRange(structures.Keys);
			typeNames.Sort();

			// Check all field types are known
			for (int i = 0; i < structures.Count; ++i)
			{
				structure = structures.Values[i];
				for (int j = 0; j < structure.Fields.Count; ++j)
				{
					field = structure.Fields[j];
					if (!typeNames.Contains(field.Name))
						return false;
				}
			}
			return true;
		}

		#endregion
	}
}
