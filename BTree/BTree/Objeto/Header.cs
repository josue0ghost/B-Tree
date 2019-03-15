using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Util;

namespace BTree.Obj
{
	public class Header
	{
		public int Root { get; set; }
		public int NextPosition { get; set; }
		public int Order { get; set; }

		public static int FixedSize { get { return 34; } }
		// Raiz + Orden + SiguientePosicion + \r\n

		public string ToFixedSizeString()
		{
			return $"{Root.ToString("0000000000;-000000000")}" + Utilities.Separator.ToString()
				+ $"{Order.ToString("0000000000;-000000000")}" + Utilities.Separator.ToString()
				+ $"{NextPosition.ToString("0000000000;-000000000")}\r\n";
		}

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}
	}
}
