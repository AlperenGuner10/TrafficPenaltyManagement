# Trafik Cezası Yönetim Sistemi

ASP.NET Core MVC ile geliştirilen bu uygulama, araçların ve trafik cezalarının yönetilmesini ve cezaların yetkili kullanıcılar tarafından aşamalı olarak onaylanmasını sağlar.

## 1. Araç Yönetimi

Sistemde araçlar plaka, araç türü, marka ve model bilgileriyle kaydedilir ve listelenir. Araç bilgileri düzenlenebilir; kullanılmayan araçlar pasife alınabilir. Pasif araçlar geçmiş kayıtları korunarak sistemde tutulur ve yeni trafik cezası oluşturulurken seçilemez.

Araç türleri **Binek Araç, Çekici, Dorse ve Kiralık Araç** olarak tanımlanmıştır.

## 2. Trafik Cezası ve Onay Süreci

Yetkili çalışan, araç seçerek ceza tarihi, açıklama ve tutar bilgileriyle yeni trafik cezası oluşturur. Kayıt oluşturulduğunda yönetici onayına gönderilir.

**Onay akışı:**

`Yeni → Yönetici Onayı → Finans Onayı → Tamamlandı`

- **Yönetici**, kendi aşamasındaki cezayı onaylayarak finans aşamasına aktarabilir veya gerekçe belirterek reddedebilir.
- **Finans**, yönetici onayından geçen cezayı onaylayarak süreci tamamlayabilir veya gerekçesiyle reddedebilir.
- **Reddedilen cezalar** silinmez. Kaydı oluşturan çalışan veya Admin, cezayı düzenleyip yeniden yönetici onayına gönderebilir.
- **Tamamlanan cezalar** üzerinde düzenleme yapılmasına izin verilmez.

## 3. Onay ve İşlem Geçmişi

Ceza detay ekranında onaya gönderme, onaylama, reddetme, güncelleme ve yeniden onaya gönderme işlemleri takip edilir. İşlem geçmişinde işlemi gerçekleştiren kullanıcı, tarih, önceki ve yeni durum ile varsa ret gerekçesi gösterilir.

## 4. Kullanıcı Rolleri ve Örnek Hesaplar

Uygulamada ASP.NET Core Identity ile kullanıcı girişi ve rol bazlı yetkilendirme uygulanmıştır.

| Rol      | E-posta           | Görevi                                                                     |
| -------- | ----------------- | -------------------------------------------------------------------------- |
| Admin    | alperen@gmail.com | Kullanıcı ve rol yönetimi; yetkili yönetim işlemleri                       |
| Finance  | fatih@gmail.com   | Finans onayı veya gerekçeli ret                                            |
| Manager  | mehmet@gmail.com  | Yönetici onayı veya gerekçeli ret                                          |
| Employee | erhan@gmail.com   | Ceza oluşturma; kendi reddedilen kayıtlarını düzenleme ve yeniden gönderme |

> 

## 5. Teknik Yapı ve İş Kuralları

Proje **Web**, **Application**, **Domain** ve **Infrastructure** katmanlarından oluşur. Veri erişiminde Entity Framework Core, Generic Repository ve Unit of Work yaklaşımı kullanılmıştır.

Onay süreci durumlar üzerinden yönetilir. Her kullanıcı yalnızca yetkili olduğu aşamada işlem yapabilir. Ret gerekçesi zorunludur; yapılan işlemler geçmişte saklanır. Reddedilen kayıtlar yeniden onaya gönderildiğinde süreç yönetici aşamasından başlar. Tamamlanmış kayıtların değiştirilmesi engellenerek onaylanmış verilerin bütünlüğü korunur.
