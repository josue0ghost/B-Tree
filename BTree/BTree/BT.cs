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
	public class BT<T> where T : IComparable, IFixedSizeText
	{
		internal int Order { get; set; }
		internal int Root { get; set; }
		internal string Path { get; set; }
		internal int LastPosition { get; set; }
		internal FileStream File { get; set; }
		private ICreateFixedSizeText<T> createFixedSizeText = null;

		/// <summary>
		/// To create a new file. Creates a Header and the Root node
		/// </summary>
		/// <param name="Order"></param>
		/// <param name="Path"></param>
		private BT(int Order, string Path)
		{
			Header e = CreateHeader(Order);
			Node<T> root = CreateNode(Order);

			using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
			{
				fs.Write(ByteGenerator.ConvertToBytes(e.ToFixedSizeString()), 0, e.FixedSizeText);
				fs.Write(ByteGenerator.ConvertToBytes(root.ToFixedSizeString()), 0, root.FixedSizeText);
			}
		}

		/// <summary>
		/// To read an existing file. Reads the Header
		/// </summary>
		/// <param name="Order"></param>
		/// <param name="Path"></param>
		/// <param name="createFixedSizeText"></param>
		public BT(int Order, string Path, ICreateFixedSizeText<T> createFixedSizeText)
		{
			this.Order = Order;
			this.Path = Path;
			this.createFixedSizeText = createFixedSizeText;

			var buffer = new byte[Header.FixedSize];
			using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
			{
				fs.Seek(0, SeekOrigin.Begin);
				fs.Read(buffer, 0, Header.FixedSize);
			}

			var HeaderString = ByteGenerator.ConvertToString(buffer);
			var values = HeaderString.Split(Utilities.Separator);

			this.Root = Convert.ToInt16(values[0]);
			this.Order = Convert.ToInt16(values[1]);
			this.LastPosition = Convert.ToInt16(values[2]);


		}

		#region Iniciadores
		private void WriteHeader()
		{
			Header header = new Header
			{
				Root = this.Root,
				NextPosition = this.LastPosition,
				Order = this.Order
			};

			using (var fs = new FileStream(this.Path, FileMode.OpenOrCreate))
			{
				fs.Seek(0, SeekOrigin.Begin);
				fs.Write(ByteGenerator.ConvertToBytes(header.ToFixedSizeString()), 0, header.FixedSizeText);
			}
		}

		private Header CreateHeader(int Order)
		{
			Header e = new Header
			{
				Order = Order,
				Root = 1,
				NextPosition = 2
			};
			return e;
		}

		private Node<T> CreateNode(int Order)
		{
			Node<T> node = new Node<T>
			{
				Order = Order,
				Father = int.MinValue, // es la raiz actual
				Position = 1
			};

			node.Data = new List<T>();
			node.Children = new List<int>();

			for (int i = 0; i < Order; i++)
			{
				node.Children.Add(int.MinValue);
			}

			for (int i = 0; i < Order - 1; i++)
			{
				node.Data.Add(createFixedSizeText.CreateNull());
			}

			this.Root = node.Position;
			this.LastPosition++;

			return node;
		}
		#endregion

		#region insert

		public void Add(T data)
		{
			if (data == null)
			{
				throw new ArgumentException("El valor es nulo");
			}

			Insert(this.Root, data);
		}

		private void Insert(int PosicionActual, T data)
		{
			Node<T> node = new Node<T>();

			node = node.ReadNode(this.Path, this.Order, this.Root, PosicionActual, this.createFixedSizeText);

			if (node.PositionInNode(data) != -1)
				throw new ArgumentException("El dato ya está incluido en el nodo");

			if (node.IsLeaf)
			{
				UpData(node, data, Utilities.NullPointer);
				WriteHeader();
			}
			else
			{
				Insert(node.Children[node.PositionInNode(data)], data);
			}
		}

		private void UpData(Node<T> node, T data, int Right)
		{
			if (!node.Full)
			{
				node.InsertData(data);
				node.WriteNodeOnDisk(this.Path);
				return;
			}

			Node<T> NewNode = new Node<T>(this.Order, this.LastPosition, node.Father, createFixedSizeText);
			this.LastPosition++;

			T ToUpdata = createFixedSizeText.CreateNull();

			node.SplitNode(data, Right, NewNode, ToUpdata, createFixedSizeText);

			Node<T> NodeChildern = null;
			for (int i = 0; i < NewNode.Children.Count; i++)
			{
				if (NewNode.Children[i] != Utilities.NullPointer)
				{
					NodeChildern = NodeChildern.ReadNode(this.Path, this.Order, this.Root, NewNode.Children[i], createFixedSizeText);
					NodeChildern.Father = NewNode.Position;
					NodeChildern.WriteNodeOnDisk(Path);
				}
				else
				{
					break;
				}
			}

			if (node.Father == Utilities.NullPointer)
			{
				Node<T> newRoot = new Node<T>(this.Order, this.LastPosition, Utilities.NullPointer, createFixedSizeText);
				this.LastPosition++;

				newRoot.Children[0] = node.Position;
				newRoot.InsertData(data, NewNode.Position);

				node.Father = newRoot.Position;
				newRoot.Father = newRoot.Position;
				this.Root = newRoot.Position;

				newRoot.WriteNodeOnDisk(this.Path);
				node.WriteNodeOnDisk(this.Path);
				NewNode.WriteNodeOnDisk(this.Path);
			}
			else
			{
				node.WriteNodeOnDisk(this.Path);
				NewNode.WriteNodeOnDisk(this.Path);

				Node<T> Father = new Node<T>();
				Father.ReadNode(this.Path, this.Order, this.Root, node.Father, createFixedSizeText);
				UpData(Father, data, NewNode.Position);
			}
		}

		#endregion

		public void Delete()
		{

		}

		private Node<T> Obtain(int ActualPosition, out int position, T data)
		{
			Node<T> nActual = new Node<T>();
			nActual.ReadNode(this.Path, this.Order, this.Root, ActualPosition, this.createFixedSizeText);
			position = nActual.PositionInNode(data);

			if (position != -1)
			{
				return nActual;
			}
			else
			{
				if (nActual.IsLeaf)
				{
					return null;
				}
				else
				{
					int AproxPosition = nActual.AproxPosition(data);
					return Obtain(nActual.Children[AproxPosition], out position, data);
				}
			}
		}

		public T Obtain(T data)
		{
			int position = -1;
			Node<T> nObtained = Obtain(this.Root, out position, data);

			if (nObtained == null)
			{
				throw new ArgumentException("El dato no está en el árbol");
			}
			else
			{
				return nObtained.Data[position];
			}
		}

		public bool Contains(T data)
		{
			int position = -1;
			Node<T> nObtained = Obtain(this.Root, out position, data);

			if (nObtained != null)
			{
				return true;
			}

			return false;
		}

		#region Orders

		private void WriteNode(Node<T> node, StringBuilder text)
		{
			Obj.Object obj = new Obj.Object();

			for (int i = 0; i < node.Data.Count; i++)
			{
				if (node.Data[i].ToFixedSizeString() != obj.ToFixedSizeString()) // if not null
				{
					text.Append(node.Data[i].ToString());
					text.Append("---");
				}
				else
				{
					return;
				}
			}
		}

		private void PreOrder(int ActualPosition, StringBuilder text)
		{
			if (ActualPosition == Utilities.NullPointer)
			{
				return;
			}
			Node<T> node = new Node<T>();
			node.ReadNode(this.Path, this.Order, this.Root, ActualPosition, this.createFixedSizeText);

			WriteNode(node, text);

			for (int i = 0; i < node.Children.Count; i++)
			{
				PreOrder(node.Children[i], text);
			}
		}

		public string PrintPreOrder()
		{
			StringBuilder text = new StringBuilder();
			PreOrder(this.Root, text);
			return text.ToString();
		}

		private void InOrder(int ActualPosition, StringBuilder text)
		{
			Obj.Object obj = new Obj.Object();

			if (ActualPosition == Utilities.NullPointer)
			{
				return;
			}
			Node<T> node = new Node<T>();

			node.ReadNode(this.Path, this.Order, this.Root, ActualPosition, this.createFixedSizeText);

			for (int i = 0; i < node.Data.Count; i++)
			{
				InOrder(node.Children[i], text);
				if ((i < node.Data.Count) && (node.Data[i].ToFixedSizeString() != obj.ToFixedSizeString()))
				{
					text.AppendLine(node.Data[i].ToString());
					text.AppendLine("---");
				}
			}
		}

		public string PrintInOrder()
		{
			StringBuilder text = new StringBuilder();
			InOrder(this.Root, text);
			return text.ToString();
		}

		private void PostOrder(int ActualPosition, StringBuilder text)
		{
			if (ActualPosition == Utilities.NullPointer)
			{
				return;
			}
			Node<T> node = new Node<T>();
			node.ReadNode(this.Path, this.Order, this.Root, ActualPosition, this.createFixedSizeText);
			for (int i = 0; i < node.Children.Count; i++)
			{
				PostOrder(node.Children[i], text);
			}
			WriteNode(node, text);
		}

		public string PrintPostOrder()
		{
			StringBuilder text = new StringBuilder();
			PostOrder(this.Root, text);
			return text.ToString();
		}
		#endregion
	}
}