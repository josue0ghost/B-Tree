using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Util;
using BTree.Objecto;

namespace BTree
{
	public class Node<T> where T : IComparable, IFixedSizeText
	{
		//Listado de elementos (nodos)
		//Listado de hijos (nodos)
		public List<T> Valores { get; set; }
		public List<int> Hijos { get; set; }
		public int Padre { get; set; }
		public int Posicion { get; set; }
		public int Orden { get; set; }

		/*
		* 25 = Posición + | + Padre + | + | + \r\n
		* ((Objeto.FixedSize * (Orden - 1)) + 1) = (FixedSize del Objeto * Cantidad de valores posibles) + ~
		* ((10 * Orden) + (Orden - 1)) = (Hijos * Cantidad de hijos posibles) + (Cantidad de ~ posibles)
		*/

		public int FixedSize { get { return 25 + ((Objeto.FixedSize * (Orden - 1)) + 1) + ((10 * Orden) + (Orden - 1)); } }

		#region Formato
		public string HijosFormat(int Orden)
		{
			string hijos = null;
			for (int i = 0; i < Orden; i++)
			{
				hijos = hijos + $"~{Hijos[i].ToString("0000000000;-0000000000")}";
			}
			return hijos;
		}

		public string ValoresFormat(int Orden)
		{
			string valores = null;
			for (int i = 0; i < Orden - 1; i++)
			{
				valores = valores + $"~{Valores[i].ToFixedSizeString()}";
			}
			return valores;
		}

		public string ToFixedSizeString(int Orden)
		{
			string Hijos = HijosFormat(Orden);
			string Valores = ValoresFormat(Orden);
			return $"{Posicion.ToString("0000000000;-0000000000")}|{Padre.ToString("0000000000;-0000000000")}|"
				+ Valores + "|" + Hijos + "\r\n";
		}
		#endregion

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}
	}
}
