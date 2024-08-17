using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class EmailHelper
{
    private static IAmazonSimpleEmailService _sesClient;
    private static string _defaultFromEmail;

    // Initialize static fields
    public static void Initialize(IAmazonSimpleEmailService sesClient, string defaultFromEmail)
    {
        _sesClient = sesClient ?? throw new ArgumentNullException(nameof(sesClient));
        _defaultFromEmail = defaultFromEmail ?? throw new ArgumentNullException(nameof(defaultFromEmail));
    }

    public static async Task SendEmailAsync(string toEmail, string subject, string body, string? fromEmail = null)
    {
        if (_sesClient == null)
        {
            throw new InvalidOperationException("EmailHelper is not initialized. Call Initialize() before using.");
        }

        fromEmail ??= _defaultFromEmail;

        var sendRequest = new SendEmailRequest
        {
            Source = fromEmail,
            Destination = new Destination
            {
                ToAddresses = new List<string> { toEmail }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content(body)
                }
            }
        };

        try
        {
            await _sesClient.SendEmailAsync(sendRequest);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
}