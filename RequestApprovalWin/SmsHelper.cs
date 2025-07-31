using System;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public static class SmsHelper
{
    static readonly string AccountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
    static readonly string AuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
    static readonly string FromNumber = ConfigurationManager.AppSettings["TwilioFromNumber"];

    static SmsHelper()
    {
        TwilioClient.Init(AccountSid, AuthToken);
    }

    /// <summary>
    /// Belirtilen numaraya rezervasyon bilgisini SMS olarak yollar.
    /// </summary>
    public static void SendReservationSms(string toNumber,
                                          int reservationId,
                                          DateTime requestedDate,
                                          int personCount)
    {
        var body = $"Rezervasyon onaylandı! ID: {reservationId}, " +
                   $"Tarih: {requestedDate:dd.MM.yyyy}, Kişi: {personCount}.";
        MessageResource.Create(
            to: new Twilio.Types.PhoneNumber(toNumber),
            from: new Twilio.Types.PhoneNumber(FromNumber),
            body: body
        );
    }
}
