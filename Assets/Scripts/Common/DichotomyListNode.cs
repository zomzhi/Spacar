using System;

namespace MyCompany
{
	public class DichotomyListNode<T> where T : class
	{
		public DichotomyListNode ()
		{
		}

		/// <summary>
		/// 上一个节点
		/// </summary>
		public T prev;

		/// <summary>
		/// 下一个左/0节点
		/// </summary>
		public T next0;

		/// <summary>
		/// 下一个右/1节点
		/// </summary>
		public T next1;

		private T _next;
		/// <summary>
		/// 下一个节点，必须为next0或next1中的一个
		/// </summary>
		public T next{ get { return _next; } }

		public bool IsValid ()
		{
			return (next == next0 || next == next1);
		}

		public void NextLeft ()
		{
			_next = next0;
		}

		public void NextRight ()
		{
			_next = next1;
		}

		public bool NextIsLeft ()
		{
			return (_next == next0);
		}

		public void CutNext ()
		{
			next0 = null;
			next1 = null;
			_next = next0;
		}
	}
}

