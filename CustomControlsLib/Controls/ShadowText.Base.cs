using System.Numerics;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Windows.UI;
using Windows.UI.Text;
using Microsoft.UI.Xaml.Controls;

namespace CustomControlsLib.Controls
{
    public sealed partial class ShadowText
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ShadowText), new PropertyMetadata("", OnPropertyChangedCallback));

        public static DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(ShadowText), new PropertyMetadata(12.0, OnPropertyChangedCallback));
        public static DependencyProperty FontWeightProperty = DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(ShadowText), new PropertyMetadata(FontWeights.Normal, OnPropertyChangedCallback));
        public static DependencyProperty ShadowColorProperty = DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(ShadowText), new PropertyMetadata(Color.FromArgb(255, 190, 87, 199), OnPropertyChangedCallback));
        public static DependencyProperty ShadowRadiusProperty = DependencyProperty.Register(nameof(ShadowRadius), typeof(float), typeof(ShadowText), new PropertyMetadata(20f, OnPropertyChangedCallback));
        public static DependencyProperty ShadowOpacityProperty = DependencyProperty.Register(nameof(ShadowOpacity), typeof(float), typeof(ShadowText), new PropertyMetadata(0.1f, OnPropertyChangedCallback));
 
        public static readonly DependencyProperty CasingProperty = DependencyProperty.Register(
	        "Casing", typeof(CharacterCasing), typeof(ShadowText), new PropertyMetadata(default(CharacterCasing), OnPropertyChangedCallback));

        public CharacterCasing Casing
        {
	        get => (CharacterCasing)GetValue(CasingProperty);
	        set => SetValue(CasingProperty, value);
        }

        public string Text
        {
	        get => (string)this.GetValue(TextProperty);
	        set => this.SetValue(TextProperty, value);
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShadowText control)
            {
                control.MakeShadow();
            }
        }

        public double FontSize
        {
            get => (double)this.GetValue(FontSizeProperty);
            set => this.SetValue(FontSizeProperty, value);
        }

        public FontWeight FontWeight
        {
            get => (FontWeight)this.GetValue(FontWeightProperty);
            set => this.SetValue(FontWeightProperty, value);
        }

        public Color ShadowColor
        {
            get => (Color)this.GetValue(ShadowColorProperty);
            set => this.SetValue(ShadowColorProperty, value);
        }

        public float ShadowRadius
        {
            get => (float)this.GetValue(ShadowRadiusProperty);
            set => this.SetValue(ShadowRadiusProperty, value);
        }

        public float ShadowOpacity
        {
            get => (float)this.GetValue(ShadowOpacityProperty);
            set => this.SetValue(ShadowOpacityProperty, value);
        }

        private void MakeShadow()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this.Host).Compositor;

            var dropShadow = compositor.CreateDropShadow();
            dropShadow.Color = this.ShadowColor;
            dropShadow.BlurRadius = this.ShadowRadius;
            dropShadow.Opacity = this.ShadowOpacity;

            var mask = this.TextBlock.GetAlphaMask();
            dropShadow.Mask = mask;

            var spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Size = new Vector2((float)this.Host.ActualWidth, (float)this.Host.ActualHeight);
            spriteVisual.Shadow = dropShadow;
            ElementCompositionPreview.SetElementChildVisual(this.Host, spriteVisual);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.MakeShadow();
        }

        public ShadowText()
        {
            this.InitializeComponent();
        }

        private void ShadowText_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MakeShadow();
        }
    }
}