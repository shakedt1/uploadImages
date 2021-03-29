using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Net;
using System.IO; 
using static Android.Provider.MediaStore;
using Xamarin.Android;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Net.Http;


namespace uploadImages
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string url = "http://10.42.128.73/demo_uploads/api/Files/Upload"; 
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            while (true)
            {
                var file = await MediaPicker.PickPhotoAsync();
                if (file == null)
                {
                    return;
                }
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);
                var HttpClient = new HttpClient();
                var response = await HttpClient.PostAsync(url, content);
                System.Console.WriteLine(response.StatusCode.ToString());
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        

    }
}