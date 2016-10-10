Rinity2
====
SuperPower Unity!

Features
----
__Dispatch__
```cs
[Dispatch(ThreadType.ThreadPool)]
public void DownloadJson(string uri) {
    // 이 메소드는 스레드풀에서 실행됩니다.
    var json = SomeDownloadMethod(uri);

    OnDownloadJson(json);
}

[Dispatch(ThreadType.MainThread)]
public void () OnDownloadJson(string json) {
    // 이 메소드는 메인 스레드에서 실행됩니다.

    /* UI DODODODODO */
}
```

__Trace__
```cs
https://github.com/pjc0247/Unity.Profiler.Ext
```

__NotifyChange__
```cs
class Player : INotifyPropertyChanged {}
    public event PropertyChangedEventHandler PropertyChanged;

    public int level { get; set; }

    void Start () {
        PropertyChanged += OnPropertyChanged;
	}
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        // 프로퍼티가 변경되면 자동으로 호출됩니다.
        Debug.Log("PROPERTY CHANGED : " + e.PropertyName);
    }
}
```