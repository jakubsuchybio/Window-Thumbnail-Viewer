using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowThumbViewer
{
	public static class Win32Funcs
	{
		public static readonly int GWL_STYLE = -16;

		public const int DWM_TNP_RECTDESTINATION = 0x00000001;
		public const int DWM_TNP_RECTSOURCE = 0x00000002;
		public const int DWM_TNP_OPACITY = 0x00000004;
		public const int DWM_TNP_VISIBLE = 0x00000008;
		public const int DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;

		public const ulong WS_VISIBLE = 0x10000000L;
		public const ulong WS_BORDER = 0x00800000L;
		public const ulong TARGETWINDOW = WS_BORDER | WS_VISIBLE;

		public const uint WM_KEYDOWN = 0x0100;
		public const int VK_F5 = 0x74;

		[DllImport( "user32.dll" )]
		public static extern bool PostMessage( IntPtr hWnd, UInt32 Msg, int wParam, int lParam );

		[DllImport( "user32.dll" )]
		public static extern ulong GetWindowLongA( IntPtr hWnd, int nIndex );

		[DllImport( "user32.dll" )]
		public static extern int EnumWindows( EnumWindowsCallback lpEnumFunc, int lParam );
		public delegate bool EnumWindowsCallback( IntPtr hwnd, int lParam );

		[DllImport( "user32.dll" )]
		public static extern void GetWindowText( IntPtr hWnd, StringBuilder lpString, int nMaxCount );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		public static extern bool SetForegroundWindow( IntPtr hWnd );


		[DllImport( "dwmapi.dll" )]
		public static extern int DwmRegisterThumbnail( IntPtr dest, IntPtr src, out IntPtr thumb );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmUnregisterThumbnail( IntPtr thumb );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmQueryThumbnailSourceSize( IntPtr thumb, out PSIZE size );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmUpdateThumbnailProperties( IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props );



	}

	#region Interop structs

	[StructLayout( LayoutKind.Sequential )]
	public struct DWM_THUMBNAIL_PROPERTIES
	{
		public int dwFlags;
		public Rect rcDestination;
		public Rect rcSource;
		public byte opacity;
		public bool fVisible;
		public bool fSourceClientAreaOnly;
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct Rect
	{
		public Rect( int left, int top, int right, int bottom )
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public List<Rect> AsList() => new List<Rect> { this };

		public Rect Scale( double percentage ) =>
			new Rect(
				 (int)( Left * percentage ),
				 (int)( Top * percentage ),
				 (int)( Right * percentage ),
				 (int)( Bottom * percentage ) );

		public Rect MakeSmaller( int size ) =>
			new Rect(
				 Left + size,
				 Top + size,
				 Right - size,
				 Bottom - size );
	}

	public static class RectUtils
	{

		public static List<Rect> SplitHorizontaly( this List<Rect> rects, int splits = 2 )
		{
			var ret = new List<Rect>();
			foreach( var item in rects )
			{
				var step = (item.Right - item.Left) / splits;
				for( int i = 0; i < splits; i++ )
				{
					ret.Add( new Rect( item.Left + step * i, item.Top, item.Left + step * ( i + 1 ), item.Bottom ) );
				}
			}
			return ret;
		}

		public static List<Rect> SplitVertically( this List<Rect> rects, int splits = 2 )
		{
			var ret = new List<Rect>();
			foreach( var item in rects )
			{
				var step = (item.Bottom - item.Top) / splits;
				for( int i = 0; i < splits; i++ )
				{
					ret.Add( new Rect( item.Left, item.Top + step * i, item.Right, item.Top + step * ( i + 1 ) ) );
				}
			}
			return ret;
		}

		public static List<Rect> SortRects( this List<Rect> rects ) =>
			rects.OrderBy( r => r.Top ).ThenBy( r => r.Left ).ToList();
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct PSIZE
	{
		public int x;
		public int y;
	}

	#endregion
}
