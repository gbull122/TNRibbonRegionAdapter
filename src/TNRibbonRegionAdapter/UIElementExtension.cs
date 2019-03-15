using System.Windows;

namespace TNRibbonRegionAdapter
{

    public static class UIElementExtension
	{
		public static readonly DependencyProperty MergeOrderProperty = DependencyProperty.RegisterAttached("MergeOrder", typeof(double), typeof(UIElementExtension));

		public static void SetMergeOrder(UIElement element, double value)
		{
			element.SetValue(MergeOrderProperty, value);
		}

		public static double GetMergeOrder(UIElement element)
		{
			return (double)element.GetValue(MergeOrderProperty);
		}

	}
}
