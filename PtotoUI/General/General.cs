using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace ProtoUI
{
	public enum LibraryScreens
	{
		HOME,
		SEARCH,
		MANIPULATE_RECORDS,
		MANIPULATE_BOOKDETAILS,
		MANIPULATE_AUTHORS,
		MANIPULATE_PUBLISHERS,
		MANIPULATE_MEMBERS,
		MANIPULATE_STAFF_ACCOUNTS,
		TRANSACTIONS_ENTERID,
		TRANSACTIONS,
		SETTINGS
	}
	
	public enum TransitionStyles
	{
		DIRECT = 1,
		FADE_SCALE_INWARDS,
		FADE_SCALE_OUTWARDS,		
	}
}
