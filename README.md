# 황금열쇠 Mk2
이 프로그램은 트위치 스트리머 [김편집_](https://www.twitch.tv/arpa__)님의 컨텐츠 **리듬마블**에 사용될 목적으로 제작되었습니다. 
해당 컨텐츠에 대해서는 [여기](https://tgd.kr/s/arpa__/66930418)를 참고해주세요.
## [황금열쇠 Mk1](https://github.com/smh0505/GoldenKey)과의 차이점
* 라이브러리를 Windows Forms에서 [Raylib](https://www.raylib.com/index.html)로 변경하였습니다.
  * OpenGL을 베이스로 하는 Raylib으로 구현한 돌림판은 그래픽 깨짐이 없어 보기 편해졌습니다.
* Marquee 기능을 추가하였습니다.
  * 개별 항목이 30개가 넘어가는 경우, 우측 리스트는 Marquee 기능을 위한 31개만 로드하도록 설정했습니다.
  * 돌림판과는 별개로 그리므로 항목 누락에 대해서는 안심하셔도 됩니다.
  * 개별 항목이 30개 이하로 내려가면 자동으로 Marquee 기능을 비활성화 하고 첫번째 항목부터 재배열합니다.
## 사용법
1. `default.json` 파일이 있으면 좋습니다. 투네이션 통합 위젯 비밀키와 기본 항목들을 저장해두는 파일입니다. **보안을 위해 방송화면에 해당 파일을 띄우고 수정하지 말아주세요.**

해당 파일의 구조는 다음과 같습니다. 수정 시에는 아래와 같은 구조를 유지해주세요. 

기본 항목들은 (1) 큰따옴표 안에 넣어서 (2) 대괄호 안에 추가해야 하며 (3) 쉼표로 구분합니다.
```json
{
    "Key": *투네이션 통합 위젯 비밀키*,
    "Values": [
        *1번 항목*,
        *2번 항목*,
        ...
    ]
}
```
2. `Release` 탭에서 `Release.zip` 을 다운로드 받아서 압축을 풀고 `GoldenKeyMK2.exe` 파일을 실행시키면 됩니다.

![image](https://user-images.githubusercontent.com/42821865/199403449-e91f86f4-b104-47e4-ada1-0fae6478610f.png)

`default.json` 파일의 내용물을 읽어오기 위해서는 `GoldenKeyMK2.exe` 와 같은 폴더 내에 있어야 합니다.

![image](https://user-images.githubusercontent.com/42821865/199405009-b0054eac-b59d-4baa-8962-966ec1322029.png)

3. 앱 실행 직후 첫 화면에서 투네이션 통합 위젯 비밀키를 입력해주세요. Ver.1.1부터 `Ctrl+V` 기능이 추가되었습니다.

![image](https://user-images.githubusercontent.com/42821865/199405094-87e90ce9-6e20-4dc9-838a-c897cf2b3c5e.png)

`default.json` 에 비밀키가 있다면 미리 입력되어 있을 것입니다. 엔터 키를 눌러주세요.

![image](https://user-images.githubusercontent.com/42821865/199405129-6ebb8bf2-0f78-49dc-ae9c-5c8e5092bae2.png)

4. 투네이션 연결에 성공하면 다음 화면으로 넘어갑니다. 이 화면에서는 투네이션 룰렛 항목을 읽어와 `꽝`이 아니면 돌림판에 추가합니다.

**돌림판이 돌아가는 동안에는 룰렛 항목을 별도로 저장해 뒀다가 돌림판이 완전히 멈추고 결과가 나오고 나서 모두 추가합니다.**

![image](https://user-images.githubusercontent.com/42821865/199405179-3ea15bd2-f19d-4906-abb1-9682e5b6a194.png)

돌림판은 스페이스 바로 돌리고 멈출 수 있습니다. 컨트롤 방법은 화면 좌측 하단에 표시됩니다.
## 빌드하는 법
이것은 이 프로그램을 스스로 빌드하고 싶은 사람들을 위한 안내문입니다. 터미널 또는 명령 프롬프트에 다음을 한줄씩 입력하시면 됩니다.
```cmd
git clone https://github.com/smh0505/GoldenKeyMK2.git
cd .\GoldenKeyMK2
dotnet restore
dotnet run
```
## 사용된 라이브러리
* [Raylib-cs](https://github.com/ChrisDill/Raylib-cs)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)
* [Websocket.Client](https://github.com/Marfusios/websocket-client)
## Special Thanks
[김편집_](https://www.twitch.tv/arpa__)  
[Eunbin Jeong (Dalgona.)](https://neodgm.dalgona.dev/)  
[cannabee](https://www.youtube.com/channel/UCKfk3-j0PHrNt37lMRccFmQ)  
And You
## [이보시오 지나가는 나그네여](https://ko-fi.com/bloppyhb)
가는 길에 500원짜리 동전 3개정도 던져주지 않겠나?
