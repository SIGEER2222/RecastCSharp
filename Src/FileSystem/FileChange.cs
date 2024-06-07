namespace RecastCSharp.FileSystem;

public struct FileChange
{
  public string Filename;
  public ChangeType Change;
}

public enum ChangeType { Added, Changed, Removed }