using System;
using System.Collections;
using Heli;


namespace NetLayer
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class SynchFactory
	{
		static private SynchFactory _Instance = null;
		protected ArrayList objects = null; // objects


		static public bool CreateSynchFactory()
		{
			if (_Instance==null)
			{
				_Instance=new SynchFactory();
				if (_Instance!=null)				
					return true;
			}
			return false;
		}

		static public SynchFactory Instance
		{
			get {return _Instance;}
		}

		private SynchFactory()
		{
			objects = new ArrayList(); 
			if ( objects == null )
				throw new NullReferenceException("SynchFacory failure");
		}

		public void InsertObject(SynchObject obj)
		{
			if ( obj != null )
				objects.Add(obj);
			else
				throw new NullReferenceException("InsertObject fail");
		}

		public void Interpolate(float elapsedtime)
		{
			foreach( SynchObject obj in objects ) 
			{
				obj.Interpolate(elapsedtime);
			}
		}
		public void RemoveObject(SynchObject obj)
		{
			objects.Remove(obj);
		}
	}
}
