using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Friday.AudioPlayer;
using Friday.Dsp;
using Friday.Spectrum;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Friday
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        #region Properties

        LineSpectrum _lineSpectrum;

        public LineSpectrum LineSpectrum
        {
            get => _lineSpectrum;
            set
            {
                _lineSpectrum = value;
                RaisePropertyChanged("LineSpectrum");
            }
        }

        #endregion

        private IAudioProvider _audioProvider;

        private readonly FftSize fftSize = FftSize.Fft4096;

        public MainPage()
        {
            InitializeComponent();

            _audioProvider = new BassAudioPlayer();

            //linespectrum and voiceprint3dspectrum used for rendering some fft data
            //in oder to get some fft data, set the previously created spectrumprovider 
            LineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = _audioProvider,
                UseAverage = true,
                BarCount = 200,
                BarSpacing = 1,
                IsXLogScale = false,
                ScalingStrategy = ScalingStrategy.Sqrt,
                MinimumFrequency = 20,
                MaximumFrequency = 20000
            };
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await OnLoadAsync();
        }

        private async Task OnLoadAsync()
        {
            var audioFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                   { DevicePlatform.iOS, new[]
                       {
                           "public.audio",
                           "public.mp3",
                           "public.mpeg-4-audio",
                           "com.apple.protected-​mpeg-4-audio",
                           "public.ulaw-audio",
                           "public.aifc-audio",
                           "public.aiff-audio",
                           "com.apple.coreaudio-​format",
                           "com.microsoft.waveform-​audio",
                           "aac",
                           "aiff",
                           "au",
                           "caf",
                           "flac",
                           "m4a",
                           "mp2",
                           "mp3",
                           "oga",
                           "ogg",
                           "opus",
                           "spx",
                           "wav",
                           "wma"
                       }
                   },
                   { DevicePlatform.Android, new[]
                       {
						   //"application/octet-stream", // caf and other audio extensions on some Android devices
						   //"audio/*",
                           "audio/aac",
                           "audio/x-aac",
                           "audio/ac3",
                           "audio/aiff",
                           "audio/x-aiff",
                           "audio/basic",
                           "audio/x-caf",
                           "audio/flac",
                           "audio/x-flac",
                           "audio/x-matroska",
                           "audio/m4a",
                           "audio/x-m4a",
                           "audio/mpeg",
                           "audio/mp3",
                           "audio/mpeg3",
                           "audio/x-mpeg-3",
                           "audio/ogg",
                           "audio/ogg; codecs=opus",
                           "application/x-ogg",
                           "audio/wav",
                           "audio/x-wav",
                           "audio/x-ms-wma"
                       }
                   },
                });
            var pickOptions = new PickOptions
            {
                PickerTitle = "Select your audio",
                FileTypes = audioFileTypes
            };
            var fileData = await FilePicker.PickAsync(pickOptions);

            if (fileData == null) return;

            fileNameLabel.Text = fileData.FileName;

            _audioProvider.CurrentPlayingFile = fileData.FullPath;

            if (_audioProvider.IsPlaying)
                _audioProvider.Stop();

            await _audioProvider.Play();
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}