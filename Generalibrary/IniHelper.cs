using IniParser;
using IniParser.Model;

namespace Generalibrary
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.08
     *  
     *  < 목적 >
     *  - Ini 데이터를 효율적으로 불러올 수 있다.
     *  
     *  < 중요:ini 작성 규칙 >
     *  - 섹션의 이름은 대문자이다.    (e.g. [URL], [KEY], [PIPE:DISCORD])
     *  - 키의 이름은 카멜케이스이다.  (e.g. upbitBase, accessKey, use, name)
     *  - 주석은 '#'으로 시작한다.     (e.g. # 이것은 주석입니다.)
     *  - 주석 외 한글은 작성하지 않는다.
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.06.12 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public abstract class IniHelper
    {
        private FileIniDataParser? _parser;
        private IniData? _data = null;

        private IniHelper() { }

        protected IniHelper(string iniPath)
        {
            if (string.IsNullOrEmpty(iniPath))
                throw new ArgumentNullException("ini파일의 경로를 입력하지 않았습니다.");
            if (!Uri.TryCreate(iniPath, UriKind.Relative, out var uri))
                throw new ArgumentException("ini파일의 경로가 유효하지 않습니다.");

            _parser = new FileIniDataParser();
            _data   = _parser.ReadFile(iniPath);
        }

        public string GetIniData(string section, string key)
        {
            return _data[section][key];
        }
    }
}
