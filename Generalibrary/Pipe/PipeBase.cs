﻿using Generalibrary.Pipe;
using System.IO.Pipes;
using System.Reflection;

namespace Generalibrary
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.07
     *  
     *  < 목적 >
     *  - 파이프 통신 클래스의 베이스
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.07 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public abstract class PipeBase : IPipeBase
    {
        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private const string LOG_TYPE = "PipeBase";

        protected readonly string PIPE_NAME;


        // ====================================================================
        // FIELDS
        // ====================================================================

        /// <summary>
        /// log
        /// </summary>
        protected ILogManager _log = LogManager.Instance;
        /// <summary>
        /// 파이프 문자열 스트림
        /// </summary>
        protected StreamString? _streamString;
        /// <summary>
        /// 파이프 파일 스트림
        /// </summary>
        protected ReadFileToStream? _readFileToSteam;

        private object _lock = new object();


        // ====================================================================
        // PROPERTIES
        // ====================================================================

        /// <summary>
        /// 파이프 연결 여부
        /// </summary>
        public abstract bool IsConnected { get; }


        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// 파이프 베이스 생성자
        /// </summary>
        /// <param name="pipeName">파이프 이름</param>
        /// <exception cref="PipeNameNullOrEmptyException">파이프 라인 이름이 공백</exception>
        public PipeBase(string pipeName)
        {
            PIPE_NAME = pipeName;

            if (string.IsNullOrEmpty(PIPE_NAME))
                throw new PipeNameNullOrEmptyException("파이프 라인 이름이 공백입니다.");
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// 스트림에 문자열을 쓴다.
        /// </summary>
        /// <param name="message">쓸 문자열</param>
        public void Write(string message)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            if (string.IsNullOrEmpty(message))
                return;

            if (!IsConnected)
                return;

            lock (_lock)
            {
                try
                {
                    _streamString.WriteString(message);
                    _log.Info(LOG_TYPE, doc, $"메시지 전송\n{message}");
                }
                catch (IOException ex)
                {
                    _log.Error(LOG_TYPE, doc, $"[{PIPE_NAME}] 파이프의 메시지가 정상적으로 전달되지 않았습니다.", exception: ex);
                }
                catch (NullReferenceException ex)
                {
                    _log.Error(LOG_TYPE, doc, $"[{PIPE_NAME}] 파이프 문자열 스트림이 NULL 입니다.", exception: ex);
                }
            }
        }

        /// <summary>
        /// 스트림에서 문자열을 읽어 반환한다. 스트림이 비어있거나 Null이라면 <seealso cref="string.Empty"/>를 반환한다.
        /// </summary>
        /// <returns>읽은 문자열</returns>
        public string ReadString()
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            lock (_lock)
            {
                return _streamString.ReadString();
            }
        }
    }
}
