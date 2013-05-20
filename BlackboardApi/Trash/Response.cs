using System;
using System.Collections.Generic;
using System.Text;

namespace Blackboard
{
	/// <summary>
	/// Represents a command response
	/// </summary>
	public class Response : Command
	{
		#region Variables

		/// <summary>
		/// The result of the message. 1 if success, 0 if not success or this message is not response
		/// </summary>
		private int result;

		#endregion

		#region Constructores

		/// <summary>
		/// Initiates a new instance of Response
		/// </summary>
		/// <param name="module">Module which generated the response</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the command</param>
		/// <param name="result">Result of the command execution</param>
		public Response(Module module, string command, string param, int id, int result) :  base(module, command, param, id)
		{
			this.result = result;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the result of the message (if message contains any)
		/// </summary>
		public int Result
		{
			get { return result; }
		}

		#endregion

		#region Eventos
		#endregion

		#region Metodos

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			return (source.Name == null ? "Unknown" : source.Name) + "" + (destination.Name == null ? "Unknown" : destination.Name) + " " + command + " " + parameters + (id >= 0 ? " @" + id : "");
		}

		#endregion

		#region Event Handler Functions
		#endregion

		#region Metodos de Clase (Estáticos)

		/// <summary>
		/// Creates a Response for a message
		/// </summary>
		/// <param name="m">The base message</param>
		/// <param name="success">True if the result succeded, false otherwise</param>
		/// <returns></returns>
		public static Response CreateFromMessage(Message m, bool success)
		{
			//Response r = new Response(m.Source, m.Command, m.Parameters, m.Id);
			//r.result = success ? 1 : 0;
			//r.originalString = "";
			//return r;
			return null;
		}

		#endregion
	}
}
