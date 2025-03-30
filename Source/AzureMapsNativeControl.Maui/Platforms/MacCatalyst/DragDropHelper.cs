using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using CoreFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace AzureMapsNativeControl.Platforms
{
    public static class DragDropHelper
    {
        public static void RegisterDrop(UIView view, Action<IList<MapFileStream>> content)
        {
            var dropInteraction = new UIDropInteraction(new DropInteractionDelegate()
            {
                Content = (files) =>
                {
                    if (files.Count > 0)
                    {
                        content(files);
                    }
                }
            });
            view.AddInteraction(dropInteraction);
        }

        public static void UnRegisterDrop(UIView view)
        {
            var dropInteractions = view.Interactions.OfType<UIDropInteraction>();
            foreach (var interaction in dropInteractions)
            {
                view.RemoveInteraction(interaction);
            }
        }
    }

    class DropInteractionDelegate : UIDropInteractionDelegate
    {
        public Action<IList<MapFileStream>>? Content { get; init; }

        public override UIDropProposal SessionDidUpdate(UIDropInteraction interaction, IUIDropSession session)
        {
            return new UIDropProposal(UIDropOperation.Copy);
        }

        public override void PerformDrop(UIDropInteraction interaction, IUIDropSession session)
        {
            if (Content is null)
            {
                return;
            }

            List<MapFileStream> files = new();

            foreach (var item in session.Items)
            {
                item.ItemProvider.LoadItem(UniformTypeIdentifiers.UTTypes.Json.Identifier, null, async (data, error) =>
                {
                    if (data is NSUrl nsData && !string.IsNullOrEmpty(nsData.Path))
                    {
                        var bytes = await File.ReadAllBytesAsync(nsData.Path);

                        //Try and get the mime type from file informatino. 
                        string? mimeType = null;

                        if (Utils.TryGetMimeType(nsData.PathExtension, out var mt))
                        {
                            mimeType = mt;
                        }
                        else if(Utils.TryGetMimeType(nsData.Path, out var mt2))
                        {
                            mimeType = mt2;
                        }
                        else
                        {
                            mimeType = "text/plain";
                        }

                        files.Add(new MapFileStream(new MemoryStream(bytes), mimeType, null, null, nsData.LastPathComponent));
                    }
                });
            }

            if (files.Count > 0)
            {
                Content?.Invoke(files);
            }
        }
    }
}