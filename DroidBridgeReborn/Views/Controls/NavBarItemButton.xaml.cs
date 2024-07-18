using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroidBridgeReborn.Views.Controls
{
	using Utils.Controls;

	public sealed partial class NavBarItemButton : TwoStateButton
	{
		/// <summary>
		/// �������� ����������� <see cref="DisabledIcon"/>
		/// </summary>
		public static readonly DependencyProperty DisabledIconProperty = DependencyProperty.Register(
			"DisabledIcon", typeof(FrameworkElement), typeof(NavBarItemButton), new PropertyMetadata(new Grid()));

		/// <summary>
		/// �������� ������
		/// </summary>
		public FrameworkElement DisabledIcon
		{
			get => (FrameworkElement)GetValue(DisabledIconProperty);
			set => SetValue(DisabledIconProperty, value);
		}

		/// <summary>
		/// �������� ����������� <see cref="EnabledIcon"/>
		/// </summary>
		public static readonly DependencyProperty EnabledIconProperty = DependencyProperty.Register(
			"EnabledIcon", typeof(FrameworkElement), typeof(NavBarItemButton), new PropertyMetadata(default(FrameworkElement)));

		/// <summary>
		/// �������� brjyb
		/// </summary>
		public FrameworkElement EnabledIcon
		{
			get => (FrameworkElement)GetValue(EnabledIconProperty);
			set => SetValue(EnabledIconProperty, value);
		}

		/// <summary>
		/// �������� ����������� <see cref="Text"/>
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text", typeof(string), typeof(NavBarItemButton), new PropertyMetadata(default(string)));

		/// <summary>
		/// �������� ������
		/// </summary>
		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}
		public NavBarItemButton()
		{
			this.InitializeComponent();
		}
	}
}