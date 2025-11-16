using Steamworks.Mainframe;

SteamApi.Initialize(460, false);

bool exit = false;
Console.CancelKeyPress += (sender, e) =>
{
    exit = true;
};

Console.WriteLine($"SteamAppId: {Steam.AppId}");
Console.WriteLine($"SteamId: {Steam.SteamId}");
Console.WriteLine($"SteamUsername: {Steam.Username}");

var friends = Steam.GetFriends().ToArray();
Console.WriteLine($"FriendsCount: {friends.Length}");
foreach (var friend in friends)
    Console.WriteLine($"SteamFriend: {friend}");

while (!exit)
{
    SteamApi.RunCallbacks();
    await Task.Delay(16); // 16ms (60fps)
}
SteamApi.Shutdown();
Console.WriteLine("Testing Complete");
