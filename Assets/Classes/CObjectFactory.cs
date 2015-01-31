using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public delegate Match.Actions.IAction ObjectFactoryDelegate();

public class CObjectFactory
{
	static private Dictionary<string, ObjectFactoryDelegate> mDictionary = new Dictionary<string, ObjectFactoryDelegate>();

	static public void registerCreator(string aKey, ObjectFactoryDelegate aFunc)
	{
		mDictionary.Add(aKey, aFunc);
	}

	static public void unregisterCreator(string aKey)
	{
		mDictionary.Remove(aKey);
	}

	static public Object createObjectByKey(string aKey)
	{
		if(mDictionary.ContainsKey(aKey))
		{
			ObjectFactoryDelegate _delegate = mDictionary[aKey];

			return _delegate.Invoke() as Object;
		}

		return null;
	}
}
