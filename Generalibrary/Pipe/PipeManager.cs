namespace Generalibrary
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.08
     *  
     *  < 목적 >
     *  - 파이프 매니저
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.07 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class PipeManager
    {

        #region SINGLETON

        private static PipeManager _instance;
        public static PipeManager Instance
        {
            get
            {
                _instance ??= new PipeManager();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        // ====================================================================
        // PROPERTIES
        // ====================================================================

        public Dictionary<string, PipeServer> Servers;

        public Dictionary<string, PipeClient> Clients;


        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        private PipeManager()
        {
            Servers = new Dictionary<string, PipeServer>();
            Clients = new Dictionary<string, PipeClient>();
        }
    }
}
