using System;

namespace dex.net
{
	public class Indentation
	{
		private ushort _level;
		private char _spacing;
		private ushort _count;

		public Indentation (ushort level=0)
		{
			_level = level;
			_count = 1;
			_spacing = '\t';
		}
		
		public Indentation (ushort level, ushort count, char spacing)
		{
			_level = level;
			_count = count;
			_spacing = spacing;
		}

		public void Increment()
		{
			_level++;
		}
		
		public void Decrement()
		{
			_level--;
		}

		public static Indentation operator ++(Indentation indent)
		{
			indent._level++;
			return indent;
		}
		
		public static Indentation operator --(Indentation indent)
		{
			indent._level--;
			return indent;
		}

		public override string ToString ()
		{
			if (_level <= 0) {
				return string.Empty;
			}

			return new string (_spacing, _level*_count);
		}
	}
}