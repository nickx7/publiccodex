using App1.Data;
using App1.Models;
using App1.Views;
using App1.Helpers;
using App1.Views.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace App1
{
    public partial class App1 : Application
    {
         static RestAPI restService;
        private static Label labelScreen;
        private static bool hasInternet;
        private static Page currentpage;
        //private static Timer timer;
        
        public App1()
        {
            InitializeComponent();
            if (Settings.TokenSettings != string.Empty && Settings.ExpirySettings > DateTime.Now)
            {
                MainPage = new  MainMenuPage();
            }
            else
            {
                MainPage = new NavigationPage(new WelcomePage());
            }
           
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        
     

        private static RestAPI RestAPI {
            get {
                if (restService == null)
                {
                    restService = new RestAPI();
                }
                return restService;
            }
        }

        public static void StartCheckIfInternet(Label label, Page page) {
            labelScreen = label;
            label.Text = Classconstants.NoInternetText;
            label.IsVisible = false;
            hasInternet = true;
            currentpage = page;
            //if (timer == null)
            //{
            //    timer = new Timer((e) =>
            //    {
            //        CheckIfInternetOverTime();

            //    }, null, 10(int)TimeSpan.FromSeconds(3).TotalMilliseconds);
            }
        }

    }
=================================================
using App1.Models;
using iAd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using App1.Helpers;
 
namespace App1.Data
{
    public class RestAPI
    {

       
        HttpClient client;
        string grant_type = "password";
        string Email = "Username";

        public RestAPI()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }

//The login button functionality:
        public async Task<Token> Login(ClassUser user)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", grant_type));
            postData.Add(new KeyValuePair<string, string>("username", user.Username));
            postData.Add(new KeyValuePair<string, string>("password", user.Password));
            var content = new FormUrlEncodedContent(postData);
            var response = await PostResponseLogin<Token>(Classconstants.LoginUrl, content);
            DateTime dt = new DateTime();
            dt = DateTime.Today;
           response.expires_date = dt.AddSeconds(response.expires_in);
            return response;
        }
        //The register button GET request functionality:
        public async Task<Registermodel> RegisterAsync(ClassUser reguser)
        {
            var postRegData = new List<KeyValuePair<string, string>>();
            postRegData.Add(new KeyValuePair<string, string>("Email", reguser.Username));
            postRegData.Add(new KeyValuePair<string, string>("Password", reguser.Password));
            postRegData.Add(new KeyValuePair<string, string>("ConfirmPassword", reguser.Password));
            var content = new FormUrlEncodedContent(postRegData);
            var response = await client.PostAsync("https://www.regionales-konjunkturbarometer.de/api/account/register", content);
            var json = response.Content.ReadAsStringAsync().Result;
            var responseob = JsonConvert.DeserializeObject<Registermodel>(json);
            
            return responseob;
        }

        public async Task<T> PostResponseLogin<T>(string weburl, FormUrlEncodedContent content) where T : class
        {

            var response = await client.PostAsync(weburl, content);
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            var responseObject = JsonConvert.DeserializeObject<T>(jsonResult);
            return responseObject;
        }
        

        
        //The survey quesitons GET request functionality:
        public async Task<SurveyModel> GetQuestions()
        {
            var client = new HttpClient();
            SurveyModel questions = new SurveyModel();
            var uri = new Uri(string.Format(Classconstants.QnUrl, string.Empty));
            //Add header for token authorization
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Settings.TokenSettings);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer" +Settings.TokenSettings);
            var response = await client.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content =  response.Content.ReadAsStringAsync().Result;
                var result_questions = JsonConvert.DeserializeObject<SurveyModel>(content);
                questions = result_questions;
            }
            return questions;
        }

       
        

         //The survey answer quesitons POST request functionality:
         
    }
}
z