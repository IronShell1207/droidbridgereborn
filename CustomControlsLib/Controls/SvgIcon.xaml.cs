using CommunityToolkit.WinUI.Helpers;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Serilog;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using CustomControlsLib.Helpers;
using Microsoft.UI;

namespace CustomControlsLib.Controls
{
	using Microsoft.UI.Xaml.Controls;
	using System.IO;
	using System.Text;

	public sealed partial class SvgIcon : UserControl
	{
		/// <summary>
		/// Свойство зависимости <see cref="FillColor"/>
		/// </summary>
		public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register(
			"FillColor", typeof(Color), typeof(SvgIcon), new PropertyMetadata(Colors.White, OnImagePathChanged));

		/// <summary>
		/// Свойство зависимости <see cref="ImagePath"/>
		/// </summary>
		public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
			"ImagePath", typeof(string), typeof(SvgIcon), new PropertyMetadata(default(string), OnImagePathChanged));

		/// <summary>
		/// Свойство зависимости <see cref="StrokeColor"/>
		/// </summary>
		public static readonly DependencyProperty StrokeColorProperty = DependencyProperty.Register(
			"StrokeColor", typeof(Color), typeof(SvgIcon), new PropertyMetadata(Colors.White, OnImagePathChanged));

		/// <summary>
		/// Свойство зависимости <see cref="ImageSource"/>
		/// </summary>
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			"ImageSource", typeof(SvgImageSource), typeof(SvgIcon), new PropertyMetadata(null, OnImagePathChanged));

		public SvgIcon()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Свойство fill
		/// </summary>
		public Color FillColor {
			get => (Color)GetValue(FillColorProperty);
			set => SetValue(FillColorProperty, value);
		}

		/// <summary>
		/// Свойство пути к картинке
		/// </summary>
		public string ImagePath {
			get => (string)GetValue(ImagePathProperty);
			set => SetValue(ImagePathProperty, value);
		}

		/// <summary>
		/// Свойство источника
		/// </summary>
		public SvgImageSource ImageSource {
			get => (SvgImageSource)GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}

		/// <summary>
		/// Свойство stroke
		/// </summary>
		public Color StrokeColor {
			get => (Color)GetValue(StrokeColorProperty);
			set => SetValue(StrokeColorProperty, value);
		}

		private static async void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SvgIcon iconControl)
			{
				await iconControl.RedrawImage();
			}
		}

		private async Task RedrawImage()
		{
			try
			{
				string imagePath = ImageSource != null ? ImageSource.UriSource.AbsoluteUri : ImagePath;
				if (string.IsNullOrWhiteSpace(imagePath))
					throw new ArgumentNullException();

				StorageFile imageFile;

				if (imagePath.StartsWith("ms-appx"))
					imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(imagePath));
				else
					imageFile = await StorageFile.GetFileFromPathAsync(imagePath);

				if (imageFile == null)
					throw new ArgumentNullException();

				string svgContent = await FileIO.ReadTextAsync(imageFile);
				svgContent = svgContent.Replace("{strokecolor}", StrokeColor.ToHexWithoutOpacity());
				svgContent = svgContent.Replace("{fillcolor}", FillColor.ToHexWithoutOpacity());

				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent)))
				{
					var svgImageSource = new SvgImageSource();
					await svgImageSource.SetSourceAsync(stream.AsRandomAccessStream());
					// Assign the SvgImageSource to an Image control
					_image.Source = svgImageSource;
				}
			}
			catch (ArgumentNullException)
			{
				_image.Source = new BitmapImage();
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
				_image.Source = new BitmapImage();
			}
		}
	}
}