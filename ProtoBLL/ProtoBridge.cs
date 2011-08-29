using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.EntityManagers;

namespace ProtoBLL
{
	/// <summary>
	/// Description of ProtoBridge.
	/// </summary>
	public class ProtoBridge
	{
		public ProtoBridge()
		{
			BookDetailsMgr = new BookDetailsManager();
			StaffAccountMgr = new StaffAccountManager();
			AuthorsMgr = new AuthorsManager();
			MemberMgr = new MemberManager();
			LibraryBookMgr = new LibraryBookManager();
			TransactionMgr = new TransactionManager();
		}
		
		public IBookDetailsManager BookDetailsMgr
		{
			get;
			private set;
		}
		
		public IStaffAccountManager StaffAccountMgr
		{
			get;
			private set;
		}
		
		public IAuthorsManager AuthorsMgr
		{
			get;
			private set;
		}
		
		public IMemberManager MemberMgr
		{
			get;
			private set;
		}
		
		public ILibraryBookManager LibraryBookMgr
		{
			get;
			private set;
		}
		
		public ITransactionManager TransactionMgr
		{
			get;
			private set;
		}
	}
}
