using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace FuelDiary.Models
{
    public class PhoneOrientationTrigger : StateTriggerBase
    {
        private bool isLandscape;

        public PhoneOrientationTrigger()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public bool IsLandscape
        {
            get
            {
                return isLandscape;
            }
            set
            {
                isLandscape = value;
            }
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var currentview = ApplicationView.GetForCurrentView();
            if (currentview.Orientation == ApplicationViewOrientation.Portrait)
            {
                SetActive(!IsLandscape);
            }
            else if (currentview.Orientation == ApplicationViewOrientation.Landscape)
            {
                SetActive(IsLandscape);
            }
        }
    }
}
