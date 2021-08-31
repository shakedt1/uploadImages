using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Xamarin.Essentials;
using System.Net.Http;
using Plugin.CurrentActivity;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace uploadImages
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]

    public class MainActivity : AppCompatActivity
    {
        // server IP, may change
        private string url = "https://774ucrxojh.execute-api.us-east-1.amazonaws.com/dev";

        // place to store taken images
        private string galleryPath = "/storage/emulated/0/Pictures/";

        class Data
        {
            public string image { get; set; }
            public string client { get; set; }
            public string imageName { get; set; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            FindViewById<Button>(Resource.Id.uploadFromGallery).Click += uploadFromGallery;
            FindViewById<Button>(Resource.Id.uploadFromCamera).Click += uploadFromCamera;
            FindViewById<TextInputEditText>(Resource.Id.usernameInput).KeyPress += ValidateName;
        }

        private void ValidateName(object sender, EventArgs e)
        {
            TextInputEditText senderObject = sender as TextInputEditText;
            FindViewById<Button>(Resource.Id.uploadFromCamera).Enabled
                = FindViewById<Button>(Resource.Id.uploadFromGallery).Enabled
                = Regex.IsMatch(senderObject.Text.Trim(), @"^[A-Za-z\d\s]+$");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // choose picture from gallery, and upload it
        private async void uploadFromGallery(object sender, EventArgs args)
        {
            try
            {
                // pick photo of the media
                var photo = await MediaPicker.PickPhotoAsync();

                // canceled
                if (photo == null)
                {
                    return;
                }
                await uploadPhotoAsync(photo); // upload the photo
            }
            catch (Exception ex)
            {

                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }

        }
        private async void uploadFromCamera(object sender, EventArgs args)
        {
            try
            {
                // open the camera and take photo (need to enable restrictions)
                var photo = await MediaPicker.CapturePhotoAsync();
                // canceled
                if (photo == null)
                {
                    return;
                }

                await loadPhotoAsync(photo); // save the photo to device
                await uploadPhotoAsync(photo); // upload the photo
            }
            catch (Exception ex)
            {

                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }

        }

        // save the photo into local storage
        private async Task loadPhotoAsync(FileResult photo)
        {
            // create file path
            var newFile = Path.Combine(galleryPath, photo.FileName);
            // read the photo
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream); // save the photo

        }

        // upload photo to the server
        private async Task uploadPhotoAsync(FileResult photo)
        {
            // container for MIME data type
            byte[] bytes = File.ReadAllBytes(photo.FullPath);

            Data data = new Data
            {
                image = Convert.ToBase64String(bytes),
                client = FindViewById<TextInputEditText>(Resource.Id.usernameInput).Text,
                imageName = photo.FileName
            };

            string serializedData = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            // post our Http request
            try
            {
                var HttpClient = new HttpClient();
                var response = await HttpClient.PostAsync(url, httpContent);

                if (response.Content != null)
                {
                    SnackbarShow(response.StatusCode.ToString() + " " + response.Content.ReadAsStringAsync().Result);
                }
            }

            catch (Exception ex)
            {
                SnackbarShow(ex.Message);
            }
            FindViewById<TextInputEditText>(Resource.Id.usernameInput).Text = String.Empty;

        }

        // raise bottom bar that show the status of the upload
        private void SnackbarShow(string message)
        {
            Activity activity = CrossCurrentActivity.Current.Activity;
            View view = activity.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view, message, Snackbar.LengthLong).Show();
        }
    }
}