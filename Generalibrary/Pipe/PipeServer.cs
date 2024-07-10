using System.Diagnostics;
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
     *  - 파이프 서버 클래스
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.07 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class PipeServer : PipeBase
    {
        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private const string LOG_TYPE = "PipeServer";


        // ====================================================================
        // FIELDS
        // ====================================================================

        /// <summary>
        /// 파이프 서버 스트림
        /// </summary>
        private NamedPipeServerStream _server;


        // ====================================================================
        // PROPERTIES
        // ====================================================================
        
        /// <summary>
        /// 파이프 연결 여부
        /// </summary>
        public override bool IsConnected => _server != null && _server.IsConnected;


        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// 서버 파이프 생성자
        /// </summary>
        /// <param name="pipeName">파이프 이름</param>
        /// <param name="filePath">클라이언트 프로그램 경로</param>
        /// <param name="pipeDirection">파이프 방향 (In,Out,InOut)</param>
        public PipeServer(string pipeName, string filePath, PipeDirection pipeDirection) : base(pipeName)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            Process client = new Process();
            client.StartInfo.FileName = filePath;
            client.StartInfo.ArgumentList.Add(PIPE_NAME);
            client.StartInfo.UseShellExecute = true;
            client.StartInfo.CreateNoWindow = false;
            
            _server = new NamedPipeServerStream(PIPE_NAME, pipeDirection);
            client.Start();
            _server.WaitForConnection();
            
            _streamString = new StreamString(_server);
        }
    }
}
