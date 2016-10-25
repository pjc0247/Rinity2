PubSub
====
__PubSub__는 직접적으로 연결되어 있지 않은 오브젝트, 또는 컴포넌트 간 데이터 통신 또는 시그널을 보낼 수 있도록 해주는 기능입니다. 

메세지 정의하기
----
```cs
using Rinity;

class SomePubSubMessage : IPubSubMessage
{
    public string hello { get; set; }
}
```


구독하기
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

__어트리뷰트를 통한 구독__<br>
```cs
[Subscribe("channel_name")]
public void OnSomePubSubMessage(SomePubSubMessage msg) {
    Console.WriteLine("I got a message : " + message.hello);
}
```
어트리뷰트를 통한 구독은, 오브젝트의 라이프사이클에 따라 자동으로 구독/구독 취소가 실행되며 이를 위해서 두가지의 분기 처리를 가집니다.

* MonoBehaviour 상속 클래스일 경우
    * `OnEnable` 시에 구독을 시작합니다.
    * `OnDisable` 시에 구독을 취소합니다.

* 일반 C# 오브젝트의 경우
    * `생성자(ctor)` 에서 구독을 시작합니다.
    * `소멸자(Finalize)` 에서 구독을 취소합니다.

Rinity는 이러한 구독/구독 취소 처리를 위한 코드를 자동으로 생성하며, 오브젝트는 위와같은 라이프사이클에 따라 한번 이상 구독 또는 구독 취소가 실행될 수 있습니다.


발송하기
----
```cs
using Rinity;

PubSub.Publish("channel_name", new SomePubSubMessage());
```


예약 그룹
----
몇몇 prefix 는 예약되어 있으며, Rinity 내부적으로 사용됩니다.

* __sharedvar.{variable_name}__
