using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;

namespace Assets.SimpleFileBrowserForWindows
{
    public static class WindowsFileBrowser
    {
#if UNITY_EDITOR

        /// <summary>
        /// Open file dialog and read bytes.
        /// [extensions] should start with the period, ".".
        /// [callback] return (bool) success, (string) path and (byte[]) bytes.
        /// </summary>
        public static IEnumerator OpenFile(string title, string initialDirectory, string fileType, IEnumerable<string> extensions, Action<bool, string, byte[]> callback)
        {
            var path = UnityEditor.EditorUtility.OpenFilePanel(title, initialDirectory, string.Join(",", extensions).Replace(".", null));

            if (!string.IsNullOrEmpty(path))
            {
                var bytes = File.ReadAllBytes(path);

                callback(true, path, bytes);
            }
            else
            {
                callback(false, null, null);
            }

            yield break;
        }


        /// <summary>
        /// Open folder dialog.
        /// [callback] returns (bool) success and (string) folderPath.
        /// </summary>
        public static IEnumerator OpenFolder(string title, string initialDirectory, Action<bool, string> callback)
        {
            var folderPath = UnityEditor.EditorUtility.OpenFolderPanel(title, initialDirectory, "");

            if (!string.IsNullOrEmpty(folderPath))
            {
                callback(true, folderPath);
            }
            else
            {
                callback(false, null);
            }

            yield break;
        }

        /// <summary>
        /// Open file dialog and write bytes.
        /// [extension] should start with the period, ".".
        /// [callback] returns (bool) success and (string) path.
        /// </summary>
        public static IEnumerator SaveFile(string title, string initialDirectory, string fileName, string fileType, string extension, IEnumerable<string> contents, Action<bool, string> callback)
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel(title, initialDirectory, fileName, extension.Replace(".", null));

            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllLines(path, contents);
                callback(true, path);
            }
            else
            {
                callback(false, null);
            }

            yield break;
        }

#elif UNITY_STANDALONE_WIN

			[DllImport("user32.dll")]
            private static extern IntPtr GetActiveWindow();

            private class WindowWrapper : System.Windows.Forms.IWin32Window
            {
                public WindowWrapper(IntPtr handle) { Handle = handle; }
                public IntPtr Handle { get; }
            }

            /// <summary>
            /// Open file dialog and read bytes.
            /// [extensions] should start with the period, ".".
            /// [callback] return (bool) success, (string) path and (byte[]) bytes.
            /// </summary>
            public static IEnumerator OpenFile(string title, string initialDirectory, string fileType, IEnumerable<string> extensions, Action<bool, string, byte[]> callback)
            {
                var extension = string.Join(";", extensions.Select(i => "*" + i));
                var dialog = new Ookii.Dialogs.VistaOpenFileDialog
                {
                    Title = title,
                    InitialDirectory = Directory.Exists(initialDirectory) ? initialDirectory : null,
                    Filter = $"{fileType} ({extension})|{extension}"
                };
                var result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow()));

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var path = dialog.FileName;

                    callback(true, path, File.ReadAllBytes(path));
                }
                else
                {
                    callback(false, null, null);
                }

                yield break;
            }

            /// <summary>
            /// Open folder dialog and return the selected folder path.
            /// [callback] returns (bool) success and (string) folder path.
            /// </summary>
            public static IEnumerator OpenFolder(string title, string initialDirectory, Action<bool, string> callback)
            {
                var dialog = new Ookii.Dialogs.VistaFolderBrowserDialog
                {
                    Description = title,
                    SelectedPath = initialDirectory
                };
                var result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow()));

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var folderPath = dialog.SelectedPath;
                    callback(true, folderPath);
                }
                else
                {
                    callback(false, null);
                }

                yield break;
            }

            /// <summary>
            /// Open file dialog and write bytes.
            /// [extension] should start with the period, ".".
            /// [callback] returns (bool) success and (string) path.
            /// </summary>
            public static IEnumerator SaveFile(string title, string initialDirectory, string fileName, string fileType, string extension, IEnumerable<string> contents, Action<bool, string> callback)
            {
                var dialog = new Ookii.Dialogs.VistaSaveFileDialog
                {
                    Title = title,
                    InitialDirectory = Directory.Exists(initialDirectory) ? initialDirectory : null,
                    FileName = fileName + extension,
                    Filter = $"{fileType} (*{extension})|*{extension}"
                };
                var result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow()));

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var path = dialog.FileName;

                    File.WriteAllLines(path, contents);
                    callback(true, path);
                }
                else
                {
                    callback(false, null);
                }

                yield break;
            }
	
#elif UNITY_WSA

				        /// <summary>
				        /// Open file dialog and read bytes.
				        /// [extensions] should start with the period, ".".
				        /// [callback] return (bool) success, (string) path and (byte[]) bytes.
				        /// </summary>
				        public static IEnumerator OpenFile(string title, string initialDirectory, string fileType, IEnumerable<string> extensions, Action<bool, string, byte[]> callback)
				        {
					        var opened = false;
					        string path = null;
					        byte[] bytes = null;
					        var pickerClosed = false;

					        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
					        {
						        var filePicker = new Windows.Storage.Pickers.FileOpenPicker
						        {
							        SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
						        }; // If you have an error on this line, just switch Unity to Android and then switch back to UWP. This helped me for Unity 2018.1.

						        foreach (var extension in extensions)
						        {
							        filePicker.FileTypeFilter.Add(extension);
						        }

						        var file = await filePicker.PickSingleFileAsync();

						        if (file != null)
						        {
							        path = file.Path;

							        var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);

							        bytes = new byte[buffer.Length];

							        using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
							        {
								        dataReader.ReadBytes(bytes);
							        }

							        opened = true;
						        }

						        pickerClosed = true;
					        }, false);

					        while (!pickerClosed)
					        {
						        yield return null;
					        }

					        callback(opened, path, bytes);
				        }

				        /// <summary>
				        /// Open file dialog and write bytes.
				        /// [extension] should start with the period, ".".
				        /// [callback] returns (bool) success and (string) path.
				        /// </summary>
				        public static IEnumerator SaveFile(string title, string initialDirectory, string defaultName, string fileType, string extension, byte[] bytes, Action<bool, string> callback)
				        {
					        var saved = false;
					        string path = null;
					        var pickerClosed = false;

					        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
					        {
						        var savePicker = new Windows.Storage.Pickers.FileSavePicker(); // If you have an error on this line, just switch Unity to Android and then switch back to UWP. This helped me for Unity 2018.1.

						        savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
						        savePicker.SuggestedFileName = defaultName;
						        savePicker.FileTypeChoices.Add(fileType, new List<string> { extension });

						        var file = await savePicker.PickSaveFileAsync();

						        if (file != null)
						        {
							        path = file.Path;
							        Windows.Storage.CachedFileManager.DeferUpdates(file);

							        var buffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(bytes);

							        await Windows.Storage.FileIO.WriteBufferAsync(file, buffer);

							        var status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

							        saved = status == Windows.Storage.Provider.FileUpdateStatus.Complete;
						        }

						        pickerClosed = true;
					        }, false);

					        while (!pickerClosed)
					        {
						        yield return null;
					        }

					        callback(saved, path);
				        }

#else

				        public static IEnumerator OpenFile(string title, string directory, string fileType, IEnumerable<string> extensions, Action<bool, string, byte[]> callback)
				        {
					        throw new NotSupportedException();
				        }

				        public static IEnumerator SaveFile(string title, string directory, string defaultName, string fileType, string extension, byte[] bytes, Action<bool, string> callback)
				        {
					        throw new NotSupportedException();
				        }

#endif
    }
}