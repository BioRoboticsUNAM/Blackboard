using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine
{
	/// <summary>
	/// Represents a ResponseCandidate for a command
	/// </summary>
	internal class ResponseCandidate : IComparable<ResponseCandidate>
	{
		#region Variables
		/// <summary>
		/// Asociated command
		/// </summary>
		private Command command;
		/// <summary>
		/// Candidate response
		/// </summary>
		private Response response;
		/// <summary>
		/// Affinity between Command and response
		/// </summary>
		private int affinity;

		#endregion

		#region Constructors
		
		/// <summary>
		/// Initiates a new instance of ResponseCandidate
		/// </summary>
		/// <param name="response">Candidate Response</param>
		/// <param name="affinity">Affinity between Command and Response</param>
		public ResponseCandidate(Response response, int affinity)
		{
			this.response = response;
			this.affinity = affinity;
		}

		/// <summary>
		/// Initiates a new instance of ResponseCandidate
		/// </summary>
		/// <param name="command">Asociated command</param>
		/// <param name="response">Candidate Response</param>
		/// <param name="affinity">Affinity between Command and Response</param>
		public ResponseCandidate(Command command, Response response, int affinity)
		{
			this.command = command;
			this.response = response;
			this.affinity = affinity;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the affinity between Command and Response
		/// </summary>
		public int Affinity
		{
			get { return affinity; }
		}

		/// <summary>
		/// Gets the asociated Command
		/// </summary>
		public Command Command
		{
			get { return command; }
		}

		/// <summary>
		/// Gets the candidate Response
		/// </summary>
		public Response Response
		{
			get { return response; }
		}		

		#endregion

		#region Operators

		/// <summary>
		///  Gets the asociated Command
		/// </summary>
		/// <param name="value">ResponseCandidate object to convert</param>
		/// <returns>Asociated Command</returns>
		public static implicit operator Command(ResponseCandidate value)
		{
			return value.command;
		}

		/// <summary>
		///  Gets the candidate Response
		/// </summary>
		/// <param name="value">ResponseCandidate object to convert</param>
		/// <returns>The candidate Response</returns>
		public static implicit operator Response(ResponseCandidate value)
		{
			return value.response;
		}

		/// <summary>
		/// Gets the affinity between Command and Response
		/// </summary>
		/// <param name="value">ResponseCandidate object to convert</param>
		/// <returns>The affinity between Command and Response</returns>
		public static implicit operator int(ResponseCandidate value)
		{
			return value.affinity;
		}

		#endregion

		#region IComparable<ResponseCandidate> Members
		/// <summary>
		/// Compares this instance of ResponseCandidate with another
		/// </summary>
		/// <param name="other">ResponseCandidate to compare to</param>
		/// <returns>Sort order</returns>
		int IComparable<ResponseCandidate>.CompareTo(ResponseCandidate other)
		{
			return this.affinity - other.affinity;
			//if (this.affinity < other.affinity) return -1;
			//if (this.affinity > other.affinity) return 1;
			//if (this.response.ArrivalTime < other.response.ArrivalTime) return -1;
			//if (this.response.ArrivalTime > other.response.ArrivalTime) return 1;
			//return 0;
		}

		#endregion
	}
}
