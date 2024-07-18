
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DrStrange.Controls
{
    /// <summary>
    /// Представляет расширенный контрол <see cref="ToggleButton"/>.
    /// </summary>
    public sealed class ImageToggleButton : ToggleButton
    {
        #region Public Fields

        /// <summary>
        /// Иконка кнопки, когда кнопка находится в нажатом состоянии.
        /// </summary>
        public static readonly DependencyProperty ActiveImageProperty = DependencyProperty.Register(
            nameof(ActiveImage),
            typeof(BitmapImage),
            typeof(ImageToggleButton),
            new PropertyMetadata(null));

        /// <summary>
        /// Иконка кнопки по умолчанию.
        /// </summary>
        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register(
            nameof(DefaultImage),
            typeof(BitmapImage),
            typeof(ImageToggleButton),
            new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Иконка кнопки, когда кнопка находится в нажатом состоянии.
        /// </summary>
        public BitmapImage ActiveImage
        {
            get => (BitmapImage)GetValue(ActiveImageProperty);
            set => SetValue(ActiveImageProperty, value);
        }

        /// <summary>
        /// Иконка кнопки по умолчанию.
        /// </summary>
        public BitmapImage DefaultImage
        {
            get => (BitmapImage)GetValue(DefaultImageProperty);
            set => SetValue(DefaultImageProperty, value);
        }

        #endregion Public Properties
    }
}