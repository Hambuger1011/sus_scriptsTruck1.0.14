using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TPImporter
{
	
	public class SpritesheetCollection
    {

        
        private const int SUPPORTED_TPSHEET_VERSION = 40300;

        
        private Dictionary<string, SheetInfo> spriteSheetData = new Dictionary<string, SheetInfo>();

        
        private SpriteGeometryHash spriteGeometries = new SpriteGeometryHash();

        
        private string getAssetDependencyHash(string assetFile)
		{
			string result;
			try
			{
				object[] parameters = new object[]
				{
					assetFile
				};
				MethodInfo method = typeof(AssetDatabase).GetMethod("GetAssetDependencyHash", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(string)
				}, null);
				if (method == null)
				{
					Dbg.Log("AssetDatabase.GetAssetDependencyHash() not defined, disabling optimized loader");
					result = null;
				}
				else
				{
					result = ((Hash128)method.Invoke(null, parameters)).ToString();
				}
			}
			catch (Exception)
			{
				Dbg.Log("Invoking AssetDatabase.GetAssetDependencyHash() failed, disabling optimized loader");
				result = null;
			}
			return result;
		}

		
		public LoaderResult loadSheetData(string dataFile)
		{
			if (this.spriteSheetData.ContainsKey(dataFile) && SettingsDatabase.getInstance().containsDataFile(dataFile))
			{
				string assetDependencyHash = this.getAssetDependencyHash(dataFile);
				if (assetDependencyHash != null)
				{
					if (assetDependencyHash == this.spriteSheetData[dataFile].sheetHash)
					{
						Dbg.Log("Checked " + dataFile + ", no changes found");
						return LoaderResult.NoChanges;
					}
					Dbg.Log("Data file changed, old hash: " + this.spriteSheetData[dataFile].sheetHash + ", new hash: " + assetDependencyHash);
				}
			}
			if (this.spriteSheetData.ContainsKey(dataFile) || SettingsDatabase.getInstance().containsDataFile(dataFile))
			{
				this.unloadSheetData(dataFile);
			}
			string[] array = File.ReadAllLines(dataFile);
			int num = 30302;
			string text = null;
			string text2 = null;
			SheetInfo sheetInfo = new SheetInfo();
			foreach (string text3 in array)
			{
				if (text3.StartsWith(":format="))
				{
					num = int.Parse(text3.Remove(0, 8));
				}
				if (text3.StartsWith(":normalmap="))
				{
					text2 = text3.Remove(0, 11);
				}
				if (text3.StartsWith(":texture="))
				{
					text = text3.Remove(0, 9);
				}
				if (text3.StartsWith(":size="))
				{
					string[] array3 = text3.Remove(0, 6).Split(new char[]
					{
						'x'
					});
					sheetInfo.width = int.Parse(array3[0]);
					sheetInfo.height = int.Parse(array3[1]);
				}
				if (text3.StartsWith(":pivotpoints="))
				{
					string a = text3.Remove(0, 13).ToLower().Trim();
					sheetInfo.pivotPointsEnabled = (a == "enabled");
				}
				if (text3.StartsWith(":borders="))
				{
					string a2 = text3.Remove(0, 9).ToLower().Trim();
					sheetInfo.bordersEnabled = (a2 == "enabled");
				}
				if (text3.StartsWith("# Sprite sheet: "))
				{
					text = text3.Remove(0, 16);
					text = text.Remove(text.LastIndexOf("(") - 1);
				}
			}
			bool flag = array[0].StartsWith("{\"frames\": {");
			bool flag2 = array[0].StartsWith("{");
			if (flag)
			{
				EditorUtility.DisplayDialog("Incompatible data format", "The data file '" + dataFile + "' contains an invalid format!\n\nPlease select 'Unity - Texture2D sprite sheet' as data format in your TexturePacker project!", "Ok");
				return LoaderResult.Error;
			}
			if (num > 40300 || flag2)
			{
				EditorUtility.DisplayDialog("Please update TexturePacker Importer", "Your TexturePacker Importer is too old to import '" + dataFile + "', please load a new version from the asset store!\n\nEnsure that you have selected 'Unity - Texture2D sprite sheet' as data format in your TexturePacker project!", "Ok");
				return LoaderResult.Error;
			}
			text = SpritesheetCollection.fixPath(Path.GetDirectoryName(dataFile) + "/" + text);
			if (text2 != null)
			{
				text2 = SpritesheetCollection.fixPath(Path.GetDirectoryName(dataFile) + "/" + text2);
			}
			SettingsDatabase.getInstance().addSheet(dataFile, text, text2);
			Dictionary<string, SpriteMetaData> dictionary = new Dictionary<string, SpriteMetaData>();
			foreach (string text4 in array)
			{
				if (!string.IsNullOrEmpty(text4) && !text4.StartsWith("#") && !text4.StartsWith(":"))
				{
					string[] array5 = text4.Split(new char[]
					{
						';'
					});
					int num2 = 0;
					if (array5.Length < 7)
					{
						EditorUtility.DisplayDialog("File format error", "Failed to import '" + dataFile + "'", "Ok");
						return LoaderResult.Error;
					}
					SpriteMetaData value = default(SpriteMetaData);
					value.name = this.unescapeName(array5[num2++]);
					if (dictionary.ContainsKey(value.name))
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dataFile);
						EditorUtility.DisplayDialog("Sprite sheet import failed", string.Concat(new string[]
						{
							"Name conflict: Sprite sheet '",
							fileNameWithoutExtension,
							"' contains multiple sprites with name '",
							value.name,
							"'"
						}), "Abort");
						return LoaderResult.Error;
					}
					float x = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float y = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num3 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					float num4 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
					value.rect = new Rect(x, y, num3, num4);
					string x2 = array5[num2++];
					string y2 = array5[num2++];
					sheetInfo.pivotPointsEnabled = (sheetInfo.pivotPointsEnabled && this.parsePivotPoint(x2, y2, ref value));
					if (num >= 40300)
					{
						float x3 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
						float z = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
						float w = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
						float y3 = float.Parse(array5[num2++], CultureInfo.InvariantCulture);
						value.border = new Vector4(x3, y3, z, w);
					}
					dictionary.Add(value.name, value);
					if (Application.unityVersion[0] == '4')
					{
						if (num2 < array5.Length)
						{
							EditorUtility.DisplayDialog("Sprite sheet import failed.", "Import of polygon sprites requires Unity 5!", "Abort");
							return LoaderResult.Error;
						}
					}
					else
					{
						SpriteGeometry spriteGeometry = new SpriteGeometry();
						if (num2 < array5.Length)
						{
							num2 = spriteGeometry.parse(array5, num2);
							if (num2 < 0)
							{
								Debug.LogError("format error in file " + dataFile);
								return LoaderResult.Error;
							}
							sheetInfo.polygonsEnabled = true;
						}
						else
						{
							spriteGeometry.setQuad((int)num3, (int)num4);
						}
						this.spriteGeometries.addGeometry(text, value.name, spriteGeometry);
					}
				}
			}
			sheetInfo.metadata = new SpriteMetaData[dictionary.Count];
			dictionary.Values.CopyTo(sheetInfo.metadata, 0);
			if (!sheetInfo.pivotPointsEnabled)
			{
				Dbg.Log("Pivot points disabled in .tpsheet, using 'center'");
				for (int k = 0; k < sheetInfo.metadata.Length; k++)
				{
					sheetInfo.metadata[k].pivot = new Vector2(0.5f, 0.5f);
					sheetInfo.metadata[k].alignment = 0;
				}
			}
			sheetInfo.sheetHash = this.getAssetDependencyHash(dataFile);
			this.spriteSheetData[dataFile] = sheetInfo;
			Dbg.Log("Successfully loaded tpsheet file containing " + sheetInfo.metadata.Length + " sprite definitions");
			return LoaderResult.Loaded;
		}

		
		private static string fixPath(string path)
		{
			return (path != null) ? path.Replace("\\", "/").Replace("//", "/") : null;
		}

		
		private bool parsePivotPoint(string sx, string sy, ref SpriteMetaData smd)
		{
			float x;
			float y;
			try
			{
				x = float.Parse(sx, CultureInfo.InvariantCulture);
				y = float.Parse(sy, CultureInfo.InvariantCulture);
			}
			catch
			{
				return false;
			}
			smd.pivot = new Vector2(x, y);
			if (x == 0f && y == 0f)
			{
				smd.alignment = 6;
			}
			else if ((double)x == 0.5 && y == 0f)
			{
				smd.alignment = 7;
			}
			else if (x == 1f && y == 0f)
			{
				smd.alignment = 8;
			}
			else if (x == 0f && (double)y == 0.5)
			{
				smd.alignment = 4;
			}
			else if ((double)x == 0.5 && (double)y == 0.5)
			{
				smd.alignment = 0;
			}
			else if (x == 1f && (double)y == 0.5)
			{
				smd.alignment = 5;
			}
			else if (x == 0f && y == 1f)
			{
				smd.alignment = 1;
			}
			else if ((double)x == 0.5 && y == 1f)
			{
				smd.alignment = 2;
			}
			else if (x == 1f && y == 1f)
			{
				smd.alignment = 3;
			}
			else
			{
				smd.alignment = 9;
			}
			return true;
		}

		
		public void unloadSheetData(string dataFile)
		{
			this.spriteSheetData.Remove(dataFile);
			SettingsDatabase.getInstance().removeSheet(dataFile, true);
		}

		
		private string unescapeName(string name)
		{
			return name.Replace("%23", "#").Replace("%3A", ":").Replace("%3B", ";").Replace("%25", "%").Replace("/", "-");
		}

		
		public SheetInfo sheetInfoForSpriteFile(string textureFile)
		{
			string text = SettingsDatabase.getInstance().dataFileForSpriteFile(textureFile);
			if (text == null)
			{
				return null;
			}
			if (!this.spriteSheetData.ContainsKey(text))
			{
				this.loadSheetData(text);
			}
			return (!this.spriteSheetData.ContainsKey(text)) ? null : this.spriteSheetData[text];
		}

		
		public void assignGeometries(string texturePath, Sprite[] sprites)
		{
			SheetInfo sheetInfo = this.sheetInfoForSpriteFile(texturePath);
			if (sheetInfo == null || !sheetInfo.polygonsEnabled)
			{
				return;
			}
			float num = 1f;
			if (sheetInfo.width > 0 && sprites.Length > 0)
			{
				num = Math.Min((float)sprites[0].texture.width / (float)sheetInfo.width, (float)sprites[0].texture.height / (float)sheetInfo.height);
				Dbg.Log("Texture scale factor: " + num);
			}
			Dbg.Log("Updating geometry of " + sprites.Length + " sprites ");
			try
			{
				foreach (Sprite sprite in sprites)
				{
					this.spriteGeometries.assignGeometryToSprite(texturePath, sprite, num);
				}
			}
			catch (InvalidVertexException)
			{
				Dbg.Log("Sprite geometry doesn't match sprite frame (texture meta data might be outdated, geometries will be updated later)");
			}
		}
	}




    public class SheetInfo
    {
        public SpriteMetaData[] metadata = null;
        public int width = -1;
        public int height = -1;
        public bool pivotPointsEnabled = true;
        public bool bordersEnabled = false;
        public bool polygonsEnabled = false;
        public string sheetHash;
    }
}
