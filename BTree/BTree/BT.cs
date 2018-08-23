using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Obj;
using BTree.Util;

namespace BTree
{
	/*
	 * Cuando un nodo llega a n-1 elementos y se agrega un elemento nuevo al mismo nodo
	 * Se lleva el elemento medio (n/2) como una nueva raíz y las dos partes izquierda y derecha
	 * Se convierten en sus respectivos hijos
	 * Los elementos en los nodos siempre están ordenados
	 * 
	 * Este será un árbol de grado 3
	 */
	public class BT<T> where T: IComparable, IFixedSizeText
	{
		public int Orden { get; set; }
		public int Raiz { get; set; }

		private Encabezado CrearEncabezado()
		{
			Encabezado e = new Encabezado
			{
				Orden = this.Orden,
				Raiz = 1,
				SiguientePosicion = 2
			};
			return e;
		}
		
		private Node<T> CrearNodo()
		{
			Node<T> node = new Node<T>
			{
				Orden = this.Orden,
				Padre = int.MinValue, // es la raiz actual
				Posicion = 1
			};

			node.Valores = new List<T>();
			node.Hijos = new List<int>();

			for (int i = 0; i < Orden; i++)
			{
				node.Hijos.Add(int.MinValue);
			}

			return node;
		}

		//public void IniciarArbol(int Orden)
		//{
		//	Encabezado e = new Encabezado();
		//	Node<T> node = new Node<T>();
		//	this.Orden = Orden;

		//	e = CrearEncabezado();
		//	node = CrearNodo();			
		//	node.Orden = Orden;

		//	for (int i = 0; i < Orden - 1; i++)
		//	{
		//		node.Valores.Add(T);
		//	}
		//	for (int i = 0; i < Orden; i++)
		//	{
		//		node.Hijos.Add(int.MinValue);
		//	}

		//	using (var fs = new FileStream("C:\\Users\\llaaj\\OneDrive\\Escritorio\\test.txt", FileMode.OpenOrCreate))
		//	{
		//		fs.Write(ByteGenerator.ConvertToBytes(e.ToFixedSizeString()), 0, e.FixedSizeText);
		//		fs.Write(ByteGenerator.ConvertToBytes(node.ToFixedSizeString(Orden)), 0, node.FixedSizeText);
		//	}
		//}
	}
}
