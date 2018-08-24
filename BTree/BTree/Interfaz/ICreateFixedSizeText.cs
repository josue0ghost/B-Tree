using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree.Interfaz
{
	interface ICreateFixedSizeText<T> where T: IFixedSizeText
	{
		T Crear(string FixedSizeText);
		T CrearNulo();
	}
}
