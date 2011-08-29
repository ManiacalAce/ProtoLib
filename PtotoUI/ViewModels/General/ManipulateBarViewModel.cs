using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL.BusinessEntities;
using ProtoBLL.BusinessInterfaces;
using ProtoBLL.EntityManagers;
using ProtoUI.General;

namespace ProtoUI.ViewModels.General
{
	/// <summary>
	/// Description of ManipulateBarViewModel.
	/// </summary>
	public class ManipulateBarViewModel<TEntity> : ViewModelBase where TEntity : IKeyedItem, IDataErrorInfo, new()
	{
		
		
		public ManipulateBarViewModel(IEntityManagerCore<TEntity> manager, VoidFunc currItemChangedHandler)
		{
			_manager = manager;
			_dirtyItems = new Dictionary<int, TEntity>();
			
			
			_cache = _manager.GetFirstItems(DEFAULT_CACHE_SIZE);
			
			CurrentItemChanged += currItemChangedHandler;
			
			if (_cache.Count != 0)
			{
				CacheSize = _cache.Count;
				_currItemPointer = 0;
				_firstElementReached = true;
				
				//Find out first & last item IDs and store them
				_firstItemID = _cache[0].ItemID;
				_lastItemID = (_manager.GetLastItems(1))[0].ItemID;
				
			}
			
			_recentlyDeletedIds = new SortedSet<int>();
			
		}
		
		public int CacheSize
		{
			get
			{
				return _cacheSize;
			}
			set
			{
				if (value < 1)
					_cacheSize = DEFAULT_CACHE_SIZE;
				else
				{
					//Make sure there is an easy middle value. Can have one more
					//item in cache than user asked for. Poor sucker will never
					//know what hit him.
					_cacheSize = ((value % 2 == 0)?(value + 1): value);
					if (_cacheSize > _cache.Count) _cacheSize = _cache.Count;
					_halfCacheSize = _cacheSize/2;
				}
				
			}
		}
		
		public bool AnyModificationModeOn
		{
			get
			{
				return (EditModeOn || AddModeOn);
			}
		}
		
		public bool EditModeOn 
		{
			get { return _editModeOn; }
			private set 
			{
				if (_editModeOn == value)
					return;
				_editModeOn = value;
				base.OnPropertyChanged("EditModeOn");
				base.OnPropertyChanged("AnyModificationModeOn");
			}
		}
		
		public bool AddModeOn 
		{
			get { return _addModeOn; }
			private set 
			{
				if (_addModeOn == value)
					return;
				_addModeOn = value;
				base.OnPropertyChanged("AddModeOn");
				base.OnPropertyChanged("AnyModificationModeOn");
			}
		}
		
		
		public void DirtyCurrentRecord()
		{
			if (EditModeOn)
			{
				_dirtyItems[CurrentItem.ItemID] = CurrentItem;
//				MessageBox.Show("Dirtied!");
			}
		}
		
		#region Browsing records

		public delegate void VoidFunc();
		public event VoidFunc CurrentItemChanged;
		
		public TEntity CurrentItem
		{
			get
			{
				if (EditModeOn)
				{
					TEntity modifiedEntity;
					if (_dirtyItems.TryGetValue(_cache[_currItemPointer].ItemID, out modifiedEntity))
						return modifiedEntity;
				}
				else if (AddModeOn)
				{
					return _tentativeItem;
				}
				
				return _cache[_currItemPointer];
			}

		}

		#endregion //Browsing records

		//This is needed since we can't have the same text box to display
		//browsing-dependant item-IDs and user-entered item-IDs. WPF's two way
		//data binding system gets in the way.
		//Thus this prop is for user-entered IDs while browsing-dependant IDs
		//will be displayed in a another text box - bind to CurrentItem.ItemID
		public string UserEnteredRecordID
		{
			get
			{
				return _userEnteredRecID.ToString();
			}
			
			set
			{
				int temp = 0;
				if (int.TryParse(value, out temp))
				{
					if (temp == _userEnteredRecID)
						return;
					
					_userEnteredRecID = temp;
					base.OnPropertyChanged("UserEnteredRecordID");
				}
			}
		}
		

		
		#region Commands
		
