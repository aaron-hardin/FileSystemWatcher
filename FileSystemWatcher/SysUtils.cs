﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
	public static class SysUtils
	{
		public static void ReportErrorMessageToEventLog(string addedMessage, Exception exception)
		{
			ReportErrorToEventLog(DefaultEventSourceIdentifier, addedMessage, exception);
		}

		private static void ReportErrorToEventLog(string eventSource, string addedMessage, Exception exception)
		{
			ReportErrorMessageToEventLog(eventSource, addedMessage, exception);
		}

		private static void ReportErrorMessageToEventLog(string eventSource, string addedMessage, Exception exception)
		{
			string exceptionMessage = string.Empty;

			Exception innerException = exception;
			while(innerException != null)
			{
				exceptionMessage = innerException.Message + Environment.NewLine + Environment.NewLine +
				                   innerException.StackTrace + Environment.NewLine + Environment.NewLine;
				innerException = innerException.InnerException;
			}

			string finalString = string.Empty;

			if(!string.IsNullOrWhiteSpace(addedMessage))
			{
				finalString += addedMessage;
			}

			if(!string.IsNullOrWhiteSpace(exceptionMessage))
			{
				if(!string.IsNullOrWhiteSpace(finalString))
				{
					finalString += Environment.NewLine + Environment.NewLine;
				}

				finalString += exceptionMessage;
			}

			ReportErrorToEventLog(eventSource, finalString);
		}

		private static void ReportErrorToEventLog(string eventSource, string message)
		{
			ReportToEventLog(eventSource, message, EventLogEntryType.Error);
		}

		public static void ReportToEventLog(string eventSource, string message, EventLogEntryType type)
		{
			if(!EventLog.SourceExists(eventSource))
			{
				EventLog.CreateEventSource(eventSource, "Application");
			}

			if(message.Length > 10000)
			{
				message = message.Substring(0, 10000);
			}

			try
			{
				EventLog.WriteEntry(eventSource,message,type);
			}
			catch(Exception)
			{
				// Ignore.
			}
		}

		public const string DefaultEventSourceIdentifier = "File Watcher";
	}
}