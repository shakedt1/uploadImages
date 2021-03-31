﻿using Android.App;
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

namespace uploadImages
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        // server IP, may change
        private string url = "http://10.42.128.73/demo_uploads/api/Files/Upload";

        // place to store taken images
        private string galleryPath = "/storage/emulated/0/Pictures/";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            FindViewById<Button>(Resource.Id.uploadFromGallery).Click += uploadFromGallery;
            FindViewById<Button>(Resource.Id.uploadFromCamera).Click += uploadFromCamera;

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // choose picture from galler, and upload it 
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
            // cretae file path
            var newFile = Path.Combine(galleryPath, photo.FileName);
            // read the photo
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream); // save the photo

        }

        // upload photo to the srver
        private async Task uploadPhotoAsync(FileResult photo)
        {
            // container for MIME data type
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(await photo.OpenReadAsync()), "file", photo.FileName);

            // post our http request
            try
            {
                var HttpClient = new HttpClient();
                var response = await HttpClient.PostAsync(url, content);
                SnackbarShow(response.StatusCode.ToString() + " " + response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                SnackbarShow(ex.Message);
            }
        }

        // raise buttom bar that show the status of the upload
        private void SnackbarShow(string message)
        {
            Activity activity = CrossCurrentActivity.Current.Activity;
            View view = activity.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view, message, Snackbar.LengthLong).Show();
        }
    }
}