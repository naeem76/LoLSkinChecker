using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace LoLSkinChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Client myClient;
        public MainWindow()
        {
            InitializeComponent();



        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLoginDialog();
        }


        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            skinListView.Items.Clear();
            newBtn.IsEnabled = false;
            ShowLoginDialog();
        }

        private async void ShowLoginDialog()
        {
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps" });
            if (result == null)
            {
                newBtn.IsEnabled = true;
            }
            else
            {
                string server = await this.ShowInputAsync("Server", "Enter the server: EUW,EUN,KR,NA,BR,LAN,LAS,OCE,TR,RU", new MetroDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme });
                if (parseServer(server) == PVPNetConnect.Region.PH)
                {
                    newBtn.IsEnabled = true;
                }
                else
                {
                    myClient = new Client(parseServer(server), result.Username, result.Password);
                    myClient.onLoading += myClient_onLoading;
                    myClient.onCompleteLoad += myClient_onCompleteLoad;
                }
                
            }
            /*myClient = new Client(PVPNetConnect.Region.EUW, "imkoreann", "imkoreann12345");
            myClient.onLoading += myClient_onLoading;
            myClient.onCompleteLoad += myClient_onCompleteLoad;*/
        }

        private PVPNetConnect.Region parseServer(string str)
        {
            switch (str)
            {
                case "EUW":
                    return PVPNetConnect.Region.EUW;
                case "EUN":
                    return PVPNetConnect.Region.EUN;
                case "KR":
                    return PVPNetConnect.Region.KR;
                case "BR":
                    return PVPNetConnect.Region.BR;
                case "LAS":
                    return PVPNetConnect.Region.LAS;
                case "LAN":
                    return PVPNetConnect.Region.LAN;
                case "OCE":
                    return PVPNetConnect.Region.OCE;
                case "TR":
                    return PVPNetConnect.Region.TR;
                case "RU":
                    return PVPNetConnect.Region.RU;
                default:
                    return PVPNetConnect.Region.PH;
            }
        }

        private void myClient_onCompleteLoad(object sender, ProgressEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                populateData();
                skinsLabel.Content = "Total Skins: " + skinListView.Items.Count;
                newBtn.IsEnabled = true;
                progressRing.IsActive = false;
                progressLabel.Content = e.progressString;

            }));
            
        }

        void myClient_onLoading(object sender, ProgressEventArgs e)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                newBtn.IsEnabled = false;
                progressRing.IsActive = true;
                progressLabel.Content = e.progressString;

            }));

        }

        private void populateData()
        {
            foreach (var c in myClient.myChamps)
            {
                Console.WriteLine(c.Name);
                foreach (var s in c.Skins)
                {
                    string skinImageUrl = "http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + c.Name + "_" + s.Number + ".jpg";
                    
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var skinHolder = new SkinHolder();
                        skinHolder.skinImage.Source = new BitmapImage(new Uri(skinImageUrl, UriKind.Absolute));
                        skinHolder.skinName.Content = s.Name;
                        skinListView.Items.Add(skinHolder);
                    }));
                    
                        
                    //Console.WriteLine(" - " + s.Name);
                }
            }
        }








    }
}
