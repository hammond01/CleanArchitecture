# 📧 Email Configuration Guide

## Quick Setup Options

### Option 1: Ethereal Email (Recommended for Development)

Ethereal is a fake SMTP service perfect for testing. Emails are captured but not sent.

**Steps:**
1. Go to https://ethereal.email/
2. Click "Create Ethereal Account"
3. Copy the credentials shown
4. Update `appsettings.Development.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.ethereal.email",
    "SmtpPort": 587,
    "SmtpUsername": "YOUR_ETHEREAL_USERNAME",
    "SmtpPassword": "YOUR_ETHEREAL_PASSWORD",
    "FromEmail": "noreply@identityserver.dev",
    "FromName": "Identity Server Dev",
    "EnableSsl": true
  }
}
```

5. Check emails at: https://ethereal.email/messages

---

### Option 2: Gmail (For Real Emails)

**⚠️ Security Note:** Use App Password, NOT your real Gmail password.

**Steps:**

1. **Enable 2-Step Verification** on your Google Account
   - Go to https://myaccount.google.com/security
   - Enable "2-Step Verification"

2. **Generate App Password**
   - Go to https://myaccount.google.com/apppasswords
   - Select "Mail" and "Other (Custom name)"
   - Name it "Identity Server"
   - Copy the 16-character password

3. **Update appsettings.json:**

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "xxxx xxxx xxxx xxxx",  // 16-char App Password
    "FromEmail": "your-email@gmail.com",
    "FromName": "Identity Server",
    "EnableSsl": true
  }
}
```

---

### Option 3: Outlook/Microsoft 365

```json
{
  "Email": {
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@outlook.com",
    "SmtpPassword": "your-password",
    "FromEmail": "your-email@outlook.com",
    "FromName": "Identity Server",
    "EnableSsl": true
  }
}
```

---

### Option 4: SendGrid (For Production)

1. Sign up at https://sendgrid.com/
2. Create an API Key
3. Update config:

```json
{
  "Email": {
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SmtpUsername": "apikey",
    "SmtpPassword": "YOUR_SENDGRID_API_KEY",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "Your App Name",
    "EnableSsl": true
  }
}
```

---

## Testing Email Configuration

### 1. Register a New User

```http
POST https://localhost:5219/api/auth/register
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "Test@123456",
  "firstName": "Test",
  "lastName": "User"
}
```

### 2. Check Logs

Look for:
```
Email sent successfully to test@example.com
```

Or error messages:
```
Failed to send email to test@example.com
```

### 3. View Email (Ethereal)

- Go to https://ethereal.email/messages
- Login with your Ethereal credentials
- See all captured emails

---

## Common Issues

### Issue: "Authentication failed"

**Solution:**
- Gmail: Make sure you're using App Password, not regular password
- Check 2FA is enabled
- Verify username/password are correct

### Issue: "Connection refused" or "Timeout"

**Solution:**
- Check firewall/antivirus isn't blocking port 587
- Try port 465 with `EnableSsl = true`
- Check internet connection

### Issue: Emails not arriving (Gmail)

**Solution:**
- Check spam folder
- Verify "Less secure app access" or use App Password
- Check daily sending limit (500 for free Gmail)

---

## Email Templates

Current templates in `EmailTemplateService.cs`:

| Template | Purpose | Variables |
|----------|---------|-----------|
| `Welcome` | Account created | `FirstName`, `ConfirmUrl` |
| `ConfirmEmail` | Email verification | `FirstName`, `ConfirmUrl` |
| `ResetPassword` | Password reset | `FirstName`, `ResetUrl` |
| `SecurityAlert` | Security notification | `FirstName`, `Event`, `Timestamp`, `IPAddress` |
| `TwoFactorCode` | 2FA code | `FirstName`, `Code` |

---

## Production Recommendations

1. **Use a dedicated email service** (SendGrid, AWS SES, Mailgun)
2. **Set up SPF, DKIM, DMARC** records for your domain
3. **Monitor bounce rates** and unsubscribes
4. **Rate limit** email sending
5. **Queue emails** for retry on failure
6. **Use environment variables** for credentials (not appsettings.json)

Example with environment variables:

```csharp
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "smtp.gmail.com";
    options.SmtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
    options.SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
});
```

---

## Disable Email (For Testing)

If you want to disable emails temporarily:

```csharp
// In Program.cs, replace:
builder.Services.AddScoped<IEmailSender, EmailSender>();

// With a fake implementation:
builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
```

Create `FakeEmailSender.cs`:
```csharp
public class FakeEmailSender : IEmailSender
{
    private readonly ILogger<FakeEmailSender> _logger;

    public FakeEmailSender(ILogger<FakeEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string htmlMessage, string? textMessage = null)
    {
        _logger.LogInformation("📧 [FAKE] Email to {To}: {Subject}", to, subject);
        _logger.LogDebug("Content: {Content}", htmlMessage);
        return Task.CompletedTask;
    }

    public Task SendTemplatedEmailAsync(string to, string templateName, object templateData)
    {
        _logger.LogInformation("📧 [FAKE] Templated email to {To}: {Template}", to, templateName);
        return Task.CompletedTask;
    }
}
```

---

## Resources

- [Ethereal Email](https://ethereal.email/) - Free fake SMTP
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
- [SendGrid](https://sendgrid.com/) - Production email service
- [AWS SES](https://aws.amazon.com/ses/) - Amazon email service
- [Mailgun](https://www.mailgun.com/) - Developer-friendly email API
