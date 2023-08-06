# Some Unity URP Cases with friend LaFurr;

直接下载本工程, 用 unity 打开即可使用;
目前实现的案例有:

=========================================

### 02: -- 修改颜色对比度 --
在一个全局后处理的 render pass 中实现 Color Contrast 功能;
最简单的 render pass 编写演示;

### 03: -- 控制常规 shader pass 的执行时机 --
依靠 LightMode == "KOKO" 来指定 pass 在何时运行;

### 04: -- 控制旧粒子系统用 shader pass 的执行时机 --
和 03 相似;

### 05: -- 扭曲 -- 
让 扭曲_shader 运行在一个旧粒子系统上; 