		//NextCommand  and PreviousCommand make sure that they only load
		//necessary records from the server. In general, they 'reuse' half 
		//the records present in the cache. Yay efficiency.
		#region NextCommand
		public ICommand NextCommand
		{
			get
			{
				if (_nextCmd == null)
					_nextCmd = new RelayCommand(GoToNextRecord, CanGoToNextRecord);
				
				return _nextCmd;
			}
		}
		
		private void GoToNextRecord(object o)
		{
			++_currItemPointer;
			_firstElementReached = false;
			
			if (_currItemPointer < _cacheSize)
			{
				if (_recentlyDeletedIds.Contains(_cache[_currItemPointer].ItemID))
				    ++_currItemPointer;
			}
			
			
			if (_currItemPointer < _cacheSize && _cache[_currItemPointer].ItemID == _lastItemID) //check to see if we're at the last database record
			{
				_lastElementReached = true;
			}						
			else if (_currItemPointer >= _cacheSize) //check if ptr is PAST last item
			{
				
				int lastID = _cache[_cache.Count - 1].ItemID;

				
				//+1 done so as to load that extra item... the mid-item
				List<TEntity> l = _manager.GetNextItems(lastID, _halfCacheSize + 1);
				

				//Remove only enough items so that the
				//cache size ALWAYS remains the same
				//Normal case: l.Count == _halfCacheSize + 1
				//Corner case: l.Count > _halfCacheSize + 1
				_cache.RemoveRange(0, l.Count);
				_cache.AddRange(l);
				
//				_currItemPointer = _halfCacheSize;
							
				TEntity ent = (from b in _cache where b.ItemID > lastID && !_recentlyDeletedIds.Contains(b.ItemID) select b).FirstOrDefault();
				if (ent != null)
				{
					_currItemPointer = _cache.IndexOf(ent);
					if (_currItemPointer < _cacheSize && _cache[_currItemPointer].ItemID == _lastItemID)
					{
						_lastElementReached = true;
					}	
				}
				else
					LastCommand.Execute(o);
				
				_recentlyDeletedIds.RemoveWhere(id => id < _cache[0].ItemID);
				
				#region Simplified test code
				/*
				List<int> l = new List<int>(){1,2,3,4,5};
				//cache size = 5
				l.RemoveRange(0, 2 + 1);
				
				l.AddRange(new int[] {9,9,  9}); //extra 1 item
				
				foreach (int i in l)
					Console.WriteLine(i);
				*/
				#endregion
			}
			if (CurrentItemChanged != null)
				CurrentItemChanged();
			
		}
		
		private bool CanGoToNextRecord(object o)
		{
			if (_lastElementReached)
				return false;
			
			return true;
		}
		#endregion //NextCommand
		
		#region PreviousCommand
		public ICommand PreviousCommand
		{
			get
			{
				if (_prevCmd == null)
					_prevCmd = new RelayCommand(GoToPreviousRecord, CanGoToPreviousRecord);
				
				return _prevCmd;
			}
		}
		
