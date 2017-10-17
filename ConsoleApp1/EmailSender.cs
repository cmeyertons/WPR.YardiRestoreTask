using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
    public class EmailSender
    {
		private readonly string Host;
		private readonly int Port;
		private readonly string Username;
		private readonly string Password;
		private readonly string[] Recipients;

		public EmailSender()
		{
			this.Host = AppSettings.Email.Host;
			this.Port = AppSettings.Email.Port;
			this.Username = AppSettings.Email.Username;
			this.Password = AppSettings.Email.Password;
			this.Recipients = AppSettings.Email.Recipients.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public void Send(string subject, string body)
		{
			var client = new SmtpClient(this.Host, this.Port)
			{
				Credentials = new NetworkCredential(this.Username, this.Password),
				EnableSsl = true
			};

			MailMessage msg = new MailMessage();

			msg.From = new MailAddress(this.Username);

			foreach (var recipient in this.Recipients)
			{
				msg.To.Add(new MailAddress(recipient));
			}

			msg.Subject = subject;
			msg.Body = body;

			client.Send(msg);
		}
    }
}
