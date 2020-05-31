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
        #region Field

        /// <summary>
        /// Map 입니다.
        /// </summary>
        GoogleMap googleMap;
        /// <summary>
        /// Map 에 표시될 Pin 입니다.
        /// </summary>
        List<CustomPin> customPins;
        /// <summary>
        /// 100 개 이상인 경우 초록색
        /// </summary>
        Android.Graphics.Color Color_100 = Android.Graphics.Color.Green;
        /// <summary>
        /// 30~99 개 인경우 노란색
        /// </summary>
        Android.Graphics.Color Color_9930 = Android.Graphics.Color.Yellow;
        /// <summary>
        /// 2~29 개 인 경우 빨간색
        /// </summary>
        Android.Graphics.Color Color_2902 = Android.Graphics.Color.Red;
        /// <summary>
        /// 0~1 개 인 경우 회색
        /// </summary>
        Android.Graphics.Color Color_0100 = Android.Graphics.Color.Gray;

        #endregion

        // Constructor
        #region CustomMapRenderer

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CustomMapRenderer(Context context) : base(context)
        {
        }

        #endregion

        // Event Methods (override)
        #region OnElementChanged(e)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var customMap = (CustomMap)e.NewElement;
                customPins = customMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        #endregion

        // Methods (override)
        #region CreateMarker(Pin)

        /// <summary>
        /// 마커의 위치와 라벨을 설정합니다.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        protected override MarkerOptions CreateMarker(Pin pin)
        {
            CustomPin customPin = pin as CustomPin;

            if (customPin == null) return null;

            var marker = new MarkerOptions();

            try
            {
                marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                marker.SetTitle(customPin.Name);
                marker.SetSnippet(pin.Address);
                marker.SetIcon(GenerateMyCustomDrawnOverlay(125, 60, customPin.Label));
            }
            catch
            {
                // 에러무시
            }

            return marker;
        }

        #endregion
        #region OnMapReady(GoogleMap)

        /// <summary>
        /// 맵이 준비 상태인 경우 처리합니다.
        /// </summary>
        /// <param name="map">map</param>
        protected override void OnMapReady(GoogleMap googleMap)
        {
            base.OnMapReady(map);
            this.map = googleMap;
        }

        #endregion

        // Methods (private)
        #region GenerateMyCustomDrawnOverlay(pintWidth, pintHeight, text)

        /// <summary>
        /// 마커를 표시할 도형과 텍스트를 그립니다.
        /// </summary>
        /// <param name="pintWidth">그려질 도형의 크기 너비</param>
        /// <param name="pintHeight">그려질 도형의 크기 높이</param>
        /// <param name="text">내용</param>
        /// <returns></returns>
        private BitmapDescriptor GenerateMyCustomDrawnOverlay(int pintWidth, int pintHeight, string text)
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

                // 비정상적인 값(공백)이 들어온 경우 x 로 표시
                if (string.IsNullOrEmpty(maskCountLabel))
                {
                    maskCountLabel = "x";
                }

                Android.Graphics.Color color = Color_0100;

                if (text == "plenty")
                {
                    color = Color_100;
                }
                else if (text == "some")
                {
                    color = Color_9930;
                }
                else if (text == "few")
                {
                    color = Color_2902;
                }
                else if (text == "empty")
                {
                    color = Color_0100;
                }
                else
                {
                    color = Color_0100;
                }

                // 도형안의 텍스트의 왼쪽 마진
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

            return icon;
        }

        #endregion
    }
}