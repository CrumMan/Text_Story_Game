using Supabase;
namespace textSim.database
{
    public static class SupabaseService
    {
        public static Client? SupabaseClient;

        public static async Task Initialize()
        {
            var url = "https://eeytxahbxtsbdpgsjpyc.supabase.co";
            var key = "sb_secret_LK4V-g4Rf2TqsVzj_cMEDw_XBkWPhOb";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            SupabaseClient = new Client(url, key, options);
            await SupabaseClient.InitializeAsync();
        }
    }
}
// using Supabase var key = "sb_secret_LK4V-g4Rf2TqsVzj_cMEDw_XBkWPhOb";
