namespace Utils.Helpers
{
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Controls;
	using Microsoft.UI.Xaml.Data;
	using System;

	public partial class TextBlockAttach
	{
		#region Public Fields

		public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.RegisterAttached(
			"CharacterCasing",
			typeof(CharacterCasing),
			typeof(TextBlock),
			new PropertyMetadata(
				CharacterCasing.Normal,
				OnCharacterCasingChanged));

		#endregion Public Fields

		#region Private Fields

		private static readonly PropertyPath TextPropertyPath = new PropertyPath("Text");

		private static readonly DependencyProperty TextProxyProperty = DependencyProperty.RegisterAttached(
			"TextProxy",
			typeof(string),
			typeof(TextBlock),
			new PropertyMetadata(default(string), OnTextProxyChanged));

		#endregion Private Fields

		#region Public Methods

		public static CharacterCasing GetCharacterCasing(DependencyObject element)
		{
			return (CharacterCasing)element.GetValue(CharacterCasingProperty);
		}

		public static void SetCharacterCasing(DependencyObject element, CharacterCasing value)
		{
			element.SetValue(CharacterCasingProperty, value);
		}

		#endregion Public Methods

		#region Private Methods

		private static void OnCharacterCasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBlock textBlock)
			{
				UpdateText(textBlock, textBlock.Text);
				if (textBlock.GetBindingExpression(TextProxyProperty) == null)
				{
					BindingOperations.SetBinding(
						textBlock,
						TextProxyProperty,
						new Binding
						{
							Path = TextPropertyPath,
							RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self },
							Mode = BindingMode.OneWay,
						});
				}
			}
		}

		private static void OnTextProxyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBlock textBlock)
			{
				UpdateText(textBlock, (string)e.NewValue);
			}
		}

		private static void UpdateText(TextBlock textBlock, string newText)
		{
			textBlock.SetValue(TextBlock.TextProperty, Format(newText, GetCharacterCasing(textBlock)));

			string Format(string text, CharacterCasing casing)
			{
				if (string.IsNullOrEmpty(text))
				{
					return text;
				}

				switch (casing)
				{
					case CharacterCasing.Normal:
						return text;

					case CharacterCasing.Lower:
						return text.ToLower();

					case CharacterCasing.Upper:
						return text.ToUpper();

					default:
						throw new ArgumentOutOfRangeException(nameof(casing), casing, null);
				}
			}
		}

		#endregion Private Methods
	}
}