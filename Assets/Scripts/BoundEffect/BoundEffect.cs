using System.Collections.Generic;
using UnityEngine;
namespace MeshTools
{
    //利用RequireComponent特性，添加实现效果所需要的组件
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BoundEffect : MonoBehaviour
    {
        private Mesh _mesh = null;
        private MeshFilter _meshFilter = null;
        private float _lineLength = 0.25f;
        private float _lineWidth = 0.025f;
        private Bounds _bounds;//要模拟的包围盒
        private Vector3 _boundsPostion;//包围盒的实际位置
        private Transform _target = null;//这里是需要添加包围盒特表的模型

        public Transform Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value == _target) return;
                _target = value;
                _bounds = value.GetComponent<Renderer>().bounds;
                _boundsPostion = value.position;
                Refresh();
            }
        }
        /// <summary>
        /// 初始化组件信息
        /// </summary>
        private void Start()
        {
            _meshFilter = this.GetComponent<MeshFilter>();
            _mesh = new Mesh { name = "BoundEffect" };
            _meshFilter.mesh = _mesh;

        }
        /// <summary>
        /// 包围盒大小，以及包围盒位置
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="boundsPosition"></param>
        public void SetEffect(Bounds bounds, Vector3 boundsPosition)
        {
            _bounds = bounds;
            _boundsPostion = boundsPosition;
            Refresh();
        }
        private void Refresh()
        {
            InitPosition();
            //获取包围盒的八个顶点，按序存储到vertexList
            List<Vector3> vertexList = new List<Vector3>();
            Vector3 unit = _bounds.extents;
            float x = unit.x;
            float y = unit.y;
            float z = unit.z;

            vertexList.Add(new Vector3(x, y, z));
            vertexList.Add(new Vector3(x, y, -z));
            vertexList.Add(new Vector3(x, -y, z));
            vertexList.Add(new Vector3(x, -y, -z));
            vertexList.Add(new Vector3(-x, y, z));
            vertexList.Add(new Vector3(-x, y, -z));
            vertexList.Add(new Vector3(-x, -y, z));
            vertexList.Add(new Vector3(-x, -y, -z));
            //计算每个顶点向其他三个方向延申的三个顶点，并将它们记录到字典中
            Dictionary<int, List<Vector3>> children = new Dictionary<int, List<Vector3>>();
            for (int i = 0; i < vertexList.Count; i++)
            {
                Vector3 vertex = vertexList[i];
                //计算延申的顶点
                List<Vector3> childVertexs = new List<Vector3>
                {
                    vertex+new Vector3(-vertex.x/Mathf.Abs(vertex.x)*_lineLength,0,0),
                    vertex+new Vector3(0,-vertex.y/Mathf.Abs(vertex.y)*_lineLength,0),
                    vertex+new Vector3(0,0,-vertex.z/Mathf.Abs(vertex.z))
                };
                children.Add(i, childVertexs);
            }
            //初始化网格数据，计算每个角上三个长方体八个顶点的位置，并加入vertexs中
            List<Vector3> vertexs = new List<Vector3>();
            //三角面数量计算 每个长方体12个三角面，36个三角面的点，一共8*3*36
            int[] triangle = new int[8 * 3 * 36];
            //根据线条宽度换算成半径数值
            float lineUnit = _lineWidth / 2.0f;
            for (int i = 0; i < children.Count; i++)
            {
                Vector3 center = vertexList[i];
                List<Vector3> childs = children[i];
                //按序号取出延申点进行长方体顶点的计算
                for (int j = 0; j < childs.Count; j++)
                {
                    if (j.Equals(0))
                    {
                        vertexs.AddRange(GetVertexsBy2Point(center + new Vector3(center.x / Mathf.Abs(center.x) * lineUnit, 0, 0), childs[j], "X"));
                    }
                    if (j.Equals(1))
                    {
                        vertexs.AddRange(GetVertexsBy2Point(center + new Vector3(0, center.y / Mathf.Abs(center.y) * lineUnit, 0), childs[j], "Y"));
                    }
                    if (j.Equals(2))
                    {
                        vertexs.AddRange(GetVertexsBy2Point(center + new Vector3(0, 0, center.z / Mathf.Abs(center.z) * lineUnit), childs[j], "Z"));
                    }
                }
            }
            //设置三角面顶点序号 分为六个面，共十二个三角面分别赋值
            for (int i = 0, count = 0; count < triangle.Length; i += 8, count += 36)
            {
                triangle[count + 0] = i + 0;
                triangle[count + 1] = i + 2;
                triangle[count + 2] = i + 1;
                triangle[count + 3] = i + 0;
                triangle[count + 4] = i + 3;
                triangle[count + 5] = i + 2;

                triangle[count + 6] = i + 4;
                triangle[count + 7] = i + 5;
                triangle[count + 8] = i + 7;
                triangle[count + 9] = i + 5;
                triangle[count + 10] = i + 6;
                triangle[count + 11] = i + 7;

                triangle[count + 12] = i + 0;
                triangle[count + 13] = i + 1;
                triangle[count + 14] = i + 5;
                triangle[count + 15] = i + 0;
                triangle[count + 16] = i + 5;
                triangle[count + 17] = i + 4;

                triangle[count + 18] = i + 1;
                triangle[count + 19] = i + 2;
                triangle[count + 20] = i + 6;
                triangle[count + 21] = i + 1;
                triangle[count + 22] = i + 6;
                triangle[count + 23] = i + 5;

                triangle[count + 24] = i + 2;
                triangle[count + 25] = i + 3;
                triangle[count + 26] = i + 7;
                triangle[count + 27] = i + 2;
                triangle[count + 28] = i + 7;
                triangle[count + 29] = i + 6;

                triangle[count + 30] = i + 3;
                triangle[count + 31] = i + 0;
                triangle[count + 32] = i + 4;
                triangle[count + 33] = i + 3;
                triangle[count + 34] = i + 4;
                triangle[count + 35] = i + 7;
            }
            //最后设置网格的顶点和三角面
            _mesh.vertices = vertexs.ToArray();
            _mesh.triangles = triangle;
        }


        /// <summary>
        /// 初始化包围盒的位置
        /// </summary>
        void InitPosition()
        {
            transform.position = _boundsPostion + _bounds.center;
        }
        List<Vector3> GetVertexsBy2Point(Vector3 first, Vector3 second, string axis)
        {
            List<Vector3> vertexs = new List<Vector3>();
            float lineUnit = _lineWidth / 2.0f;
            switch (axis)
            {
                case "X":
                    vertexs.Add(first + new Vector3(0, lineUnit, lineUnit));
                    vertexs.Add(first + new Vector3(0, lineUnit, -lineUnit));
                    vertexs.Add(first + new Vector3(0, -lineUnit, lineUnit));
                    vertexs.Add(first + new Vector3(0, -lineUnit, lineUnit));
                    vertexs.Add(second + new Vector3(0, lineUnit, lineUnit));
                    vertexs.Add(second + new Vector3(0, lineUnit, -lineUnit));
                    vertexs.Add(second + new Vector3(0, -lineUnit, lineUnit));
                    vertexs.Add(second + new Vector3(0, -lineUnit, lineUnit));
                    return vertexs;
                case "Y":
                    vertexs.Add(first + new Vector3(lineUnit, 0, lineUnit));
                    vertexs.Add(first + new Vector3(lineUnit, 0, -lineUnit));
                    vertexs.Add(first + new Vector3(-lineUnit, 0, lineUnit));
                    vertexs.Add(first + new Vector3(-lineUnit, 0, lineUnit));
                    vertexs.Add(second + new Vector3(lineUnit, 0, lineUnit));
                    vertexs.Add(second + new Vector3(lineUnit, 0, -lineUnit));
                    vertexs.Add(second + new Vector3(-lineUnit, 0, lineUnit));
                    vertexs.Add(second + new Vector3(-lineUnit, 0, lineUnit));
                    return vertexs;
                case "Z":
                    vertexs.Add(first + new Vector3(lineUnit, lineUnit, 0));
                    vertexs.Add(first + new Vector3(lineUnit, -lineUnit, 0));
                    vertexs.Add(first + new Vector3(-lineUnit, lineUnit, 0));
                    vertexs.Add(first + new Vector3(-lineUnit, lineUnit, 0));
                    vertexs.Add(second + new Vector3(lineUnit, lineUnit, 0));
                    vertexs.Add(second + new Vector3(lineUnit, -lineUnit, 0));
                    vertexs.Add(second + new Vector3(-lineUnit, lineUnit, 0));
                    vertexs.Add(second + new Vector3(-lineUnit, lineUnit, 0));
                    return vertexs;
                default:
                    return vertexs;
            }
        }
    }
}

