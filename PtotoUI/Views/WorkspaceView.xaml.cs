using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ProtoUI.Views
{
	/// <summary>
	/// Interaction logic for WorkspaceView.xaml
	/// </summary>
	public partial class WorkspaceView : UserControl
	{
		public WorkspaceView()
		{
			InitializeComponent();
			
		}
		
		void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show(this.DataContext.GetType().Name);
		}
	}
}