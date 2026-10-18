# Bizim Köşemiz — Unity 3D Sosyal Yaşam Oyunu (Aşama 1–3 Temel Sürüm)

Sıcak, samimi ve stilize bir bahçede arkadaşlarla çevrimiçi buluşulabilen üçüncü şahıs sosyal yaşam oyunu.

## 1. Hızlı Kurulum (Unity Hub)
1. Arşivi bilgisayarınızda bir klasöre çıkarın.
2. **Unity Hub** üzerinden **Add > Add project from disk** seçeneğiyle `BizimKosemiz` klasörünü ekleyin ve **Unity 2022.3 LTS** (veya **Unity 6000.0 LTS**) ile açın.
3. Proje açıldığında `Assets/Scenes/GardenScene.unity` sahnesini çift tıklayarak açın.
4. Üst menüden **`Bizim Köşemiz > 1. Sahneyi, URP ve Animator Varlıklarını Doğrula - Onar`** seçeneğine tıklayın.
5. **Play** tuşuna basın:
   - **Hareket:** `W, A, S, D` veya Yön Tuşları
   - **Koşma:** `Sol Shift`
   - **Kamera Döndürme:** Sağ Fare Tuşuna basılı tutarak fareyi hareket ettirin.

## 2. Git Branch ve Commit Yapısı
Proje içinde `.git` deposu hazır olarak başlatılmıştır ve aşamalar ayrı commit'lerle kaydedilmiştir:
- **Commit 1 (Aşama 1):** Temiz Unity 2022.3 LTS proje yapısı, klasörler, paketler ve `.asmdef` tanımları.
- **Commit 2 (Aşama 2):** Stilize bahçe çevre oluşturucu, URP uyumlu materyaller ve ışıklandırma sistemi.
- **Commit 3 (Aşama 3):** Üçüncü şahıs oyuncu hareketi, ayak hizalı (`y=0`) `CharacterController`, çarpışma korumalı kamera ve Multiplayer yetki soyutlaması.

GitHub'a göndermek için proje klasöründe şu komutları çalıştırmanız yeterlidir:
```bash
git remote add origin https://github.com/<KULLANICI_ADINIZ>/BizimKosemiz.git
git push -u origin main
```


## 3. WebGL + mobil tarayıcı hedefi (geliştirme aşamasında)

Hedef: oyunu HTTPS üzerinden yayımlayıp iOS Safari ve Android tarayıcısından açmak; iki oyuncunun Unity Relay üzerinden aynı oturuma katılması.

### Ağ gereksinimleri
- WebGL tarayıcıları doğrudan UDP soketi açamaz. Relay bağlantısı WebGL için **WSS (WebSocket Secure)** kullanmalıdır.
- `RelayNetworkSession`, Relay transport ayarını WSS kullanacak şekilde yapılandırır. Bu kod tek başına sahne kurulumunu, oyuncu prefab atamasını veya karakter hareketi senkronizasyonunu tamamlamaz.
- `GardenScene` içinde `NetworkManager` + `UnityTransport` bulunmalı; `Player Prefab` alanına `NetworkObject` içeren prefab atanmalıdır.
- Unity Services projesi Unity Dashboard'da oluşturulup bu Unity projesine bağlanmalı ve Relay etkin olmalıdır.

### WebGL derleme
1. Unity Hub'da kurulu Unity sürümüne **WebGL Build Support** modülünü ekleyin.
2. Unity'de **File > Build Settings** (veya sürüme göre **Build Profiles**) içinden WebGL hedefini seçin.
3. Oyunu HTTPS destekleyen bir statik hosting sağlayıcısında yayımlayın. Dosyayı doğrudan `file://` yolundan açmayın.
4. Gerçek iPhone Safari ve Android tarayıcısında test edin. Unity Editor'de başarılı çalışması mobil WebGL performansını veya ağ bağlantısını tek başına doğrulamaz.

### Henüz tamamlanmayanlar
- Yerel karakter prefablarının ve animasyonlarının projeye entegre edilmesi
- NetworkObject spawn akışı ve oyuncu başına karakter seçimi
- Hareket/kamera/animasyon senkronizasyonu
- Dokunmatik joystick ve mobil arayüz
- Gerçek WebGL build, hosting ve iki ayrı telefonla Relay testi

Bu nedenle mevcut branch bir **altyapı geliştirme adımıdır**, tamamlanmış ve test edilmiş bir çevrimiçi oyun sürümü değildir.
