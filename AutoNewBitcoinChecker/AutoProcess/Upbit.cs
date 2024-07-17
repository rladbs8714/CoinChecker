using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;
using BitcoinChecker.Core;
using Generalibrary;

namespace BitcoinChecker
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.12
     *  
     *  < 목적 >
     *  - 
     *  
     *  < TODO >
     *  - Process() 메서드 내에 로그를 좀 더 상세히 작성
     *  - Process() 메서드에서 UpbitCore가 계속해서 선언됨. 이걸 재사용할 수는 없을까?
     *  
     *  < History >
     *  2024.06.12 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class Upbit : IniHelper
    {
        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private const string LOG_TYPE = "UPBIT";

        private const string INI_PATH = "system.ini";

        /// <summary>
        /// 디스코드 파이프 이름
        /// </summary>
        private readonly string DISCORD_PIPE_NAME = string.Empty;

        // ====================================================================
        // FIELDS
        // ====================================================================

        /// <summary>
        /// 로그
        /// </summary>
        private ILogManager _log = LogManager.Instance;
        /// <summary>
        /// 마지막으로 수정된 마켓 목록
        /// </summary>
        private MarketAll_VO? _startingMarketAll;
        /// <summary>
        /// 처음 또는 새로운 코인을 찾고 난 직후에 로깅했는지 여부 
        /// 처음 또는 새로운 코인을 찾고 난 직후에 false로 초기화 된다.
        /// </summary>
        private bool _isStartCoinsLoged = false;


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public Upbit() : base(Path.Combine(Environment.CurrentDirectory, INI_PATH))
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            // 디스코드 봇 마이크로 서비스 실행
            bool    use;
            string  section = $"PIPE:DISCORD";
            if (bool.TryParse(GetIniData(section, nameof(use)), out use) && use)
            {
                string name = GetIniData(section, nameof(name));
                if (string.IsNullOrEmpty(name))
                    throw new IniDataException($"[{section}]섹션의 [{nameof(name)}]값을 찾을 수 없거나 공백입니다.");
                DISCORD_PIPE_NAME = name;

                string path = GetIniData(section, nameof(path));
                if (string.IsNullOrEmpty(path))
                    throw new IniDataException($"[{section}]섹션의 [{nameof(path)}]값을 찾을 수 없거나 공백입니다.");

                _log.Info(LOG_TYPE, doc, "디스코드 봇 서비스 실행");
                _log.Info(LOG_TYPE, doc, "디스코드 봇 서비스 파이프 연결 시도");
                PipeManager.Instance.Servers.Add(DISCORD_PIPE_NAME, new PipeServer(DISCORD_PIPE_NAME, path, PipeDirection.InOut));
                _log.Info(LOG_TYPE, doc, "디스코드 봇 서비스 파이프 연결 성공");
            }
            else
            {
                throw new IniDataException($"[{section}]섹션의 [{nameof(use)}]값이 형식에 맞지 않게 입력되어있습니다.");
            }
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// 자동 프로세스 시작
        /// </summary>
        public void StartAutoProcess(bool isWinform = true)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            _log.Info(LOG_TYPE, doc, "프로그램 시작");

            for (int i = 1; i <= 5; i++)
            {
                new UpbitCore().TryGetMarketAll(out _startingMarketAll);

                if (_startingMarketAll == null || _startingMarketAll.Markets == null)
                {
                    _log.Error(LOG_TYPE, doc, $"모든 종목의 값을 제대로 받아오지 못했습니다. 5초 후 재시도 합니다. [{i}/5]");
                    Thread.Sleep(5000);
                    continue;
                }

                break;
            }

            if (_startingMarketAll == null || _startingMarketAll.Markets == null)
            {
                _log.Error(LOG_TYPE, doc, "서버가 원할하지 않습니다. 프로그램을 종료합니다.");
                return;
            }

            DateTime processTime = DateTime.Now;
            _log.Info(LOG_TYPE, doc, "코인 감지 시작");

            //var autoTradeTask = 
            Task.Run(() =>
            {
                UpbitCore upbit = new UpbitCore(isDebug: true);
                while (true)
                {
                    // 200ms 마다 새로운 코인을 체크하고, 새로운 코인이 있다면 해당 코인을 구매한다.
                    // 옵션에 따라 구매하면 알림을 보내는 기능을 추가한다.
                    StartAutoTrade(upbit);
                    Thread.Sleep(200);
                }
            });
            //if (!isWinform)
            //    autoTradeTask.Wait();
            //else
            //    autoTradeTask.Start();
        }

        [Obsolete]
        /// <summary>
        /// 자동 프로세스 종료
        /// </summary>
        public void Exit()
        {
            
        }

        /// <summary>
        /// 업비트 자동 프로세스
        /// </summary>
        private void StartAutoTrade(UpbitCore upbit)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            // 1. 새로 생긴 코인이 있는지 확인
            // _log.Info(LOG_TYPE, doc, "마켓 정보를 로드합니다.");
            upbit.TryGetMarketAll(out var marketAll);
            if (marketAll == null || marketAll.Markets == null)
            {
                _log.Error(LOG_TYPE, doc, "마켓 정보 로드에 실패했습니다.");
                return;
            }

            if (_startingMarketAll == null || _startingMarketAll.Markets == null)
            {
                _log.Error(LOG_TYPE, doc, "기존의 마켓 정보가 올바르지 않습니다.");
                return;
            }
            // _log.Info(LOG_TYPE, doc, "마켓 정보를 로드에 성공했습니다.");

            if (!_isStartCoinsLoged)
            {
                _log.Info(LOG_TYPE, doc, $"처음 시작 마켓 정보\n[{string.Join("],[", marketAll.Markets.Select(x => x.Market))}]");

                _isStartCoinsLoged = true;
            }

            // 시작할 때의 종목 갯수와 현재 종목 갯수를 비교
            List<string>? newMarketIDs = null;
            if (marketAll.Markets.Count == _startingMarketAll.Markets.Count)
            {
                // _log.Info(LOG_TYPE, doc, "마켓 개수에 변화가 감지되지 않았습니다.");
                return;
            }
            else if (marketAll.Markets.Count > _startingMarketAll.Markets.Count)
            {
                //newMarketIDs = marketAll.Markets.Except(_startingMarketAll.Markets)
                //              .ToList()
                //              .FindAll(x => x.Market.Contains("KRW"))
                //              .Select(x => x.Market)
                //              .ToList();

                var newMarkets   = marketAll.Markets.Select(x => x.Market);
                var startMarkets = _startingMarketAll.Markets.Select(x => x.Market);
                newMarketIDs     = newMarkets.Except(startMarkets)
                                             .ToList();
            }
            else
            {
                int gap = _startingMarketAll.Markets.Count - marketAll.Markets.Count;
                _log.Info(LOG_TYPE, doc, $"거래소에서 {gap}개의 마켓이 문을 닫았습니다. 프로그램을 계속 실행됩니다.");
            }
            _startingMarketAll = marketAll;

            // 2. 새로운 코인이 없다면 구문 종료
            if (newMarketIDs == null || newMarketIDs.Count == 0) 
                return;

            // 새로운 코인 감지
            string newCoinDetectedMsg = $"새로운 코인 감지\n[{string.Join("],[", newMarketIDs)}]";
            _log.Info(LOG_TYPE, doc, newCoinDetectedMsg);
            // * 옵션에 따라 특정 플랫폼으로 알림 (일정 시간에 따라 반복하는 기능까지 있으면 좋을 것 같음)
            // 디스코드 알림
            Task.Run(() =>
            {
                while (true)
                {
                    // DiscordHelper.Instance.SendMessage(DiscordHelper.EMarket.Upbit, $"@everyone 새로운 코인 감지 [{string.Join("],[", newMarketIDs)}]");
                    PipeManager.Instance.Servers[DISCORD_PIPE_NAME].Write($"@everyone {newCoinDetectedMsg}");
                    Thread.Sleep(1000);
                }
            });

            // 2-1. 소유 금액(KRW)을 확인할 수 없다면 구문 종료
            upbit.TryGetAccounts(out var accounts);
            if (accounts == null)   // Exception이나 에러 로그가 발생해야 함. 정상적이지 않은 동작이기 때문임.
                return;

            // 2-2. 소유 금액(KRW)을 확인
            bool isHavingKRW = false;
            double haveKRW = 0;
            foreach (var account in accounts.Accounts)
            {
                if (account.Currency == "KRW")
                {
                    isHavingKRW = double.TryParse(account.Balance, out haveKRW);
                    break;
                }
            }

            if (!isHavingKRW || haveKRW == 0)
            {
                _log.Warning(LOG_TYPE, doc, $"소유한 한화(KRW)가 존재하지 않거나, '0'원입니다.");
                return;
            }

            _log.Info(LOG_TYPE, doc, $"소유한 한화(KRW)가 {haveKRW}원 입니다.");

            // 2-2. 새로운 코인의 가격을 확인
            //      만약 복수개의 새로운 코인을 감지했다면 일단은 1개 종류의 코인만 구매할 수 있도록 구현함.
            //      추후 다른 방법을 찾아야 함.
            if (!upbit.TryGetTicker(out var tickers, newMarketIDs[0]) || tickers == null)
                return;
            double newCoinPrice = tickers.Tickers[0].TradePrice;
            _log.Info(LOG_TYPE, doc, $"현재 구매하고자 하는 코인의 정보\n[{newMarketIDs[0]}] | {newCoinPrice} 원");

            // 2-3. 가용할 수 있는 최대범위로 구매
            if (isHavingKRW && haveKRW > newCoinPrice)
            {
                bool orderSuccess = new UpbitCore(isDebug: true).TryOrders(out var order, newMarketIDs[0], UpbitCore.ESide.bid, (haveKRW / newCoinPrice) * 0.9, newCoinPrice + (newCoinPrice * 0.001), UpbitCore.EOrderType.limit);
                if (!orderSuccess || order == null)
                {
                    _log.Warning(LOG_TYPE, doc, "거래가 정상적으로 이루어지지 않았습니다.");
                    return;
                }

                _log.Info(LOG_TYPE, doc, $"{JsonSerializer.Serialize<Orders_VO>(order)}");
            }

            _isStartCoinsLoged = false;
        }
    }
}
