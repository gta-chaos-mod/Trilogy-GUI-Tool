namespace GTAChaos.Utils.Stream
{
    public class StreamHandler
    {
        public enum STREAM_MODE
        {
            TWITCH,
            YOUTUBE
        };

        public static IStreamConnection GetStreamConnection()
        {
            switch (Config.Instance().StreamMode)
            {
                case STREAM_MODE.TWITCH:
                    {
                        return Config.Instance().TwitchUsePolls ? new TwitchPollConnection() : new TwitchChatConnection();
                    }
                case STREAM_MODE.YOUTUBE: return new YouTubeChatConnection();
                default: return new DebugConnection();
            }
        }

        public static STREAM_MODE GetStreamMode() => Config.Instance().StreamMode;

        public static bool IsTwitchMode() => GetStreamMode() == STREAM_MODE.TWITCH;

        public static bool IsYouTubeMode() => GetStreamMode() == STREAM_MODE.YOUTUBE;
    }
}
