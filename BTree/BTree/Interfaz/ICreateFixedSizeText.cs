using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree.Interfaz
{
	public interface ICreateFixedSizeText<T> where T: IFixedSizeText
	{
		T Create(string FixedSizeText);
		T CreateNull();
	}
}
