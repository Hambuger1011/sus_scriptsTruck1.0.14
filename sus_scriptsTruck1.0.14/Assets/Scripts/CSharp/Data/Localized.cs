using UnityEngine;
using System.Collections;
/// <summary>
/// 本地化
/// </summary>
public class Localized 
{
	private static Localized	instance = null; // singleton
    private readonly string LocalizedKey = "OnyxLanguage";
	public static Localized Instance
	{
		get
		{
			if (instance == null) instance = new Localized();
			return instance;    
		}
	}
	protected Localized() {}

	public void Init(){CheckLanguage();}

	public void SetCurrentLanguage(SystemLanguage vLanguage)
	{
		SaveSerializeLanguage(vLanguage);
	}

	public SystemLanguage GetCurrentLanguage()
	{
		CheckLanguage();
		return GetDeserializationLanguage();
	}

    public bool HasCurrentLanguage()
    {
        return PlayerPrefs.HasKey(LocalizedKey);
    }

	#region BaseFunction
	private void CheckLanguage()
	{
        string vString = string.Empty;
        if (PlayerPrefs.HasKey(LocalizedKey))
            vString = PlayerPrefs.GetString(LocalizedKey); 
       
        SaveSerializeLanguage(string.IsNullOrEmpty(vString) ?
            Application.systemLanguage :
            (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), vString));
    }

	private void SaveSerializeLanguage(SystemLanguage vLanguage)
    {
        switch(vLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.English:
                PlayerPrefs.SetString(LocalizedKey, vLanguage.ToString());
                break;
            default:
                vLanguage = SystemLanguage.Chinese;
                PlayerPrefs.SetString(LocalizedKey, vLanguage.ToString());
                break;
        }
	}
	private SystemLanguage GetDeserializationLanguage()
	{
		return (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage),PlayerPrefs.GetString(LocalizedKey));
	}
	#endregion
}
