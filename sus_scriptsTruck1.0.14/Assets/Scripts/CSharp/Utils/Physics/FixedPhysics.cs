using System.Collections.Generic;

namespace GameLogic
{
    // 定点数四叉树节点
    public class FixedQuadTreeNode
    {
        private const int NODE_ZONE_COUNT = 4;  //节点固定4个子节点
        private const int MAX_LEVEL_COUNT = 4;  // 整棵树固定4层（也许可以根据benchmark结果修改为3层或5层)

        private int level;      // 根节点为第0级
        private FixedRect rect; // 节点AABB
        private FixedQuadTreeNode[] children;   // 叶子节点为空
        private List<uint> entityList;  // 叶子节点非空
        private List<uint> blockList;   // 叶子节点非空

        public FixedQuadTreeNode(int _level, FixedRect _rect)
        {
            level = _level;
            rect = _rect;

            if(level < MAX_LEVEL_COUNT - 1)
            {
                children = new FixedQuadTreeNode[NODE_ZONE_COUNT];
                FixedNumber half_size_x = rect.SizeX / 2;
                FixedNumber half_size_y = rect.SizeY / 2;
                FixedNumber quarter_size_x = rect.SizeX / 4;
                FixedNumber quarter_size_y = rect.SizeY / 4;

                // zone 0:
                FixedRect child_rect_0 = new FixedRect(rect.Center + new FixedVector(-quarter_size_x, -quarter_size_y), half_size_x, half_size_y);
                children[0] = new FixedQuadTreeNode(level + 1, child_rect_0);

                // zone 1:
                FixedRect child_rect_1 = new FixedRect(rect.Center + new FixedVector(quarter_size_x, -quarter_size_y), half_size_x, half_size_y);
                children[1] = new FixedQuadTreeNode(level + 1, child_rect_1);

                // zone 2:
                FixedRect child_rect_2 = new FixedRect(rect.Center + new FixedVector(quarter_size_x, quarter_size_y), half_size_x, half_size_y);
                children[2] = new FixedQuadTreeNode(level + 1, child_rect_2);

                // zone 3:
                FixedRect child_rect_3 = new FixedRect(rect.Center + new FixedVector(-quarter_size_x, quarter_size_y), half_size_x, half_size_y);
                children[3] = new FixedQuadTreeNode(level + 1, child_rect_3);
            }
            else
            {
                entityList = new List<uint>();
                blockList = new List<uint>();
            }
        }

