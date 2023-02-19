using System;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Friday.Spectrum
{
    public class LineSpectrumView : SKCanvasView
    {
        #region LineSpectrumProperty

        public static readonly BindableProperty LineSpectrumProperty =
            BindableProperty.Create(
                nameof(LineSpectrum),
                typeof(LineSpectrum),
                typeof(LineSpectrumView),
                null,
                propertyChanged: OnPropertyChanged);

        public LineSpectrum LineSpectrum
        {
            get => (LineSpectrum)GetValue(LineSpectrumProperty);
            set => SetValue(LineSpectrumProperty, value);
        }

        #endregion

        public LineSpectrumView()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                this.InvalidateSurface();
                return true;
            });
        }

        static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var lineSpectrumView = bindable as LineSpectrumView;
            lineSpectrumView?.InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            if (LineSpectrum != null && this.IsEnabled)
            {
                var size = new Size(args.Info.Width, args.Info.Height);
                LineSpectrum.CreateSpectrumLine(args.Surface, size);
            }
        }
    }
}