using Framework;

using System;
using UnityEngine;

namespace Framework
{
	public class DetectRenderQuality
	{
		private static bool TryGetInt(ref int val, string str)
		{
			val = 0;
			bool result;
			try
			{
				val = Convert.ToInt32(str);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private static enGameRenderQuality checkGPU_Adreno(string[] tokens)
		{
			int num = 0;
			for (int i = 1; i < tokens.Length; i++)
			{
				if (DetectRenderQuality.TryGetInt(ref num, tokens[i]))
				{
					if (num < 200)
					{
						return enGameRenderQuality.eLow;
					}
					if (num < 300)
					{
						if (num > 220)
						{
							return enGameRenderQuality.eLow;
						}
						return enGameRenderQuality.eLow;
					}
					else if (num < 400)
					{
						if (num >= 330)
						{
							return enGameRenderQuality.eHigh;
						}
						if (num >= 320)
						{
							return enGameRenderQuality.eMedium;
						}
						return enGameRenderQuality.eLow;
					}
					else if (num >= 400)
					{
						if (num < 420)
						{
							return enGameRenderQuality.eMedium;
						}
						return enGameRenderQuality.eHigh;
					}
				}
			}
			return enGameRenderQuality.eLow;
		}

		private static enGameRenderQuality checkGPU_PowerVR(string[] tokens)
		{
			bool flag = false;
			bool flag2 = false;
			enGameRenderQuality result = enGameRenderQuality.eLow;
			int num = 0;
			for (int i = 1; i < tokens.Length; i++)
			{
				string text = tokens[i];
				if (text == "sgx")
				{
					flag = true;
				}
				else
				{
					if (text == "rogue")
					{
						flag2 = true;
						break;
					}
					if (flag)
					{
						bool flag3 = false;
						int num2 = text.IndexOf("mp");
						if (num2 > 0)
						{
							DetectRenderQuality.TryGetInt(ref num, text.Substring(0, num2));
							flag3 = true;
						}
						else if (DetectRenderQuality.TryGetInt(ref num, text))
						{
							for (int j = i + 1; j < tokens.Length; j++)
							{
								text = tokens[j].ToLower();
								if (text.IndexOf("mp") >= 0)
								{
									flag3 = true;
									break;
								}
							}
						}
						if (num > 0)
						{
							if (num < 543)
							{
								result = enGameRenderQuality.eLow;
							}
							else if (num == 543)
							{
								result = enGameRenderQuality.eLow;
							}
							else if (num == 544)
							{
								result = enGameRenderQuality.eLow;
								if (flag3)
								{
									result = enGameRenderQuality.eMedium;
								}
							}
							else
							{
								result = enGameRenderQuality.eMedium;
							}
							break;
						}
					}
					else if (text.Length > 4)
					{
						char c = text[0];
						char c2 = text[1];
						if (c == 'g')
						{
							if (c2 >= '0' && c2 <= '9')
							{
								DetectRenderQuality.TryGetInt(ref num, text.Substring(1));
							}
							else
							{
								DetectRenderQuality.TryGetInt(ref num, text.Substring(2));
							}
							if (num > 0)
							{
								if (num >= 7000)
								{
									result = enGameRenderQuality.eHigh;
								}
								else if (num >= 6000)
								{
									if (num < 6100)
									{
										result = enGameRenderQuality.eLow;
									}
									else if (num < 6400)
									{
										result = enGameRenderQuality.eMedium;
									}
									else
									{
										result = enGameRenderQuality.eHigh;
									}
								}
								else
								{
									result = enGameRenderQuality.eLow;
								}
								break;
							}
						}
					}
				}
			}
			if (flag2)
			{
				result = enGameRenderQuality.eHigh;
			}
			return result;
		}

		private static enGameRenderQuality checkGPU_Mali(string[] tokens)
		{
			int num = 0;
			enGameRenderQuality result = enGameRenderQuality.eLow;
			for (int i = 1; i < tokens.Length; i++)
			{
				string text = tokens[i];
				if (text.Length >= 3)
				{
					int num2 = text.LastIndexOf("mp");
					bool flag = text[0] == 't';
					if (num2 > 0)
					{
						int num3 = (!flag) ? 0 : 1;
						text = text.Substring(num3, num2 - num3);
						DetectRenderQuality.TryGetInt(ref num, text);
					}
					else
					{
						if (flag)
						{
							text = text.Substring(1);
						}
						if (DetectRenderQuality.TryGetInt(ref num, text))
						{
							for (int j = i + 1; j < tokens.Length; j++)
							{
								text = tokens[j];
								if (text.IndexOf("mp") >= 0)
								{
									break;
								}
							}
						}
					}
					if (num > 0)
					{
						if (num < 400)
						{
							result = enGameRenderQuality.eLow;
						}
						else if (num < 500)
						{
							if (num == 400)
							{
								result = enGameRenderQuality.eLow;
							}
							else if (num == 450)
							{
								result = enGameRenderQuality.eMedium;
							}
							else
							{
								result = enGameRenderQuality.eLow;
							}
						}
						else if (num < 700)
						{
							if (!flag)
							{
								result = enGameRenderQuality.eLow;
							}
							else if (num < 620)
							{
								result = enGameRenderQuality.eLow;
							}
							else if (num < 628)
							{
								result = enGameRenderQuality.eMedium;
							}
							else
							{
								result = enGameRenderQuality.eHigh;
							}
						}
						else if (!flag)
						{
							result = enGameRenderQuality.eLow;
						}
						else
						{
							result = enGameRenderQuality.eHigh;
						}
						break;
					}
				}
			}
			return result;
		}

		private static enGameRenderQuality checkGPU_Tegra(string[] tokens)
		{
			bool flag = false;
			int num = 0;
			enGameRenderQuality result = enGameRenderQuality.eLow;
			for (int i = 1; i < tokens.Length; i++)
			{
				if (DetectRenderQuality.TryGetInt(ref num, tokens[i]))
				{
					flag = true;
					if (num >= 4)
					{
						result = enGameRenderQuality.eHigh;
						break;
					}
					if (num == 3)
					{
						result = enGameRenderQuality.eMedium;
						break;
					}
				}
				else
				{
					string text = tokens[i];
					if (text == "k1")
					{
						result = enGameRenderQuality.eHigh;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				result = enGameRenderQuality.eMedium;
			}
			return result;
		}

		private static enGameRenderQuality checkGPU_Android(string gpuName)
		{
			enGameRenderQuality result = enGameRenderQuality.eLow;
			int systemMemorySize = SystemInfo.systemMemorySize;
			if (systemMemorySize < 1500)
			{
				return enGameRenderQuality.eLow;
			}
			gpuName = gpuName.ToLower();
			char[] array = new char[]
			{
				' ',
				'\t',
				'\r',
				'\n',
				'+',
				'-',
				':'
			};
			string[] array2 = gpuName.Split(array, 1);
			if (array2 == null || array2.Length == 0)
			{
				return enGameRenderQuality.eLow;
			}
			if (array2[0].Contains("vivante"))
			{
				result = enGameRenderQuality.eLow;
			}
			else if (array2[0] == "adreno")
			{
				result = DetectRenderQuality.checkGPU_Adreno(array2);
			}
			else if (array2[0] == "powervr" || array2[0] == "imagination" || array2[0] == "sgx")
			{
				result = DetectRenderQuality.checkGPU_PowerVR(array2);
			}
			else if (array2[0] == "arm" || array2[0] == "mali" || (array2.Length > 1 && array2[1] == "mali"))
			{
				result = DetectRenderQuality.checkGPU_Mali(array2);
			}
			else if (array2[0] == "tegra" || array2[0] == "nvidia")
			{
				result = DetectRenderQuality.checkGPU_Tegra(array2);
			}
			return result;
		}

		private static void checkDevice_Android(ref enGameRenderQuality q)
		{
			string text = SystemInfo.deviceModel.ToLower();
			if (text == "samsung gt-s7568i")
			{
				q = enGameRenderQuality.eLow;
			}
			else if (text == "xiaomi 1s")
			{
				q = enGameRenderQuality.eMedium;
			}
			else if (text == "xiaomi 2013022")
			{
				q = enGameRenderQuality.eMedium;
			}
			else if (text == "samsung sch-i959")
			{
				q = enGameRenderQuality.eMedium;
			}
			else if (text == "xiaomi mi 3")
			{
				q = enGameRenderQuality.eHigh;
			}
			else if (text == "xiaomi mi 2a")
			{
				q = enGameRenderQuality.eMedium;
			}
			else if (text == "xiaomi hm 1sc")
			{
				q = enGameRenderQuality.eLow;
			}
		}

		public static enGameRenderQuality check_Android()
		{
			enGameRenderQuality result = DetectRenderQuality.checkGPU_Android(SystemInfo.graphicsDeviceName);
			DetectRenderQuality.checkDevice_Android(ref result);
			return result;
		}
	}
}
