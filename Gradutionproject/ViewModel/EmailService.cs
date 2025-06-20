using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    private readonly string smtpServer = "smtp.gmail.com";
    private readonly int smtpPort = 587;
    private readonly string senderEmail = "ai.tutor.sha@gmail.com"; // بريدك الحقيقي
    private readonly string senderPassword = "fqxp swqe zrwv wuhs"; // كلمة مرور التطبيق (App Password)

    public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink, string subject, string code)
    {

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
        <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>
                    <h2 style='color: #333;'>AI Tutor - Password Reset</h2>
                    <p style='font-size: 16px; color: #555;'>Hello,</p>
                    <p style='font-size: 16px; color: #555;'>
                        Your password reset code is:
                        <strong style='font-size: 20px; color: #000;'>{code}</strong>
                    </p>
                    
                    <p style='font-size: 14px; color: #999;'>If you didn’t request this, you can safely ignore this email.</p>
                    <p style='font-size: 14px; color: #999;'>Regards,<br/> <strong style='font-size: 14px; '>AI Tutor Team</strong> </p>
                </div>
            </body>
        </html>"
        };
        // Link of reset password directly
        //< p style = 'font-size: 16px; color: #555;' > Click the button below to reset your password:</ p >
        //            < p >
        //               < a href = '{resetLink}'
        //                  style = 'display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; 
        //                         text - decoration: none; border - radius: 5px; '>Reset Password</a>
        //            ! </ p >
        using var smtp = new SmtpClient();
        smtp.Timeout = 10000; // 10 ثواني
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendPasswordChangedConfirmationEmailAsync(string toEmail)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(toEmail));

        if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            throw new InvalidOperationException("Sender email or password is not configured properly.");

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Your Password Has Been Changed";

        email.Body = new TextPart(TextFormat.Html)
        {
        Text = $@"
            <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>
                        <h2 style='color: #333;'>AI Tutor - Password Changed</h2>
                        <p style='font-size: 18px; color: #555;'>
                            Your password has been <strong style='color: #27ae60;'>changed successfully </strong>.
                        </p>
                        <p style='font-size: 14px; color: #999;'>
                            If you did not request this change, please contact support immediately.
                        </p>
                        <p style='font-size: 14px; color: #999;'>
                            Regards,<br/>
                            <strong>AI Tutor Team</strong>
                        </p>
                    </div>
                </body>
            </html>"

        };
      

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendVerificationCodeEmailAsync(string toEmail, string link, string subject, string code)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
<html>
  <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>

      <!-- صورة اللوجو + اسم التطبيق جنب بعض -->
      <table style='width: 100%; margin-bottom: 20px;'>
        <tr>
          <td style='vertical-align: middle; width: 60px;'>
            <img src='https://raw.githubusercontent.com/AhmedMagdy61/Test/main/images.png' 
                 alt='AI Tutor Logo' 
                 style='width: 50px; height: 50px; border-radius: 5px;' />
          </td>
          <td style='vertical-align: middle;'>
            <h2 style='color: #333; margin: 0;'>AI Tutor - Verify Your Email</h2>
          </td>
        </tr>
      </table>

      <p style='font-size: 16px; color: #555;'>
        Your verification code is:
        <strong style='font-size: 20px; color: #000;'>{code}</strong>
      </p>

      <p style='font-size: 14px; color: #999;'>If you did not register, ignore this email.</p>

      <p style='font-size: 14px; color: #999;'>
        Regards,<br/>
        <strong style='font-size: 14px;'>AI Tutor Team</strong>
      </p>

    </div>
  </body>
</html>"

        };
        // Link of Verfiy password directly
          //< p style = 'font-size: 16px; color: #555;' > Or click the link below to verify directly:</ p >
          //     < p >
          //             < a href = '{link}'
          //                style = 'display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; 
          //                      text - decoration: none; border - radius: 5px; '>Reset Password</a>
          //          ! </ p >
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendAccountDeletedEmailAsync(string toEmail, string subject)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
        <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>
                <!-- صورة اللوجو + اسم التطبيق جنب بعض -->
                      <table style='width: 100%; margin-bottom: 20px;'>
                        <tr>
                          <td style='vertical-align: middle; width: 60px;'>
                            <img src='https://raw.githubusercontent.com/AhmedMagdy61/Test/main/images.png' 
                                 alt='AI Tutor Logo' 
                                 style='width: 50px; height: 50px; border-radius: 5px;' />
                          </td>
                          <td style='vertical-align: middle;'>
                            <h2 style='color: #333; margin: 0;'>AI Tutor - Account Deleted</h2>
                          </td>
                        </tr>
                      </table>
                    <p style='font-size: 16px; color: #555;'>
                        Your account has been successfully <strong style='color: #c0392b;'>deleted</strong> from our system.
                    </p>
                    <p style='font-size: 14px; color: #999;'>
                        We're sorry to see you go. If this was a mistake, please contact support.
                    </p>
                    <p style='font-size: 14px; color: #999;'>
                        Regards,<br/>
                        <strong>AI Tutor Team</strong>
                    </p>
                </div>
            </body>
        </html>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendProfileUpdatedEmailAsync(string toEmail, string subject)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
        <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>
                    <h2 style='color: #333;'>AI Tutor - Profile Updated</h2>
                    <p style='font-size: 16px; color: #555;'>
                        Your profile information has been <strong style='color: #27ae60;'>updated successfully</strong>.
                    </p>
                    <p style='font-size: 14px; color: #999;'>
                        If you did not make this change, please contact support immediately.
                    </p>
                    <p style='font-size: 14px; color: #999;'>
                        Regards,<br/>
                        <strong>AI Tutor Team</strong>
                    </p>
                </div>
            </body>
        </html>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendVerificationSuccessEmailAsync(string toEmail, string subject)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(toEmail));

        if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            throw new InvalidOperationException("Sender email or password is not configured properly.");

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(senderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"
        <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='background-color: #fff; padding: 20px; border-radius: 5px; max-width: 600px; margin: auto;'>
                    <h2 style='color: #333;'>AI Tutor</h2>
                    <p style='font-size: 16px; color: #555;'>Hello,</p>
                    <p><strong style='font-size: 20px; color: #000;'>Your email verification code was confirmed successfully.</strong></p>
                    <p style='font-size: 14px; color: #999;'>If you didn’t request this code, please ignore this email.</p>
                    <p style='font-size: 14px; color: #999;'>Regards,<br/> <strong style='font-size: 14px;'>AI Tutor Team</strong></p>
                </div>
            </body>
        </html>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }


}
