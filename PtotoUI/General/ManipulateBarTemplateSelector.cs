using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using ProtoBLL.BusinessEntities;
using ProtoUI.ViewModels.General;

namespace ProtoUI.General
{
	/// <summary>
	/// This solves the problem of having to identify the exact type of the
	/// ManipulateBarViewModel<T> generic type when trying to apply a view to it.
	/// Alternate (ugly) method: Perform a dummy derivation:
	/// class BookDetailsManipulateBarVM : ManipulateBarVM<BookDetailsBLL> {}
	/// and use the above type in ManipBookDetailsVM. Inelegant.
	/// </summary>
	public class ManipulateBarTemplateSelector : DataTemplateSelector
	{
		public ManipulateBarTemplateSelector()
		{
			
		}
		
		public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
		{
	        DataTemplate template = null;
	        if (item != null)
	        {
	            FrameworkElement element = container as FrameworkElement;
	            if (element != null)
	            {
	                string templateName = null;
	                
	                if (item is ManipulateBarViewModel<BookDetailsBLL>)
	                	templateName = "BookDetailsManipulateBarTemplate";
	                
	
	                template = element.FindResource(templateName) as DataTemplate;
	            } 
	        }
	        return template;
		}
	}
}
