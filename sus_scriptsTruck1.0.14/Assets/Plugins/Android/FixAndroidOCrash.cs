using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAndroidOCrash : MonoBehaviour {
    
	void Start() {
		StartCoroutine(BeginFix());
	}

	IEnumerator BeginFix()
	{
		while (true)
		{
			try
			{
				if (DoFixAndroidOCrash())
				{
					Debug.Log( "FixAndroidOCrash Suceecss" );
					break;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("FixAndroidOCrash Error ! "+ ex.ToString());
				break;
			}
			
			yield return new WaitForSeconds(1);
		}
        Destroy(this);
		
	}

	public bool DoFixAndroidOCrash()
	{
#if UNITY_ANDROID
		using (AndroidJavaClass jc = new AndroidJavaClass("com.fuckunity.FixAndroidOCrash"))
		{
			return jc.CallStatic<bool>("TryFix", true);
		}
#else
		return false;
#endif
	}
}
