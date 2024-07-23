using CustomControlsLib.Abstract;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Xaml;

namespace DroidBridgeReborn.Views.Controls.Dialogs
{
	public sealed partial class ConnectedDevicesListDialog : UserControl, IAnimatableDialog, IDialogView
	{
		public ConnectedDevicesListDialog()
		{
			this.InitializeComponent();
		}

		public void AnimateShow()
		{
			ShowingAnimationStoryboard.Begin();
		}

		public async Task AnimateCloseAsync()
		{
			await ClosingAnimationStoryboard.BeginAsync();
		}

		public void ChangeToWindowStyle()
		{
			ShadowRect.Visibility = Visibility.Collapsed;
			ShowingAnimationStoryboard.Children.Remove(ShadowShowAnim);
			ClosingAnimationStoryboard.Children.Remove(ShadowCloseAnim);
		}
	}
}