using System.Reflection;

namespace Generalibrary
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.11
     *  
     *  < 설명 >
     *  - 이 클래스는 상속받아 사용하는 것이 가장 적합하다.
     *  
     *  < 목적 >
     *  - .ini 파일을 유연하게 파싱하기 위함
     *  - 항목을 Dictionary로 관리하면 좋겠음. 그래서 나중에 .ini파일 저장할 때도 항목 키값을 변수 이름처럼 지으면 관리하기 편할것임.
     *  
     *  < TODO >
     *  - 파일 읽기
     *  - 파싱되는 항목 정의
     *  - 파싱이 무시되는 항목 정의 (주석 등)
     *  
     *  < History >
     *  2024.07.11 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class IniParser
    {
        // ====================================================================
        // CONSTANTS
        // ====================================================================

        /// <summary>
        /// log type
        /// </summary>
        private string LOG_TYPE = "IniParser";
        /// <summary>
        /// log
        /// </summary>
        private readonly ILogManager LOG = LogManager.Instance;


        // ====================================================================
        // PROPERTIES
        // ====================================================================

        /// <summary>
        /// ini 파일에서 읽은 설정 값
        /// </summary>
        protected Dictionary<string, string> Options { get; private set; }


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        /// <summary>
        /// IniParser의 기본 생성자
        /// 여기서 파싱을 진행하고, 파싱에 실패하면 Exception이 발생한다.
        /// </summary>
        /// <param name="fileName">.ini 파일의 경로 (상대/절대 경로 모두 사용 가능하다.)</param>
        /// <exception cref="IniParsingException">ini파일을 정상적으로 읽을 수 없었을 때 발생하는 Exception</exception>
        public IniParser(string fileName, char separator = '=')
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            if (string.IsNullOrEmpty(fileName))
            {
                string errMsg = $"파일 경로가 공백 혹은 Null 입니다.";
                LOG.Error(LOG_TYPE, doc, errMsg);
                throw new ArgumentException(errMsg);
            }

            // 파일 경로가 상대 경로라면 절대경로로 바꿔줌
            if (Uri.TryCreate(fileName, UriKind.Relative, out _))
                fileName = Path.Combine(Environment.CurrentDirectory, fileName);

            // 파일 유효성 검사
            if (!File.Exists(fileName))
            {
                string errMsg = $"{fileName}경로에 파일이 없습니다. 파일 경로를 다시 확인해주세요.";
                LOG.Error(LOG_TYPE, doc, errMsg);
                throw new FileNotFoundException(errMsg);
            }

            string[] lines;
            try
            {
                lines = File.ReadAllLines(fileName);
            }
            catch (IOException ex)
            {
                LOG.Error(LOG_TYPE, doc, ex.Message, exception: ex);
                throw;
            }

            if (lines.Length > 0)
            {
                LOG.Warning(LOG_TYPE, doc, $"{fileName}의 내용이 비어있습니다.");
            }
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 라인이 공백이거나, 주석으로 시작되는 라인은 건너뜀.
                if (string.IsNullOrEmpty(line) ||
                    line[0] == '#' || 
                    line[0] == ';') 
                    continue;

                // 라인에서 
                int separatorIdx = line.IndexOf(separator);

                if (separatorIdx == -1)
                {
                    LOG.Warning(LOG_TYPE, doc, $"{fileName}파일의 {i}번째 줄에는 구분자\'{separator}\'가 없습니다.");
                    continue;
                }

                if (separatorIdx == 0)
                {
                    LOG.Warning(LOG_TYPE, doc, $"{fileName}파일의 {i}번째 줄에는 Key값이 없습니다.");
                    continue;
                }

                if (separatorIdx + 1 > line.Length)
                {
                    LOG.Warning(LOG_TYPE, doc, $"{fileName}파일의 {i}번째 줄에는 Value값이 없습니다.");
                    continue;
                }

                string key   = line.Substring(0, separatorIdx);
                string value = line.Substring(separatorIdx + 1);

                Options.Add(key, value);
            }

            Options = new Dictionary<string, string>();
        }
    }
}
