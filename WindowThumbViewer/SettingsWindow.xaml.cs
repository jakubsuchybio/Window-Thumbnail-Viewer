using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowThumbViewer
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public IntPtr Handle;

		public SettingsWindow()
		{
			InitializeComponent();
			listBox.ItemsSource = MainWindow.SelectedHandles;
			foreach( var item in Enumerable.Range( 1, 16 ) )
				comboBox.Items.Add( item );
			comboBox.SelectedIndex = ( MainWindow.SelectedHandles?.Count ?? 9 ) - 1;
			Handle = new WindowInteropHelper( this ).Handle;
		}

		private void Window_KeyUp( object sender, KeyEventArgs e )
		{
			switch( e.Key )
			{
				case Key.F1:
				case Key.Escape: this.Close(); break;
			}
		}

		private void button_Click( object sender, RoutedEventArgs e )
		{
			MainWindow.LoadWindows();
		}

		private void comboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			ReloadBoxes();
		}

		private void ReloadBoxes()
		{
			int num = (int)comboBox.SelectedItem;

			if( MainWindow.SelectedHandles == null )
				MainWindow.SelectedHandles = new ObservableCollection<ListItemTemplate>();

			//Fill to required number
			while( num > MainWindow.SelectedHandles.Count )
			{
				var last = MainWindow.SelectedHandles.Count;
				MainWindow.SelectedHandles.Add( new ListItemTemplate( last, IntPtr.Zero, "Not selected" ) );
			}

			while( num < MainWindow.SelectedHandles.Count )
			{
				var last = MainWindow.SelectedHandles.Count-1;
				MainWindow.SelectedHandles.RemoveAt( last );
			}
		}

		private void row_Button_Click( object sender, RoutedEventArgs e )
		{
			var button = sender as Button;
			var num = int.Parse(button.Tag.ToString());

			var dialog = new SelectWindowDialog();
			var point = button.TransformToAncestor(this).Transform(new Point(0,0));
			dialog.Left = this.Left + point.X;
			dialog.Top = this.Top + point.Y + 60;
			dialog.ShowDialog();
			var row = dialog.SelectedRow;
			dialog.Close();
			MainWindow.SelectedHandles[num].Handle = row.Key;
			MainWindow.SelectedHandles[num].WindowName = row.Value;
		}
	}

	public class ListItemTemplate : ObservableObject
	{
		IntPtr handle;
		string windowName;

		public int Id { get; set; }
		public string Name { get; set; }

		public IntPtr ThumbnailHandle { get; set; }

		public IntPtr Handle
		{
			get { return handle; }

			set
			{
				handle = value;
				RaisePropertyChangedEvent( "Handle" );
			}
		}

		public string WindowName
		{
			get { return windowName; }

			set
			{
				windowName = value;
				RaisePropertyChangedEvent( "WindowName" );
			}
		}

		public ListItemTemplate( int id, IntPtr handle, string windowName )
		{
			this.Id = id;
			this.Name = $"No.{id + 1}";
			this.Handle = handle;
			this.WindowName = windowName;
		}
	}

	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChangedEvent( string propertyName )
		{
			var handler = PropertyChanged;
			if( handler != null )
				handler( this, new PropertyChangedEventArgs( propertyName ) );
		}
	}
}
