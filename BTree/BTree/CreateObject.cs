using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTree.Interfaz;
using BTree.Obj;

namespace BTree
{
	class CreateObject : ICreateFixedSizeText<Obj.Object>
	{
		public Obj.Object Create(string FixedSizeText)
		{
			Obj.Object ob = new Obj.Object();
			ob.Id = Convert.ToInt32(FixedSizeText.Substring(0, 10));
			/*ob. = Convert.ToString(FixedSizeText.Substring(11, 20));
			ob.Campos = Convert.ToString(FixedSizeText.Substring(21, 222));
			*/
			return ob;
		}

		public Obj.Object CreateNull()
		{
			return new Obj.Object();
		}
	}
}
