using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree.Obj
{
	public class Encabezado
	{
		public int Raiz { get; set; }
		public int SiguientePosicion { get; set; }
		public int Orden { get; set; }

		public static int FixedSize { get { return 34; } }
		// Raiz + Orden + SiguientePosicion + \r\n

		public string ToFixedSizeString()
		{
			return $"{Raiz.ToString("0000000000;-000000000")}|{Orden.ToString("0000000000;-000000000")}|{SiguientePosicion.ToString("0000000000;-000000000")}\r\n";
		}

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}
	}
}
