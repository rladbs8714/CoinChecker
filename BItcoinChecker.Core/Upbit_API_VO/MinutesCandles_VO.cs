using BItcoinChecker.Core;
using System.Text.Json.Serialization;

namespace BitcoinChecker.Core
{

    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.16
     *  
     *  < 목적 >
     *  - 분(minute) 캔들 VO 클래스
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.16 @yoon
     *  - 최초 작성
     *  2024.11.22 @yoon
     *  - SecondsCandles_VO 클래스를 상속받는 구조로 변경
     *  ===========================================================================
     */

    public class MinutesCandles_VO : CandlesBase_VO
    {
        /// <summary>
        /// 분 단위(유닛)
        /// </summary>
        [JsonPropertyName("unit")]
        public int Unit { get; set; }
    }
}
