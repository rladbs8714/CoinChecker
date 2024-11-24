using System.Reflection;
using System.Text.Json;
using BItcoinChecker.Core;
using Generalibrary;

namespace BitcoinChecker.Core
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.11
     *  
     *  < 목적 >
     *  - 업비트 API Helper를 작성한다.
     *  
     *  < TODO >
     *  - Exception을 좀 더 세분화 할 필요가 있음. 현재는 일반 Exception 클래스로 처리함. 하지만 발생하는 이유는 각양각색..
     *  - 현재 각 명령 메서드마다 중복되는 코드가 정말 많은데 이것 좀 정리해야 할 듯
     *  
     *  < History >
     *  2024.06.11 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class UpbitCore : RestHelper
    {
        // ====================================================================
        // ENUMS
        // ====================================================================

        public enum ESide
        {
            /// <summary>
            /// 매수
            /// </summary>
            bid,

            /// <summary>
            /// 매도
            /// </summary>
            ask
        }

        public enum EOrderType
        {
            /// <summary>
            /// 지정가 주문
            /// </summary>
            limit,

            /// <summary>
            /// 시장가 주문 (매수)
            /// </summary>
            price,

            /// <summary>
            /// 시장가 주문 (매도)
            /// </summary>
            market,

            /// <summary>
            /// 최유리 주문 (time_in_force 설정 필수)
            /// </summary>
            best
        }

        public enum ETimeInForce
        {
            /// <summary>
            /// None
            /// </summary>
            none,

            /// <summary>
            /// Immediate or Cancel
            /// </summary>
            ioc,

            /// <summary>
            /// Fill or Kill
            /// </summary>
            fok
        }


        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private const string LOG_TYPE = "UPBIT_CORE";

        private readonly ILogManager LOG = LogManager.Instance;


        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        public UpbitCore(bool isDebug = false)
        {
            IsDebug = isDebug;
        }


        // ====================================================================
        // METHODS - EXCHANGE API
        // ====================================================================

        /// <summary>
        /// 업비트에서 계정이 보유한 자산 리스트를 보여준다.
        /// </summary>
        /// <param name="accounts">계정이 보유한 자산 리스트</param>
        /// <returns>성공시 true, 실패시 false</returns>
        public bool TryGetAccounts(out Accounts_VO? accounts)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            accounts = null;
            List<Accounts_VO.Account>? accountList;
            if (!TryRequest(out string json, "/v1/accounts", ERestMethod.Get) || string.IsNullOrEmpty(json))
                return false;

            try
            {
                accountList = JsonSerializer.Deserialize<List<Accounts_VO.Account>>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            if (accountList == null)
                return false;

            accounts = new Accounts_VO(accountList);
            return true;
        }

        /// <summary>
        /// 마켓별 주문 가능 정보를 확인한다.
        /// </summary>
        /// <param name="ordersChance">마켓 별 주문 가능 정보</param>
        /// <param name="market">확인할 마켓 ID</param>
        /// <returns>성공시 true, 실패시 false</returns>
        public bool TryGetOrdersChance(out OrdersChance_VO? ordersChance, string market)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            ordersChance = null;

            if (string.IsNullOrEmpty(market))
                return false;

            if (!TryRequest(out string json, "/v1/orders/chance", ERestMethod.Get, new Dictionary<string, string>() { { "market", market } }) || string.IsNullOrEmpty(json))
                return false;

            try
            {
                ordersChance = JsonSerializer.Deserialize<OrdersChance_VO>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            if (ordersChance == null)
                return false;

            return true;
        }

        /// <summary>
        /// 주문하기 요청
        /// 이 메서드는 내부에서 TryOrders 디테일 버전을 재호출한다.
        /// </summary>
        /// <param name="orders">주문 결과</param>
        /// <param name="market">마켓 ID (필수)</param>
        /// <param name="side">
        /// 주문 종류 (필수)  <br />
        /// - bid : 매수      <br />
        /// - ask : 매도
        /// </param>
        /// <param name="volume">주문량 (지정가, 시장가 매도 시 필수)</param>
        /// <param name="price">
        /// 주문 가격 (지정가, 시장가 매수 시 필수)                                                                          <br />
        /// ex) KRW-BTC 마켓에서 1BTC당 1,000 KRW로 거래할 경우, 값은 1000 이 된다.                                          <br />
        /// ex) KRW-BTC 마켓에서 1BTC당 매도 1호가가 500 KRW 인 경우, 시장가 매수 시 값을 1000으로 세팅하면 2BTC가 매수된다. <br />
        /// (수수료가 존재하거나 매도 1호가의 수량에 따라 상이할 수 있음)
        /// </param>
        /// <param name="orderType">
        /// 주문 타입 (필수)             <br />
        /// - limit : 지정가 주문        <br />
        /// - price : 시장가 주문 (매수) <br />
        /// - market: 시장가 주문 (매도) <br />
        /// - best  : 최유리 주문 (time_in_force 설정 필수)
        /// </param>
        /// <returns>성공하면 true, 실패하면 false</returns>
        public bool TryOrders(out Orders_VO? orders, string market, ESide side, double volume, double price, EOrderType orderType)
        {
            return TryPostOrders(out orders, market, side, volume, price, orderType, string.Empty, ETimeInForce.none);
        }

        /// <summary>
        /// 주문하기 요청
        /// </summary>
        /// <param name="orders">주문 결과</param>
        /// <param name="market">마켓 ID (필수)</param>
        /// <param name="side">
        /// 주문 종류 (필수)  <br />
        /// - bid : 매수      <br />
        /// - ask : 매도
        /// </param>
        /// <param name="volume">주문량 (지정가, 시장가 매도 시 필수)</param>
        /// <param name="price">
        /// 주문 가격 (지정가, 시장가 매수 시 필수)                                                                          <br />
        /// ex) KRW-BTC 마켓에서 1BTC당 1,000 KRW로 거래할 경우, 값은 1000 이 된다.                                          <br />
        /// ex) KRW-BTC 마켓에서 1BTC당 매도 1호가가 500 KRW 인 경우, 시장가 매수 시 값을 1000으로 세팅하면 2BTC가 매수된다. <br />
        /// (수수료가 존재하거나 매도 1호가의 수량에 따라 상이할 수 있음)
        /// </param>
        /// <param name="orderType">
        /// 주문 타입 (필수)             <br />
        /// - limit : 지정가 주문        <br />
        /// - price : 시장가 주문 (매수) <br />
        /// - market: 시장가 주문 (매도) <br />
        /// - best  : 최유리 주문 (time_in_force 설정 필수)
        /// </param>
        /// <param name="identifier">조회용 사용자 지정값 (선택)</param>
        /// <param name="timeInForce">
        /// IOC, FOK 주문 설정          <br />
        /// - ioc : Immediate or Cancel <br />
        /// - fok : Fill or Kill        <br />
        /// * ord_type이 <b>best</b> 혹은 <b>limit</b> 일때만 적용
        /// </param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TryPostOrders(out Orders_VO? orders, string market, ESide side, double volume, double price, EOrderType orderType, string identifier, ETimeInForce timeInForce)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            orders = null;

            if (string.IsNullOrEmpty(market))
                throw new Exception("the market is required.");

            if (side == ESide.bid && orderType == EOrderType.market)
                throw new Exception("order type is \'buy\', but order_type is \'sell\'.");
            else if (side == ESide.ask && orderType == EOrderType.price)
                throw new Exception("order type is \'sell\', but order_type is \'buy\'.");

            // 주문 명령의 옵션을 생성한다.
            // 'market', 'side', 'ord_type'은 필수 값이기에 기본값으로 추가한다.
            Dictionary<string, string> options = new Dictionary<string, string>()
            {
                { "market"  , market          },        // 마켓 ID    (필수)
                { "side"    , side.ToString() },        // 주문 종류  (필수)
                { "ord_type", orderType.ToString() }    // 주문 타입  (필수)
            };

            if (orderType == EOrderType.limit || orderType == EOrderType.market)
                options.Add("volume", volume.ToString());   // 주문량    (지정가, 시장가 매도시 필수)

            if (orderType == EOrderType.limit || orderType == EOrderType.price)
                options.Add("price", price.ToString());     // 주문 가격 (지정가, 시장가 매수 시 필수)

            if (!string.IsNullOrEmpty(identifier))
                options.Add("identifier", identifier);      // 조회용 사용자 지정값 (선택)

            if (timeInForce != ETimeInForce.none && (orderType == EOrderType.limit || orderType == EOrderType.best))
                options.Add("time_in_force", timeInForce.ToString());   // IOC, FOK 주문 설정 (ord_type이 limit 혹은 best일 때만 지원)

            if (!TryRequest(out string json, "/v1/orders", ERestMethod.Post, options) || string.IsNullOrEmpty(json))
                return false;

            try
            {
                orders = JsonSerializer.Deserialize<Orders_VO>(json);
            }
            catch (Exception ex)
            {
                var err = JsonSerializer.Deserialize<Error_VO>(json);
                LOG.Error(LOG_TYPE, doc, $"요청이 정상적으로 이루어지지 않았습니다.\n{err.Name}: {err.Message}", exception: ex);
                return false;
            }

            if (orders == null)
            {
                LOG.Error(LOG_TYPE, doc, $"요청이 정상적으로 이루어지지 않았습니다.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 주문 UUID를 통해 해당 주문에 대한 취소 접수를 한다.
        /// </summary>
        /// <param name="order">주문 취소 결과</param>
        /// <param name="uuid">취소할 주문의 UUID</param>
        /// <param name="identifier">조회용 사용자 지정값</param>
        /// <returns>주문 취소에 성공했다면 true, 그렇지 않다면 false</returns>
        /// <exception cref="Exception"></exception>
        public bool TryDeleteOrder(out Order_VO? order, string uuid, string identifier)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            order = null;

            if (string.IsNullOrEmpty(uuid) && string.IsNullOrEmpty(identifier))
                throw new Exception($"\'{nameof(uuid)}\' 혹은 \'{nameof(identifier)}\' 둘 중 하나의 값이 반드시 유효해야 합니다.");

            Dictionary<string, string> options = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(uuid))       options.Add("uuid", uuid);
            if (!string.IsNullOrEmpty(identifier)) options.Add("identifier", identifier);

            if (!TryRequest(out string json, "/v1/order", ERestMethod.Delete, options) || string.IsNullOrEmpty(json))
                return false;

            try
            {
                order = JsonSerializer.Deserialize<Order_VO>(json);
            }
            catch (Exception ex)
            {
                var err = JsonSerializer.Deserialize<Error_VO>(json);
                LOG.Error(LOG_TYPE, doc, $"요청이 정상적으로 이루어지지 않았습니다.\n{err.Name}: {err.Message}", exception: ex);
                return false;
            }

            if (order == null)
            {
                LOG.Error(LOG_TYPE, doc, $"요청이 정상적으로 이루어지지 않았습니다.");
                return false;
            }

            return true;
        }


        // ====================================================================
        // METHODS - QUOTATION API
        // ====================================================================

        /// <summary>
        /// 업비트에서 거래 가능한 마켓 목록을 가져오고 성공 여부를 반환한다.
        /// </summary>
        /// <param name="marketAll">마켓 목록. 목록 로드 실패시 null 이다.</param>
        /// <param name="isDetails">디테일 옵션</param>
        /// <returns>성공시 true, 실패시 false</returns>
        public bool TryGetMarketAll(out MarketAll_VO? marketAll, bool isDetails = true)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            marketAll = null;
            List<MarketAll_VO.Market_VO>? markets;
            string isDetailsStr = isDetails ? "true" : "false";
            if (!TryRequest(out string json, "/v1/market/all", ERestMethod.Get, new Dictionary<string, string>() { { "isDetails", isDetailsStr } }) || string.IsNullOrEmpty(json))
                return false;

            try
            {    
                markets = JsonSerializer.Deserialize<List<MarketAll_VO.Market_VO>>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            if (markets == null)
                return false;

            marketAll = new MarketAll_VO(markets);
            return true;
        }

        /// <summary>
        /// 요청 당시 종목의 스냅샷을 반환한다.
        /// </summary>
        /// <param name="tickerCollection">요청 당시 종목의 스냅샷</param>
        /// <param name="marketCode">요청할 종목의 코드 (KRW-BTC)</param>
        /// <returns>성공시 true, 실패시 false</returns>
        public bool TryGetTicker(out TickerCollection_VO? tickerCollection, string marketCode)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            tickerCollection = null;
            List<TickerCollection_VO.Ticker_VO>? tickers;

            if (string.IsNullOrEmpty(marketCode))
                return false;

            if (!TryRequest(out string json, "/v1/ticker", ERestMethod.Get, new Dictionary<string, string>() { { "markets", marketCode } }) || string.IsNullOrEmpty(json))
                return false;

            try
            {
                tickers = JsonSerializer.Deserialize<List<TickerCollection_VO.Ticker_VO>>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            if (tickers == null)
                return false;

            tickerCollection = new TickerCollection_VO(tickers);
            return true;
        }

        /// <summary>
        /// 분(minute) 캔들
        /// </summary>
        /// <param name="minutesCandles">응답 데이터</param>
        /// <param name="unit">분 단위</param>
        /// <param name="market">마켓 코드 (ex. KRW-BTC)</param>
        /// <param name="to">
        /// 마지막 캔들 시각 (exclusive). <br />
        /// ISO8061 포맷(yyyy-MM-dd'T'HH:mm:ss'Z' or yyyy-MM-dd HH:mm:ss). 기본적으로 UTC 기준 시간이며 2023-01-01T00:00:00+09:00 과 같이 KST 시간으로 요청 가능. <br />
        /// 비워서 요청시 가장 최근 캔들
        /// </param>
        /// <param name="count">캔들 개수(최대 200개까지 요청 가능)</param>
        public bool TryGetMinuteCandles(out MinutesCandles_VO[]? minutesCandles, int unit, string market, string to, int count)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            minutesCandles = null;

            if (string.IsNullOrEmpty(market) || count == 0 || count > 200)
                return false;

            switch (unit)
            {
                case 1:
                case 3:
                case 5:
                case 10:
                case 15:
                case 30:
                case 60:
                case 240:
                    break;

                default: 
                    return false;
            }

            if (!TryRequest(out string json, $"/v1/candles/minutes/{unit}", ERestMethod.Get, new Dictionary<string, string>() { { "market", market }, { "to", to }, { "count", count.ToString() } }))
                return false;

            try
            {
                minutesCandles = JsonSerializer.Deserialize<MinutesCandles_VO[]>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            if (minutesCandles == null)
                return false;

            return true;
        }

        /// <summary>
        /// 분(minute) 캔들
        /// </summary>
        /// <param name="secondsCandles">응답 데이터</param>
        /// <param name="market">마켓 코드 (ex. KRW-BTC)</param>
        /// <param name="to">
        /// 마지막 캔들 시각 (exclusive). <br />
        /// ISO8061 포맷(yyyy-MM-dd'T'HH:mm:ss'Z' or yyyy-MM-dd HH:mm:ss). 기본적으로 UTC 기준 시간이며 2023-01-01T00:00:00+09:00 과 같이 KST 시간으로 요청 가능. <br />
        /// 비워서 요청시 가장 최근 캔들
        /// </param>
        /// <param name="count">캔들 개수(최대 200개까지 요청 가능)</param>
        public bool TryGetSecondCandles(out SecondsCandles_VO[]? secondsCandles, string market, string to, int count)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            secondsCandles = null;

            if (string.IsNullOrEmpty(market) || count == 0 || count > 200)
                return false;

            if (!TryRequest(out string json, $"/v1/candles/seconds", ERestMethod.Get, new Dictionary<string, string>() { { "market", market }, { "to", to }, { "count", count.ToString() } }))
                return false;

            try
            {
                secondsCandles = JsonSerializer.Deserialize<SecondsCandles_VO[]>(json);
            }
            catch (Exception ex)
            {
                LOG.Warning(LOG_TYPE, doc, ex.Message);
                return false;
            }

            return true;
        }

        // ====================================================================
        // METHODS - PRIVATE
        // ====================================================================

    }
}
