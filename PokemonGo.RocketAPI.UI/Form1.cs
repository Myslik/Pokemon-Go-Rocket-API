using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Logic;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            
            var position = new GMap.NET.PointLatLng(settings.DefaultLatitude, settings.DefaultLongitude);
            gMapControl1.Position = position;

            marker = new GMarkerGoogle(position, GMarkerGoogleType.green);
            markersOverlay.Markers.Add(marker);

            gMapControl1.Overlays.Add(markersOverlay);
        }

        Settings settings = new Settings();
        GMapOverlay markersOverlay = new GMapOverlay("markers");
        GMarkerGoogle marker;

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.SetLogger(new RichTextBoxLogger(LogLevel.Info, richTextBox1));

            Task.Run(() =>
            {
                try
                {
                    var logic = new Logic.Logic(settings);
                    logic.PlayerPositionChanged += Logic_PlayerPositionChanged;
                    logic.Execute().Wait();
                }
                catch (PtcOfflineException)
                {
                    Logger.Write("PTC Servers are probably down OR your credentials are wrong. Try google", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    Logger.Write($"Unhandled exception: {ex}", LogLevel.Error);
                }
            });
        }

        private void Logic_PlayerPositionChanged(object sender, PlayerPositionChangedEventArgs e)
        {
            this.Invoke(new UpdatePlayerPositionDelegate(UpdatePlayerPosition),
                new object[] { e.Latitude, e.Longitude });
        }

        private delegate void UpdatePlayerPositionDelegate(double latitude, double longitude);


        private void UpdatePlayerPosition( double latitude, double longitude)
        {
            var position = new GMap.NET.PointLatLng(latitude, longitude);
            gMapControl1.Position = position;
            marker.Position = position;
        }
    }
}
