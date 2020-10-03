﻿using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_127
{
	/// <summary>
	/// Class SaveFileHandler.xaml
	/// </summary>
	public partial class SaveFileHandler : Window
	{
		/// <summary>
		/// Constructor of SaveFileHandler
		/// </summary>
		public SaveFileHandler()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Used for DataBinding
			this.DataContext = this;

			btn_Refresh_Click(null, null);
		}


		/// <summary>
		/// Sorts a DataGrid
		/// </summary>
		/// <param name="pDataGrid"></param>
		private void Sort(DataGrid pDataGrid)
		{
			// What can I say...
			// https://stackoverflow.com/a/40395019

			// Since we always add one more MySaveFile to the collection we could 
			// Loop through the childitems and then move it to the correct index
			// Pretty much implement our own Sorting Method which uses MySaveFile.BackupSaves.Move(a,b);

			if (pDataGrid.ItemsSource == null)
				pDataGrid.ItemsSource = MySaveFile.BackupSaves;
			CollectionViewSource.GetDefaultView(pDataGrid.ItemsSource).Refresh();
			pDataGrid.Items.SortDescriptions.Clear();
			pDataGrid.Items.SortDescriptions.Add(new SortDescription(pDataGrid.Columns[0].SortMemberPath, ListSortDirection.Ascending));
			foreach (var col in pDataGrid.Columns)
			{
				col.SortDirection = null;
			}
			pDataGrid.Columns[0].SortDirection = ListSortDirection.Ascending;
			pDataGrid.Items.Refresh();
		}

		/// <summary>
		/// Click on the Refresh Button. Reads files from disk again.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Refresh_Click(object sender = null, RoutedEventArgs e = null)
		{
			// Resetting the Obvservable Collections:
			MySaveFile.BackupSaves = new ObservableCollection<MySaveFile>();
			MySaveFile.GTASaves = new ObservableCollection<MySaveFile>();

			// Files in BackupSaves (own File Path)
			string[] MyBackupSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.BackupSavesPath);
			foreach (string MyBackupSaveFile in MyBackupSaveFiles)
			{
				if (!MyBackupSaveFile.Contains(".bak"))
				{
					MySaveFile.BackupSaves.Add(new MySaveFile(MyBackupSaveFile));
				}
			}

			// Files in actual GTAV Save File Locations
			string[] MyGTAVSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.GTAVSavesPath);
			foreach (string MyGTAVSaveFile in MyGTAVSaveFiles)
			{
				if (!MyGTAVSaveFile.Contains(".bak") && MyGTAVSaveFile.Contains("SGTA500"))
				{
					MySaveFile.GTASaves.Add(new MySaveFile(MyGTAVSaveFile));
				}
			}


			// Set the ItemSource of Both Datagrids for the DataBinding
			dg_BackupFiles.ItemsSource = MySaveFile.BackupSaves;
			dg_GTAFiles.ItemsSource = MySaveFile.GTASaves;
		}


		/// <summary>
		/// Button Click on the LeftArrow (From GTA Path to Backup Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_LeftArrow_Click(object sender, RoutedEventArgs e)
		{
			// Copying a File from the GTA V Saves to the Backup Saves
			// We ask User for Name

			// Null Check
			if (dg_GTAFiles.SelectedItem != null)
			{
				// Get MySaveFile from the selected Item
				MySaveFile tmp = (MySaveFile)dg_GTAFiles.SelectedItem;

				// Get the Name for it
				string newName = GetNewFileName(tmp, MySaveFile.BackupSavesPath);
				if (!string.IsNullOrWhiteSpace(newName))
				{
					// Only do if it the name is not "" or null
					tmp.CopyToBackup(newName);
					Sort(dg_BackupFiles);
				}
			}
		}




		/// <summary>
		/// Click on the Right Arrow (From Backup Path to GTA Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
		{
			// Copying a File from the Backup Saves to the GTA V Saves
			// Build name which GTA Uses

			// Null checker
			if (dg_BackupFiles.SelectedItem != null)
			{
				// Get MySaveFile from the selected Item
				MySaveFile tmp = (MySaveFile)dg_BackupFiles.SelectedItem;

				// Building the first theoretical FileName
				int i = 0;
				string NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
				string FilePathInsideGTA = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + NewFileName;

				// While Loop through all 16 names, breaking out when File does NOT exist or when we reached the manimum
				while (HelperClasses.FileHandling.doesFileExist(FilePathInsideGTA))
				{
					if (i >= 15)
					{
						// Save Files with a Number larger than that will not be read by game
						new Popup(Popup.PopupWindowTypes.PopupOk, "No free SaveSlot (00-15) inside the GTA Save File Path.\nDelete one and try again").ShowDialog();
						return;
					}

					// Build new theoretical FileName
					i++;
					NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
					FilePathInsideGTA = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + NewFileName;
				}
				tmp.CopyToGTA(NewFileName);
				Sort(dg_GTAFiles);
				new Popup(Popup.PopupWindowTypes.PopupOk, "Copied '" + tmp.FileName + "' to the GTA V Saves Location under the Name '" + NewFileName + "' !").ShowDialog();
			}
		}



		/// <summary>
		/// Rename Button for the Files in the "our" Folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Rename_Click(object sender, RoutedEventArgs e)
		{
			MySaveFile tmp = GetSelectedSaveFile();
			if (tmp != null)
			{
				if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Re-Naming something inside the GTA V Saves Location makes no sense,\nsince it wont get recognized by the game.\nStill want to continue?");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						return;
					}
				}
				string newName = GetNewFileName(tmp, tmp.Path);
				if (!string.IsNullOrWhiteSpace(newName))
				{

					tmp.Rename(newName);
					if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
					{
						Sort(dg_GTAFiles);
					}
					else
					{
						Sort(dg_BackupFiles);
					}
				}
			}
		}



		/// <summary>
		/// Delete Button for the files in the GTAV Location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			MySaveFile tmp = GetSelectedSaveFile();
			if (tmp != null)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Are you sure you want to delete this SaveFile?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					tmp.Delete();
					if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
					{
						Sort(dg_GTAFiles);
					}
					else
					{
						Sort(dg_BackupFiles);
					}
				}
			}
		}

		/// <summary>
		/// Gets the single selected File from Both Grids
		/// </summary>
		/// <returns></returns>
		private MySaveFile GetSelectedSaveFile()
		{
			if (dg_BackupFiles.SelectedItem != null)
			{
				return (MySaveFile)dg_BackupFiles.SelectedItem;
			}
			else if (dg_GTAFiles.SelectedItem != null)
			{
				return (MySaveFile)dg_GTAFiles.SelectedItem;
			}
			return null;
		}

		/// <summary>
		/// New Name Popup Logic
		/// </summary>
		/// <param name="pMySaveFile"></param>
		/// <param name="pDestination"></param>
		/// <returns></returns>
		private string GetNewFileName(MySaveFile pMySaveFile, string pPathToCheck)
		{
			string newName = "";

			// Asking for Name 
			Popup newNamePU = new Popup(Popup.PopupWindowTypes.PopupOkTextBox, "Enter new Name for the SaveFile: ", pDefaultTBText: pMySaveFile.FileName);
			newNamePU.ShowDialog();
			if (newNamePU.DialogResult == true)
			{
				// Getting the Name chosen
				newName = newNamePU.MyReturnString;

				// While name was give OR fikle exists
				while (String.IsNullOrWhiteSpace(newName) || HelperClasses.FileHandling.doesFileExist(pPathToCheck.TrimEnd('\\') + @"\" + newName))
				{
					// Not a Valid FilePath
					Globals.DebugPopup("File: '" + pPathToCheck.TrimEnd('\\') + @"\" + newName + "'\ndoes it exist: '" + HelperClasses.FileHandling.doesFileExist(pPathToCheck.TrimEnd('\\') + @"\" + newName) + "'");
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "File does already exists or is not a valid FileName.\n" +
																					"Click yes if you want to try again.");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						// When you wanna exit
						return "";
					}
					else
					{
						// When you wanna stay in while loop
						newNamePU = new Popup(Popup.PopupWindowTypes.PopupOkTextBox, "Enter new Name for the SaveFile: ", pDefaultTBText: pMySaveFile.FileName);
						newNamePU.ShowDialog();
						if (newNamePU.DialogResult == true)
						{
							newName = newNamePU.MyReturnString;
						}
					}
				}
			}
			return newName;
		}




		/// <summary>
		/// Close Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		// Below are Methods we need to make the behaviour of this nice.


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for Backup Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_BackupFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_BackupFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for GTA Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_GTAFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_GTAFiles.ScrollToVerticalOffset(sv_GTAFiles.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Reset Selection when the other datagrid was clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_GTAFiles_GotFocus(object sender, RoutedEventArgs e)
		{
			dg_BackupFiles.SelectedItem = null;
		}

		/// <summary>
		/// Reset Selection when the other datagrid was clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_BackupFiles_GotFocus(object sender, RoutedEventArgs e)
		{
			dg_GTAFiles.SelectedItem = null;
		}

		private void dg_KeyDown(object sender, KeyEventArgs e)
		{
			DataGrid asdf = (DataGrid)sender;
			if (e.Key == Key.Delete)
			{
				btn_Delete_Click(null, null);
			}
			else if (e.Key == Key.F2)
			{
				btn_Rename_Click(null, null);
			}
		}


	} // End of Class
} // End of Namespace
