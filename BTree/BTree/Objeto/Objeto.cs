using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;

namespace BTree.Obj
{
	public class Objeto: IComparable, IFixedSizeText
	{
		public int Id { get; set; }

		public int CompareTo(object obj)
		{
			var s2 = (Objeto)obj;
			return Id.CompareTo(s2.Id);
		}

		public static int FixedSize { get { return 10; } }

		public string ToFixedSizeString()
		{
			return $"{Id.ToString("0000000000;-000000000")}";
		}

		//public string ToFixedSizeString()
		//{
		//	return $"{Id.ToString("00000000000;-0000000000")}~{string.Format("{0,-20}", Nombre)}~" +
		//		   $"{string.Format("{0,-20}", Year)}~{string.Format("{0,-20}", Genero)}~" +
		//		   $"{string.Format("{0,-20}", Tipo)}";
		//}

		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}

		//public override string ToString()
		//{
		//    return string.Format("ID: {0}\r\nNombre: {1}\r\nAño: {2}\r\nGénero: {3}\r\nTipo: {4}\r\n"
		//        , Id.ToString("00000000000;-0000000000"), string.Format("{0,-20}", Nombre)
		//        , string.Format("{0,-20}", Year), string.Format("{0,-20}", Genero), string.Format("{0,-50}", Tipo));
		//}
	}
}
