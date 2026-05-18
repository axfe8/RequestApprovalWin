## RequestApprovalWin

RequestApprovalWin, rezervasyon taleplerinin masaüstü uygulama üzerinden incelenmesi, onaylanması ve onay sonrası bilgilendirme SMS’lerinin gönderilmesi için geliştirilmiş bir **Windows Forms (.NET Framework 4.7.2)** uygulamasıdır.

## Projenin Amacı

Uygulama, operasyon ekiplerinin bekleyen rezervasyon taleplerini tek ekranda yönetmesini sağlar:

- Talep ve misafir bilgilerini filtreleyerek listeleme  
- Talep sahibi doğrulamasıyla onay süreci  
- Misafir görseli yükleme ve saklama  
- Onay sonrası rezervasyon aktarımı  
- Twilio üzerinden SMS bilgilendirmesi

## Nasıl Çalışır?

1. Kullanıcı, talep sahibi adı ve tarih filtresiyle “Bekleyen Talepleri Yükle” işlemini başlatır.
2. Uygulama, veritabanındaki `sp_GetRequestsWithGuests` prosedürüyle talepleri çeker.
3. Sonuçlar grid üzerinde gösterilir; satır bazında:
   - Onay (`IsApproved`)
   - Talep sahipliği (`IsOwner`)
   - Resim yükleme (`colUpload`)
   - Resim görüntüleme (`colPicture`)
4. Sadece talep sahibi olarak işaretli satırlar onaylanabilir.
5. Onay verildiğinde:
   - `ReservationRequests.Onayla` alanı güncellenir.
   - `sp_ApproveRequest` çağrılır.
   - Talep sahibine ve ilgili misafirlere SMS gönderilir.
6. Liste yenilenerek güncel durum ekrana alınır.

## Kullanılan Teknolojiler

- C# 8.0  
- Windows Forms  
- .NET Framework 4.7.2  
- ADO.NET (`System.Data.SqlClient`)  
- SQL Server Stored Procedure yaklaşımı  
- Twilio SMS API

## Proje Yapısı

- `RequestApprovalWin/Form1.cs`: Ana iş akışı, grid event’leri, onay işlemleri
- `RequestApprovalWin/SmsHelper.cs`: Twilio SMS gönderimi
- `RequestApprovalWin/App.config`: Bağlantı dizesi ve Twilio ayarları
- `RequestApprovalWin/Program.cs`: Uygulama giriş noktası

## Kurulum ve Çalıştırma

### Ön Gereksinimler

- Windows işletim sistemi
- Visual Studio 2019+ (veya .NET Framework 4.7.2 Developer Pack)
- Erişilebilir bir SQL Server veritabanı
- Geçerli Twilio hesap bilgileri

### Adımlar

1. Çözümü Visual Studio ile açın: `RequestApprovalWin.sln`
2. NuGet paketlerini geri yükleyin.
3. `RequestApprovalWin/App.config` içinde aşağıdaki ayarları düzenleyin:
   - `BeachDb` connection string
   - `TwilioAccountSid`
   - `TwilioAuthToken`
   - `TwilioFromNumber`
4. Uygulamayı `Debug` veya `Release` modunda çalıştırın.

## Operasyonel Notlar

- Uygulama veritabanı operasyonlarını doğrudan gerçekleştirir; stored procedure ve tablo şemasıyla uyum kritik önemdedir.
- SMS gönderimi dış servis bağımlılığı içerir; Twilio hataları kullanıcıya mesaj kutusu ile yansıtılır.

## Güvenlik Notu

`App.config` içinde hassas bilgilerin (veritabanı parola/token gibi) düz metin tutulması önerilmez. Üretim ortamında bu değerleri güvenli bir yapılandırma/secret yönetimi mekanizması üzerinden sağlamanız önerilir.
