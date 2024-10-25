using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Manager
{
	// Token: 0x0200000F RID: 15
	public sealed class VistaFolderBrowserDialog
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00004FC5 File Offset: 0x000031C5
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00004FCD File Offset: 0x000031CD
		public string SelectedPath { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00004FD6 File Offset: 0x000031D6
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00004FDE File Offset: 0x000031DE
		public string SelectedElementName { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00004FE7 File Offset: 0x000031E7
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00004FEF File Offset: 0x000031EF
		public string[] SelectedPaths { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00004FF8 File Offset: 0x000031F8
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00005000 File Offset: 0x00003200
		public string[] SelectedElementNames { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005009 File Offset: 0x00003209
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00005011 File Offset: 0x00003211
		public bool AllowNonStoragePlaces { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000501A File Offset: 0x0000321A
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00005022 File Offset: 0x00003222
		public bool Multiselect { get; set; }

		// Token: 0x06000125 RID: 293 RVA: 0x0000502C File Offset: 0x0000322C
		public bool ShowDialog(IntPtr owner)
		{
			if (Environment.OSVersion.Version.Major < 6)
			{
				throw new InvalidOperationException("The dialog need at least Windows Vista to work.");
			}
			VistaFolderBrowserDialog.IFileOpenDialog fileOpenDialog = this.CreateNativeDialog();
			bool result;
			try
			{
				this.SetInitialFolder(fileOpenDialog);
				this.SetOptions(fileOpenDialog);
				if (fileOpenDialog.Show(owner) != 0)
				{
					result = false;
				}
				else
				{
					this.SetDialogResults(fileOpenDialog);
					result = true;
				}
			}
			finally
			{
				Marshal.ReleaseComObject(fileOpenDialog);
			}
			return result;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000509C File Offset: 0x0000329C
		private void GetPathAndElementName(VistaFolderBrowserDialog.IShellItem item, out string path, out string elementName)
		{
			item.GetDisplayName((VistaFolderBrowserDialog.SIGDN)2147991553U, out elementName);
			try
			{
				item.GetDisplayName((VistaFolderBrowserDialog.SIGDN)2147844096U, out path);
			}
			catch (ArgumentException)
			{
				path = null;
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000050DC File Offset: 0x000032DC
		private VistaFolderBrowserDialog.IFileOpenDialog CreateNativeDialog()
		{
			return new VistaFolderBrowserDialog.FileOpenDialog() as VistaFolderBrowserDialog.IFileOpenDialog;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000050E8 File Offset: 0x000032E8
		private void SetInitialFolder(VistaFolderBrowserDialog.IFileOpenDialog dialog)
		{
			if (!string.IsNullOrEmpty(this.SelectedPath))
			{
				uint num = 0U;
				IntPtr pidl;
				VistaFolderBrowserDialog.IShellItem folder;
				if (VistaFolderBrowserDialog.NativeMethods.SHILCreateFromPath(this.SelectedPath, out pidl, ref num) == 0 && VistaFolderBrowserDialog.NativeMethods.SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, pidl, out folder) == 0)
				{
					dialog.SetFolder(folder);
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00005130 File Offset: 0x00003330
		private void SetOptions(VistaFolderBrowserDialog.IFileOpenDialog dialog)
		{
			dialog.SetOptions(this.GetDialogOptions());
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00005140 File Offset: 0x00003340
		private VistaFolderBrowserDialog.FOS GetDialogOptions()
		{
			VistaFolderBrowserDialog.FOS fos = VistaFolderBrowserDialog.FOS.PICKFOLDERS;
			if (this.Multiselect)
			{
				fos |= VistaFolderBrowserDialog.FOS.ALLOWMULTISELECT;
			}
			if (!this.AllowNonStoragePlaces)
			{
				fos |= VistaFolderBrowserDialog.FOS.FORCEFILESYSTEM;
			}
			return fos;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00005170 File Offset: 0x00003370
		private void SetDialogResults(VistaFolderBrowserDialog.IFileOpenDialog dialog)
		{
			if (!this.Multiselect)
			{
				VistaFolderBrowserDialog.IShellItem item;
				dialog.GetResult(out item);
				string text;
				string text2;
				this.GetPathAndElementName(item, out text, out text2);
				this.SelectedPath = text;
				this.SelectedPaths = new string[]
				{
					text
				};
				this.SelectedElementName = text2;
				this.SelectedElementNames = new string[]
				{
					text2
				};
				return;
			}
			VistaFolderBrowserDialog.IShellItemArray shellItemArray;
			dialog.GetResults(out shellItemArray);
			uint num;
			shellItemArray.GetCount(out num);
			this.SelectedPaths = new string[num];
			this.SelectedElementNames = new string[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				VistaFolderBrowserDialog.IShellItem item;
				shellItemArray.GetItemAt(num2, out item);
				string text3;
				string text4;
				this.GetPathAndElementName(item, out text3, out text4);
				this.SelectedPaths[(int)num2] = text3;
				this.SelectedElementNames[(int)num2] = text4;
			}
			this.SelectedPath = null;
			this.SelectedElementName = null;
		}

		// Token: 0x02000016 RID: 22
		private class NativeMethods
		{
			// Token: 0x060001A8 RID: 424
			[DllImport("shell32.dll")]
			public static extern int SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, out IntPtr ppIdl, ref uint rgflnOut);

			// Token: 0x060001A9 RID: 425
			[DllImport("shell32.dll")]
			public static extern int SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001AA RID: 426
			[DllImport("user32.dll")]
			public static extern IntPtr GetActiveWindow();
		}

		// Token: 0x02000017 RID: 23
		[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IShellItem
		{
			// Token: 0x060001AC RID: 428
			void BindToHandler([MarshalAs(UnmanagedType.Interface)][In] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

			// Token: 0x060001AD RID: 429
			void GetParent([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001AE RID: 430
			void GetDisplayName([In] VistaFolderBrowserDialog.SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

			// Token: 0x060001AF RID: 431
			void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

			// Token: 0x060001B0 RID: 432
			void Compare([MarshalAs(UnmanagedType.Interface)][In] VistaFolderBrowserDialog.IShellItem psi, [In] uint hint, out int piOrder);
		}

		// Token: 0x02000018 RID: 24
		[Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IShellItemArray
		{
			// Token: 0x060001B1 RID: 433
			void BindToHandler([MarshalAs(UnmanagedType.Interface)][In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

			// Token: 0x060001B2 RID: 434
			void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

			// Token: 0x060001B3 RID: 435
			void GetPropertyDescriptionList([MarshalAs(UnmanagedType.Struct)][In] ref IntPtr keyType, [In] ref Guid riid, out IntPtr ppv);

			// Token: 0x060001B4 RID: 436
			void GetAttributes([MarshalAs(UnmanagedType.I4)][In] IntPtr dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

			// Token: 0x060001B5 RID: 437
			void GetCount(out uint pdwNumItems);

			// Token: 0x060001B6 RID: 438
			void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001B7 RID: 439
			void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
		}

		// Token: 0x02000019 RID: 25
		[Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[CoClass(typeof(VistaFolderBrowserDialog.FileOpenDialog))]
		[ComImport]
		private interface IFileOpenDialog
		{
			// Token: 0x060001B8 RID: 440
			[PreserveSig]
			int Show([In] IntPtr parent);

			// Token: 0x060001B9 RID: 441
			void SetFileTypes([In] uint cFileTypes, [MarshalAs(UnmanagedType.Struct)][In] ref IntPtr rgFilterSpec);

			// Token: 0x060001BA RID: 442
			void SetFileTypeIndex([In] uint iFileType);

			// Token: 0x060001BB RID: 443
			void GetFileTypeIndex(out uint piFileType);

			// Token: 0x060001BC RID: 444
			void Advise([MarshalAs(UnmanagedType.Interface)][In] IntPtr pfde, out uint pdwCookie);

			// Token: 0x060001BD RID: 445
			void Unadvise([In] uint dwCookie);

			// Token: 0x060001BE RID: 446
			void SetOptions([In] VistaFolderBrowserDialog.FOS fos);

			// Token: 0x060001BF RID: 447
			void GetOptions(out VistaFolderBrowserDialog.FOS pfos);

			// Token: 0x060001C0 RID: 448
			void SetDefaultFolder([MarshalAs(UnmanagedType.Interface)][In] VistaFolderBrowserDialog.IShellItem psi);

			// Token: 0x060001C1 RID: 449
			void SetFolder([MarshalAs(UnmanagedType.Interface)][In] VistaFolderBrowserDialog.IShellItem psi);

			// Token: 0x060001C2 RID: 450
			void GetFolder([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001C3 RID: 451
			void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001C4 RID: 452
			void SetFileName([MarshalAs(UnmanagedType.LPWStr)][In] string pszName);

			// Token: 0x060001C5 RID: 453
			void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

			// Token: 0x060001C6 RID: 454
			void SetTitle([MarshalAs(UnmanagedType.LPWStr)][In] string pszTitle);

			// Token: 0x060001C7 RID: 455
			void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)][In] string pszText);

			// Token: 0x060001C8 RID: 456
			void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)][In] string pszLabel);

			// Token: 0x060001C9 RID: 457
			void GetResult([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItem ppsi);

			// Token: 0x060001CA RID: 458
			void AddPlace([MarshalAs(UnmanagedType.Interface)][In] VistaFolderBrowserDialog.IShellItem psi, FileDialogCustomPlace fdcp);

			// Token: 0x060001CB RID: 459
			void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)][In] string pszDefaultExtension);

			// Token: 0x060001CC RID: 460
			void Close([MarshalAs(UnmanagedType.Error)] int hr);

			// Token: 0x060001CD RID: 461
			void SetClientGuid([In] ref Guid guid);

			// Token: 0x060001CE RID: 462
			void ClearClientData();

			// Token: 0x060001CF RID: 463
			void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

			// Token: 0x060001D0 RID: 464
			void GetResults([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItemArray ppenum);

			// Token: 0x060001D1 RID: 465
			void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out VistaFolderBrowserDialog.IShellItemArray ppsai);
		}

		// Token: 0x0200001A RID: 26
		[Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
		[ComImport]
		private class FileOpenDialog
		{


		}

		// Token: 0x0200001B RID: 27
		private enum SIGDN : uint
		{
			// Token: 0x040000BD RID: 189
			DESKTOPABSOLUTEEDITING = 2147794944U,
			// Token: 0x040000BE RID: 190
			DESKTOPABSOLUTEPARSING = 2147647488U,
			// Token: 0x040000BF RID: 191
			FILESYSPATH = 2147844096U,
			// Token: 0x040000C0 RID: 192
			NORMALDISPLAY = 0U,
			// Token: 0x040000C1 RID: 193
			PARENTRELATIVE = 2148007937U,
			// Token: 0x040000C2 RID: 194
			PARENTRELATIVEEDITING = 2147684353U,
			// Token: 0x040000C3 RID: 195
			PARENTRELATIVEFORADDRESSBAR = 2147991553U,
			// Token: 0x040000C4 RID: 196
			PARENTRELATIVEPARSING = 2147581953U,
			// Token: 0x040000C5 RID: 197
			URL = 2147909632U
		}

		// Token: 0x0200001C RID: 28
		[Flags]
		private enum FOS
		{
			// Token: 0x040000C7 RID: 199
			ALLNONSTORAGEITEMS = 128,
			// Token: 0x040000C8 RID: 200
			ALLOWMULTISELECT = 512,
			// Token: 0x040000C9 RID: 201
			CREATEPROMPT = 8192,
			// Token: 0x040000CA RID: 202
			DEFAULTNOMINIMODE = 536870912,
			// Token: 0x040000CB RID: 203
			DONTADDTORECENT = 33554432,
			// Token: 0x040000CC RID: 204
			FILEMUSTEXIST = 4096,
			// Token: 0x040000CD RID: 205
			FORCEFILESYSTEM = 64,
			// Token: 0x040000CE RID: 206
			FORCESHOWHIDDEN = 268435456,
			// Token: 0x040000CF RID: 207
			HIDEMRUPLACES = 131072,
			// Token: 0x040000D0 RID: 208
			HIDEPINNEDPLACES = 262144,
			// Token: 0x040000D1 RID: 209
			NOCHANGEDIR = 8,
			// Token: 0x040000D2 RID: 210
			NODEREFERENCELINKS = 1048576,
			// Token: 0x040000D3 RID: 211
			NOREADONLYRETURN = 32768,
			// Token: 0x040000D4 RID: 212
			NOTESTFILECREATE = 65536,
			// Token: 0x040000D5 RID: 213
			NOVALIDATE = 256,
			// Token: 0x040000D6 RID: 214
			OVERWRITEPROMPT = 2,
			// Token: 0x040000D7 RID: 215
			PATHMUSTEXIST = 2048,
			// Token: 0x040000D8 RID: 216
			PICKFOLDERS = 32,
			// Token: 0x040000D9 RID: 217
			SHAREAWARE = 16384,
			// Token: 0x040000DA RID: 218
			STRICTFILETYPES = 4
		}
	}
}
