using System;
using System.Collections;

namespace UPNPAVCds
{
	/// <summary>
	/// Summary description for _ArrayList.
	/// </summary>
	public class _ArrayList
	{
		public static int Add(ref object[] objects, object obj)
		{
			int retVal = -1;
			object[] newObjects = null;

			if (objects == null)
			{
				objects = new object[1];
				objects[0] = obj;
				retVal = 0;
			}
			else
			{
				newObjects = new object[objects.Length+1];
				for (int i=0; i < objects.Length; i++)
				{
					newObjects[i] = objects[i];
				}
				newObjects[objects.Length] = obj;
				retVal = objects.Length;
				objects = newObjects;
			}
			
			return retVal;
		}

		public static void AddRange(ref object[] objects, ICollection objs)
		{
			object[] newObjects = null;

			if (objects == null)
			{
				objects = new object[objs.Count];
				int i=0;
				foreach (object o in objs)
				{
					objects[i] = o;
					i++;
				}
			}
			else
			{
				newObjects = new object[objects.Length + objs.Count];
				int i=0;
				for (i=0; i < objects.Length; i++)
				{
					newObjects[i] = objects[i];
				}
				foreach (object o in objs)
				{
					newObjects[i] = o;
					i++;
				}
				objects = newObjects;
			}
		}

		public static void Remove(ref object[] objects, object obj)
		{
			object[] newObjects = new object[objects.Length - 1];
			int removed = 0;

			for (int i=0; i < objects.Length; i++)
			{
				if ((objects[i] == obj) && (removed == 0))
				{
					removed++;
				}
				else
				{
					newObjects[i-removed] = objects[i];
				}
			}
		}

		public static void RemoveAt(ref object[] objects, int index)
		{
			object[] newObjects = new object[objects.Length - 1];
			int removed = 0;

			for (int i=0; i < objects.Length; i++)
			{
				if ((i == index) && (removed == 0))
				{
					removed++;
				}
				else
				{
					newObjects[i-removed] = objects[i];
				}
			}
		}
	}
}
