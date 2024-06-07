using System;
using System.Collections.Generic;
using System.IO;

public static class FileHelper
{
  public static string GetPath(string name) => GetPathUpToFolder(AppContext.BaseDirectory, name);

  public static string GetPathUpToFolder(string relativePath, string folderName)
  {
    if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(folderName))
    {
      throw new ArgumentException("Path and folder name cannot be null or empty.");
    }

    string fullPath = Path.GetFullPath(relativePath);
    string[] pathParts = fullPath.Split(Path.DirectorySeparatorChar);
    int folderIndex = Array.IndexOf(pathParts, folderName);

    if (folderIndex == -1)
    {
      return string.Empty;
    }

    string pathUpToFolder = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts, 0, folderIndex + 1);
    if (!pathUpToFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
    {
      pathUpToFolder += Path.DirectorySeparatorChar;
    }

    return pathUpToFolder;
  }

  public static List<string> ReadFilesFromFolder(string folderName)
  {
    string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
    string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
    string projectDirectory = Directory.GetParent(assemblyDirectory).FullName;

    string folderPath = Path.Combine(projectDirectory, folderName);

    if (!Directory.Exists(folderPath))
    {
      Console.WriteLine($"Folder not found: {folderPath}");
      return null;
    }

    List<string> files = new List<string>();
    foreach (string file in Directory.GetFiles(folderPath))
    {
      string content = File.ReadAllText(file);
      files.Add(content);
    }

    return files;
  }
}
