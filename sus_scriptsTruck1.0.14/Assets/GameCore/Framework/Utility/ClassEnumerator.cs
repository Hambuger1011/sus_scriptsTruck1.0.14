using Framework;

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ClassEnumerator
{
	protected List<Type> Results = new List<Type>();

	private Type AttributeType;

	private Type InterfaceType;

	public List<Type> results
	{
		get
		{
			return Results;
		}
	}

	public ClassEnumerator(Type InAttributeType, Type InInterfaceType, Assembly InAssembly, bool bIgnoreAbstract = true, bool bInheritAttribute = false, bool bShouldCrossAssembly = false)
	{
		AttributeType = InAttributeType;
		InterfaceType = InInterfaceType;
		try
		{
			if (bShouldCrossAssembly)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				if (assemblies != null)
				{
					foreach (Assembly inAssembly in assemblies)
					{
						CheckInAssembly(inAssembly, bIgnoreAbstract, bInheritAttribute);
					}
				}
			}
			else
			{
				CheckInAssembly(InAssembly, bIgnoreAbstract, bInheritAttribute);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in enumerate classes :" + ex.Message);
		}
	}

	protected void CheckInAssembly(Assembly InAssembly, bool bInIgnoreAbstract, bool bInInheritAttribute)
	{
		Type[] types = InAssembly.GetTypes();
		if (types != null)
		{
			foreach (Type type in types)
			{
				if (InterfaceType == null || InterfaceType.IsAssignableFrom(type))
				{
					switch (bInIgnoreAbstract)
					{
					default:
						if (type.IsAbstract)
						{
							break;
						}
						goto case false;
					case false:
						if (type.GetCustomAttributes(AttributeType, bInInheritAttribute).Length > 0)
						{
							Results.Add(type);
						}
						break;
					}
				}
			}
		}
	}
}
