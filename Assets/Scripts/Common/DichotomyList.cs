using System;
using System.Collections;

namespace MyCompany
{
	public class DichotomyList<T> : IEnumerable where T : DichotomyListNode<T>
	{
		public DichotomyList (T node)
		{
			head = tail = node;
		}

		/// <summary>
		/// 头节点
		/// </summary>
		public T head;

		/// <summary>
		/// 尾节点
		/// </summary>
		public T tail;

		protected bool dirty = false;

		protected int _count;
		public int Count
		{
			private set
			{
				if (value < 0)
				{
					dirty = true;
					_count = Count;
				}
				_count = value;
			}
			get
			{
				if (dirty)
				{
					_count = 0;
					if (head != null)
					{
						_count++;
						T p = head;
						while (p.next != null)
						{
							p = p.next;
							_count++;
						}
					}
					dirty = false;
				}
				return _count;
			}
		}

		#region Public methods

		public void MarkDirty ()
		{
			dirty = true;

			tail = head;
			if (head != null)
			{
				while (tail.next != null)
					tail = tail.next;
			}
		}

		/// <summary>
		/// 刷新tail指针及Count
		/// </summary>
		public void Refresh ()
		{
			T p = head;
			int count = 0;
			if (p != null)
			{
				count = 1;
				while (p.next != null)
				{
					p = p.next;
					count++;
				}
			}
			tail = p;
			dirty = false;
		}

		public T AppendLeft (T node, bool nextLeft = false)
		{
			if (head == null)
			{
				head = tail = node;
			}
			else
			{
				tail.next0 = node;
				if (nextLeft)
					tail.NextLeft ();
				node.prev = tail;
				tail = node;
				if (node.next != null)
					MarkDirty ();
				else
					Count++;
			}
			return node;
		}

		public T AppendRight (T node, bool nextRight = false)
		{
			if (head == null)
			{
				head = tail = node;
			}
			else
			{
				tail.next1 = node;
				if (nextRight)
					tail.NextRight ();
				node.prev = tail;
				tail = node;
				if (node.next != null)
					MarkDirty ();
				else
					Count++;
			}
			return node;
		}

		/// <summary>
		/// 将另一个列表追加在末尾左侧，与Connect操作不同的是Append会改变tail指针
		/// </summary>
		/// <param name="otherList">Other list.</param>
		public void AppendListLeft (DichotomyList<T> otherList)
		{
			ConnectListLeft (otherList);
			tail = otherList.tail;
			MarkDirty ();
		}

		/// <summary>
		/// 将另一个列表追加在末尾右侧，与Connect操作不同的是Append会改变tail指针
		/// </summary>
		/// <param name="otherList">Other list.</param>
		public void AppendListRight (DichotomyList<T> otherList)
		{
			ConnectListRight (otherList);
			tail = otherList.tail;
			MarkDirty ();
		}

		/// <summary>
		/// 连接另一个列表至末尾左侧，该操作不会改变tail指针
		/// </summary>
		/// <param name="otherList">Other list.</param>
		public void ConnectListLeft (DichotomyList<T> otherList)
		{
			if (head == null)
				head = otherList.head;

			if (tail != null)
			{
				otherList.head.prev = tail;
				tail.next0 = otherList.head;
				tail.NextLeft ();
			}
		}

		/// <summary>
		/// 连接另一个列表至末尾右侧，该操作不会改变tail指针
		/// </summary>
		/// <param name="otherList">Other list.</param>
		public void ConnectListRight (DichotomyList<T> otherList)
		{
			if (head == null)
				head = otherList.head;

			if (tail != null)
			{
				otherList.head.prev = tail;
				tail.next1 = otherList.head;
				tail.NextRight ();
			}
		}

		public void Clear ()
		{
			head = tail = null;
			_count = 0;
		}

		#endregion

		#region Interface implementation

		public IEnumerator GetEnumerator ()
		{
			T p = head;
			if (p == null)
			{
				yield return null;
				yield break;
			}
			while (p != null)
			{
				yield return p;
				p = p.next;
			}
		}

		#endregion
	}
}

