// #define DISABLESTEAMWORKS

#if !DISABLESTEAMWORKS
using System.Text;
#endif

namespace Steamworks.Mainframe;

/// <summary>
/// Manages the initialization and usage of the Steamworks API for the application.
/// Ensures that the Steamworks API is properly initialized, allows running Steam-specific callbacks, and handles cleanup of Steam resources upon disposal.
/// This class enforces a singleton pattern to prevent multiple instances of the Steam API being initialized simultaneously.
/// </summary>
public static class SteamManager
{
#if !DISABLESTEAMWORKS
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private static SteamAPIWarningMessageHook_t? m_SteamAPIWarningMessageHook { get;set; }
    internal static bool Initialized { get; private set; }
    public static void Initialize(uint appId, bool drmCheck = true)
    {
        // Only one instance of SteamManager at a time!
        if (Initialized)
            throw new Exception("[Steamworks.NET] Tried to Initialize the SteamAPI twice in one session!");

        // CreateSteamAppIdTxt(appId);
        Steam.AppId = appId;
        
        if (!Environment.Is64BitProcess)
            throw new Exception("[Steamworks.NET] Must be 64 bit");

        if (!Packsize.Test())
            throw new Exception("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");

        if (!DllCheck.Test())
            throw new Exception("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");

        // DRM-Check
        if (drmCheck)
        {
            try 
            {
        	    // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
        	    // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
            
        	    // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
        	    // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
        	    // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
        	    if (SteamAPI.RestartAppIfNecessary(new AppId_t(appId)))
        	        throw new Exception("[Steamworks.NET] Steam is not running or the game wasn't started through Steam");
            }
            catch (DllNotFoundException e)
            { 
                // We catch this exception here, as it will be the first occurrence of it.
        	    throw new Exception("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.", e);
            }
        }

        // Initializes the Steamworks API.
        // If this returns false then this indicates one of the following conditions:
        // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
        // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
        // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
        // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
        // [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
        // Valve's documentation for this is located here:
        // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
        Initialized = SteamAPI.Init();
        if (!Initialized)
            throw new Exception("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
        
        // Set up our callback to receive warning messages from Steam.
        // You must launch with "-debug_steamapi" in the launch args to receive warnings.
        m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
        SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
    }

    public static void Shutdown()
    {
        if (!Initialized)
            return;
        
        SteamAPI.Shutdown();
    }

    /// <summary>
    /// Processes pending Steam API callbacks.
    /// Ensures that Steamworks-related events and responses are handled correctly within the application.
    /// This method should be called frequently to maintain proper communication with the Steamworks API.
    /// </summary>
    public static void RunCallbacks()
    {
        if (!Initialized)
            return;

        // Run Steam client callbacks
        SteamAPI.RunCallbacks();
    }
    
    /// <summary>
    /// Handles debug messages from the Steam API when the application is launched with the "-debug_steamapi" argument.
    /// Logs the debug text received from the Steam API as warnings.
    /// </summary>
    /// <param name="nSeverity">The severity level of the debug message. Higher values indicate more severe messages.</param>
    /// <param name="pchDebugText">A string builder containing the debug message text provided by the Steam API.</param>
    private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
    {
        SteamLogger.Warning(pchDebugText.ToString());
    }
    
#else
    public static bool Initialized => false;
#endif // !DISABLESTEAMWORKS
}