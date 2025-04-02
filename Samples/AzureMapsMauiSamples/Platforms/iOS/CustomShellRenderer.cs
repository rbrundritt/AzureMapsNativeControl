using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;

namespace AzureMapsMauiSamples.Platforms
{
    internal class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer()
        {

        }
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomSectionRenderer(this);
        }


        IShellFlyoutRenderer flyoutRenderer;

        protected override IShellFlyoutRenderer CreateFlyoutRenderer()
        {
            flyoutRenderer = base.CreateFlyoutRenderer();
            return flyoutRenderer;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var type = flyoutRenderer.GetType();
            var property = type.GetProperty("PanGestureRecognizer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = property.GetValue(flyoutRenderer);

            UIPanGestureRecognizer recognizer = value as UIPanGestureRecognizer;
            recognizer.Enabled = false;
        }

        public class CustomSectionRenderer : ShellSectionRenderer
        {
            public CustomSectionRenderer(IShellContext context) : base(context)
            {

            }
            public override void ViewDidLoad()
            {
                base.ViewDidLoad();
                InteractivePopGestureRecognizer.Enabled = false;
            }
        }
    }
}
