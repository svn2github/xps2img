using System;
using System.Windows.Forms;

using Windows7.DesktopIntegration.Interop;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows7.DesktopIntegration
{
	public static class Windows7Taskbar
	{
		#region Infrastructure

		private static readonly object _syncRoot = new Object();

		private static volatile ITaskbarList3 _taskbarList;
		private static ITaskbarList3 TaskbarList
		{
			get
			{
				if (_taskbarList == null)
				{
					lock (_syncRoot)
					{
						try
						{
							if (_taskbarList == null)
							{
								_taskbarList = (ITaskbarList3)new TaskbarList();
								_taskbarList.HrInit();
							}
						}
						catch
						{
							_taskbarList = new TaskbarListStub();
						}
					}
				}
				return _taskbarList;
			}
		}

		#endregion

		#region Taskbar Progress Bar

		/// <summary>
		/// Represents the thumbnail progress bar state.
		/// </summary>
		public enum ThumbnailProgressState
		{
			/// <summary>
			/// No progress is displayed.
			/// </summary>
			NoProgress = 0,
			/// <summary>
			/// The progress is indeterminate (marquee).
			/// </summary>
			Indeterminate = 0x1,
			/// <summary>
			/// Normal progress is displayed.
			/// </summary>
			Normal = 0x2,
			/// <summary>
			/// An error occurred (red).
			/// </summary>
			Error = 0x4,
			/// <summary>
			/// The operation is paused (yellow).
			/// </summary>
			Paused = 0x8
		}

		public static void SetProgressState(IntPtr hwnd, ThumbnailProgressState state)
		{
			TaskbarList.SetProgressState(hwnd, (TBPFLAG)state);
		}

		public static void SetProgressState(this Form control, ThumbnailProgressState state)
		{
			try
			{
				SetProgressState(control.Handle, state);
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch
			// ReSharper restore EmptyGeneralCatchClause
			{
				// Ignore.
			}
		}

		public static void SetProgressValue(IntPtr hwnd, int current, int maximum)
		{
			TaskbarList.SetProgressValue(hwnd, (ulong)current, (ulong)maximum);
		}

		public static void SetProgressValue(this Form control, int current, int maximum)
		{
			try
			{
				SetProgressValue(control.Handle, current, maximum);
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch
			// ReSharper restore EmptyGeneralCatchClause
			{
				// Ignore.
			}
		}

		#endregion
	}
}
