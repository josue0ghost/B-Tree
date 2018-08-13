using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Objecto;
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
	public class BT_G3<T> where T: IComparable, IFixedSizeText
	{
		public int Orden { get; set; }
		public int Raiz { get; set; }

		public void IniciarArbol(T objeto)
		{
			Encabezado e = new Encabezado
			{
				Orden = this.Orden,
				Raiz = 1,
				SiguientePosicion = 1
			};

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
				node.Valores.Add(objeto);
			}
		}
	}
}
