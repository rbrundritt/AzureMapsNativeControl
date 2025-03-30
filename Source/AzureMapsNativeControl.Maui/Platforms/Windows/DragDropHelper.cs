using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using DataPackageOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation;
using DragEventArgs = Microsoft.UI.Xaml.DragEventArgs;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using DragStartingEventArgs = Microsoft.UI.Xaml.DragStartingEventArgs;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;

namespace AzureMapsNativeControl.Platforms
{
    /// <summary>
    /// Helper class for drag and drop functionality in Windows.
    /// </summary>
    internal static class DragDropHelper
    {
        private static readonly Dictionary<UIElement, DragEventHandler> DragEventHandlers = new();
        public static void RegisterDrop(UIElement element, Action<IList<MapFileStream>> content)
        {
            if (content is null)
            {
                return;
            }

            element.AllowDrop = true;
            async void DropHandler(object s, DragEventArgs e)
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    List<MapFileStream> files = new();

                    var items = await e.DataView.GetStorageItemsAsync();
                    foreach (var item in items)
                    {
                        if (item is StorageFile file)
                        {
                            var text = await FileIO.ReadTextAsync(file);
                            var bytes = Encoding.Default.GetBytes(text);

                            files.Add(new MapFileStream(new MemoryStream(bytes), file.ContentType, null, null, file.Name));
                        }
                    }

                    if (files.Count > 0)
                    {
                        content?.Invoke(files);
                    }
                }
            }

            element.Drop += DropHandler;
            DragEventHandlers[element] = DropHandler;
            element.DragOver += OnDragOver;
        }

        public static void UnRegisterDrop(UIElement element)
        {
            element.AllowDrop = false;
            if (DragEventHandlers.TryGetValue(element, out var dragEventHandler))
            {
                element.Drop -= dragEventHandler;
                DragEventHandlers.Remove(element);
            }

            element.DragOver -= OnDragOver;
        }

        private static async void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var deferral = e.GetDeferral();
                var isAllowed = false;
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        isAllowed = true;
                        break;
                    }
                }

                e.AcceptedOperation = isAllowed ? DataPackageOperation.Copy : DataPackageOperation.None;
                deferral.Complete();
            }

            e.AcceptedOperation = DataPackageOperation.None;
        }
    }
}