        public void ClearEntities()
        {
            if(entityList != null)
            {
                entityList.Clear();
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].ClearEntities();
                }
            }
        }

        public void AddEntity(uint eid, FixedCircle circle)
        {
            if(!rect.IsIntersectCircle(circle))
                return;
            if(entityList != null)
            {
                entityList.Add(eid);
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].AddEntity(eid, circle);
                }
            }
        }

        public void QueryEntities(FixedObb obb, ref HashSet<uint> result)
        {
            if(!rect.IsIntersectObb(obb))
                return;
            if(entityList != null)
            {
                foreach(var item in entityList)
                {
                    result.Add(item);
                }
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].QueryEntities(obb, ref result);
                }
            }
        }

        public void QueryEntities(FixedCircle circle, ref HashSet<uint> result)
        {
            if(!rect.IsIntersectCircle(circle))
                return;
            if(entityList != null)
            {
                foreach(var item in entityList)
                {
                    result.Add(item);
                }
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].QueryEntities(circle, ref result);
                }
            }
        }

        public void ClearBlocks()
        {
            if(blockList != null)
            {
                blockList.Clear();
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].ClearBlocks();
                }
            }
        }

        public void AddBlock(uint bid, FixedObb obb)
        {
            if(!rect.IsIntersectObb(obb))
                return;
            if(blockList != null)
            {
                blockList.Add(bid);
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].AddBlock(bid, obb);
                }
            }
        }

        public void QueryBlock(FixedCircle circle, ref HashSet<uint> result)
        {
            if(!rect.IsIntersectCircle(circle))
                return;
            if(blockList != null)
            {
                foreach(var item in blockList)
                {
                    result.Add(item);
                }
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].QueryBlock(circle, ref result);
                }
            }
        }

        public void QueryBlock(FixedRect _rect, ref HashSet<uint> result)
        {
            if(!rect.IsIntersectRect(_rect))
                return;
            if(blockList != null)
            {
                foreach(var item in blockList)
                {
                    result.Add(item);
                }
            }
            else
            {
                for(int i = 0; i < NODE_ZONE_COUNT; i++)
                {
                    if(children[i] != null)
                        children[i].QueryBlock(_rect, ref result);
                }
            }
        }
    }

    /// 定点数物理场景障碍物
    public struct FixedSceneBlock
    {
        // 障碍物id
        private uint id;

        // OBB形状
        private FixedObb obb;

        // 构造函数
        public FixedSceneBlock(uint _id, FixedObb _obb)
        {
            id = _id;
            obb = _obb;
        }

        // 获取障碍物id
        public uint GetId()
        {
            return id;
        }

        // 获取OBB形状
        public FixedObb GetObb()
        {
            return obb;
        }
    }

    /// 定点数物理场景
    public class FixedPhysSceneBlock
    {
        //场景范围
        private FixedRect rect;
        public FixedRect Rect
        {
            get { return rect; }
        }

        // 四叉树根节点
        private FixedQuadTreeNode root;

        // 障碍物字典
        private Dictionary<uint, FixedSceneBlock> blockDic;

        // 初始化
        public void Init(List<FixedSceneBlock> block_list)
        {
            //确定场景范围
            FixedVector rect_center = FixedVector.Zero;
            FixedVector rect_size = FixedVector.Zero;
            if(block_list.Count > 0)
            {
                var minAabbPoint = block_list[0].GetObb().MinAabbPoint;
                var maxAabbPoint = block_list[0].GetObb().MaxAabbPoint;
                for(int i = 1; i < block_list.Count; i++)
                {
                    var obb = block_list[i].GetObb();
                    if(obb.MinAabbPoint.x < minAabbPoint.x)
                        minAabbPoint.x = obb.MinAabbPoint.x;
                    if(obb.MaxAabbPoint.x > maxAabbPoint.x)
                        maxAabbPoint.x = obb.MaxAabbPoint.x;
                    if(obb.MinAabbPoint.y < minAabbPoint.y)
                        minAabbPoint.y = obb.MinAabbPoint.y;
                    if(obb.MaxAabbPoint.y > maxAabbPoint.y)
                        maxAabbPoint.y = obb.MaxAabbPoint.y;
                }
                rect_center = (minAabbPoint + maxAabbPoint) / new FixedNumber(2);
                rect_size = maxAabbPoint - minAabbPoint;
                LOG.Info(string.Format("物理场景信息：矩形中心={0}，矩形大小={1}", rect_center, rect_size));
            }
            else
            {
                LOG.Error("场景中没有任何障碍物，将无法确定范围");
            }
			rect = new FixedRect(rect_center, rect_size.x, rect_size.y);

            //生成四叉树
            root = new FixedQuadTreeNode(0, rect);

            //初始化障碍物
            blockDic = new Dictionary<uint, FixedSceneBlock>();
            root.ClearBlocks();
            foreach(var item in block_list)
            {
                blockDic.Add(item.GetId(), item);
                root.AddBlock(item.GetId(), item.GetObb());
            }
        }

        // 平移滑动函数
        // Note: Circle在OBB的4个圆角或多个OBB交叉位置附近，最终可能存在轻微相交，然而并不影响体验，计算性能更好一些
        //public FixedVector MoveSlide(EntityFightable owner, FixedVector motion)
        //{
        //    FixedCircle old_circle = owner.GetCurrentBound();
        //    if(motion == FixedVector.Zero)
        //        return old_circle.center;
        //    var new_circle = new FixedCircle(old_circle.center + motion, old_circle.radius);

        //    // 检查场景碰撞
        //    var query_block_result = new HashSet<uint>();
        //    root.QueryBlock(new_circle, ref query_block_result);
        //    foreach(var bid in query_block_result)
        //    {
        //        if(blockDic.ContainsKey(bid))
        //        {
        //            var block = blockDic[bid];
        //            var block_obb = block.GetObb();
        //            if(block_obb.IsIntersectCircle(new_circle))
        //            {
        //                new_circle = MoveSlideInternal_CircleWithObb(block_obb, old_circle, motion);    // 用位移产生新圆心
        //                motion = new_circle.center - old_circle.center; //迭代计算新位移
        //            }
        //        }
        //    }
        //    return new_circle.center;
        //}

        //AABB是否和场景障碍物相交
        public bool IsIntersectBlocks(FixedRect _rect)
        {
            var query_result = new HashSet<uint>();
            root.QueryBlock(_rect, ref query_result);
            foreach(var bid in query_result)
            {
                if(blockDic.ContainsKey(bid))
                {
                    var block = blockDic[bid];
                    var block_obb = block.GetObb();
                    if(block_obb.IsIntersectRect(_rect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //点的区域是否与OBB相交
        public bool IsContainRectCenter(FixedRect _rect)
        {
            //_rect.Center 中心点附近的圆圈
            FixedCircle circle = new FixedCircle(_rect.Center, new FixedNumber(0.2f) * _rect.SizeX);
            var query_result = new HashSet<uint>();
            root.QueryBlock(_rect, ref query_result);
            foreach(var bid in query_result)
            {
                if(blockDic.ContainsKey(bid))
                {
                    var block = blockDic[bid];
                    var block_obb = block.GetObb();
                    if(block_obb.IsIntersectCircle(circle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 每个逻辑帧调用一次
        //public bool OnFrameTick(Dictionary<uint, EntityFightable> entity_dict, List<BulletBase> bullets)
        //{
        //    //刷新四叉树
        //    RefreshQuadTree(entity_dict);

        //    //检测子弹和实体的碰撞
        //    foreach(var item in bullets)
        //    {
        //        item.OnCllideEntities(root, entity_dict);
        //    }
        //}

        //// 刷新四叉树
        //private void RefreshQuadTree(Dictionary<uint, EntityFightable> entity_dict)
        //{
        //    root.ClearEntities();
        //    foreach(var kv in entity_dict)
        //    {
        //        var entity = kv.Value;
        //        root.AddEntity(entity.eid, entity.GetCurrentBound());
        //    }
        //}

        // 平滑移动内部函数：圆在某个OBB上
        private FixedCircle MoveSlideInternal_CircleWithObb(FixedObb obb, FixedCircle old_circle, FixedVector motion)
        {
            // 定义圆心和半径
            var circle_radius = old_circle.radius;
            var old_cc = old_circle.center;
            var new_cc = old_circle.center + motion;
            var obb_cos = FixedNumber.Cos(obb.Face);
            var obb_sin = FixedNumber.Sin(obb.Face);

            // 将旧圆心转换到OBB坐标系
            old_cc -= obb.Center;
            old_cc = new FixedVector(
                obb_cos * old_cc.x - obb_sin * old_cc.y,
                obb_sin * old_cc.x + obb_cos * old_cc.y);

            // 将新圆心转换到OBB坐标系
            new_cc -= obb.Center;
            new_cc = new FixedVector(
                obb_cos * new_cc.x - obb_sin * new_cc.y,
                obb_sin * new_cc.x + obb_cos * new_cc.y);

            // OBB半长
            var half_obb_size_x = obb.SizeX / 2;
            var half_obb_size_y = obb.SizeY / 2;

            // 修正新圆心
            if(old_cc.y >= FixedNumber.Zero && old_cc.x >= -half_obb_size_x && old_cc.x <= half_obb_size_x)
            {
                new_cc.y = half_obb_size_y + circle_radius;
            }
            else if(old_cc.y <= FixedNumber.Zero && old_cc.x >= -half_obb_size_x && old_cc.x <= half_obb_size_x)
            {
                new_cc.y = -half_obb_size_y - circle_radius;
            }
            else if(old_cc.x >= FixedNumber.Zero && old_cc.y >= -half_obb_size_y && old_cc.y <= half_obb_size_y)
            {
                new_cc.x = half_obb_size_x + circle_radius;
            }
            else if(old_cc.x <= FixedNumber.Zero && old_cc.y >= -half_obb_size_y && old_cc.y <= half_obb_size_y)
            {
                new_cc.x = -half_obb_size_x - circle_radius;
            }
            else
            {
                // OBB的四个圆角范围，将来再考虑
            }

            // 将新圆心转换到世界坐标系
            obb_cos = FixedNumber.Cos(-obb.Face);
            obb_sin = FixedNumber.Sin(-obb.Face);
            new_cc = new FixedVector(
                obb_cos * new_cc.x - obb_sin * new_cc.y,
                obb_sin * new_cc.x + obb_cos * new_cc.y);
            new_cc += obb.Center;

            // 返回结果
            return new FixedCircle(new_cc, circle_radius);
        }

        // 平滑移动内部函数：圆在某个阻挡圆上
        private FixedCircle MoveSlideInternal_CircleWithCircle(FixedCircle block_circle, FixedCircle old_circle, FixedVector motion)
        {
            var circle_radius = old_circle.radius;
            var new_cc = old_circle.center + motion;
            var dist_vector = new_cc - block_circle.center;
            var dist = dist_vector.GetLength();
            var bound_dir = dist_vector.Normalized();   //反弹方向
            var bound_motion = bound_dir * FixedNumber.Max(block_circle.radius + circle_radius - dist, FixedNumber.Zero);
            new_cc += bound_motion;
            return new FixedCircle(new_cc, circle_radius);
        }

        //// 是否需要检查某个实体的阻挡
        //private bool IsNeedCheckBlockingForEntity(EntityFightable entity)
        //{
        //    if(entity is EntityMonster)
        //        return true;
        //    return false;
        //}

        // 返回两个实体间是否有阻挡关系
        //private bool HasBlockingRelationBetweenTowEntities(EntityFightable entity0, EntityFightable entity1)
        //{
        //    if(entity0 is EntityMonster && entity1 is EntityMonster)
        //        return true;
        //    return false;
        //}
    }
}