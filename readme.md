Rinity2
====
SuperPower Unity!<br>
<br>
__Rinity2__ performs IL(C# bytecode) modification to accomplish things which cannot be done in vanilla C#/Unity.<br>
This may sensative to Unity version changes and not work with latest Unity.

Requirements
----
* Unity 2017.3 or higher

Features
----
* __[2-Way Binding](https://github.com/pjc0247/Rinity2/tree/master/docs/2way_binding)__
* __[String Interpolation](https://github.com/pjc0247/Rinity2/blob/master/docs/string_interpolation.md)__
* __[PubSub Messaging](https://github.com/pjc0247/Rinity2/tree/master/docs/pubsub)__
* __Threading Utilities__
* __Object Pooling__

Overview
----
__Recycle__
```cs
[Recycle]
public class Player {
    /* ... */
}

/* auto pooling */
var player = new Player();
```

__String Interpolation__
```cs
int a = 1234;

Debug.Log("a is {{a}}.")
```

__Dispatch__
```cs
[Dispatch(ThreadType.ThreadPool)]
public void DownloadJson(string uri) {
    // This will be executed in background thread.
    var json = SomeDownloadMethod(uri);

    OnDownloadJson(json);
}

[Dispatch(ThreadType.MainThread)]
public void OnDownloadJson(string json) {
    // Will be executed in main thread.

    /* do UI tasks */
}
```

__SharedVar__
```cs
[SharedVariable("shared_level")]
public int level {get;set;}

[SharedVariable("shared_level")]
public int level2 {get;set;}
```

__Trace__
```cs
[Trace]
public void WorkWork() {
    /* .... */
}
```

__NotifyChange__
```cs
class Player : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    public int level { get; set; }

    void Start () {
        PropertyChanged += OnPropertyChanged;
    }

    // Automatically called when property is changed.
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        Debug.Log("PROPERTY CHANGED : " + e.PropertyName);
    }
}
```

__DbgHelper__
```cs
// Same as __LINE__ in C++
Debug.Log( DbgHelper.CurrentLine );
```
