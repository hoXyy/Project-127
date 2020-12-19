﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.IO.Compression;
using Project_127.MySettings;
using Project_127.HelperClasses;

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for CopyFileProgress.xaml
	/// This is responsible for ProgressBar on File copying (etc.) and Extracting the ZIP File
	/// </summary>
	public partial class PopupProgress : Window
	{
		/// <summary>
		/// Enum of ProgressTypes
		/// </summary>
		public enum ProgressTypes
		{
			ZIPFile,
			FileOperation,
			Upgrade,
			Downgrade,
			DidUpdateHit
		}

		/// <summary>
		/// ProgressType of Instance
		/// </summary>
		ProgressTypes ProgressType;

		/// <summary>
		/// List of File Operations
		/// </summary>
		List<HelperClasses.MyFileOperation> MyFileOperations;

		/// <summary>
		/// List of File Operations
		/// </summary>
		public List<HelperClasses.MyFileOperation> RtrnMyFileOperations;

		public bool RtrnBool = false;

		/// <summary>
		/// Name of Operation for GUI
		/// </summary>
		string Operation;

		/// <summary>
		/// Path of ZIP File (on disk) we are extracting
		/// </summary>
		string ZipFileWeWannaExtract;

		string myZipExtractionPath;

		/// <summary>
		/// Constructor of PopupProgress.
		/// String pParam1 is either the GUI Text for File Operations or the ZIP File physical location
		/// </summary>
		/// <param name="pProgressType"></param>
		/// <param name="pParam1"></param>
		/// <param name="pMyFileOperations"></param>
		public PopupProgress(ProgressTypes pProgressType, string pParam1, List<HelperClasses.MyFileOperation> pMyFileOperations = null, string zipExtractionPath = "")
		{
			if (zipExtractionPath == "")
			{
				zipExtractionPath = LauncherLogic.ZIPFilePath;
			}
			myZipExtractionPath = zipExtractionPath;

			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			InitializeComponent();

			// Setting all Properties needed later
			ProgressType = pProgressType;

			if (ProgressType == ProgressTypes.FileOperation)
			{
				Operation = pParam1;
				MyFileOperations = pMyFileOperations;
				myLBL.Content = Operation + "...(0%)";
			}
			else if (ProgressType == ProgressTypes.ZIPFile)
			{
				ZipFileWeWannaExtract = pParam1;
				myLBL.Content = "Extracting ZIP...(0%)";
			}
			else if (ProgressType == ProgressTypes.Downgrade)
			{
				myPB.Value = 0;
				myLBL.Content = "Gathering Information";
			}
			else if (ProgressType == ProgressTypes.Upgrade)
			{
				myPB.Value = 0;
				myLBL.Content = "Gathering Information";
			}
			else if (ProgressType == ProgressTypes.DidUpdateHit)
			{
				myPB.Value = 0;
				myLBL.Content = "Checking if GTA Update hit";
			}

			// Lets do some shit
			StartWork();
		}

		[STAThread]
		public async void StartWork()
		{
			// Awaiting the Task of the Actual Work
			await Task.Run(new Action(ActualWork));

			// Close this
			this.Close();
		}

		[STAThread]
		public void ActualWork()
		{
			HelperClasses.Logger.Log("ProgressType: '" + ProgressType + "'");

			if (ProgressType == ProgressTypes.FileOperation)
			{
				double count = MyFileOperations.Count;
				double j = 0;

				HelperClasses.Logger.Log("Lets do some File Operation Stuff");
				for (int i = 0; i <= MyFileOperations.Count - 1; i++)
				{
					MyFileOperation.Execute(MyFileOperations[i]);

					j++;
					this.Dispatcher.Invoke(() =>
					{
						myPB.Value = (int)(j / count * 100);
						myLBL.Content = "Doing a " + Operation + "...(" + myPB.Value + "%)";
					});
				}
				HelperClasses.Logger.Log("Done with File Operation Stuff");
			}
			else if (ProgressType == ProgressTypes.ZIPFile)
			{
				HelperClasses.Logger.Log("ZipFileWeWannaExtract: '" + ZipFileWeWannaExtract + "'");
				HelperClasses.Logger.Log("ZIPExtractPath: '" + myZipExtractionPath + "'");

				List<System.IO.Compression.ZipArchiveEntry> fileList = new List<System.IO.Compression.ZipArchiveEntry>();
				var totalFiles = 0;
				var filesExtracted = 0;

				if (!HelperClasses.FileHandling.doesFileExist(ZipFileWeWannaExtract))
				{
					HelperClasses.Logger.Log("ERROR. ZIP File we are extracting in Popup window doesnt exist..", true, 0);
					new Popup(Popup.PopupWindowTypes.PopupOk, "ERROR. ZIP File we are extracting in Popup window doesnt exist..").ShowDialog();
					return;
				}

				try
				{
					using (var archive = ZipFile.OpenRead(ZipFileWeWannaExtract))
					{
						totalFiles = archive.Entries.Count();

						// Looping through all Files in the ZIPFile
						foreach (var file in archive.Entries)
						{
							// If the File exists and is not a folder
							if (!string.IsNullOrEmpty(file.Name))
							{
								bool doExtract = true;
								string PathOnDisk = myZipExtractionPath.TrimEnd('\\') + @"\" + file.FullName.Replace(@"/", @"\");
								HelperClasses.FileHandling.createPathOfFile(PathOnDisk); // 99% Chance I fixed this with the createZipPaths Method. Lets keep this to make sure...
								if (HelperClasses.FileHandling.doesFileExist(PathOnDisk))
								{
									if (PathOnDisk.Contains("UpgradeFiles"))
									//	PathOnDisk.Contains("SupportFiles"))
									//PathOnDisk.Contains(@"DowngradeFiles\GTA5.exe") ||
									//PathOnDisk.Contains(@"DowngradeFiles\x64a.rpf") ||
									//PathOnDisk.Contains(@"DowngradeFiles\update\update.rpf"))
									{
										doExtract = false;
									}
									else
									{
										HelperClasses.FileHandling.deleteFile(PathOnDisk);
									}
								}

								if (doExtract)
								{
									file.ExtractToFile(PathOnDisk);
								}
							}

							// Update GUI
							Application.Current.Dispatcher.Invoke((Action)delegate
							{
								filesExtracted++;
								long progress = (100 * filesExtracted / totalFiles);
								myPB.Value = progress;
								myLBL.Content = "Extracting ZIP...(" + progress + "%)";
							});


							// // Lets hope we never need this but I want to keep this here for now, in case that code snipped becomes useful
							//this.Dispatcher.Invoke(() =>
							//{
							//	myPB.Value = progress;
							//	myLBL.Content = "Extracting ZIP...(" + progress + "%)";
							//});
						}
					}
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("TryCatch failed while extracting ZIP with progressbar." + e.ToString());
					new Popup(Popup.PopupWindowTypes.PopupOkError, "trycatch failed while extracting zip with progressbar\n" + e.ToString());
				}
			}
			else if (ProgressType == ProgressTypes.Downgrade)
			{
				bool UpdatePopupThrownAlready = false;

				// Saving all the File Operations I want to do, executing this at the end of this Method
				List<MyFileOperation> MyFileOperationsTmp = new List<MyFileOperation>();

				HelperClasses.Logger.Log("Initiating DOWNGRADE", 0);
				HelperClasses.Logger.Log("GTAV Installation Path: " + LauncherLogic.GTAVFilePath, 1);
				HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
				HelperClasses.Logger.Log("InstallationLocationBinary: " + Globals.ProjectInstallationPathBinary, 1);
				HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
				HelperClasses.Logger.Log("DowngradeFilePath: " + LauncherLogic.DowngradeFilePath, 1);
				HelperClasses.Logger.Log("UpgradeFilePath: " + LauncherLogic.UpgradeFilePath, 1);

				// Those are WITH the "\" at the end
				string[] FilesInDowngradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeFilePath);
				string[] CorrespondingFilePathInGTALocation = new string[FilesInDowngradeFiles.Length];
				string[] CorrespondingFilePathInUpgradeFiles = new string[FilesInDowngradeFiles.Length];

				HelperClasses.Logger.Log("Found " + FilesInDowngradeFiles.Length.ToString() + " Files in Downgrade Folder.");
				HelperClasses.Logger.Log("Found " + HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.UpgradeFilePath).Length.ToString() + " Files in Upgrade Folder.");

				// Loop through all Files in Downgrade Files Folder
				for (int i = 0; i <= FilesInDowngradeFiles.Length - 1; i++)
				{
					// Update GUI
					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = ((i + 1) * 100 / FilesInDowngradeFiles.Length);
						myPB.Value = progress;
						myLBL.Content = "Gathering Information(" + (i + 1).ToString() + "/" + (FilesInDowngradeFiles.Length).ToString() + ")";
					});


					// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
					CorrespondingFilePathInGTALocation[i] = LauncherLogic.GTAVFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);
					CorrespondingFilePathInUpgradeFiles[i] = LauncherLogic.UpgradeFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);


					if (LauncherLogic.IgnoreNewFilesWhileUpgradeDowngradeLogic)
					{
						// Move to $UpgradeFiles
						MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Deleting '" + CorrespondingFilePathInGTALocation + "' from $GTAInstallationDirectory, since this is a simple Downgrade after using a Backup", 1));
					}
					// Normal Downgrade Logic
					else
					{
						// If the File exists in GTA V Installation Path
						if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
						{
							// If the File we are replacing is the same as in DowngradeFiles
							if (HelperClasses.FileHandling.AreFilesEqual(CorrespondingFilePathInGTALocation[i], FilesInDowngradeFiles[i], MySettings.Settings.EnableSlowCompare))
							{
								// Delete from GTA V Installation Path 
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and its the same file as from $Downgrade_Files. Will delelte from GTA V Installation", 1));
							}

							// If the File we are replacing is the same as in UpgradeFiles
							else if (HelperClasses.FileHandling.AreFilesEqual(CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], MySettings.Settings.EnableSlowCompare))
							{
								// Delete from GTA V Installation Path
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and its the same file as from $Upgrade_Files. Will delelte from GTA V Installation", 1));
							}

							// If the file we are replacing matches no file from UpgradeFiles or DowngradeFiles
							else
							{

								// If it exists in UpgradeFiles (but is an outdated Upgrade...)
								if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]))
								{
									if (!UpdatePopupThrownAlready)
									{
										Application.Current.Dispatcher.Invoke((Action)delegate
										{
											Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Detected new Files inside your GTA Installation.\nP127 will use these as the files you revert to when Upgrading.\nDo you want me to back up your previous Upgrade - Files?");
											yesno.ShowDialog();
											if (yesno.DialogResult == true)
											{
												HelperClasses.Logger.Log("User does want it. Initiating CreateBackup()");

												LauncherLogic.KillRelevantProcesses();

												LauncherLogic.CreateBackup();
											}
											else
											{
												HelperClasses.Logger.Log("User doesnt want it. Alright then");
											}
										});
										UpdatePopupThrownAlready = true;
									}

									// Delte from $UpgradeFiles
									MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInUpgradeFiles[i], "", "We are overwriting a file which is not equal to the existing files in $Downgrade_Files and $Upgrade_Files. Deleting '" + CorrespondingFilePathInUpgradeFiles[i] + "' from $Upgrade_Files to use the existing File as new Backup", 1));
								}

								// Move to $UpgradeFiles
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], "Backing up '" + CorrespondingFilePathInGTALocation[i] + "' from GTA V Installation Path to $UpgradeFiles via Moving, since it either doenst exist there yet, or the file from $GTA_Installation_Path is a new one", 1));
							}
						}
					}

					// Creates actual Hard Link (this will further down check if we should copy based on settings in MyFileOperation.Execute())
					MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, FilesInDowngradeFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInDowngradeFiles[i] + "'", 1));
				}

				RtrnMyFileOperations = MyFileOperationsTmp;
			}
			else if (ProgressType == ProgressTypes.Upgrade)
			{
				// Saving all the File Operations I want to do, executing this at the end of this Method
				List<MyFileOperation> MyFileOperationsTmp = new List<MyFileOperation>();
				bool UpdatePopupThrownAlready = false;

				HelperClasses.Logger.Log("Initiating UPGRADE", 0);
				HelperClasses.Logger.Log("GTAV Installation Path: " + LauncherLogic.GTAVFilePath, 1);
				HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
				HelperClasses.Logger.Log("InstallationLocationBinary: " + Globals.ProjectInstallationPathBinary, 1);
				HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
				HelperClasses.Logger.Log("DowngradeFilePath: " + LauncherLogic.DowngradeFilePath, 1);
				HelperClasses.Logger.Log("UpgradeFilePath: " + LauncherLogic.UpgradeFilePath, 1);

				// Those are WITH the "\" at the end
				string[] FilesInDowngradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeFilePath);
				string[] CorrespondingFilePathInGTALocation = new string[FilesInDowngradeFiles.Length];
				string[] CorrespondingFilePathInUpgradeFiles = new string[FilesInDowngradeFiles.Length];
				List<string> ListOfFilePathsAboutToBeInUpgradeFiles = new List<string>();

				HelperClasses.Logger.Log("Found " + FilesInDowngradeFiles.Length.ToString() + " Files in Downgrade Folder.");
				HelperClasses.Logger.Log("Found " + HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.UpgradeFilePath).Length.ToString() + " Files in Upgrade Folder.");

				// Loop through all Files in Downgrade Files Folder
				for (int i = 0; i <= FilesInDowngradeFiles.Length - 1; i++)
				{
					// Update GUI
					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = ((i + 1) * 100 / FilesInDowngradeFiles.Length);
						myPB.Value = progress;
						myLBL.Content = "Gathering Information(" + (i + 1).ToString() + "/" + (FilesInDowngradeFiles.Length).ToString() + ")";
					});


					// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
					CorrespondingFilePathInGTALocation[i] = LauncherLogic.GTAVFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);
					CorrespondingFilePathInUpgradeFiles[i] = LauncherLogic.UpgradeFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);



					if (LauncherLogic.IgnoreNewFilesWhileUpgradeDowngradeLogic)
					{
						// Move to $UpgradeFiles
						MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Deleting '" + CorrespondingFilePathInGTALocation + "' from $GTAInstallationDirectory, since this is a simple Downgrade after using a Backup", 1));
					}
					// Normal Downgrade Logic
					else
					{
						// If the File exists in GTA V Installation Path
						if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
						{
							// If the File we are replacing is the same as in DowngradeFiles
							if (HelperClasses.FileHandling.AreFilesEqual(CorrespondingFilePathInGTALocation[i], FilesInDowngradeFiles[i], MySettings.Settings.EnableSlowCompare))
							{
								// Delete from GTA V Installation Path
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and its the same file as from $Downgrade_Files. Will delelte from GTA V Installation", 1));
							}

							// If the File we are replacing is the same as in UpgradeFiles
							else if (HelperClasses.FileHandling.AreFilesEqual(CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], MySettings.Settings.EnableSlowCompare))
							{
								// Delete from GTA V Installation Path
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and its the same file as from $Upgrade_Files. Will delelte from GTA V Installation", 1));
							}

							// If the file inside GTA Installation Directory matches no file from UpgradeFiles or DowngradeFiles
							else
							{
								// If it exists in UpgradeFiles (but is an outdated Upgrade...)
								if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]))
								{
									if (!UpdatePopupThrownAlready)
									{
										Application.Current.Dispatcher.Invoke((Action)delegate
										{
											Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Detected new Files inside your GTA Installation.\nP127 will use these as the files you revert to when Upgrading.\nDo you want me to back up your previous Upgrade - Files?");
											yesno.ShowDialog(); if (yesno.DialogResult == true)
											{
												HelperClasses.Logger.Log("User does want it. Initiating CreateBackup()");

												LauncherLogic.KillRelevantProcesses();

												LauncherLogic.CreateBackup();
											}
											else
											{
												HelperClasses.Logger.Log("User doesnt want it. Alright then");
											}
										});
										UpdatePopupThrownAlready = true;
									}
									// Delte from $UpgradeFiles
									MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInUpgradeFiles[i], "", "We are overwriting a file which is not equal to the existing files in $Downgrade_Files and $Upgrade_Files. Deleting '" + CorrespondingFilePathInUpgradeFiles[i] + "' from $Upgrade_Files to use the existing File as new Backup", 1));
								}

								// Move to $UpgradeFiles
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], "Backing up '" + CorrespondingFilePathInGTALocation[i] + "' from GTA V Installation Path to $UpgradeFiles via Moving, since it either doenst exist there yet, or the file from $GTA_Installation_Path is a new one", 1));
								ListOfFilePathsAboutToBeInUpgradeFiles.Add(CorrespondingFilePathInUpgradeFiles[i]);
							}
						}
					}


					// file we are moving there if file inside GTA matches no files is non existing at this point
					if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]) || ListOfFilePathsAboutToBeInUpgradeFiles.Contains(CorrespondingFilePathInUpgradeFiles[i]))
					{
						// Creates actual Hard Link (this will further down check if we should copy based on settings in MyFileOperation.Execute())
						MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, CorrespondingFilePathInUpgradeFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + CorrespondingFilePathInUpgradeFiles[i] + "'", 1));
					}

				}

				RtrnMyFileOperations = MyFileOperationsTmp;
			}
			else if (ProgressType == ProgressTypes.DidUpdateHit)
			{
				/* 
				
				This is basically a more complicated written (so more optimized) version of this:

				if (HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Upgrade_GTA5) &&
				HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Upgrade_PlayGTAV) &&
				HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Upgrade_UpdateRPF))
				{
					return false;
				}
				else
				{
						if (HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Downgrade_GTA5) &&
						HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Downgrade_PlayGTAV) &&
						HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Downgrade_UpdateRPF))
						{
							return false;
						}
					return true;
				}

				*/

				SetProgress("Checking if GTA Update hit", 1, 5);

				if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
				{
					RtrnBool = false;
					return;
				}

				string GTA_GTA5 = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\gta5.exe";
				string GTA_PlayGTAV = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\playgtav.exe";
				string GTA_UpdateRPF = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\update\update.rpf";

				string Upgrade_GTA5 = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gta5.exe";
				string Upgrade_PlayGTAV = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
				string Upgrade_UpdateRPF = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

				string Downgrade_GTA5 = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\gta5.exe";
				string Downgrade_PlayGTAV = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
				string Downgrade_UpdateRPF = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

				SetProgress("Checking if GTA Update hit", 2, 5);
				if (!HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Upgrade_GTA5, MySettings.Settings.EnableSlowCompare))
				{
					//SetProgress("Checking if Update hit", 3, 8);
					//if (!HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Downgrade_GTA5))
					//{
					RtrnBool = true;
					return;
					//}
				}

				SetProgress("Checking if GTA Update hit", 3, 5);
				if (!HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Upgrade_PlayGTAV, MySettings.Settings.EnableSlowCompare))
				{
					//SetProgress("Checking if Update hit", 5, 8);
					//if (!HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Downgrade_PlayGTAV))
					//{
					RtrnBool = true;
					return;
					//}
				}

				SetProgress("Checking if GTA Update hit", 4, 5);
				if (!HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Upgrade_UpdateRPF, MySettings.Settings.EnableSlowCompare))
				{
					//SetProgress("Checking if Update hit", 7, 8);
					//if (!HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Downgrade_UpdateRPF))
					//{
					RtrnBool = true;
					return;
					//}
				}

				SetProgress("Checking if GTA Update hit", 5, 5);
				RtrnBool = false;
				return;
			}
		}


		private void SetProgress(string pText, int x, int n)
		{
			Application.Current.Dispatcher.Invoke((Action)delegate
			{
				long progress = (100 * x / n);
				myPB.Value = progress;
				myLBL.Content = pText + " (" + x + "/" + n + ")";
			});
		}

		////////////////////////////////////////////////////////////////////
		// Below are Methods we need to make the behaviour of this nice. ///
		////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	}
}
