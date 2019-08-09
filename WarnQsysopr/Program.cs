using System;
using dblib;
using System.Net.Mail;

namespace WarnQsysopr
{
    class Program
    {
        static DBConnection iseries = new DBConnection("IBMI");

        static String sqlfix(String x)
        {
            return x.Replace("'", "''");
        }

        static public void SendMail(string mailTo, string mailFrom, string subject, string body, bool html = false)
        {
            using (SmtpClient mc = new SmtpClient("smtp.server"))
            {
                //Attachment att;
                Console.WriteLine("Preparing to send email message from {0} to {1}", mailFrom, mailTo);

                mailTo = mailTo.Replace(';', ',');
                MailMessage msg = new MailMessage(mailFrom, mailTo);
                msg.Body = body;
                msg.Subject = subject;

                if (html)
                {
                    msg.IsBodyHtml = true;
                }

                mc.Send(msg);
                Console.WriteLine("Email sent");
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("WarnQsysopr - Copyright (c) 2019 by Tom White");
            Console.WriteLine("Available under GPL 2.0 license");

            int msgCount = 0;

            String sql = 
                "with s1 as (select * from qsys2.message_queue_info " +
                "where message_timestamp > current timestamp - 1 day " +
                " and message_type in ('INQUIRY', 'REPLY') " +
                " and message_queue_name='QSYSOPR') " +
                "select * from s1 where message_type = 'INQUIRY' " +
                "  and message_key not in " +
                "   (select associated_message_key " +
                "    from s1 where message_type = 'REPLY') ";

            String email = "QSYSOPR Messages waiting to be answered:\r\n\r\n";

            String msgs = "";

            iseries.RunSql(sql);

            if (!iseries.eof)
            {
                // message_timestamp,message_id,message_text,message_second_level_text
                email += $"{iseries.stringValue("message_timestamp")}: " +
                    $"{iseries.stringValue("message_text")}\r\n\r\n" +
                    $"{iseries.stringValue("message_second_level_text")}\r\n\r\n" +
                    $"----------\r\n\r\n";
                msgCount++;
                msgs += iseries.stringValue("message_text")+"  ";
                iseries.MoveNext();
            }

            if (msgCount>0)
            {
                SendMail("tom.white@gmail.com", "tom.white@gmail.com",
                    $"{msgCount} unanswered QSYSOPR Messages", email);
            }
        }
    }
}
