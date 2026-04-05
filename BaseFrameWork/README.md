# BaseFramework

Bu repo iki host ile ilerliyor:

- `BaseFramework.WpfHost` -> Ana hedef (Windows/WPF).
- `BaseFramework.WebHost` -> Linux/Windows cross-platform preview.

## Bu turdaki odak
- Inspector editorlerinde Enter artik focus kaybi tetikleyip degeri aninda commit eder (numeric, string, datetime sahalari).
- Collection editor, ParameterBridgeObject koleksiyonlarini dinleyerek item bazli silme butonu (kirmizi carpi) ekler.
- `Test_Class_2` calendar senaryosu icin `StartDate/EndDate/Company/Description` alanlari ve `ICalendarEvent` uygulamasi kazandi.
- `Test_Class_3` is planlarini `ObservableCollection<Test_Class_2>` ile saklar, inspector'dan runtime ekleme/cikarma yapilabilir.
- Yeni `CalendarControl`, Outlook benzeri ay gorunumu, hover tooltip ve sirket filtresiyle schedule'inizi gosterir.
- `BaseFramework.Core.Scheduling.ICalendarEvent` + WPF calendar kombinasyonu, diger host/projelerde tekrar kullanilabilir.
- Calendar icinde hover/select duzeltmeleri, saat bazli bar uzunluklari ve yeni detay paneli ile secilen event aninda duzenlenebilir.
- NavigationHostControl ile sol tarafta modern bir navigation bar, sag tarafta secilen page icin host olusturuldu; yeni uygulamalarda yalnizca pageleri eklemen yeterli.

## Build/Test (Windows)
```bash
dotnet restore
dotnet build BaseFramework.sln
dotnet test BaseFramework.Core.Tests/BaseFramework.Core.Tests.csproj
dotnet run --project BaseFramework.WpfHost/BaseFramework.WpfHost.csproj
```

## Build/Test (Linux veya Windows)
```bash
dotnet run --project BaseFramework.WebHost/BaseFramework.WebHost.csproj
```

## GitHub'da commitleri gormek icin
```bash
git remote -v
git branch
git log --oneline -n 5
git push origin <branch-adi>
```

## Cekirdegi diger uygulamalara tasima
1. Yeni host projene `BaseFramework.Core` referansi ekle (veya bu projeyi NuGet paketi olarak dagit).
2. Domain modellerini `ParameterBridgeObject` turetip `InspectableMember` ile isaretlediginde inspector otomatik calisir.
3. Takvim/schedule senaryolari icin modele `ICalendarEvent` arabirimini uygula (Title, Company, Start, End, Description). Boylece `CalendarControl` sadece `EventsSource` binding'i ile devreye girer.
4. WPF hostlarinda inspector + calendar layout'unu `MainWindow.xaml` ornegindeki gibi iki kolonlu grid ile yerlestir; farkli projelerde sadece DataContext ve `EventsSource` binding'lerini degistirmen yeterlidir.
5. WebHost benzeri diger UI katmanlari icin ayni `BaseFramework.Core` metadata servislerini kullanarak minimum UI koduyla ilerleyebilirsin.

## WPF Debug Notu
WPF host acilip aniden kapanirsa crash log dosyasi su konuma yazilir: `%LOCALAPPDATA%/BaseFramework/logs/wpf-crash-*.log`
