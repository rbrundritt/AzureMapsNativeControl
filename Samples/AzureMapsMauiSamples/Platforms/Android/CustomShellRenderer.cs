using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace AzureMapsMauiSamples.Platforms
{
    internal class CustomShellRenderer : ShellRenderer
    {
        protected override IShellFlyoutRenderer CreateShellFlyoutRenderer()
        {
            var flyoutRenderer = base.CreateShellFlyoutRenderer();
            flyoutRenderer.AndroidView.Touch += AndroidView_Touch;

            return flyoutRenderer;
        }

        private void AndroidView_Touch(object? sender, global::Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Move)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomShellSectionRenderer(this);
        }
    }

    public class CustomShellSectionRenderer : ShellSectionRenderer
    {
        public CustomShellSectionRenderer(IShellContext shellContext) : base(shellContext)
        {
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var result = base.OnCreateView(inflater, container, savedInstanceState);
            SetViewPager2UserInputEnabled(false);
            return result;
        }
        protected override void SetViewPager2UserInputEnabled(bool value)
        {
            base.SetViewPager2UserInputEnabled(false);
        }
    }
}
