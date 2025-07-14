using System.Text;

namespace Steamworks.Mainframe;

/// <summary>
/// Represents a friend on the Steam platform, including their Steam ID and username.
/// </summary>
public readonly struct SteamFriend(ulong steamId, string username, EPersonaState state = default) : IEquatable<SteamFriend>, IComparable<SteamFriend>
{
	public readonly ulong SteamId = steamId;
	public readonly string Username = username;
	public readonly EPersonaState State = state;

	public bool IsMe => SteamId == Steam.SteamId;

	public override string ToString()
	{
		var str = new StringBuilder();
		str.AppendLine($"Username: {Username}, SteamId: {SteamId}");
		str.AppendLine($"\tState: {State}");
		return str.ToString();
	}

	public bool Equals(SteamFriend other)
	{
		return SteamId == other.SteamId;
	}

	public override bool Equals(object? obj)
	{
		return obj is SteamFriend other && Equals(other);
	}

	public override int GetHashCode()
	{
		return SteamId.GetHashCode();
	}

	public int CompareTo(SteamFriend other)
	{
		return SteamId.CompareTo(other.SteamId);
	}

	// public Sprite GetAvatarSprite()
	// {
	// 	var sprite = SteamAvatar.GetAvatar(SteamId);
	// 	if (!sprite)
	// 		SteamLogger.Debug($"Failed to get avatar for: {ToString()}");
	// 	return sprite;
	// }

    public static bool operator ==(SteamFriend left, SteamFriend right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SteamFriend left, SteamFriend right)
    {
        return !(left == right);
    }
}