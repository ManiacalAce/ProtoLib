using System.Collections;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.BusinessEntities;
using PLEF;
using ProtoBLL.General;

namespace ProtoBLL.EntityManagers
{
	/// <summary>
	/// Description of TransactionManager.
	/// </summary>
	public class TransactionManager : ITransactionManager
	{
		public TransactionManager()
		{
			
		}
		
		
		
		public bool IssueBook(int libBookID, int memberID, out string serverSideError)
		{
			serverSideError = null;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				LibraryBook libBook = (from lb in context.LibraryBooks 
				                       where lb.BookID == libBookID
				                       select lb).SingleOrDefault();
				
				if (libBook != null)
				{
					Member mem = (from m in context.Members
					              where m.MemberID == memberID
					              select m).SingleOrDefault();
					
					if (mem != null)
					{
						Transaction t = new Transaction();
						t.LibraryBook = libBook;
						t.Member = mem;
						t.Fine = 0.0;
						t.CheckedOutOn = DateTime.Now;
						t.ReturnedOn = null;
						
						libBook.BookStatus = (from st in context.BookStatus1 where st.StatusID == 101 select st).Single();
						
						context.Transactions.AddObject(t);
						context.SaveChanges();
						return true;
					}
					else
					{
						serverSideError = string.Format("No member with ID {0} exists", memberID.ToString());
						return false;
					}
				}
				else
				{
					serverSideError = string.Format("No library book with that ID {0} exists", libBookID.ToString());
					return false;					
				}
			}
		}
		
		public bool ReturnBook(int libBookID, int memberID, int finePerDay, int borrowLimit, out string serverSideError)
		{
			serverSideError = null;
			
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				LibraryBook libBook = (from lb in context.LibraryBooks 
				                       where lb.BookID == libBookID
				                       select lb).SingleOrDefault();
				
				if (libBook != null)
				{
					Member mem = (from m in context.Members
					              where m.MemberID == memberID
					              select m).SingleOrDefault();
					
					if (mem != null)
					{
						
						Transaction trans = (from t in mem.Transactions 
						                     where t.BookID == libBookID
						                     select t).SingleOrDefault();
						
						if (trans != null)
						{
							trans.ReturnedOn = DateTime.Now;
							TimeSpan ts = (DateTime)trans.ReturnedOn - trans.CheckedOutOn;
							
							if (ts.Days > borrowLimit)
							{
								trans.Fine = ts.Days * finePerDay;
							}
							
							libBook.BookStatus = (from st in context.BookStatus1 where st.StatusID == 101 select st).Single();
							
							context.SaveChanges();
							return true;
						}
						else
						{
							serverSideError = string.Format("The library book with ID {0} is not issued to member ID {1}", libBookID.ToString(), memberID.ToString());
							return false;
						}
						
						
					}
					else
					{
						serverSideError = string.Format("No member with ID {0} exists", memberID.ToString());
						return false;
					}
				}
				else
				{
					serverSideError = string.Format("No library book with that ID {0} exists", libBookID.ToString());
					return false;					
				}
			}
		}
		
		public List<TransactionBLL> GetMemberTransactions(int memberID)
		{
			using (ProtoLibEntities context = new ProtoLibEntities())
			{
				Member mem = (from m in context.Members
				              where m.MemberID == memberID
				              select m).SingleOrDefault();
				
				if (mem != null)
				{
					List<TransactionBLL> retList = new List<TransactionBLL>();
					
					foreach (Transaction t in mem.Transactions)
					{
						TransactionBLL tr = new TransactionBLL();
						CrossLayerEntityConverter.TransactionDalToBll(context, tr, t);
						retList.Add(tr);
					}
					
					return retList;
				}
				
			}
			
			return null;
		}
	}
}
