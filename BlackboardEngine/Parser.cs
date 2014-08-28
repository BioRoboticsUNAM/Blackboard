using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine
{
	/// <summary>
	/// Provides methods for data parsing
	/// </summary>
	public class Parser
	{
		#region Static Methods

		/// <summary>
		/// Extracts the first module name found inside a string
		/// </summary>
		/// <param name="s">String from which the module name will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="moduleName">When this method returns contains the first module name found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid module name was found in s starting at cc, false otherwise</returns>
		public static bool XtractModuleName(string s, ref int cc, out string moduleName)
		{
			int bcc = cc;
			int length;
			moduleName = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!IsUAlpha(s[cc])) return false;
			++cc;
			while ((cc < s.Length) && ((IsUAlNum(s[cc]) || s[cc] == '-')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			moduleName = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first command name found inside a string
		/// </summary>
		/// <param name="s">String from which the command name will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="commandName">When this method returns contains the first command name found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid command name was found in s starting at cc, false otherwise</returns>
		public static bool XtractCommandName(string s, ref int cc, out string commandName)
		{
			int bcc = cc;
			int length;
			commandName = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!IsLAlpha(s[cc]) && (s[cc] != '_')) return false;
			++cc;
			while ((cc < s.Length) && ((IsLAlNum(s[cc]) || s[cc] == '_')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			commandName = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Advances the read header until no spaces are found
		/// </summary>
		/// <param name="s">Input string</param>
		/// <param name="cc">Read header</param>
		public static void SkipSpaces(string s, ref int cc)
		{
			if ((cc < 0) || (cc >= s.Length))
				return;
			while ((cc < s.Length) && IsSpace(s[cc])) ++cc;
		}

		/// <summary>
		/// Extracts the first command parameters found inside a string
		/// </summary>
		/// <param name="s">String from which the command parameters will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="parameters">When this method returns contains the first command parameters found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid command parameters was found in s starting at cc, false otherwise</returns>
		public static bool XtractCommandParams(string s, ref int cc, out string parameters)
		{
			int bcc = cc + 1;
			int length;
			parameters = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (s[cc] != '"') return false;
			++cc;
			while ((cc < s.Length) && (s[cc] != '"') && (s[cc] != 0) && (s[cc] < 128))
			{
				if (s[cc] == '\\')
					++cc;
				++cc;
			}
			length = Math.Min(cc - bcc, s.Length - bcc);
			if ((cc >= s.Length) || (s[cc] != '"')) return false;
			++cc;
			if (length < 0)
				return false;
			parameters = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first @id found inside a string
		/// </summary>
		/// <param name="s">String from which the @id will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="id">When this method returns contains the id found in s if the extraction succeded, or -1 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid id was found in s starting at cc, false otherwise</returns>
		public static bool XtractId(string s, ref int cc, out int id)
		{
			int bcc = cc + 1;
			int length;
			string sid;
			id = -1;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (s[cc] != '@') return false;
			++cc;

			while ((cc < s.Length) && IsNumeric(s[cc])) ++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			sid = s.Substring(bcc, length);
			return Int32.TryParse(sid, out id);
		}
		
		/// <summary>
		/// Extracts the first C-type identifier found inside a string
		/// </summary>
		/// <param name="s">String from which the identifier will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="commandName">When this method returns contains the first C-type identifier found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid C-type identifier was found in s starting at cc, false otherwise</returns>
		public static bool XtractIdentifier(string s, ref int cc, out string commandName)
		{
			int bcc = cc;
			int length;
			commandName = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!IsAlpha(s[cc]) && (s[cc] != '_')) return false;
			++cc;
			while ((cc < s.Length) && (IsAlNum(s[cc]) || (s[cc] == '_')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			commandName = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first result (1 or 0) found inside a string
		/// </summary>
		/// <param name="s">String from which the result will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="result">When this method returns contains the result found in s if the extraction succeded, or -1 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid result was found in s starting at cc, false otherwise</returns>
		public static bool XtractResult(string s, ref int cc, out int result)
		{
			int ncc = cc + 1;
			result = -1;

			if ((cc < 1) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (
				IsSpace(s[cc - 1]) && 
				((s[cc] == '1') || (s[cc] == '0')) &&
				((ncc == s.Length) || (s[ncc] == 0) || IsSpace(s[ncc]))
				)
			{
				result = s[cc] - '0';
				return true;
			}
			return false;
		}

		/// <summary>
		/// Extracts an unsigned 32 bit integer from the input string
		/// </summary>
		/// <param name="input">String from which the integer will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="value">When this method returns contains the id found in s if the extraction succeded, or 0 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid id was found in s starting at cc, false otherwise</returns>
		public static bool XtractUInt32(string input, ref int cc, out uint value)
		{
			int bcc = cc;
			int length;
			string sValue;

			value = 0;
			while ((cc < input.Length) && IsNumeric(input[cc]))
				++cc;
			length = Math.Min(cc - bcc, input.Length - bcc);
			if ((length < 1) || (length > 10))
				return false;
			sValue = input.Substring(bcc, length);
			return UInt32.TryParse(sValue, out value);
		}

		/// <summary>
		/// Extracts an 32 bit integer from the input string
		/// </summary>
		/// <param name="input">String from which the integer will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="value">When this method returns contains the id found in s if the extraction succeded, or 0 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid id was found in s starting at cc, false otherwise</returns>
		public static bool XtractInt32(string input, ref int cc, out int value)
		{
			int bcc = cc;
			int length;
			string sValue;

			value = 0;
			while ((cc < input.Length) && (input[cc] == '-'))
				++cc;
			while ((cc < input.Length) && IsNumeric(input[cc]))
				++cc;
			length = Math.Min(cc - bcc, input.Length - bcc);
			if ((length < 1) || (length > 10))
				return false;
			sValue = input.Substring(bcc, length);
			return Int32.TryParse(sValue, out value);
		}

		/// <summary>
		/// Indicates whether a ANSI character is letter or digit.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is a letter or digit; otherwise, false.</returns>
		public static bool IsAlNum(char c)
		{
			return ((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is letter.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is a letter; otherwise, false.</returns>
		public static bool IsAlpha(char c)
		{
			return ((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is lower case letter.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is an lower case letter; otherwise, false.</returns>
		public static bool IsLAlpha(char c)
		{
			return ((c >= 'a') && (c <= 'z'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is lower case letter or digit.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is an lower case letter or digit; otherwise, false.</returns>
		public static bool IsLAlNum(char c)
		{
			return ((c >= '0') && (c <= '9')) || ((c >= 'a') && (c <= 'z'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is digit.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is a digit; otherwise, false.</returns>
		public static bool IsNumeric(char c)
		{
			return ((c >= '0') && (c <= '9'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is \f, \n, \r, \t, \v, or space
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is a space character; otherwise, false.</returns>
		public static bool IsSpace(char c)
		{
			switch (c)
			{
				case ' ':
				case '\f':
				case '\n':
				case '\r':
				case '\t':
				case '\v':
					return true;

				default:
					return false;

			}
		}

		/// <summary>
		/// Indicates whether a ANSI character is upper case letter.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is an upper case letter; otherwise, false.</returns>
		public static bool IsUAlpha(char c)
		{
			return ((c >= 'A') && (c <= 'Z'));
		}

		/// <summary>
		/// Indicates whether a ANSI character is upper case letter or digit.
		/// </summary>
		/// <param name="c">A ASNI character</param>
		/// <returns>true if c is an upper case letter or digit; otherwise, false.</returns>
		public static bool IsUAlNum(char c)
		{
			return ((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'Z'));
		}

		#endregion
	}
}
