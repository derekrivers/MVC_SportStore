using System.Net;
using System.Net.Mail;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {

        private readonly EmailSetings _emailSetings;

        public EmailOrderProcessor(EmailSetings emailSettings)
        {
            _emailSetings = emailSettings;
        }

        public void ProcessOrder(ShoppingCart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _emailSetings.UseSsl;
                smtpClient.Host = _emailSetings.ServerName;
                smtpClient.Port = _emailSetings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSetings.Username, _emailSetings.Password);

                if (_emailSetings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSetings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body =
                    new StringBuilder()
                        .AppendLine("A new order has been submitted")
                        .AppendLine("-----")
                        .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price*line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal:{2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.AddressLine1)
                    .AppendLine(shippingDetails.AddressLine2)
                    .AppendLine(shippingDetails.AddressLine3)
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State)
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(_emailSetings.MailFromAddress, _emailSetings.MailToAddress,"New Order Submitted", body.ToString());

                if (_emailSetings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }

    public class EmailSetings
    {
        public string MailToAddress = "derek@realtimecreate.co.uk";
        public string MailFromAddress = "dyk3rz@googlemail.com";
        public bool UseSsl = true;
        public string Username = "dyk3rz@googlemail.com";
        public string Password = "kaiser1976";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 465;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }
}
