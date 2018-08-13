using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree.Interfaz
{
	public interface IFixedSizeText
	{
		int FixedSizeText { get; }
		string ToFixedSizeString();
	}
}
