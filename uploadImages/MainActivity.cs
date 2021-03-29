using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Xamarin.Essentials;
using System.Net.Http;


namespace uploadImages
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // server IP, may change
        private string url = "http://10.42.128.73/demo_uploads/api/Files/Upload"; 
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            while (true)
            {
                // pick photo of the media
                var file = await MediaPicker.PickPhotoAsync(); 

                if (file == null)
                {
                    return;
                }

                // container for MIME data type
                var content = new MultipartFormDataContent(); 
                content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

                // post our http request
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