		private void GoToPreviousRecord(object o)
		{
			_lastElementReached = false;
			--_currItemPointer;
			
			if (_currItemPointer >= 0)
			{
				if (_recentlyDeletedIds.Contains(_cache[_currItemPointer].ItemID))
					--_currItemPointer;
				if (_currItemPointer < 0)
					FirstCommand.Execute(o);
			}
			
			//check to see if we're at the first database record
			if (_currItemPointer >= 0 && _cache[_currItemPointer].ItemID == _firstItemID)
			{
				_firstElementReached = true;
//				++_currItemPointer;
			}			
			else if (_currItemPointer < 0) //check if ptr is PAST last item
			{
				
				int firstID = _cache[0].ItemID;

				
				//+1 done so as to load that extra item... the mid-item
				List<TEntity> l = _manager.GetPreviousItems(firstID, _halfCacheSize + 1);
				
				
				//Example: NORMAL CASE: l.Count == _halfCacheSize + 1
				//cache size = 5, cacheHalfSize = 2:
				//    . 0 1 2 3 4	_currItemPointer moved to
				//+ + + 0 1 2 3 4	_halfCacheSize + 1(==l.Count) items were loaded
				//+ + + 0 1 - - -	_halfCacheSize + 1 items deleted
				//    ^				_currItemPointer
				//IMPORTANT: RemoveRange's first param = element at start point also removed!
				_cache.RemoveRange(_cacheSize - l.Count, l.Count);
				
				List<TEntity> oldItems = _cache;
				_cache = new List<TEntity>(l);
				_cache.AddRange(oldItems);
				
//				_currItemPointer = _halfCacheSize;
				TEntity ent = (from b in _cache where b.ItemID < firstID && !_recentlyDeletedIds.Contains(b.ItemID) orderby b.ItemID descending select b).FirstOrDefault();
				if (ent != null)
				{
					_currItemPointer = _cache.IndexOf(ent);
					if (_currItemPointer >= 0 && _cache[_currItemPointer].ItemID == _firstItemID)
					{
						_firstElementReached = true;
					}	
				}
				else
					FirstCommand.Execute(o);
								
				
				_recentlyDeletedIds.RemoveWhere(id => id > _cache[_cacheSize - 1].ItemID);
				
			}
			
			if (CurrentItemChanged != null)
				CurrentItemChanged();
		}
		
		private bool CanGoToPreviousRecord(object o)
		{
			if (_firstElementReached)
				return false;
			
			return true;
		}
		#endregion //PreviousCommand		
		
		#region FirstCommand
		
		public ICommand FirstCommand
		{
			get
			{
				if (_firstCmd == null)
					_firstCmd = new RelayCommand(GoToFirstRecord, CanGoToFirstRecord);
				
				return _firstCmd;
			}
		}
		
		private void GoToFirstRecord(object o)
		{
			
			if (_cache[0].ItemID != _firstItemID)
				_cache = new List<TEntity>(_manager.GetFirstItems(_cacheSize));
			
			_currItemPointer = 0;
			
			_lastElementReached = false;
			_firstElementReached = true;
			
			if (CurrentItemChanged != null)
				CurrentItemChanged();
			
		}
		
		private bool CanGoToFirstRecord(object o)
		{
			if (_firstElementReached)
				return false;
			
			return true;
		}
		
		#endregion //FirstCommand
		
		#region LastCommand
		
		public ICommand LastCommand
		{
			get
			{
				if (_lastCmd == null)
					_lastCmd = new RelayCommand(GoToLastRecord, CanGoToLastRecord);
				
				return _lastCmd;
			}
		}
		
		private void GoToLastRecord(object o)
		{
			if (_cache[_cache.Count - 1].ItemID != _lastItemID)
				_cache = new List<TEntity>(_manager.GetLastItems(_cacheSize));
			
			_currItemPointer = _cacheSize - 1;
			
			_lastElementReached = true;
			_firstElementReached = false;
			
			if (CurrentItemChanged != null)
				CurrentItemChanged();
			
		}
		
		private bool CanGoToLastRecord(object o)
		{
			if (_lastElementReached)
				return false;
			
			return true;
		}		
		
		#endregion //LastCommand
		
		#region EditModeCommand
		
		public ICommand EditModeCommand
		{
			get
			{
				if (_editModeCmd == null)
					_editModeCmd = new RelayCommand(EnterEditMode, CanEnterEditMode);
				
				return _editModeCmd;
			}
		}
		
		private void EnterEditMode(object o)
		{
			if (!EditModeOn && !AddModeOn)
				EditModeOn = true;
		}
		
		private bool CanEnterEditMode(object o)
		{
			if (EditModeOn || AddModeOn)
				return false;
			
			return true;
		}
		
		#endregion //EditModeCommand
		
		#region SaveCommand
		
		public ICommand SaveCommand
		{
			get
			{
				if (_saveCmd == null)
					_saveCmd = new RelayCommand(ExecuteSave, CanSave);
				
				return _saveCmd;
			}
		}
		
