using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Controls;
using ProtoBLL;
using ProtoUI.ViewModels;
using ProtoUI.Views;

namespace ProtoUI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		

		
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			
			MainWin window = new MainWin();
			ProtoBridge bridge = new ProtoBridge();
			MainWinViewModel vm = new MainWinViewModel(bridge);
			vm.RequestClose += delegate { window.Close(); };
			window.DataContext = vm;
			
			
			
			window.Show();
		}
		
		
	}
}