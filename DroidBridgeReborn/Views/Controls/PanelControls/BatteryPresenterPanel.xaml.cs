using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace DroidBridgeReborn.Views.Controls.PanelControls
{
	public sealed partial class BatteryPresenterPanel : UserControl
	{
		/// <summary>
		/// Свойство зависимости <see cref="BatteryLevelPercent"/>
		/// </summary>
		public static readonly DependencyProperty BatteryLevelPercentProperty = DependencyProperty.Register(
			"BatteryLevelPercent", typeof(double), typeof(BatteryPresenterPanel), new PropertyMetadata(default(double)));

		/// <summary>
		/// Свойство зависимости <see cref="IsCharging"/>
		/// </summary>
		public static readonly DependencyProperty IsChargingProperty = DependencyProperty.Register(
			"IsCharging", typeof(bool), typeof(BatteryPresenterPanel), new PropertyMetadata(default(bool)));

		/// <summary>
		/// Свойство зависимости <see cref="IsWirelessCharging"/>
		/// </summary>
		public static readonly DependencyProperty IsWirelessChargingProperty = DependencyProperty.Register(
			"IsWirelessCharging", typeof(bool), typeof(BatteryPresenterPanel), new PropertyMetadata(default(bool)));

		public BatteryPresenterPanel()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Свойство процента батареи
		/// </summary>
		public double BatteryLevelPercent {
			get => (double)GetValue(BatteryLevelPercentProperty);
			set => SetValue(BatteryLevelPercentProperty, value);
		}

		/// <summary>
		/// Свойство заряжается ли
		/// </summary>
		public bool IsCharging {
			get => (bool)GetValue(IsChargingProperty);
			set => SetValue(IsChargingProperty, value);
		}

		/// <summary>
		/// Свойство заряжается ли по беспроводу.
		/// </summary>
		public bool IsWirelessCharging {
			get => (bool)GetValue(IsWirelessChargingProperty);
			set => SetValue(IsWirelessChargingProperty, value);
		}

	}
}