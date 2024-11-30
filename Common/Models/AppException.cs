namespace Approvers.King.Common;

/// <summary>
/// アプリ内の汎用例外
/// </summary>
public class AppException(string message) : Exception(message);
