using Steamworks.Mainframe;

if(!Environment.Is64BitProcess)
    throw new Exception("Must be 64 bit");

var steamManager = new SteamManager(460, false);

bool exit = false;
Console.CancelKeyPress += (sender, e) =>
{
    exit = true;
};

while (!exit)
{
    steamManager.RunCallbacks();
    await Task.Delay(16); // 16ms (60fps)
}
steamManager.Dispose();
Console.WriteLine("Testing Complete");
