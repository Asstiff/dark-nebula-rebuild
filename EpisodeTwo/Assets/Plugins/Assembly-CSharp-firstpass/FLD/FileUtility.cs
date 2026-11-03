using System.IO;

namespace FLD
{
	public static class FileUtility
	{
		public static void EnsureDirectoryExists(string path)
		{
			string[] array = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
			EnsureDirectoryExists(array, array.Length);
		}

		private static void EnsureDirectoryExists(string[] pathComponents, int count)
		{
			if (count != 0)
			{
				string path = string.Join(Path.DirectorySeparatorChar.ToString(), pathComponents, 0, count);
				if (!Directory.Exists(path))
				{
					EnsureDirectoryExists(pathComponents, count - 1);
					Directory.CreateDirectory(path);
				}
			}
		}
	}
}
