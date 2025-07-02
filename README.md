# Stack-Runner
**Unity Version: 2021.3.16f1**

**Rendering PipeLine: URP**

## 1. Proje Amacı
Mobil hedefli, tek sahnede döngüsel çalışan Stack-Runner oyun çekirdeği:
| Hedef                                | Gerçekleştirilen                                                             |
| ------------------------------------ | ---------------------------------------------------------------------------- |
| **SOLID** prensiplerine uyum         | Her sorumluluk ayrı Service/Controller; tüm bağımlılıklar arayüz ile tanımlı |
| **DI / IoC** kullanımı               | Zenject ile sahneden bağımsız enjekte edilen singleton servisler             |
| Hafif **mobil performans**           | Object-Pool (platformlar), kinematik fizik; GC baskısı minimal               |
| Kolay **genişletilebilirlik / test** | Audio, Camera, Difficulty vb. sistemler ayrı katmanlar, mock edilebilir      |

## 2. Kullanılan Araç & Kütüphaneler
| Paket           | Sebep                               | Örnek Kod                                          |
| --------------- | ----------------------------------- | -------------------------------------------------- |
| **Zenject**     | Dependency Injection, SceneContext  | `GameInstaller.InstallBindings()`                  |
| **Cinemachine** | Takip, state geçişi, orbital kamera | `CameraService` `OrbitalTransposer`                |
| **DOTween**     | Pitch tween & kamera döngüsü        | `AudioService.PlayCut()`                           |
| **URP**         | Mobilde hafif render                | `PlatformGenerationSettings.materials` renk dizisi |

## 3. Ana Sistemler
| Sistem           | Rol                                     | Önemli Sınıflar / Pattern                                       |
| ---------------- | --------------------------------------- | --------------------------------------------------------------- |
| Platform Üretimi | Obje havuzundan spawn + start/final mantığı     | `PlatformGeneratorService` (*Factory + Object Pool*)    |
| Oyun Döngüsü     | Start / Win / Fail / NextStage          | `GameService` (*Façade*)                                        |
| Zorluk           | Her level'da kaç platform ?             | `LevelDifficultyService` + `LevelDifficultyConfig` (*Strategy*) |
| Komut            | Tap algılama                            | `InputService`                                                  |
| UI               | Panel aç/kapat                          | `UIService` + `CanvasVisibilityController`                      |
| Kamera           | State tabanlı geçiş + win’de orbital    | `CameraService`                                                 |
| Ses              | PerfectCut ile biriken Pitch.value      | `AudioService`                                                  |
| Oyuncu           | step bazlı basePlatform'a hareket, fail’de düşme      | `PlayerController`                                |

## 4. Tasarım Desenleri & SOLID
| Prensip / Pattern         | Uygulama                                   | Faydası               |
| ------------------------- | ------------------------------------------ | --------------------- |
| **S**ingle Responsibility | `PlatformView` sadece görsel/fizik         | Bakımı kolay          |
| **O**pen/Closed           | Yeni level = yeni `LevelDifficultyConfig`  | Kod değişmez          |
| **L**iskov                | Bütün servisler interface ile enjekte      | Güvenli çeşitleme     |
| **I**nterface Segregation | `IInputService` yalnız `IsTapDown`         | İnce API              |
| **D**ependency Inversion  | Constructor injection                      | Unit-test kolay       |
| **Factory + Pool**        | Platform nesneleri havuza geri döner       | 0 GC spike            |
| **Observer**              | `GameService` event’leri → UI/Camera/Audio | Gevşek bağlı iletişim |
| **MVP-benzeri**           | `PlatformModel ↔ PlatformView ↔ PlatformController`        | Görsel–mantık ayrımı  |

## 5. Veri Akışı
![image](https://github.com/user-attachments/assets/dd87a501-d1d5-45bc-832c-72daf122ff98)

## 6. Sonuç
Prototip; modüler, performans dostu ve kolay genişleyebilir bir temel sunar.
Buradan sonra yeni kameralar, ekstra efektler, farklı platform tipleri eklemek
yalnızca ilgili servis/controller’a odaklanarak yapılabilir.

---
