using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Util;
using BTree.Obj;
using System.IO;

namespace BTree
{
	internal class Node<T> where T: IComparable, IFixedSizeText
	{
		//Listado de elementos (nodos)
		//Listado de hijos (nodos)
		internal List<T> Datos { get; set; }
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
				TamañoEnTexto += (Datos[0].FixedSizeText + 1) * (Orden - 1); // Datos
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

		internal Node(int orden, int posicion, int padre, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (orden < 0)
			{
				throw new ArgumentOutOfRangeException("Orden inválido");
			}
			this.Orden = orden;
			this.Posicion = posicion;
			this.Padre = padre;

			LimpiarNodo(createFixedSizeText);
		}

		private void LimpiarNodo(ICreateFixedSizeText<T> createFixedSizeText)
		{
			Hijos = new List<int>();
			for (int i = 0; i < Orden; i++)
			{
				Hijos.Add(Utilidades.ApuntadorVacío);
			}

			Datos = new List<T>();
			for (int i = 0; i < Orden - 1; i++)
			{
				Datos.Add(createFixedSizeText.CrearNulo());
			}
		}

		#region Formato
		private string HijosFormat(int Orden)
		{	
			string hijos = "";
			for (int i = 0; i < Orden; i++)
			{
				hijos = hijos + $"{Hijos[i].ToString("0000000000;-000000000")}|"; // 10 caracteres + 1
			}
			return hijos;
		}

		private string DatosFormat(int Orden)
		{
			string valores = null;
			for (int i = 0; i < Orden - 1; i++)
			{
				valores = valores + $"{Datos[i].ToFixedSizeString()}|"; // FixedSize del T + 1
			}
			return valores;
		}

		public string ToFixedSizeString(int Orden)
		{
			string Valores = DatosFormat(Orden);
			string Hijos = HijosFormat(Orden);
			return $"{Posicion.ToString("0000000000;-000000000")}|{Padre.ToString("0000000000;-000000000")}|"
				+ Valores + Hijos + "\r\n";
		}
		#endregion

		#region Lectura y escritura
		internal static Node<T> LeerNodo(int Orden, int Raiz, int Posicion, ICreateFixedSizeText<T> createFixedSizeText)
		{
			Node<T> nodo = new Node<T>(Orden, Posicion, 0, createFixedSizeText);
			nodo.Datos = new List<T>();

			int TamañoEncabezado = Encabezado.FixedSize;

			var buffer = new byte[nodo.FixedSize];
			using (var fs = new FileStream("C:\\Users\\llaaj\\Desktop\\test.txt", FileMode.OpenOrCreate))
			{
				fs.Seek((TamañoEncabezado + ((Raiz - 1) * nodo.FixedSize)), SeekOrigin.Begin);
				fs.Read(buffer, 0, nodo.FixedSize);
			}

			var NodoString = ByteGenerator.ConvertToString(buffer);
			var Valores = NodoString.Split('|');

			nodo.Padre = Convert.ToInt32(Valores[1]);

			//Hijos
			for (int i = 2; i < nodo.Hijos.Count + 2; i++)
			{
				nodo.Hijos[i] = Convert.ToInt32(Valores[i]);
			}

			int LimDatos = nodo.Hijos.Count + 2;
			//Valores
			for (int i = LimDatos; i < nodo.Datos.Count; i++)
			{
				nodo.Datos[i] = createFixedSizeText.Crear(Valores[i]);
			}

			return nodo;
		}

		internal void GuardarNodo_Disco()
		{
			using (var fs = new FileStream("C:\\Users\\llaaj\\Desktop\\test.txt", FileMode.Open))
			{
				fs.Seek(CalcularPosicion(), SeekOrigin.Begin);
				fs.Write(ByteGenerator.ConvertToBytes(ToFixedSizeString(Orden)), 0, FixedSizeText);
			}
		}

		internal void LimpiarNodo_Disco(ICreateFixedSizeText<T> createFixedSizeText)
		{
			LimpiarNodo(createFixedSizeText);

			GuardarNodo_Disco();
		}
		#endregion

		internal int PosicionAproximadaEnNodo(T dato)
		{
			int posicion = Datos.Count;
			for (int i = 0; i < Datos.Count; i++)
			{
				if ((Datos[i].CompareTo(dato) < 0) || (Datos[i].CompareTo(Utilidades.ApuntadorVacío) == 0))
				{
					posicion = i; break;
				}
			}
			return posicion;
		}

		internal int PosicionEnNodo(T dato)
		{
			int posicion = -1;
			for (int i = 0; i < Datos.Count; i++)
			{
				if (dato.CompareTo(Datos[i]) == 0)
				{
					posicion = i;
					break;
				}
			}
			return posicion;
		}

		#region Agregar datos en nodo
		internal void AgregarDato(T dato, int HijoDer)
		{
			AgregarDato(dato, HijoDer, true);
		}

		internal void AgregarDato(T dato, int HijoDer, bool ValidarLleno)
		{
			if (Lleno && ValidarLleno)
			{
				throw new ArgumentOutOfRangeException("El nodo está lleno");
			}
			if (dato.CompareTo(Utilidades.ApuntadorVacío) == 0)
			{
				throw new ArgumentNullException("Dato con valor asignado igual al valor nulo predeterminado");
			}

			int PosicionAInsertar = PosicionAproximadaEnNodo(dato);

			// Correr hijos
			for (int i = Hijos.Count - 1; i > PosicionAInsertar + 1; i--)
			{
				Hijos[i] = Hijos[i - 1];
			}
			Hijos[PosicionAInsertar + 1] = HijoDer;

			// Correr datos
			for (int i = Datos.Count - 1; i > PosicionAInsertar; i--)
			{
				Datos[i] = Datos[i - 1];
			}
			Datos[PosicionAInsertar] = dato;
		}

		internal void AgregarDato(T dato)
		{
			AgregarDato(dato, Utilidades.ApuntadorVacío);
		}
		#endregion

		internal int CantidadDatos
		{
			get
			{
				int i = 0;
				while (i < Datos.Count && Datos[i] != null)
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