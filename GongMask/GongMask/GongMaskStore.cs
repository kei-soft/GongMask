using System.Collections.Generic;

namespace GongMask
{
    /// <summary>
    /// 공적 마스크 판매 상점 전체 정보입니다.
    /// </summary>
    public class GongMaskStore
    {
        public int count { get; set; }
        public List<Store> stores { get; set; }
    }

    /// <summary>
    /// 마스크 판매 상점 정보입니다.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// 주소
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        /// 식별코드
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 데이터생성일자
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 위도
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 경도
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// 이름
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 재고상태
        /// [100개 이상(녹색): 'plenty' / 30개 이상 100개미만(노랑색): 'some' / 2개 이상 30개 미만(빨강색): 'few' / 1개 이하(회색): 'empty']
        /// </summary>
        public string remain_stat { get; set; }
        /// <summary>
        /// 입고시간
        /// </summary>
        public string stock_at { get; set; }
        /// <summary>
        /// 판매처
        /// [판매처 유형[약국: '01', 우체국: '02', 농협: '03']
        /// </summary>
        public string type { get; set; }
    }


}
