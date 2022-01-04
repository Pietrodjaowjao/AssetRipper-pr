﻿using AssetRipper.Core.Extensions;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.Parser.Files.ResourceFiles;
using AssetRipper.Core.Parser.Files.SerializedFiles;

namespace AssetRipper.Core.Classes.AudioClip
{
	public interface IStreamedResource : IAsset
	{
		string Source { get; set; }
		ulong Offset { get; set; }
		/// <summary>
		/// 5.0.0f1 and greater (unknown version)
		/// </summary>
		ulong Size { get; set; }
	}

	public static class StreamedResourceExtensions
	{
		/// <summary>
		/// 5.0.0f1 and greater (unknown version)
		/// </summary>
		public static bool HasSize(UnityVersion version)
		{
			return version.IsGreaterEqual(5, 0, 0, UnityVersionType.Final);
		}

		public static bool CheckIntegrity(this IStreamedResource streamedResource, ISerializedFile file)
		{
			if (!streamedResource.IsSet())
			{
				return true;
			}
			if (!HasSize(file.Version))
			{
				// I think they read data by its type for this verison, so I can't even export raw data :/
				return false;
			}

			return file.Collection.FindResourceFile(streamedResource.Source) != null;
		}

		public static byte[] GetContent(this IStreamedResource streamedResource, ISerializedFile file)
		{
			IResourceFile res = file.Collection.FindResourceFile(streamedResource.Source);
			if (res == null)
			{
				return null;
			}
			if (streamedResource.Size == 0)
			{
				return null;
			}

			byte[] data = new byte[streamedResource.Size];
			res.Stream.Position = (long)streamedResource.Offset;
			res.Stream.ReadBuffer(data, 0, data.Length);
			return data;
		}

		public static bool IsSet(this IStreamedResource streamedResource) => !string.IsNullOrEmpty(streamedResource.Source);
	}
}
