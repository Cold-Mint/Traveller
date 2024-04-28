using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.dataPack.local;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using Microsoft.EntityFrameworkCore.Storage;
using Array = System.Array;

namespace ColdMint.scripts.dataPack;

/// <summary>
/// <para>Packet manager</para>
/// <para>数据包管理器</para>
/// </summary>
public static class DataPackManager
{
	/// <summary>
	/// <para>When a packet is scanned</para>
	/// <para>当一个数据包被扫描到时</para>
	/// </summary>
	public static Action<IDataPack>? OnScanComplete;


	/// <summary>
	/// <para>Load all packets in a directory</para>
	/// <para>加载某个目录下的所有数据包</para>
	/// </summary>
	/// <param name="path"></param>
	public static async Task<IDataPack[]> ScanAllDataPack(string path)
	{
		if (!Directory.Exists(path))
		{
			return Array.Empty<IDataPack>();
		}

		var dataPacks = new List<IDataPack>();
		var files = Directory.GetFiles(path);
		foreach (var file in files)
		{
			var dataPack = await ScanSingleDataPack(file);
			if (dataPack == null)
			{
				continue;
			}

			dataPacks.Add(dataPack);
		}

		return dataPacks.ToArray();
	}

	/// <summary>
	/// <para>Load a single packet</para>
	/// <para>加载单个数据包</para>
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public async static Task<IDataPack?> ScanSingleDataPack(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}

		var dataPack = new LocalDataPack(path);
		await dataPack.BuildIndex();
		await dataPack.LoadManifest();
		if (OnScanComplete != null)
		{
			OnScanComplete.Invoke(dataPack);
		}

		return dataPack;
	}
}
