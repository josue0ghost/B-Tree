using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Util;
using BTree.Obj;

namespace BTree
{
	internal class Node<T> where T: IComparable, IFixedSizeText
	{
		//Listado de elementos (nodos)
		//Listado de hijos (nodos)
		internal List<T> Valores { get; set; }
		internal List<int> Hijos { get; set; }
		internal int Padre { get; set; }
		internal int Posicion { get; set; }
		internal int Orden { get; set; }

		internal int FixedSize
		{
			get
			{
				int TamañoEnTexto = 0;

				TamañoEnTexto += Utilidades.TamañoEnteros + 1; // Posición
				TamañoEnTexto += Utilidades.TamañoEnteros + 1; // Padre
				TamañoEnTexto += (Utilidades.TamañoEnteros + 1) * Orden; // Hijos
				TamañoEnTexto += (Valores[0].FixedSizeText + 1) * (Orden - 1); // Valores
				TamañoEnTexto += 2; // \r\n

				return TamañoEnTexto;
			}
		}

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}

		private int CalcularPosicion()
		{
			return Encabezado.FixedSize + (Posicion * FixedSizeText);
		}

		//internal Node(int orden, int posicion, int padre)
		//{
		//	if (orden < 0)
		//	{
		//		throw new ArgumentOutOfRangeException("Orden inválido");
		//	}
		//	this.Orden = orden;
		//	this.Posicion = posicion;
		//	this.Padre = padre;

		//	//limpiar nodo
		//}

		private void LimpiarNodo()
		{
			Hijos = new List<int>();
			for (int i = 0; i < Orden; i++)
			{
				Hijos.Add(Utilidades.ApuntadorVacío);
			}

			Valores = new List<T>();
			for (int i = 0; i < Orden - 1; i++)
			{
				//Valores.Add(null);
			}
		}

		#region Formato
		public string HijosFormat(int Orden)
		{	
			string hijos = "";
			for (int i = 0; i < Orden; i++)
			{
				hijos = hijos + $"{Hijos[i].ToString("0000000000;-000000000")}|"; // 10 caracteres + 1
			}
			return hijos;
		}

		public string ValoresFormat(int Orden)
		{
			string valores = null;
			for (int i = 0; i < Orden - 1; i++)
			{
				valores = valores + $"{Valores[i].ToFixedSizeString()}|"; // FixedSize del T + 1
			}
			return valores;
		}

		public string ToFixedSizeString(int Orden)
		{
			string Valores = ValoresFormat(Orden);
			string Hijos = HijosFormat(Orden);
			return $"{Posicion.ToString("0000000000;-000000000")}|{Padre.ToString("0000000000;-000000000")}|"
				+ Valores + Hijos + "\r\n";
		}
		#endregion

		internal int CantidadDatos
		{
			get
			{
				int i = 0;
				while (i < Valores.Count && Valores[i] != null)
				{
					i++;
				}
				return i;
			}
		}

		internal bool Underflow
		{
			get { return (CantidadDatos < (Orden / 2) - 1); }				
		}

		internal bool Lleno
		{
			get { return (CantidadDatos >= Orden - 1); }
		}

		internal bool EsHoja
		{
			get
			{
				bool Hoja = true;
				for (int i = 0; i < Hijos.Count; i++)
				{
					if (Hijos[i] != Utilidades.ApuntadorVacío)
					{
						Hoja = false;
						break;
					}
				}
				return Hoja;
			}
		}
	}
}
