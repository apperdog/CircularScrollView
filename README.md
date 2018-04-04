# CircularScrollView
![image](https://github.com/apperdog/CircularScrollView/blob/master/QQ%E5%9B%BE%E7%89%8720180404182352.png)

弧形ScrollVieW，其實作原理為，透過三點取得圓弧的半徑與圓心。得知半徑與圓心後，就可以透過圓方程來獲取弧上的任意一點。每次滑動時便以 x軸或 y軸，透過圓方程換算當前應該偏移多少才在弧上

資料參考：

1.三點求一圓:
https://blog.csdn.net/kezunhai/article/details/39476691

2.圓方程:
http://web.ntnu.edu.tw/~496403159/teachpage/eq.htm
