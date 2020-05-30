using System.Collections.Generic;

using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;

using GongMask;
using GongMask.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace GongMask.Droid
{
    public class CustomMapRenderer : MapRenderer
    {
        GoogleMap map;
        List<CustomPin> customPins;

        Android.Graphics.Color Color_100 = Android.Graphics.Color.Green;
        Android.Graphics.Color Color_9930 = Android.Graphics.Color.Yellow;
        Android.Graphics.Color Color_2902 = Android.Graphics.Color.Red;
        Android.Graphics.Color Color_0100 = Android.Graphics.Color.Gray;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            this.map = map;
            //this.map.MarkerClick += OnPinClicked;
        }

        private void OnPinClicked(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            e.Handled = true;
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            CustomPin customPin = pin as CustomPin;

            var marker = new MarkerOptions();

            try
            {
                marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                marker.SetTitle(customPin.Name);
                marker.SetSnippet(pin.Address);

                if (customPin == null)
                {
                    marker.SetIcon(GenerateMyCustomDrawnOverlay(125, 60, "", pin.Address));
                }
                else
                {
                    marker.SetIcon(GenerateMyCustomDrawnOverlay(125, 60, customPin.Label, pin.Address));
                }
            }
            catch
            {
                // 에러무시
            }

            return marker;
        }
       
        private BitmapDescriptor GenerateMyCustomDrawnOverlay(int pintWidth, int pintHeight, string text, string sDataType)
        {
            string maskCountLabel = string.Empty;

            Bitmap objBitmap = Bitmap.CreateBitmap(pintWidth, pintHeight, Bitmap.Config.Argb8888);

            try
            {
                // stock_at : 입고시간
                // 재고 상태[판매중지 : 'break' / 100개 이상(녹색): 'plenty' / 30개 이상 100개미만(노랑색): 'some' / 2개 이상 30개 미만(빨강색): 'few' / 1개 이하(회색): 'empty']

                // 라벨 중간 위치하기위해 공백 이 들어감
                if (text == "plenty")
                {
                    maskCountLabel = " 100~ ";
                }
                else if (text == "some")
                {
                    maskCountLabel = "30~100";
                }
                else if (text == "few")
                {
                    maskCountLabel = " 2~30 ";
                }
                else if (text == "empty")
                {
                    maskCountLabel = "     0   ";
                }
                else if (text == "break")
                {
                    maskCountLabel = "x";
                }

                if (string.IsNullOrEmpty(maskCountLabel))
                {
                    maskCountLabel = "x";
                }

                Android.Graphics.Color color = Color_0100;
                if (text == "plenty") color = Color_100;
                else if (text == "some") color = Color_9930;
                else if (text == "few") color = Color_2902;
                else if (text == "empty") color = Color_0100;
                else color = Color_0100;

                //int leftmargin = 0;
                int leftmargin = 19;

                Canvas objCanvas = new Canvas(objBitmap);
                //Rect r = new Rect();
                //objCanvas.GetClipBounds(r);

                var circlePaint = new global::Android.Graphics.Paint();
                circlePaint.AntiAlias = true;
                circlePaint.Color = color;
                circlePaint.Alpha = 255;

                var fontPaint = new global::Android.Graphics.Paint();
                fontPaint.AntiAlias = true;
                fontPaint.Color = Android.Graphics.Color.Black;
                fontPaint.TextSize = 27;
                fontPaint.Alpha = 255;

                fontPaint.TextAlign = Paint.Align.Left;
                fontPaint.SetTypeface(Typeface.Create("", TypefaceStyle.Bold));
                //fontPaint.GetTextBounds(maskCountLabel, 0, maskCountLabel.Length, r);

                int intHalfWidth = pintWidth / 2;
                int intHalfHeight = pintHeight / 2;

                objCanvas.DrawRoundRect(new global::Android.Graphics.RectF(0, 0, (float)pintWidth, (float)pintHeight), intHalfWidth, intHalfHeight, circlePaint);
                objCanvas.DrawText(maskCountLabel, leftmargin, 37, fontPaint);
                //objCanvas.DrawText(maskCountLabel, objCanvas.Height/2 , 37, fontPaint);
            }
            catch
            {
                // 에러무시
            }

            BitmapDescriptor icon = BitmapDescriptorFactory.FromBitmap(objBitmap);
            objBitmap.Recycle();
            return (icon);
        }
    }
}