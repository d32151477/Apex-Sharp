# Apex-Sharp
Linux용 Apex Legends 외부 치트입니다.

## Sense
적 플레이어의 위치를 보여주는 기능입니다.

## Recoil
반동을 감소시키는 기능입니다.

- ```RECOIL_STRENGTH``` 반동을 해당 수치만큼 감소시킵니다.

## Aimbot
#### 인간적이지 않은 세팅값은 통계와 모니터링을 통해 EAC 밴 될 수 있습니다.
탄도 궤적과 해당 타겟 플레이어의 이동 방향과 속도를 계산하여 에임을 이동시킵니다.

- ```AIMBOT_MAX_FOV``` 해당 FOV 값 안에 타겟이 존재한다면 에임을 이동시킵니다.
- ```AIMBOT_SMOOTHNESS``` 타겟으로 에임을 이동시키는 속도입니다.

## Assist
오토 슈퍼글라이드 / 오토 버니합 기능입니다.

- ```ASSIST_AUTO_SUPERGLIDE_KEY``` 오토 슈퍼글라이드 핫키
- ```ASSIST_AUTO_BUNNYHOP_KEY``` 오토 버니합 핫키

## Input
/dev/input/eventX의 입력 이벤트를 실시간으로 읽어옵니다.

``` xinput list ``` 입력 장치 번호를 확인하고 주소를 변경하세요.

- ```INPUT_MOUSE_DEVICE_PATH``` 마우스 입력 장치 주소
- ```INPUT_KEYBOARD_DEVICE_PATH``` 키보드 입력 장치 주소

## Flatpak / Steam 설치
루트 프로세스 격리를 사용하려면 Flatpak을 통해 Steam을 설치해야합니다.

1. Flatpak 설치
   
```
sudo add-apt-repository ppa:alexlarsson/flatpak
sudo apt update && sudo apt install flatpak xdg-desktop-portal
```

2. Steam 설치
   
``` sudo flatpak install com.valvesoftware.Steam ```

## 루트 프로세스 격리 / 프로세스 추적 끄기
루트 프로세스를 숨기고 루트가 아닌 계정의 프로세스 감시를 차단합니다.

1. /etc/fstab 열기

``` sudo vim /etc/fstab ```

2. 파일에 해당 줄 추가 후 저장합니다.

``` proc /proc proc defaults,nosuid,nodev,noexec,relatime,hidepid=1 0 0 ```

3. /etc/sysctl.d/10-ptrace.conf 열기

``` sudo vim /etc/sysctl.d/10-ptrace.conf ```

4. kernel.yama.ptrace_scope 값 변경 후 저장합니다.

``` kernel.yama.ptrace_scope = 2 ```

6. 다시 시작

``` reboot ```

7. 확인

```
ps aux
sysctl kernel.yama.ptrace_scope
```

## 설치

1. Dotnet SDK / Runtime을 설치합니다.

```
sudo apt-get update && sudo apt-get install -y dotnet-sdk-7.0
sudo apt-get install -y dotnet-runtime-7.0
```


2. Git 프로젝트를 생성합니다.

```git clone https://github.com/d32151477/Apex-Sharp```

## 실행
- ```sudo dotnet run``` 프로젝트 실행하기
- ```sudo killall dotnet``` 프로젝트 종료하기

## 오프셋 덤프

1. 윈도우 환경에서 [Scylla]를 설치합니다. 

2. EAC를 비활성화합니다.

- 파일 이름을 EasyAntiCheat_launcher.exe에서 EasyAntiCheat_launcher.bak로 수정합니다.
- 파일 이름을 r5apex.exe에서 EasyAntiCheat_launcher.exe로 수정 후 실행

3. [Scylla]로 프로세스 덤프
   
- Scylla를 관리자 권한으로 실행합니다.
- EasyAntiCheat_launcher.exe 프로세스를 선택합니다.
- IAT Autosearch 를 선택하고 Yes를 선택합니다.
- Get Import 를 선택합니다.
- Dump 선택 후 덤프된 프로그램을 저장합니다.

4. [Apexdream]으로 오프셋 추출

- [Apexdream]을 빌드하여 추출하거나 미리 빌드된 사이트 [ApexDumper]에서 추출합니다.  
- 추출된 파일을 Dump.bin로 저장하여 Apex-Sharp 폴더에 붙여넣습니다.

[Apexdream]: https://github.com/CasualX/apexdream/tree/master/offsets
[ApexDumper]: https://casualhacks.net/apexdream/apexdumper.html
[Scylla]: https://github.com/scylladb/scylladb
