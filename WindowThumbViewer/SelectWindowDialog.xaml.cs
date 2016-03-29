using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowThumbViewer
{
	/// <summary>
	/// Interaction logic for SelectWindowDialog.xaml
	/// </summary>
	public partial class SelectWindowDialog : Window
	{
		public KeyValuePair<IntPtr,string> SelectedRow;

		public SelectWindowDialog()
		{
			InitializeComponent();
			dataGrid.ItemsSource = new ReadOnlyDictionary<IntPtr, string>( MainWindow.AllHandles );
		}

		private void dataGrid_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			var grid = sender as DataGrid;
			SelectedRow = (KeyValuePair<IntPtr, string>)grid.SelectedItem;
			this.Close();
		}
	}
}