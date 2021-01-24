# 응용수학: 3차원 평면과 직선이 교차하는가?
<hr>
<br>

### 제시 문제
```
3차원 공간에 삼각형 ABC가 있을 때, 사용자가 P 위치에서 점 Q를 타겟으로 하여 총을 쏜다고 가정하자.
직선 운동을 하는 총알이 삼각형 ABC를 맞추었는지 판별하시오.
(단 중력은 작용하지 않는다.)
```

<br><br>

### 해결 알고리즘
1. 세 점 A, B, C를 지나는 평면의 방정식을 구한다.
    - 점 A를 기준점으로 잡고, 기준점에서 점 B, C에 이르는 벡터 BA, CA를 설정
    - 이 때 세 점 A, B, C를 지나는 평면의 법선벡터를 n이라고 하면, 법선벡터 n은 벡터 BA, CA와 동시에 수직하므로 n = BA X CA
    - 이 때 n의 x, y, z 성분을 각각 res1, res2, res3이라고 하면, 세 점 A, B, C를 지나는
   평면은 점 A를 지나므로 평면의 방정식은 <br>
   res1 * (x - x1) + res2 * (y – y1) + res3 * (z – z1) = 0 <br>
   ∴res1 * x + res2 * y + res3 * z = res1 * x1 + res2 * x2 + res3 * x3
   <br>
   
2. 두 점을 지나는 직선의 대칭 방정식을 이용해 벡터 PQ와 [1.]에서 구한 평면이 충돌하는 지점을 구한다.
    - P, Q를 지나는 대칭방정식 구하기
    - t에 관해 x, y, z 정리하기
    - 이를 평면방정식에 대입하기
    - 계수 부분 상수부분 계산후 x, y, z로 정리된 식에 대입해 교점 좌표 구하기
  
3. 아래의 그림과 같이 삼각형 안에 교점이 있는 경우, (∆ABC의 면적) = (∆ABK의 면적) + (∆KBC의 면적) + (∆AKC의 면적) 이 성립한다. 

<br><br>

### 시뮬레이션 제작
- C# WPF를 기반으로 작성
- 삼각형 3개의 좌표와 총알궤적 2개의 좌표를 입력
- 결과확인 버튼을 누르면 접점, 각 넓이, Alpha/Beta/Gamma 값, 충돌여부, 3d 그래픽을 출력한다.
