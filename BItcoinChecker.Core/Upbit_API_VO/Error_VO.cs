using System.Text.Json.Serialization;

namespace BitcoinChecker.Core
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.19
     *  
     *  < 목적 >
     *  - 업비트에서 에러 응답 Json을 반환하였을 시 저장할 VO 클래스
     *  
     *  < TODO >
     *  - 
     *  
     *  < History >
     *  2024.06.19 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class Error_VO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
