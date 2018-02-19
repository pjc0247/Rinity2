PubSub
====
__PubSub__ 는 직접적으로 연결되어 있지 않은 오브젝트, 또는 컴포넌트 간 데이터 통신 또는 시그널을 보낼 수 있도록 해주는 기능입니다. 

Defining messages
----
```cs
using Rinity;

class SomePubSubMessage : IPubSubMessage
{
    public string hello { get; set; }
}
```


Subscribing
----
```cs
using Rinity;

PubSub.Subscribe("channel_name", (message) => {
    if (message is SomePubSubMessage) {
        var somePubSubMessage = (SomePubSubMessage)message;

        Console.WriteLine("I got a message : " + somePubSubMessage.hello);
    }
});
```

__Subscribe using Attribute__<br>
```cs
[Subscribe("channel_name")]
public void OnSomePubSubMessage(SomePubSubMessage msg) {
    Console.WriteLine("I got a message : " + message.hello);
}
```
어트리뷰트를 통한 구독은, 오브젝트의 라이프사이클에 따라 자동으로 구독/구독 취소가 실행되며 이를 위해서 두가지의 분기 처리를 가집니다.

* MonoBehaviour 상속 클래스일 경우
    * Subscribe on `OnEnable`
    * Unsubscribe on `OnDisable`

* Other C# objects
    * Subscribe on `생성자(ctor)`
    * Unsubscribe on `소멸자(Finalize)`

Rinity는 이러한 구독/구독 취소 처리를 위한 코드를 자동으로 생성하며, 오브젝트는 위와같은 라이프사이클에 따라 한번 이상 구독 또는 구독 취소가 실행될 수 있습니다.


Publishing
----
```cs
using Rinity;

PubSub.Publish("channel_name", new SomePubSubMessage());
```


Reserved message channels
----
몇몇 prefix 는 예약되어 있으며, Rinity 내부적으로 사용됩니다.

* __sharedvar.{variable_name}__
