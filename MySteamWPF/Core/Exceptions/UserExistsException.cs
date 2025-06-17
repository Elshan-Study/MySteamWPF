namespace MySteamWPF.Core.Exceptions;

public class UserExistsException(string message) : Exception(message);