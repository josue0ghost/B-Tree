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
		internal List<T> Data { get; set; }
		internal List<int> Children { get; set; }
		internal int Father { get; set; }
		internal int Position { get; set; }
		internal int Order { get; set; }

		public Node()
		{

		}

		internal int FixedSize
		{
			get
			{
				int InTextSize = 0;

				InTextSize += Utilities.IntegerSize + 1; // Posición
				InTextSize += Utilities.IntegerSize + 1; // Padre
				InTextSize += (Utilities.IntegerSize + 1) * Order; // Hijos
				InTextSize += (Data[0].FixedSizeText + 1) * (Order - 1); // Datos
				InTextSize += 2; // \r\n

				return InTextSize;
			}
		}

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}

		private int SeekPosition()
		{
			return Header.FixedSize + (Position * FixedSizeText);
		}

		internal Node(int order, int position, int father, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException("Orden inválido");
			}
			this.Order = order;
			this.Position = position;
			this.Father = father;

			ClearNode(createFixedSizeText);
		}

		private void ClearNode(ICreateFixedSizeText<T> createFixedSizeText)
		{
			Children = new List<int>();
			for (int i = 0; i < Order; i++)
			{
				Children.Add(Utilities.NullPointer);
			}

			Data = new List<T>();
			for (int i = 0; i < Order - 1; i++)
			{
				Data.Add(createFixedSizeText.CreateNull());
			}
		}

		#region Format
		private string ChildrensFormat(int Order)
		{	
			string Children = "";
			for (int i = 0; i < Order; i++)
			{
				Children = Children + $"{this.Children[i].ToString("0000000000;-000000000")}"+Utilities.Separator.ToString(); // 10 caracteres + 1
			}
			return Children;
		}

		private string DataFormat(int Order)
		{
			string values = null;
			for (int i = 0; i < Order - 1; i++)
			{
				values = values + $"{Data[i].ToFixedSizeString()}"+Utilities.Separator.ToString(); // FixedSize del T + 1
			}
			return values;
		}

		public string ToFixedSizeString()
		{
			string values = DataFormat(this.Order);
			string childrens = ChildrensFormat(this.Order);
			return $"{Position.ToString("0000000000;-000000000")}" + Utilities.Separator.ToString() + $"{Father.ToString("0000000000;-000000000")}" + Utilities.Separator.ToString()
				+ values + childrens + "\r\n";
		}
		#endregion

		#region Read n' Write
		internal Node<T> ReadNode(string Path, int Order, int Root, int Position, ICreateFixedSizeText<T> createFixedSizeText)
		{
			Node<T> node = new Node<T>(Order, Position, 0, createFixedSizeText);
			node.Data = new List<T>();

			int HeaderSize = Header.FixedSize;

			var buffer = new byte[node.FixedSize];
			using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
			{
				fs.Seek((HeaderSize + ((Root - 1) * node.FixedSize)), SeekOrigin.Begin);
				fs.Read(buffer, 0, node.FixedSize);
			}

			var NodeString = ByteGenerator.ConvertToString(buffer);
			var Values = NodeString.Split(Utilities.Separator);

			node.Father = Convert.ToInt32(Values[1]);

			//Hijos
			for (int i = 2; i < node.Children.Count + 2; i++)
			{
				node.Children[i] = Convert.ToInt32(Values[i]);
			}

			int DataLimit = node.Children.Count + 2;
			//Valores
			for (int i = DataLimit; i < node.Data.Count; i++)
			{
				node.Data[i] = createFixedSizeText.Create(Values[i]);
			}

			return node;
		}

		internal void WriteNodeOnDisk(string Path)
		{
			using (var fs = new FileStream(Path, FileMode.Open))
			{
				fs.Seek(SeekPosition(), SeekOrigin.Begin);
				fs.Write(ByteGenerator.ConvertToBytes(ToFixedSizeString()), 0, FixedSizeText);
			}
		}

		internal void LimpiarNodo_Disco(string Path, ICreateFixedSizeText<T> createFixedSizeText)
		{
			ClearNode(createFixedSizeText);

			WriteNodeOnDisk(Path);
		}
		#endregion

		internal int AproxPosition(T data)
		{
			int position = Data.Count;
			for (int i = 0; i < Data.Count; i++)
			{
				if ((Data[i].CompareTo(data) < 0) || (Data[i].CompareTo(Utilities.NullPointer) == 0))
				{
					position = i; break;
				}
			}
			return position;
		}

		internal int PositionInNode(T data)
		{
			int position = -1;
			for (int i = 0; i < Data.Count; i++)
			{
				if (data.CompareTo(Data[i]) == 0)
				{
					position = i;
					break;
				}
			}
			return position;
		}

		#region Insert data

		internal void InsertData(T data, int Right)
		{
			InsertData(data, Right, true);
		}

		internal void InsertData(T data, int Right, bool ValidateIfFull)
		{
			if (Full && ValidateIfFull)
			{
				throw new ArgumentOutOfRangeException("El nodo está lleno");
			}
			if (data.CompareTo(Utilities.NullPointer) == 0)
			{
				throw new ArgumentNullException("Dato con valor asignado igual al valor nulo predeterminado");
			}

			int PositionToInsert = AproxPosition(data);

			// Correr hijos
			for (int i = Children.Count - 1; i > PositionToInsert + 1; i--)
			{
				Children[i] = Children[i - 1];
			}
			Children[PositionToInsert + 1] = Right;

			// Correr datos
			for (int i = Data.Count - 1; i > PositionToInsert; i--)
			{
				Data[i] = Data[i - 1];
			}
			Data[PositionToInsert] = data;
		}

		internal void InsertData(T data)
		{
			InsertData(data, Utilities.NullPointer);
		}
		#endregion

		#region Delete n' split
		internal void DeleteData(T data, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (!IsLeaf)
			{
				throw new Exception("Solo pueden eliminarse en nodos hoja");
			}

			int PositionToDelete = PositionInNode(data);

			if (PositionToDelete == -1)
			{
				throw new ArgumentException("El dato no existe en el nodo");
			}

			for (int i = Data.Count - 1; i > PositionToDelete; i--)
			{
				Data[i - 1] = Data[i];
			}		

			Data[Data.Count - 1] = createFixedSizeText.CreateNull();
		}

		internal void SplitNode(T data, int Right, Node<T> Node, T ToUpData, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (!Full)
			{
				throw new ArgumentException("No se puede serparar porque no está lleno");
			}

			// Incrementar en una posición
			Data.Add(data);
			Children.Add(Utilities.NullPointer);

			// Agregarlos en orden
			InsertData(data, Right, false);

			// Dato a subir
			int Middle = Order / 2;
			ToUpData = Data[Middle];
			Data[Middle] = createFixedSizeText.CreateNull();

			// Llenar datos que suben
			int j = 0;
			for (int i = Middle + 1; i < Children.Count; i++)
			{
				Node.Data[j] = Data[i];
				Data[i] = createFixedSizeText.CreateNull();
				j++;
			}

			// Llenar hijos que suben
			j = 0;
			for (int i = Middle + 1; i < Children.Count; i++)
			{
				Node.Children[j] = Children[i];
				Children[i] = Utilities.NullPointer;
				j++;
			}

			Data.RemoveAt(Data.Count - 1);
			Children.RemoveAt(Children.Count - 1);
		}
		#endregion

		internal int CountData
		{
			get
			{
				int i = 0;
				while (i < Data.Count && Data[i] != null)
				{
					i++;
				}
				return i;
			}
		}

		internal bool Underflow
		{
			get { return (CountData < (Order / 2) - 1); }				
		}

		internal bool Full
		{
			get { return (CountData >= Order - 1); }
		}

		internal bool IsLeaf
		{
			get
			{
				bool Leaf = true;
				for (int i = 0; i < Children.Count; i++)
				{
					if (Children[i] != Utilities.NullPointer)
					{
						Leaf = false;
						break;
					}
				}
				return Leaf;
			}
		}
	}
}