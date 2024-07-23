namespace Utils.Controls
{
	using CommunityToolkit.Mvvm.Input;
	using Microsoft.UI.Xaml.Controls;
	using Microsoft.UI.Xaml;

	/// <summary>
	/// Кнопка с двумя состояниями
	/// </summary>
	public class TwoStateButton : Button
	{
		#region Public Fields

		/// <summary>
		/// Свойство зависимости <see cref="DisabledStateContent"/>
		/// </summary>
		public static readonly DependencyProperty DisabledStateContentProperty = DependencyProperty.Register(
			"DisabledStateContent", typeof(FrameworkElement), typeof(TwoStateButton), new PropertyMetadata(default(FrameworkElement)));

		/// <summary>
		/// Свойство зависимости <see cref="EnabledStateContent"/>
		/// </summary>
		public static readonly DependencyProperty EnabledStateContentProperty = DependencyProperty.Register(
			"EnabledStateContent", typeof(FrameworkElement), typeof(TwoStateButton), new PropertyMetadata(default(FrameworkElement)));

		/// <summary>
		/// Свойство зависимости <see cref="IsAutoButton"/>
		/// </summary>
		public static readonly DependencyProperty IsAutoButtonProperty = DependencyProperty.Register(
			"IsAutoButton", typeof(bool), typeof(TwoStateButton), new PropertyMetadata(default(bool), OnIsAutoButtonChanged));

		/// <summary>
		/// Свойство зависимости <see cref="IsEnabledState"/>
		/// </summary>
		public static readonly DependencyProperty IsEnabledStateProperty = DependencyProperty.Register(
			"IsEnabledState", typeof(bool), typeof(TwoStateButton), new PropertyMetadata(default(bool), OnStateChanged));

		#endregion Public Fields

		#region Public Properties

		/// <summary>
		/// Свойство контента при неактивном состоянии
		/// </summary>
		public FrameworkElement DisabledStateContent {
			get { return (FrameworkElement)GetValue(DisabledStateContentProperty); }
			set { SetValue(DisabledStateContentProperty, value); }
		}

		/// <summary>
		/// Свойство контента при активном состоянии
		/// </summary>
		public FrameworkElement EnabledStateContent {
			get { return (FrameworkElement)GetValue(EnabledStateContentProperty); }
			set { SetValue(EnabledStateContentProperty, value); }
		}

		/// <summary>
		/// Свойство автоматическая ли кнопка.
		/// </summary>
		public bool IsAutoButton {
			get { return (bool)GetValue(IsAutoButtonProperty); }
			set { SetValue(IsAutoButtonProperty, value); }
		}

		/// <summary>
		/// Свойство активности кнопки
		/// </summary>
		public bool IsEnabledState {
			get { return (bool)GetValue(IsEnabledStateProperty); }
			set { SetValue(IsEnabledStateProperty, value); }
		}

		#endregion Public Properties

		#region Public Constructors

		/// <summary>
		/// Инициализирует экземпляр <see cref="TwoStateButton"/>.
		/// </summary>
		public TwoStateButton()
		{
			this.DefaultStyleKey = typeof(Button);
			Loaded += TwoStateButton_Loaded; PointerEntered += TwoStateButton_PointerEntered;
			PointerExited += TwoStateButton_PointerExited;
			PointerReleased += TwoStateButton_PointerReleased;
			LostFocus += TwoStateButton_LostFocus;
			PointerPressed += TwoStateButton_PointerPressed;
			KeyDown += TwoStateButton_KeyDown;
			PointerCaptureLost += TwoStateButton_PointerCaptureLost;
		}

		private void TwoStateButton_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (IsEnabledState)
			{
				VisualStateManager.GoToState(this, "SelectedPressed", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "Pressed", true);
			}
		}

		private void TwoStateButton_PointerCaptureLost(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			UpdateVisualState();
		}

		private void TwoStateButton_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			if (IsEnabledState)
			{
				VisualStateManager.GoToState(this, "SelectedPressed", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "Pressed", true);
			}
		}

		private void TwoStateButton_LostFocus(object sender, RoutedEventArgs e)
		{
			UpdateVisualState();
		}

		private void TwoStateButton_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			UpdateVisualState();
		}

		private void TwoStateButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			UpdateVisualState();
		}

		private void TwoStateButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			if (IsEnabledState)
			{
				VisualStateManager.GoToState(this, "SelectedPointerOver", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "PointerOver", true);
			}
		}

		#endregion Public Constructors

		#region Private Methods

		/// <summary>
		/// Обрабатывает изменение свойства <see cref="IsAutoButtonProperty"/>
		/// </summary>
		private static void OnIsAutoButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TwoStateButton btn)
			{
				if (btn.IsAutoButton)
				{
					btn.Command = new RelayCommand(btn.SwitchBindedPropertyState);
				}
				else
				{
					btn.Command = null;
				}
			}
		}

		/// <summary>
		/// Обработка смены состояния.
		/// </summary>
		private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TwoStateButton sender && sender.EnabledStateContent != null && sender.DisabledStateContent != null)
			{
				sender.Content = sender.IsEnabledState ? sender.EnabledStateContent : sender.DisabledStateContent;
				sender.UpdateVisualState();
			}
		}

		private void UpdateVisualState()
		{
			if (IsEnabledState)
			{
				VisualStateManager.GoToState(this, "Selected", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", true);
			}
		}

		/// <summary>
		/// Переключает статус выбранности кнопки.
		/// </summary>
		private void SwitchBindedPropertyState()
		{
			IsEnabledState = !IsEnabledState;
		}

		/// <summary>
		/// Обрабатывает загрузку.
		/// </summary>
		private void TwoStateButton_Loaded(object sender, RoutedEventArgs e)
		{
			if (EnabledStateContent != null && DisabledStateContent != null)
			{
				Content = this.IsEnabledState ? this.EnabledStateContent : this.DisabledStateContent;
			}
		}

		#endregion Private Methods
	}
}
