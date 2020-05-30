using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

using Plugin.Geolocator;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GongMask.Views
{
    [DesignTimeVisible(false)]
    public partial class MapPage : ContentPage
    {
        // Constructor
        #region MapPage()

        public MapPage()
        {
            InitializeComponent();

            ClearMapPins();

            this.indicator.IsRunning = true;
            this.indicator.IsVisible = true;

            // 지도 설정
            this.map.IsShowingUser = true;
            this.map.MapType = MapType.Street;
            this.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(36.00677876098392, 127.66082354552458), Distance.FromMeters(500)));

            // 지도 이동시 발생되는 이벤트입니다.
            this.map.PropertyChanged += Map_PropertyChanged;
        }

        #endregion

        // Methods (override)
        #region OnAppearing()

        /// <summary>
        /// 화면 표시 될때 동작합니다.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Thread t = new Thread(() => SetLocation());
            t.Start();
        }

        #endregion

        // Methods
        #region GetMaskData()

        /// <summary>
        /// 지도의 현재 위치 기준으로 마스크 정보를 가져옵니다.
        /// </summary>
        private void GetMaskData()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    WebClient wc = new WebClient();

                    MapSpan mapSpan = map.VisibleRegion;

                    // 5000 미터 제한
                    double meter = mapSpan.Radius.Meters;
                    if (meter > 5000)
                    {
                        meter = 5000;
                        
                        //this.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(mapSpan.Center.Latitude, mapSpan.Center.Longitude), Distance.FromMeters(5000)));
                    }

                    // 지도기반 상점 데이터를 가져옵니다.
                    byte[] data = wc.DownloadData($@"https://8oi9s0nnth.apigw.ntruss.com/corona19-masks/v1/storesByGeo/json?lng={mapSpan.Center.Longitude}&lat={mapSpan.Center.Latitude}&m={mapSpan.Radius.Meters}");

                    var json = Encoding.UTF8.GetString(data);

                    GongMaskStore gongMaskStore = JsonConvert.DeserializeObject<GongMaskStore>(json);

                    // 상점 정보를 표시합니다.
                    if (gongMaskStore.stores != null && gongMaskStore.stores.Count > 0)
                    {
                        MakePins(gongMaskStore.stores);
                    }
                }
                finally
                {
                    this.indicator.IsRunning = false;
                    this.indicator.IsVisible = false;
                }
            });
        }

        #endregion
        #region SetLocation()

        /// <summary>
        /// 현재 위치를 찾아 지도에 표시합니다.
        /// </summary>
        private void SetLocation()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var locator = CrossGeolocator.Current;

                    Plugin.Geolocator.Abstractions.Position geoLocation = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                    if (geoLocation != null)
                    {
                        this.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(geoLocation.Latitude, geoLocation.Longitude), Distance.FromMeters(500)));
                    }
                }
                finally
                {
                    this.indicator.IsRunning = false;
                    this.indicator.IsVisible = false;
                }
            });
        }

        #endregion
        #region ClearMapPins()

        /// <summary>
        /// 지도상의 pin 을 제거합니다.
        /// </summary>
        private void ClearMapPins()
        {
            this.map.CustomPins = new List<CustomPin>();
            this.map.Pins.Clear();
        }

        #endregion
        #region MakePins(satoreList)

        /// <summary>
        /// 위치 정보를 지도에 표시합니다.
        /// </summary>
        /// <param name="satoreList"></param>
        private void MakePins(List<Store> satoreList)
        {
            try
            {
                foreach (var store in satoreList.Where(c=>!string.IsNullOrEmpty(c.remain_stat)))
                {
                    var pin = this.map.CustomPins.Where(c => c.MarkerId.ToString() == store.code).FirstOrDefault();

                    if (pin == null)
                    {
                        // 판매 중지라면 지도에 표시 하지 않습니다.
                        if (store.remain_stat == "break") continue;

                        // 신규인 경우 추가합니다.
                        var newPin = new CustomPin
                        {
                            Type     = PinType.Place,
                            MarkerId = store.code,
                            Position = new Position(store.lat, store.lng),
                            Address  = store.addr,
                            Label    = store.remain_stat,
                            Name     = store.name
                        };

                        this.map.Pins.Add(newPin);
                        this.map.CustomPins.Add(newPin);
                    }
                    else
                    {
                        // 기존에 표시되었던 경우 상태를 확인해 상태가 변경되었다면 삭제하고 추가합니다.
                        if (pin.Label != store.remain_stat)
                        {
                            this.map.Pins.Remove(pin);
                            this.map.CustomPins.Remove(pin);

                            // 판매 중지라면 지도에서 지운 후 다시 지도에 표시 하지 않습니다.
                            if (store.remain_stat == "break") continue;

                            var newPin = new CustomPin
                            {
                                Type     = PinType.Place,
                                MarkerId = store.code,
                                Position = new Position(store.lat, store.lng),
                                Address  = store.addr,
                                Label    = store.remain_stat,
                                Name     = store.name
                            };

                            this.map.Pins.Add(newPin);
                            this.map.CustomPins.Add(newPin);
                        }
                    }
                }
            }
            catch
            {
                // 에러무시
            }
        }

        #endregion

        // Event Methods
        #region Map_PropertyChanged(sender, e)

        /// <summary>
        /// map 이동시 발생되는 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VisibleRegion")
            {
                var m = (Map)sender;
                if (m.VisibleRegion != null)
                {
                    this.indicator.IsRunning = true;
                    this.indicator.IsVisible = true;

                    // 지도 이동시 이동된 위치 기준으로 마스크 데이터를 가져와 표시합니다.
                    Thread t = new Thread(() => GetMaskData());
                    t.Start();
                    //Debug.WriteLine("Lat: " + m.VisibleRegion.Center.Latitude.ToString() + " Lon:" + m.VisibleRegion.Center.Longitude.ToString());
                }
            }
        }

        #endregion
    }
}