using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace GongMask.Droid
{
    [Activity(Label = "GongMask", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;

        readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                }
            }

            //List<string> permissions = new List<string>();
            //List<string> checkPermissions = new List<string>();
            //checkPermissions.Add(Manifest.Permission.AccessFineLocation);
            //checkPermissions.Add(Manifest.Permission.AccessCoarseLocation);
            //checkPermissions.Add(Manifest.Permission.AccessMockLocation);
            //checkPermissions.Add(Manifest.Permission.AccessCoarseLocation);

            //foreach (var checkPermission in checkPermissions)
            //{
            //    if (ContextCompat.CheckSelfPermission(this, checkPermission) != (int)Permission.Granted)
            //    {
            //        permissions.Add(checkPermission);
            //    }
            //}

            //ActivityCompat.RequestPermissions(this, permissions.ToArray(), 1);

            base.OnCreate(savedInstanceState);

            //global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == RequestLocationId)
            {
                if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted))
                {
                }
                else
                {
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
    }
}