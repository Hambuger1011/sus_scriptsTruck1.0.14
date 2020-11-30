using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// 定点数2D向量
    public struct FixedVector
    {
        public FixedNumber x;
        public FixedNumber y;

        public static readonly FixedVector Zero = new FixedVector(FixedNumber.Zero, FixedNumber.Zero);

        public FixedVector(FixedNumber _x, FixedNumber _y)
        {
            x = _x;
            y = _y;
        }

        public FixedVector(int _x, int _y)
        {
            x = new FixedNumber(_x);
            y = new FixedNumber(_y);
        }

        public FixedVector(float _x, float _y)
        {
            x = new FixedNumber(_x);
            y = new FixedNumber(_y);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[x={0}, y={1}", x, y);
        }

        public static FixedVector operator +(FixedVector a, FixedVector b)
        {
            FixedVector c;
            c.x = a.x + b.x;
            c.y = a.y + b.y;
            return c;
        }

        public static FixedVector operator -(FixedVector a, FixedVector b)
        {
            FixedVector c;
            c.x = a.x - b.x;
            c.y = a.y - b.y;
            return c;
        }

        public static FixedVector operator *(FixedVector a, FixedNumber b)
        {
            FixedVector c;
            c.x = a.x * b;
            c.y = a.y * b;
            return c;
        }

        public static FixedVector operator *(FixedNumber b, FixedVector a)
        {
            return a * b;
        }

        public static FixedVector operator /(FixedVector a, FixedNumber b)
        {
            FixedVector c;
            c.x = a.x / b;
            c.y = a.y / b;
            return c;
        }

        public static FixedVector operator -(FixedVector vector)
        {
            return new FixedVector(-vector.x, -vector.y);
        }

        public static bool operator !=(FixedVector v1, FixedVector v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static bool operator ==(FixedVector v1, FixedVector v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public FixedNumber GetLengthSqr()
        {
            FixedNumber len_sqr = x * x + y * y;
            return len_sqr;
        }

        public FixedNumber GetLength()
        {
            FixedNumber len = FixedNumber.Sqrt(GetLengthSqr());
            return len;
        }

        public FixedVector Normalized()
        {
            var len = GetLength();
            if(len == FixedNumber.Zero)
                return FixedVector.Zero;
            var v = new FixedVector(x / len, y / len);
            return v;
        }

        public void Normalize()
        {
            Normalized();
        }

        public static FixedVector Forward(FixedNumber face)
        {
            return new FixedVector(FixedNumber.Sin(face), FixedNumber.Cos(face));
        }

        public static FixedVector Right(FixedNumber face)
        {
            return new FixedVector(FixedNumber.Cos(face), -FixedNumber.Sin(face));
        }

        public FixedNumber Dot(FixedVector vector)
        {
            return x * vector.x + y * vector.y;
        }

        public FixedNumber Distance(FixedVector pos)
        {
            FixedVector v = pos - this;
            return v.GetLength();
        }

        public static FixedNumber Angle(FixedVector from, FixedVector to)
        {
            from.Normalize();
            to.Normalize();
            var cos = from.Dot(to);
            var angle = FixedNumber.Acos_Angle(cos);
            return angle;
        }

    }

    /// 定点数圆
    public struct FixedCircle
    {
        public FixedVector center;
        public FixedNumber radius;

        //构造函数
        public FixedCircle(FixedVector _center, FixedNumber _radius)
        {
            center = _center;
            radius = _radius;
        }

        // 判断圆是否和另一个圆相交
        public bool IsIntersectCircle(FixedCircle other)
        {
            FixedNumber add_radius = other.radius + radius;
            FixedVector v = other.center - center;
            return v.GetLengthSqr() <= (add_radius * add_radius);
        }

        // 判断圆是否和OBB相交
        public bool IsIntersectObb(FixedObb obb)
        {
            return obb.IsIntersectCircle(this);
        }

        // 判断圆是否和矩形相交
        public bool IsIntersectRect(FixedRect rect)
        {
            return rect.IsIntersectCircle(this);
        }

        public FixedVector GetRandomPosInside()
        {
            int randX = Random.Range(0, 200); //FixedRandom.Random(0, 200);
            int randY = Random.Range(0, 200); //FixedRandom.Random(0, 200);
            FixedNumber posX = radius * new FixedNumber((float) randX / 100f - 1f);
            FixedNumber maxRadiusY = FixedNumber.Sqrt(radius*radius - posX * posX);
            FixedNumber posY = maxRadiusY * new FixedNumber((float)randY / 100f - 1f);
            return new FixedVector(posX, posY) + center;
        }
    }

    /// 定点数轴对齐矩形
    public struct FixedRect
    {
        // 几何中心位置
        public FixedVector Center
        {
            get{ return center; }
            set
            {
                if(center != value)
                {
                    center = value;
                    obb_internal.Center = value;
                }
            }
        }
        private FixedVector center;

        // X轴尺寸
        public FixedNumber SizeX
        {
            get { return sizeX;}
        }
        private FixedNumber sizeX;

        // Y轴尺寸
        public FixedNumber SizeY
        {
            get { return sizeY;}
        }
        private FixedNumber sizeY;

        // 内部用来做obbb相交测试
        private FixedObb obb_internal;

        // 构造函数
        public FixedRect(FixedVector _center, FixedNumber _sizeX, FixedNumber _sizeY)
        {
            center = _center;
            sizeX = _sizeX;
            sizeY = _sizeY;

            // 内部数据更新
            obb_internal = new FixedObb(_center, FixedNumber.Zero, _sizeX, sizeY);
        }

        // 是否和另一个矩形相交
        public bool IsIntersectRect(FixedRect other)
        {
            var this_down_left = center - new FixedVector(sizeX / 2, sizeY / 2);
            var this_top_right = center + new FixedVector(sizeX / 2, sizeY / 2);
            var other_down_left = other.center - new FixedVector(other.sizeX / 2, other.sizeY / 2);
            var other_top_right = other.center + new FixedVector(other.sizeX / 2, other.sizeY / 2);
            return this_down_left.x <= other_top_right.x && this_top_right.x >= other_down_left.x
                && this_down_left.y <= other_top_right.y && this_top_right.y >= other_down_left.y;

        }

        // 是否和OBB相交
        public bool IsIntersectObb(FixedObb obb)
        {
            return obb.IsIntersectObb(obb_internal);
        }

        // 是否和圆相交
        public bool IsIntersectCircle(FixedCircle circle)
        {
            return obb_internal.IsIntersectCircle(circle);
        }
    }

    /// 定点数包围盒
    public struct FixedObb
    {
        // 几何中心位置
        public FixedVector Center
        {
            get { return center; }
            set
            {
                if(center != value)
                {
                    center = value;
                    UpdateInternal();
                }
            }
        }
        private FixedVector center;

        // 朝向（弧度）：y轴正向为0，顺时针增加
        public FixedNumber Face
        {
            get { return face; }
            set
            {
                if(face != value)
                {
                    face = value;
                    UpdateInternal();
                }
            }
        }
        private FixedNumber face;

        // X轴尺寸
        public FixedNumber SizeX
        {
            get { return sizeX; }
        }
        private FixedNumber sizeX;

        // Y轴尺寸
        public FixedNumber SizeY
        {
            get { return sizeY; }
        }
        private FixedNumber sizeY;

        // 左下角起，逆时针方向4个点
        public FixedVector[] Corners
        {
            get { return corners; }
        }
        private FixedVector[] corners;

        // 局部空间X和Y轴方向向量
        public FixedVector[] Axes
        {
            get { return axes; }
        }
        private FixedVector[] axes;

        // 局部空间X和Y轴的最小投影长度
        private FixedNumber[] minProjLen;

        // 局部空间X和Y轴的最大投影长度
        private FixedNumber[] maxProjLen;

        // OBB的AABB在世界空间的最小位置
        public FixedVector MinAabbPoint
        {
            get { return minAabbPoint; }
        }
        private FixedVector minAabbPoint;

        // OBB的AABB在世界空间的最小位置
        public FixedVector MaxAabbPoint
        {
            get { return maxAabbPoint; }
        }
        private FixedVector maxAabbPoint;

        // 构造函数
        public FixedObb(FixedVector _center, FixedNumber _face, FixedNumber _sizeX, FixedNumber _sizeY)
        {
            center = _center;
            face = _face;
            sizeX = _sizeX;
            sizeY = _sizeY;
            corners = new FixedVector[4];
            axes = new FixedVector[2];
            minProjLen = new FixedNumber[2];
            maxProjLen = new FixedNumber[2];
            minAabbPoint = FixedVector.Zero;
            maxAabbPoint = FixedVector.Zero;

            //内部数据更新
            UpdateInternal();
        }

        // 判断OBB和Circle是否相交
        public bool IsIntersectCircle(FixedCircle circle)
        {
            //坐标变换
            var v = circle.center - center; //平移
            FixedNumber cos = FixedNumber.Cos(face);
            FixedNumber sin = FixedNumber.Sin(face);
            v = new FixedVector(FixedNumber.Abs(cos * v.x - sin * v.y), FixedNumber.Abs(sin * v.x + cos * v.y)); //矩阵旋转，映射到第一上限

            // 求第一象限的以几何中心为起点的半个对角线长度的向量
            var h = new FixedVector(sizeX / 2, sizeY / 2);

            // 求圆心到OBB的最短距离向量
            var u = v - h;
            u = new FixedVector(FixedNumber.Max(u.x, FixedNumber.Zero), FixedNumber.Max(u.y, FixedNumber.Zero));

            // 比较u和圆半径的长度
            return u.GetLengthSqr() <= circle.radius * circle.radius;
        }

        // 判断OBB和另一个OBB是否相交
        public bool IsIntersectObb(FixedObb other)
        {
            return TestOtherObb(other) && other.TestOtherObb(this);
        }

        // 判断OBB是否和矩形相交
        public bool IsIntersectRect(FixedRect rect)
        {
            return rect.IsIntersectObb(this);
        }

        // 内部更新
        private void UpdateInternal()
        {
            // 轴更新
            axes[0] = new FixedVector(FixedNumber.Cos(face), -FixedNumber.Sin(face));
            axes[1] = new FixedVector(-axes[0].y, axes[0].x);

            // 角点更新
            FixedVector vec_x = axes[0] * (sizeX / 2);
            FixedVector vec_y = axes[1] * (sizeY / 2);
            corners[0] = center - vec_x - vec_y;
            corners[1] = center + vec_x - vec_y;
            corners[2] = center + vec_x + vec_y;
            corners[3] = center - vec_x + vec_y;

            // X和Y轴投影更新
            minProjLen[0] = axes[0].Dot(corners[0]);
            maxProjLen[0] = axes[0].Dot(corners[2]);
            minProjLen[1] = axes[1].Dot(corners[0]);
            maxProjLen[1] = axes[1].Dot(corners[2]);

            // 更新AABB
            minAabbPoint = corners[0];
            maxAabbPoint = corners[0];
            for(int i = 1; i < 4; i++)
            {
                var point = corners[i];

                if(point.x < minAabbPoint.x)
                    minAabbPoint.x = point.x;
                else if(point.x > maxAabbPoint.x)
                    maxAabbPoint.x = point.x;

                if(point.y < minAabbPoint.y)
                    minAabbPoint.y = point.y;
                else if(point.y > maxAabbPoint.y)
                    maxAabbPoint.y = point.y;
            }
        }

        // 测试另一个OBB
        private bool TestOtherObb(FixedObb other)
        {
            for(int i = 0; i < 2; i++)
            {
                FixedNumber t = axes[i].Dot(other.corners[0]);
                FixedNumber min_t = t;
                FixedNumber max_t = t;
                for(int j = 1; j < 4; j++)
                {
                    t = axes[i].Dot(other.corners[j]);
                    if(t < min_t)
                        min_t = t;
                    else if(t> max_t)
                        max_t = t;
                }
                if(min_t > maxProjLen[i] || max_t < minProjLen[i])
                    return false;
            }
            return true;
        }
    }

    /// 定点数
    /// 内部使用有符号64位整数表示, 1位符号位， 31位整数位， 16位小数位
    /// 高位必须保留16位，用于乘法或除法等运算的实现，避免数值范围溢出
    public struct FixedNumber
    {
        // 小数部分位数，必须为16
        public const int FIX_FRAC_BITS = 16;

        // 0
        public static readonly FixedNumber Zero = new FixedNumber(0);

        // 1
        public static readonly FixedNumber One = new FixedNumber(1);

        // 360
        public static readonly FixedNumber FN_360 = new FixedNumber(360);

        // 误差值
        public static readonly FixedNumber EPSILON = new FixedNumber(0.0001f);

        // 定义圆周率
        public static readonly FixedNumber PI = new FixedNumber(3.14159f);

        // 定义自然常数
        public static readonly FixedNumber E = new FixedNumber(2.71828f);

        // 内部数据位
        private long bits;

        #region 构造函数 + 重载函数 + 存取函数

        // 整数表示为定点数
        public FixedNumber(int x)
        {
            bits = (long)x << FIX_FRAC_BITS;
        }

        // 浮点数表示为定点数
        public FixedNumber(float x)
        {
            bits = (long)(x * (1 << FIX_FRAC_BITS));
        }

         // 双精度浮点数表示为定点数
        public FixedNumber(double x)
        {
            bits = (long)(x * (1 << FIX_FRAC_BITS));
        }

        // 重置内部数据位
        public FixedNumber(long _bits)
        {
            bits = _bits;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ToDouble().ToString();
        }

        public long GetBits()
        {
            return bits;
        }

        public void SetBits(long _bits)
        {
            bits = _bits;
        }

        #endregion

        #region operator +
        public static FixedNumber operator +(FixedNumber p1, FixedNumber p2)
        {
            FixedNumber tmp;
            tmp.bits = p1.bits + p2.bits;
            return tmp;
        }

        public static FixedNumber operator +(FixedNumber p1, int p2)
        {
            return p1 + new FixedNumber(p2);
        }

        public static FixedNumber operator +(int p1, FixedNumber p2)
        {
            return p2 + p1;
        }

         public static FixedNumber operator +(FixedNumber p1, float p2)
        {
            return p1 + new FixedNumber(p2);
        }

        public static FixedNumber operator +(float p1, FixedNumber p2)
        {
            return p2 + p1;
        }

        #endregion

        #region operator -
        public static FixedNumber operator -(FixedNumber p1, FixedNumber p2)
        {
            FixedNumber tmp;
            tmp.bits = p1.bits - p2.bits;
            return tmp;
        }

        public static FixedNumber operator -(FixedNumber p1, int p2)
        {
            return p1 - new FixedNumber(p2);
        }

        public static FixedNumber operator -(int p1, FixedNumber p2)
        {
            return new FixedNumber(p1) - p2;
        }

         public static FixedNumber operator -(FixedNumber p1, float p2)
        {
            return p1 - new FixedNumber(p2);
        }

        public static FixedNumber operator -(float p1, FixedNumber p2)
        {
            return new FixedNumber(p1) - p2;
        }

        #endregion

        #region operator *
        public static FixedNumber operator *(FixedNumber p1, FixedNumber p2)
        {
            FixedNumber tmp;
            tmp.bits = (p1.bits * p2.bits) >> FIX_FRAC_BITS;
            return tmp;
        }

        public static FixedNumber operator *(FixedNumber p1, int p2)
        {
            return p1 * new FixedNumber(p2);
        }

        public static FixedNumber operator *(int p1, FixedNumber p2)
        {
            return new FixedNumber(p1) * p2;
        }

         public static FixedNumber operator *(FixedNumber p1, float p2)
        {
            return p1 * new FixedNumber(p2);
        }

        public static FixedNumber operator *(float p1, FixedNumber p2)
        {
            return new FixedNumber(p1) * p2;
        }

        #endregion

        #region operator /
        public static FixedNumber operator /(FixedNumber p1, FixedNumber p2)
        {
            FixedNumber tmp;
            if(p2 == FixedNumber.Zero)
            {
                LOG.Error("/0");
                tmp.bits = Zero.bits;
            }
            else
            {
                tmp.bits = p1.bits * (1 << FIX_FRAC_BITS) / p2.bits;
            }
            
            return tmp;
        }

        public static FixedNumber operator /(FixedNumber p1, int p2)
        {
            return p1 / new FixedNumber(p2);
        }

        public static FixedNumber operator /(int p1, FixedNumber p2)
        {
            return new FixedNumber(p1) / p2;
        }

         public static FixedNumber operator /(FixedNumber p1, float p2)
        {
            return p1 / new FixedNumber(p2);
        }

        public static FixedNumber operator /(float p1, FixedNumber p2)
        {
            return new FixedNumber(p1) / p2;
        }

        #endregion

        #region operator %
        public static FixedNumber operator %(FixedNumber p1, FixedNumber p2)
        {
            FixedNumber tmp;            
            tmp.bits = p1.bits % p2.bits;  
            
            return tmp;
        }

        public static FixedNumber operator %(FixedNumber p1, int p2)
        {
            return p1 % new FixedNumber(p2);
        }

        public static FixedNumber operator %(int p1, FixedNumber p2)
        {
            return new FixedNumber(p1) % p2;
        }

         public static FixedNumber operator %(FixedNumber p1, float p2)
        {
            return p1 % new FixedNumber(p2);
        }

        public static FixedNumber operator %(float p1, FixedNumber p2)
        {
            return new FixedNumber(p1) % p2;
        }

        #endregion

        #region operator > < >= <= != ==
        public static bool operator >(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits > p2.bits;  
        }

        public static bool operator <(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits < p2.bits;  
        }

        public static bool operator >=(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits >= p2.bits;  
        }

        public static bool operator <=(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits <= p2.bits;  
        }

        public static bool operator !=(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits != p2.bits;  
        }

        public static bool operator ==(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits == p2.bits;  
        }

        #endregion

        #region 其他工具
        /// 牛顿迭代法实现的开平方运算
        public static FixedNumber Sqrt(FixedNumber num)
        {
            if(num < FixedNumber.Zero)
            {
                LOG.Error(string.Format("FixedNumber:对负数{0}进行开方，结果为零", num));
                return FixedNumber.Zero;
            }
            bool is_found = false;
            int max_iter_count = 100;   //最大迭代次数
            int cur_iter_count = 0;     // 当前迭代次数
            FixedNumber x = num / 2;    //初值
            if(x == FixedNumber.Zero)
            {
                return FixedNumber.Zero; //避免除零错误
            }
            while(cur_iter_count < max_iter_count)
            {
                cur_iter_count++;
                FixedNumber last_x = x;
                x = (x + (num / x)) / 2;
                if(FixedNumber.Abs(x - last_x) <= EPSILON)
                {
                    is_found = true;
                    break;
                }
            }
            if(!is_found)
            {
                LOG.Warn(string.Format("FixedNumber:对{0}进行开平方，结果{1}精度不足", num, x));
            }
            return x;
        }

        // 正弦函数
        // p1 弧度单位
        public static FixedNumber Sin(FixedNumber p1)
        {
            return FixedTriFunc.Sin(p1);
        }

        // 余弦函数
        // p1 弧度单位
        public static FixedNumber Cos(FixedNumber p1)
        {
            return FixedTriFunc.Cos(p1);
        }

        // 反正弦函数，返回整型角度
        public static FixedNumber Asin_Angle(FixedNumber x)
        {
            return FixedTriFunc.Asin_Angle(x);
        }

        // 反余弦函数，返回整型角度
        public static FixedNumber Acos_Angle(FixedNumber x)
        {
            return FixedTriFunc.Acos_Angle(x);
        }

        public static FixedNumber Max(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits > p2.bits ? p1 : p2;
        }

        public static FixedNumber Min(FixedNumber p1, FixedNumber p2)
        {
            return p1.bits < p2.bits ? p1 : p2;
        }

        public static FixedNumber Precision()
        {
            FixedNumber tmp;
            tmp.bits = 1;
            return tmp;
        }

        public static FixedNumber MaxValue()
        {
            FixedNumber tmp;
            tmp.bits = long.MaxValue;
            return tmp;
        }

        public static FixedNumber Abs(FixedNumber num)
        {
            FixedNumber tmp;
            if(num.bits < 0)
                tmp.bits = -num.bits;
            else
                tmp.bits = num.bits;
            return tmp;
            return tmp;
        }

        public static FixedNumber Clamp(FixedNumber num, FixedNumber min, FixedNumber max)
        {
            if(num < min)
                num = min;
            else if(num > max)
                num = max;
            return num;
        }

        public static FixedNumber Clamp01(FixedNumber num)
        {
            return Clamp(num, FixedNumber.Zero, FixedNumber.One);
        }

        public static FixedNumber operator -(FixedNumber num)
        {
           FixedNumber tmp;
           tmp.bits = -num.bits;
           return tmp;
        }

        public static FixedNumber operator +(FixedNumber num)
        {
           return num;
        }

        // 转换到浮点数，内部逻辑计算不要使用，仅提供给表现
        public float ToFloat()
        {
            return bits / (float)(1 << FIX_FRAC_BITS);
        }

        // 转换到双精度浮点数，内部逻辑计算不要使用，仅提供给表现
        public double ToDouble()
        {
            return bits / (double)(1 << FIX_FRAC_BITS);
        }

        // 获取整数部分
        public int ToInt()
        {
            return Floor();
        }

        // 向下取整函数
        public int Floor()
        {
            return (int)(bits >> FIX_FRAC_BITS);
        }

        // 向上取整
        public int Ceil()
        {
            int a = Floor();
            long frac_bits = bits & 0xffff;
            if(frac_bits > 0)
                a++;
            return a;
        }

        // 四舍五入到整数
        public int RoundToInt()
        {
            int a = ToInt();
            long frac_bits = bits & 0xffff;

            // 如果 >= 0.5
            if(frac_bits >= 0x8000)
            {
                if(a >= 0)
                    a++;
                else
                    a--;
            }
            return a;
        }

        // 角度转弧度
        public static FixedNumber AngleToRadian(FixedNumber angle)
        {
            FixedNumber radian = angle * FixedNumber.PI / 180;
            return radian;
        }

        // 弧度转角度
        public static FixedNumber RadianToAngle(FixedNumber radian)
        {
            FixedNumber angle = radian * 180 / FixedNumber.PI;
            return angle;
        }

        // 控制角度在0到360度之间
        public static FixedNumber AngleClamp360(FixedNumber angle)
        {
            if(angle < FixedNumber.Zero || angle >= FN_360)
                angle = (angle % FN_360 + FN_360) % FN_360; // 确保angle 在[0, FN_360)取值
            return angle;
        }

        // 计算以自然常数e为底的对数
        public static FixedNumber Log(FixedNumber x)
        {
            return FixedExpLogFunc.Log(x);
        }

        // 计算以newBase为底（大于0且小于1）的对数
        public static FixedNumber Exp(FixedNumber x)
        {
            return FixedExpLogFunc.Exp(x);
        }

        // 计算a的x次幂
        public static FixedNumber Pow(FixedNumber a, FixedNumber x)
        {
            return FixedExpLogFunc.Pow(a, x);
        }

        #endregion
    }

    /// 定点数三角函数
    public static class FixedTriFunc
    {
        // 定义查表的点数
        private static readonly int TAB_N = 360;

        // 定义sin表
        private static readonly List<FixedNumber> SIN_TAB = new List<FixedNumber>();

        // 静态构造函数
        static FixedTriFunc()
        {
            // 初始化（90 + 1）个角度数据
            SIN_TAB.Add(new FixedNumber(0.00000f));    // 0度
            SIN_TAB.Add(new FixedNumber(0.01745f));    // 1度
            SIN_TAB.Add(new FixedNumber(0.03490f)); 
            SIN_TAB.Add(new FixedNumber(0.05234f)); 
            SIN_TAB.Add(new FixedNumber(0.06976f)); 
            SIN_TAB.Add(new FixedNumber(0.08716f)); 
            SIN_TAB.Add(new FixedNumber(0.10453f)); 
            SIN_TAB.Add(new FixedNumber(0.12187f)); 
            SIN_TAB.Add(new FixedNumber(0.13917f)); 
            SIN_TAB.Add(new FixedNumber(0.15643f)); 

            SIN_TAB.Add(new FixedNumber(0.17365f)); 
            SIN_TAB.Add(new FixedNumber(0.19081f)); 
            SIN_TAB.Add(new FixedNumber(0.20791f)); 
            SIN_TAB.Add(new FixedNumber(0.22495f)); 
            SIN_TAB.Add(new FixedNumber(0.24192f)); 
            SIN_TAB.Add(new FixedNumber(0.25882f)); 
            SIN_TAB.Add(new FixedNumber(0.27564f)); 
            SIN_TAB.Add(new FixedNumber(0.29237f)); 
            SIN_TAB.Add(new FixedNumber(0.30902f)); 
            SIN_TAB.Add(new FixedNumber(0.32557f)); 

            SIN_TAB.Add(new FixedNumber(0.34202f)); 
            SIN_TAB.Add(new FixedNumber(0.35837f)); 
            SIN_TAB.Add(new FixedNumber(0.37461f)); 
            SIN_TAB.Add(new FixedNumber(0.39073f)); 
            SIN_TAB.Add(new FixedNumber(0.40674f)); 
            SIN_TAB.Add(new FixedNumber(0.42262f)); 
            SIN_TAB.Add(new FixedNumber(0.43837f)); 
            SIN_TAB.Add(new FixedNumber(0.45399f)); 
            SIN_TAB.Add(new FixedNumber(0.46947f)); 
            SIN_TAB.Add(new FixedNumber(0.48481f)); 

            SIN_TAB.Add(new FixedNumber(0.50000f)); 
            SIN_TAB.Add(new FixedNumber(0.51504f)); 
            SIN_TAB.Add(new FixedNumber(0.52992f)); 
            SIN_TAB.Add(new FixedNumber(0.54464f)); 
            SIN_TAB.Add(new FixedNumber(0.55919f)); 
            SIN_TAB.Add(new FixedNumber(0.57358f)); 
            SIN_TAB.Add(new FixedNumber(0.58779f)); 
            SIN_TAB.Add(new FixedNumber(0.60182f)); 
            SIN_TAB.Add(new FixedNumber(0.61566f)); 
            SIN_TAB.Add(new FixedNumber(0.62932f)); 

            SIN_TAB.Add(new FixedNumber(0.64279f)); 
            SIN_TAB.Add(new FixedNumber(0.65606f)); 
            SIN_TAB.Add(new FixedNumber(0.66913f)); 
            SIN_TAB.Add(new FixedNumber(0.68200f)); 
            SIN_TAB.Add(new FixedNumber(0.69466f)); 
            SIN_TAB.Add(new FixedNumber(0.70711f)); 
            SIN_TAB.Add(new FixedNumber(0.71934f)); 
            SIN_TAB.Add(new FixedNumber(0.73135f)); 
            SIN_TAB.Add(new FixedNumber(0.74314f)); 
            SIN_TAB.Add(new FixedNumber(0.75471f)); 

            SIN_TAB.Add(new FixedNumber(0.76604f)); 
            SIN_TAB.Add(new FixedNumber(0.77715f)); 
            SIN_TAB.Add(new FixedNumber(0.78801f)); 
            SIN_TAB.Add(new FixedNumber(0.79864f)); 
            SIN_TAB.Add(new FixedNumber(0.80902f)); 
            SIN_TAB.Add(new FixedNumber(0.81915f)); 
            SIN_TAB.Add(new FixedNumber(0.82904f)); 
            SIN_TAB.Add(new FixedNumber(0.83867f)); 
            SIN_TAB.Add(new FixedNumber(0.84805f)); 
            SIN_TAB.Add(new FixedNumber(0.85717f)); 

            SIN_TAB.Add(new FixedNumber(0.86603f)); 
            SIN_TAB.Add(new FixedNumber(0.87462f)); 
            SIN_TAB.Add(new FixedNumber(0.88295f)); 
            SIN_TAB.Add(new FixedNumber(0.89101f)); 
            SIN_TAB.Add(new FixedNumber(0.89879f)); 
            SIN_TAB.Add(new FixedNumber(0.90631f)); 
            SIN_TAB.Add(new FixedNumber(0.91355f)); 
            SIN_TAB.Add(new FixedNumber(0.92050f)); 
            SIN_TAB.Add(new FixedNumber(0.92718f)); 
            SIN_TAB.Add(new FixedNumber(0.93358f)); 

            SIN_TAB.Add(new FixedNumber(0.93969f)); 
            SIN_TAB.Add(new FixedNumber(0.94552f)); 
            SIN_TAB.Add(new FixedNumber(0.95106f)); 
            SIN_TAB.Add(new FixedNumber(0.95630f)); 
            SIN_TAB.Add(new FixedNumber(0.96126f)); 
            SIN_TAB.Add(new FixedNumber(0.96593f)); 
            SIN_TAB.Add(new FixedNumber(0.97030f)); 
            SIN_TAB.Add(new FixedNumber(0.97437f)); 
            SIN_TAB.Add(new FixedNumber(0.97815f)); 
            SIN_TAB.Add(new FixedNumber(0.98163f)); 

            SIN_TAB.Add(new FixedNumber(0.98481f)); 
            SIN_TAB.Add(new FixedNumber(0.98769f)); 
            SIN_TAB.Add(new FixedNumber(0.99027f)); 
            SIN_TAB.Add(new FixedNumber(0.99255f)); 
            SIN_TAB.Add(new FixedNumber(0.99452f)); 
            SIN_TAB.Add(new FixedNumber(0.99619f)); 
            SIN_TAB.Add(new FixedNumber(0.99756f)); 
            SIN_TAB.Add(new FixedNumber(0.99863f)); 
            SIN_TAB.Add(new FixedNumber(0.99939f)); 
            SIN_TAB.Add(new FixedNumber(0.99985f));     //89度

            SIN_TAB.Add(new FixedNumber(1.00000f));    // 90度
        }

        // 正弦函数
        public static FixedNumber Sin(FixedNumber radian)
        {
            int n = ((radian * TAB_N) / (2 * FixedNumber.PI)).RoundToInt();
            if(n < 0 || n >= TAB_N)
                n = (n % TAB_N + TAB_N) % TAB_N;    // 确保nzai[0, TAB_N) 取值
            FixedNumber a = FixedNumber.Zero;
            if(n >= 0 && n < TAB_N / 4)     //[0, PI / 2)
            {
                a = SIN_TAB[n];
            }
            else if(n >= TAB_N / 4 && n < TAB_N / 2)   // [PI / 2, PI)
            {
                n -= TAB_N / 4;
                a = SIN_TAB[TAB_N / 4 - n];
            }
            else if(n >= TAB_N / 2 && n < 3 * TAB_N / 4)    //[PI, 3 / 4 * PI)
            {
                n -= TAB_N / 2;
                a = -SIN_TAB[n];
            }
            else if(n >= 3 * TAB_N / 4 && n < TAB_N)    //[3 / 4 * PI, 2 * PI)
            {
                n = TAB_N - n;
                a = -SIN_TAB[n];
            }
            return a;
        }

        // 余弦函数
        public static FixedNumber Cos(FixedNumber radian)
        {
            //诱导公式
            FixedNumber radian2 = radian + FixedNumber.PI / 2;
            FixedNumber a = Sin(radian2);
            return a;
        }

        // 反正弦函数，返回整型角度
        // 参数 [-1, 1]
        // 返回 [-90°, 90°]
        public static FixedNumber Asin_Angle(FixedNumber x)
        {
            x = FixedNumber.Clamp(x, -FixedNumber.One, FixedNumber.One);
            var abs_x = FixedNumber.Abs(x);
            int found_index = Asin_Angle_BinarySearch(0, 90, abs_x);
            FixedNumber angle = new FixedNumber(found_index);
            if(x < FixedNumber.Zero)
                angle = -angle;
            return angle;
        }

        // 反正弦函数内部使用的二分查找算法
        private static int Asin_Angle_BinarySearch(int start_index, int end_index, FixedNumber x)
        {
            if(end_index - start_index <= 1)
            {
                FixedNumber sin_start = SIN_TAB[start_index];
                FixedNumber sin_end = SIN_TAB[end_index];
                FixedNumber sin_middle = (sin_start + sin_end) / 2;
                if(x <= sin_middle)
                    return start_index;
                else
                    return end_index;
            }
            int middle_index = (start_index + end_index) / 2;
            FixedNumber sin = SIN_TAB[middle_index];
            if(x <= sin)
                return Asin_Angle_BinarySearch(start_index, middle_index, x);
            else
                return Asin_Angle_BinarySearch(middle_index, end_index, x);
        }

        // 反余弦函数，返回整型角度
        // asin + acos = PI / 2
        // 参数 [-1, 1]
        // 返回 [0°, 180°]
        public static FixedNumber Acos_Angle(FixedNumber x)
        {
            x = FixedNumber.Clamp(x, -FixedNumber.One, FixedNumber.One);
            var angle = new FixedNumber(90) - Asin_Angle(x);
            return angle;
        }
    }

    /// 定点数指数与对数函数类
    public static class FixedExpLogFunc
    {
        // 10
        private static readonly FixedNumber TEN = new FixedNumber(10);

        // ln 10
        private static readonly FixedNumber LIN10 = new FixedNumber(2.30259f);

        // e为底的整数次幂
        private static readonly List<FixedNumber> EXP_TAB = new List<FixedNumber>();

        // 最大的e为底指数
        private static readonly FixedNumber MAX_EXP_X = new FixedNumber(21.48756f);

        //静态构造函数
        static FixedExpLogFunc()
        {
            EXP_TAB.Add(new FixedNumber(1.00000));  // e^0
            EXP_TAB.Add(new FixedNumber(2.71828));  // e^1
            EXP_TAB.Add(new FixedNumber(7.38905));  // e^2
            EXP_TAB.Add(new FixedNumber(20.08550));  // e^3
            EXP_TAB.Add(new FixedNumber(54.59800));  // e^4
            EXP_TAB.Add(new FixedNumber(148.41266));  // e^5
            EXP_TAB.Add(new FixedNumber(403.42717));  // e^6
            EXP_TAB.Add(new FixedNumber(1096.62799));  // e^7
            EXP_TAB.Add(new FixedNumber(2980.94195));  // e^8
            EXP_TAB.Add(new FixedNumber(8103.03487));  // e^9

            EXP_TAB.Add(new FixedNumber(22026.31763));  // e^10
            EXP_TAB.Add(new FixedNumber(59873.69870));  // e^11
            EXP_TAB.Add(new FixedNumber(162753.47769));  // e^12
            EXP_TAB.Add(new FixedNumber(442409.52335));  // e^13
            EXP_TAB.Add(new FixedNumber(1202592.95913));  // e^14
            EXP_TAB.Add(new FixedNumber(3268984.38894));  // e^15
            EXP_TAB.Add(new FixedNumber(8886014.88476));  // e^16
            EXP_TAB.Add(new FixedNumber(24154676.54095));  // e^17
            EXP_TAB.Add(new FixedNumber(65659174.14772));  // e^18
            EXP_TAB.Add(new FixedNumber(178480019.90227));  // e^19

            EXP_TAB.Add(new FixedNumber(485158668.49995));  // e^20
            EXP_TAB.Add(new FixedNumber(1318797105.41003));  // e^21
        }

        // 计算以自然常数e为底的对数
        // x范围在(0, +1],可保证精度
        private static FixedNumber _ln(FixedNumber x)
        {
            // 最大迭代次数
            const int MAX_LN_ITERATION_COUNT = 20;

            // 特殊点处理
            if(x== FixedNumber.One)
                return FixedNumber.Zero;

            // 换元法：x属于非-1的实数集，设t = (x - 1) / (x + 1),则t在(-1, +1), x = (1 + t) / (1 - t).
            // 对数公式与麦克劳林展开式
            // ln(x) = ln((1 + t) / (1 - t)) = ln(1 + t) - ln(1 - t) = 2 * (t + ^3 / 3 + t^5 / 5 + ...) + o(t^((2n+2))
            var t = (x - 1) / (x + 1);
            var t_sqr = t * t;
            var sum = FixedNumber.Zero;
            var a = t;
            var d = FixedNumber.One;
            for(int i = 1; i <= MAX_LN_ITERATION_COUNT; i++)
            {
                var c = 2 * (1 / d) * a; //此项收敛
                sum += c;
                a *= t_sqr;
                d += 2;
                if(FixedNumber.Abs(c) <= FixedNumber.EPSILON)
                {
                    break;
                }
            }
            return sum;
        }

        // 计算以自然常数e为底的对数
        public static FixedNumber Log(FixedNumber x)
        {
            // eg : ln(208.45) = ln(10 * 10 * 10 * 0.20845) = ln(10) * 3 + ln(0.20845)
            int n = 0;
            var m = x;
            for(n = 0; m > FixedNumber.One; n++)
                m /= TEN;
            var result = LIN10 * n + _ln(m);
            return result;
        }

        // 计算以newBase为底（大于0且不为1）的对数
        public static FixedNumber Log(FixedNumber x, FixedNumber newBase)
        {
            if(newBase > FixedNumber.Zero && newBase != FixedNumber.One)
            {
                var result = Log(x) / Log(newBase);
                return result;
            }
            else
            {
                LOG.Error(string.Format("对数的底必须大于0且不为1，newBase={0}", newBase));
                return FixedNumber.Zero;
            }
        }

        // 计算自然常数e的x次幂
        // x范围在（-1, +1), 可保证精度
        private static FixedNumber _exp(FixedNumber x)
        {
            // 最大迭代次数
            // 注意有阶乘，不能太大
            const int MAX_EXP_ITERATION_COUNT = 10;

            // 特殊点处理
            if(x == FixedNumber.Zero)
                return FixedNumber.Zero;

            var sum = FixedNumber.One;
            var a = FixedNumber.One;
            var b = FixedNumber.One;
            for(int i = 1; i <= MAX_EXP_ITERATION_COUNT; i++)
            {
                a *= x;
                b *= i;
                var c = a / b; //此项收敛
                sum += c;
                if(FixedNumber.Abs(c) <= FixedNumber.EPSILON)
                {
                    break;
                }
            }
            return sum;
        }

        //计算自然常数e的x次幂
        public static FixedNumber Exp(FixedNumber x)
        {
            var abs_x = FixedNumber.Abs(x);
            if(abs_x > MAX_EXP_X)
            {
                LOG.Error(string.Format("Exp的指数绝对值{0}大于上限{1}:", abs_x, MAX_EXP_X));
                return FixedNumber.Zero;
            }
            int int_part = abs_x.ToInt();
            var frac_part = abs_x - int_part;
            var m = EXP_TAB[int_part];
            m *= _exp(frac_part);
            if(x < FixedNumber.Zero)
            {
                m = 1 / m;
            }
            return m;
        }

        //计算a的x的次幂
        public static FixedNumber Pow(FixedNumber a, FixedNumber x)
        {
            var result = Exp(Log(a) * x);
            return result;
        }
    }

}
