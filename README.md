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
