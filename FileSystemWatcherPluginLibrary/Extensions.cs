using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileWatcherPluginLibrary
{
	public static class Extensions
	{
		public static List<Type> GetConfigurationTypes(this Assembly assembly)
		{
			List<Type> types = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.GetInterface(typeof(IPluginConfiguration).Name) != null).ToList();
			return types;
		}

		public static List<Type> GetPluginTypes(this Assembly assembly)
		{
			List<Type> types = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.GetInterface(typeof(IPlugin).Name) != null).ToList();
			return types;
		}
	}
}