		private void ExecuteSave(object o)
		{
			if (EditModeOn)
			{
				string err = null;
				_manager.Update(_dirtyItems.Values.ToList(), out err);
				
				if (string.IsNullOrWhiteSpace(err))
				{
					_dirtyItems.Clear();
					EditModeOn = false;
				}
				else
				{
					Console.Beep();
					//UNDONE: Show error message
				}
			}
			else if (AddModeOn)
			{
				
				string err = null;
				_manager.Add(_tentativeItem, out err);
				if (string.IsNullOrWhiteSpace(err))
				{
					AddModeOn = false;
					_tentativeItem = default(TEntity);
					_lastItemID = _manager.GetLastItems(1)[0].ItemID;
					LastCommand.Execute(o);
				}
				else
					Console.Beep();
			}
		}
		
		private bool CanSave(object o)
		{
			if (AddModeOn && _tentativeItem.Error == null)
				return true;
			
			else if (EditModeOn && CurrentItem.Error == null)
				return true;
			
			return false;
		}
		
		#endregion //SaveCommand
		
		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get
			{
				if (_deleteCmd == null)
					_deleteCmd = new RelayCommand(ExecuteDelete, CanDelete);
				
				return _deleteCmd;
			}
		}
		
		
		private void ExecuteDelete(object o)
		{
			if (AddModeOn)
			{
				AddModeOn = false;
				_tentativeItem = default(TEntity);
				if (CurrentItemChanged != null)
					CurrentItemChanged();
			}
			else
			{
				_recentlyDeletedIds.Add(CurrentItem.ItemID);
				_dirtyItems.Remove(CurrentItem.ItemID);
				
				_manager.Delete(CurrentItem.ItemID);
				if (_firstElementReached)
				{
					_firstItemID = _manager.GetFirstItems(1)[0].ItemID;
					FirstCommand.Execute(o);
				}
				else if (_lastElementReached)
				{
					_lastItemID = _manager.GetLastItems(1)[0].ItemID;
					LastCommand.Execute(o);
				}
				else
					NextCommand.Execute(o);
				
				if (CurrentItemChanged != null)
					CurrentItemChanged();
			}
		}
		
		public bool CanDelete(object o)
		{
			return true;
		}
		
		
		#endregion //DeleteCommand
		
		#region AddNewCommand
		public ICommand AddNewCommand
		{
			get
			{
				if (_addNewCmd == null)
					_addNewCmd = new RelayCommand(ExecuteAddNew, CanAddNew);
				
				return _addNewCmd;
			}
		}
		
		private void ExecuteAddNew(object o)
		{
			
//			TEntity newItem = new TEntity();
//			string err;
//			_manager.Add(newItem, out err);
//			_lastItemID = _manager.GetLastItems(1)[0].ItemID;
//			
//				AddModeOn = true;
//				LastCommand.Execute(o);
			
			_tentativeItem = new TEntity();
			AddModeOn = true;
			if (CurrentItemChanged != null)
				CurrentItemChanged();
				
		}
		
		private bool CanAddNew(object o)
		{
			if (AnyModificationModeOn)
				return false;
			
			return true;
		}
		
		#endregion //AddNewCommand
		
		#endregion
		
		#region Fields
		
		
		TEntity _tentativeItem;

		
		bool _editModeOn;
		bool _addModeOn;
		
		Dictionary<int, TEntity> _dirtyItems;
		
		IEntityManagerCore<TEntity> _manager;
		
		int _firstItemID;
		int _lastItemID;
		
		int _userEnteredRecID;
		
		List<TEntity> _cache;
		int _cacheSize;
		int _halfCacheSize;
		const int DEFAULT_CACHE_SIZE = 5;
		int _currItemPointer;
		
		SortedSet<int> _recentlyDeletedIds;
		
		bool _lastElementReached;
		bool _firstElementReached;
		
		RelayCommand _nextCmd;
		RelayCommand _prevCmd;
		RelayCommand _firstCmd;
		RelayCommand _lastCmd;
		RelayCommand _goToIdCmd;
		RelayCommand _addNewCmd;
		RelayCommand _deleteCmd;
		RelayCommand _editModeCmd;
		RelayCommand _saveCmd;
		
		#endregion
	}
}
