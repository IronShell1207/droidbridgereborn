using CommunityToolkit.WinUI.UI.Animations;
using CustomControlsLib.Abstract;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace DroidBridgeReborn.Views.Controls.Dialogs
{
	public sealed partial class BaseDialogView : UserControl, IAnimatableDialog, IDialogView
	{
		public BaseDialogView()
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