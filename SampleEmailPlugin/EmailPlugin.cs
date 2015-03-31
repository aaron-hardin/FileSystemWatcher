using System;
using System.IO;
using FileWatcher;
using FileWatcherPluginLibrary;
using System.Net;
using System.Net.Mail;

namespace SampleEmailPlugin
{
    public class EmailPlugin : IPlugin
    {
	    private EmailPluginConfiguration configuration;
	    
	    public EmailPlugin()
	    {
		    configuration = new EmailPluginConfiguration();
	    }

	    public void Initialize()
	    {
		    
	    }

		public IPluginConfiguration Configuration
	    {
		    get { return configuration; }
			set { configuration = (EmailPluginConfiguration) value; }
	    }

		public void Trigger(IFolderConfiguration triggeredFolder, FileSystemEventArgs args)
		{
			// TODO: send email

			MailAddress fromAddress = new MailAddress(configuration.EmailAddress, "Email Plugin");
			MailAddress toAddress = new MailAddress(configuration.EmailAddress);
			string fromPassword = configuration.DecryptedPassword;
			const string subject = "Subject";

			string msg = string.Format("{0} has triggered {1} with change type {2}",
				args.Name,
				triggeredFolder.Path,
				args.ChangeType);

			SmtpClient smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
			};
			using (MailMessage message = new MailMessage(fromAddress, toAddress))
			{
				message.Subject = subject;
				message.Body = msg;
				smtp.Send(message);
			}

			SysUtils.ReportToEventLog( msg );
		}

	    public Type ConfigurationType
	    {
		    get { return typeof(EmailPluginConfiguration); }
	    }
    }
}
