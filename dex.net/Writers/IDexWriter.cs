using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace dex.net
{
	public interface IDexWriter
	{
		Dex dex { get; set; }

		void WriteOutMethod (Class dexClass, Method method, TextWriter output, Indentation indent, bool renderOpcodes=false);
		void WriteOutClass (Class dexClass, ClassDisplayOptions options, TextWriter output);
		string GetName ();
		string GetExtension ();
		List<HightlightInfo> GetCodeHightlight ();
	}

	public class HightlightInfo
	{
		public Regex Expression { get; private set; }
		public ColorRGB Color { get; private set; }

		public HightlightInfo(string expr, uint red, uint green, uint blue)
		{
			Expression = new Regex (expr, RegexOptions.Multiline | RegexOptions.ECMAScript);
			Color = new ColorRGB (red, green, blue);
		}
	}

	public class ColorRGB
	{
		public uint Red { get; private set; }
		public uint Green { get; private set; }
		public uint Blue { get; private set; }

		public ColorRGB(uint red, uint green, uint blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
		}
	}
}