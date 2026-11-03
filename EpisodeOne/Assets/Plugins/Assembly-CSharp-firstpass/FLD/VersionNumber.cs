using UnityEngine;

namespace FLD
{
	public struct VersionNumber
	{
		public int Major;

		public int Minor;

		public int Revision;

		public int Build;

		public bool HasBuildNumber
		{
			get
			{
				return Build >= 0;
			}
		}

		public VersionNumber(int pMajor, int pMinor, int pRevision)
		{
			Major = pMajor;
			Minor = pMinor;
			Revision = pRevision;
			Build = -1;
		}

		public VersionNumber(string pVersion)
		{
			Major = (Minor = (Revision = 0));
			Build = -1;
			if (!string.IsNullOrEmpty(pVersion))
			{
				string[] array = pVersion.Split('.');
				bool flag = true;
				if (array.Length >= 1)
				{
					flag &= int.TryParse(array[0], out Major);
				}
				if (array.Length >= 2)
				{
					flag &= int.TryParse(array[1], out Minor);
				}
				if (array.Length >= 3)
				{
					flag &= int.TryParse(array[2], out Revision);
				}
				if (array.Length >= 4)
				{
					flag &= int.TryParse(array[3], out Build);
				}
				if (!flag)
				{
					Debug.LogWarning(string.Format("Failed to parse version string \"{0}\"", pVersion));
					Major = (Minor = (Revision = 0));
					Build = -1;
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is VersionNumber)
			{
				VersionNumber versionNumber = (VersionNumber)obj;
				if (Major != versionNumber.Major || Minor != versionNumber.Minor || Revision != versionNumber.Revision)
				{
					return false;
				}
				if (HasBuildNumber && versionNumber.HasBuildNumber && Build != versionNumber.Build)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Major ^ Minor ^ Revision ^ Build;
		}

		public override string ToString()
		{
			return ToString(true);
		}

		public string ToString(bool pIncludeBuildNumber)
		{
			string text = Major.ToString();
			if (Minor >= 0)
			{
				text = text + "." + Minor;
				if (Revision >= 0)
				{
					text = text + "." + Revision;
					if (pIncludeBuildNumber && Build >= 0)
					{
						text = text + "." + Build;
					}
				}
			}
			return text;
		}

		public static bool operator ==(VersionNumber lhs, VersionNumber rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(VersionNumber lhs, VersionNumber rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <=(VersionNumber lhs, VersionNumber rhs)
		{
			if (lhs.Major > rhs.Major)
			{
				return false;
			}
			if (lhs.Major == rhs.Major && lhs.Minor > rhs.Minor)
			{
				return false;
			}
			if (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Revision > rhs.Revision)
			{
				return false;
			}
			if (lhs.HasBuildNumber && rhs.HasBuildNumber && lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Revision == rhs.Revision && lhs.Build > rhs.Build)
			{
				return false;
			}
			return true;
		}

		public static bool operator >(VersionNumber lhs, VersionNumber rhs)
		{
			return !(lhs <= rhs);
		}

		public static bool operator >=(VersionNumber lhs, VersionNumber rhs)
		{
			if (lhs.Major < rhs.Major)
			{
				return false;
			}
			if (lhs.Major == rhs.Major && lhs.Minor < rhs.Minor)
			{
				return false;
			}
			if (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Revision < rhs.Revision)
			{
				return false;
			}
			if (lhs.HasBuildNumber && rhs.HasBuildNumber && lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Revision == rhs.Revision && lhs.Build < rhs.Build)
			{
				return false;
			}
			return true;
		}

		public static bool operator <(VersionNumber lhs, VersionNumber rhs)
		{
			return !(lhs >= rhs);
		}
	}
}
