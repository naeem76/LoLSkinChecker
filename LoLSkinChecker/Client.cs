using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PVPNetConnect;
using PVPNetConnect.RiotObjects.Platform.Catalog.Champion;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Script.Serialization;


namespace LoLSkinChecker
{
    public class Client
    {
        public PVPNetConnection Connection;

        public bool IsCompleted;

        public List<Champion> myChamps = new List<Champion>();

        private List<RiotChampion> allData = new List<RiotChampion>();

        public event EventHandler<ProgressEventArgs> onLoading;
        public event EventHandler<ProgressEventArgs> onCompleteLoad;

        public Client(Region region, string username, string password)
        {
            IsCompleted = false;
            Connection = new PVPNetConnection();
            Connection.OnLogin += new PVPNetConnection.OnLoginHandler(this.OnLogin);
            Connection.OnError += new PVPNetConnection.OnErrorHandler(this.OnError);
            RaiseLoadingEvent(new ProgressEventArgs(0,"Logging In"));
            Connection.Connect(username, password, Region.EUW, "5.11.15_06_05_17_09");
            Connection.OnMessageReceived += Connection_OnMessageReceived;
            
        }

        void Connection_OnMessageReceived(object sender, object message)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            if (!this.Connection.IsConnected())
                return;
            this.Connection.Disconnect();

        }

        private void OnLogin(object sender, string username, string ipAddress)
        {
            
            Console.WriteLine("Logged In As " + username + " at " + DateTime.Now);

            this.IsCompleted = true;

            //PopulateData();
            GetData();
            
        }

        private void OnError(object sender, Error error)
        {
            //this.Data.ErrorMessage = error.Message;
            //this.Data.State = Account.Result.Error;
            this.IsCompleted = true;
            Console.WriteLine(error.Message);
        }


        private async Task GetData()
        {
            //Get Riot Data
            RaiseLoadingEvent(new ProgressEventArgs(1,"Loading Riot Data"));
            var abc = new HttpClient();
            string response = await abc.GetStringAsync("http://ddragon.leagueoflegends.com/cdn/5.11.1/data/en_US/champion.json");
            abc.Dispose();
            var currData = JsonConvert.DeserializeObject<dynamic>(response);
            //dynamic json = new JavaScriptSerializer().Deserialize<object>(response);
            //string[] aatrox = json["data"]["aatrox"].Split(',');

            int count = 1;
            foreach (dynamic c in currData.data)
            {
                //Console.WriteLine(c);
                string data = c.ToString().Substring(c.ToString().IndexOf(':') + 2);
                RiotChampion m = JsonConvert.DeserializeObject<RiotChampion>(data);
                allData.Add(m);
                //RaiseLoadingEvent(new ProgressEventArgs((int)(count),"Loading Riot Data"));
                count++;
            }


            //Enumerate champs+skins
            ChampionDTO[] champs = await Connection.GetAvailableChampions();
            count = 1;
            foreach (ChampionDTO champ in champs)
            {
                //RaiseLoadingEvent(new ProgressEventArgs((int)(count/champs.Count()),"Checking for Skins"));
                if (champ.Owned)
                {
                    string[] theName = getChampName(champ.ChampionId);
                    Console.WriteLine(theName[0]);
                    List<Skin> mySkins = new List<Skin>();
                    foreach (ChampionSkinDTO skin in champ.ChampionSkins)
                    {
                        if (skin.Owned)
                        {
                            string[] skinDetails = await getSkinDetails(skin.SkinId,theName[1]);
                            Console.WriteLine(" - " + skinDetails[0]);
                            mySkins.Add(new Skin()
                            {
                                Id = skin.SkinId,
                                Name = skinDetails[0],
                                Number = Convert.ToInt32(skinDetails[1])
                            });
                        }

                    }
                    myChamps.Add(new Champion(champ.ChampionId, theName, mySkins));
                }
            }

            myChamps = myChamps.OrderBy(x => x.Name).ToList();
            Export();
            RaiseCompleteEvent(new ProgressEventArgs(100,"Done"));
        }

        private string[] getChampName(int id)
        {
            string[] name = new string[2];
            foreach (RiotChampion champ in allData)
            {
                if (champ.key == id.ToString())
                {
                    name[0] = champ.name; 
                    name[1] = champ.id;
                }
            }
            return name;
        }


        private async Task<string[]> getSkinDetails(int id,string SearchID)
        {
            var abc = new HttpClient();
            string mystr = await abc.GetStringAsync("http://ddragon.leagueoflegends.com/cdn/5.11.1/data/en_US/champion/" + SearchID + ".json");

            mystr = mystr.Substring(mystr.IndexOf('[') + 1);
            string[] abcd = mystr.Split(']')[0].Split('}');

            List<RiotSkin> skins = new List<RiotSkin>();
            foreach (string s in abcd)
            {
                if (s != "")
                {
                    string a = "";
                    if (s.StartsWith(",")) a = s.Substring(1) + "}";
                    else a = s + "}";
                    RiotSkin m = JsonConvert.DeserializeObject<RiotSkin>(a);
                    skins.Add(m);
                }
            }

            string[] name = new string[2];
            foreach (var skin in skins)
            {
                if (skin.id == id.ToString())
                {
                    name[0] = skin.name;
                    name[1] = skin.num.ToString();
                }
            }
            abc.Dispose();
            return name;
        }


        private void Export()
        {
            foreach (var c in myChamps)
            {
                Console.WriteLine(c.Name);
                foreach (var s in c.Skins)
                {
                    Console.WriteLine(" - " + s.Name);
                }
            }
        }

        protected virtual void RaiseLoadingEvent(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = onLoading;

            if (handler != null)
            {
                //this is what actually raises the event.
                handler(this, e);
            }
        }

        protected virtual void RaiseCompleteEvent(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = onCompleteLoad;

            if (handler != null)
            {
                //this is what actually raises the event.
                handler(this, e);
            }
        }

    }
}

