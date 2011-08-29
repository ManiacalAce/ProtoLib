using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProtoUI.General
{
	/// <summary>
	/// Description of TransitionPath.
	/// </summary>
	public class TransitionPath
	{
		public TransitionPath(LibraryScreens from, LibraryScreens to)
		{
			From = from;
			To = to;
		}
		
		public LibraryScreens From
		{
			get;
			private set;
		}
		
		public LibraryScreens To
		{
			get;
			private set;
		}
	}
}
