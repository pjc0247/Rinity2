2-Way Binding
====

양방향 바인딩은 __UI 컴포넌트__와 __게임 스크립트__를 서로간에 바인딩해주는 역할을 합니다.<br>
이 기능을 이용하면 C#의 프로퍼티와 UI상에 표시되는 수치를 서로 바인딩함으로써, 일일히 상태 변경 리스터를 구독하거나, `.property_setter`를 통해 프로프티가 변경될 때 마다 UI 오브젝트에 변경을 통지해야 할 필요가 전혀 없습니다.

SharedVar에 바인딩하기
----

![1](by_shared_var.PNG)
토글의 값이 `bool option_enableSound`의 값과 바인딩됩니다.<br>
양방향 바인딩이기 때문에, 유저의 토글 인풋에 의해 `option_enableSound`의 실제 값이 바뀔 수도 있고<br>
반대로 소스상에서의 `option_enableSound` 값 변경에 의해 UI의 토글 상태가 변경될 수 있습니다.


PubSubMessage에 바인딩하기
-----

![2](by_message.png)
특정한 메세지를 수신하거나, 메세지 안의 값에 의해서 토글 상태가 변경됩니다.


단반향 바인딩
----
UI 컴포넌트 중에는 유저와의 인터렉션이 없는 오브젝트들도 존재합니다.<br>
예를들어 `Text`는 단순히 텍스트를 출력할 뿐 유저로부터 입력을 받기 위해서 만들어진 오브젝트가 아닙니다.<br>

![3](single_binding_1.png)<br>
![4](single_binding.png)