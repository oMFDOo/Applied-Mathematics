using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;

namespace Math10
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 필드
        /// 3차원 모델 그룹
        private Model3DGroup model3Dgroup = new Model3DGroup();
       
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void simpleButtonClick(object sender, RoutedEventArgs e)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();

            // 삼각형의 세 점 추가
            Point3D point0 = new Point3D(0, 0, 0);
            Point3D point1 = new Point3D(5, 0, 0);
            Point3D point2 = new Point3D(0, 0, 5);

            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);

            // 법선벡터 추가
            Vector3D normal = new Vector3D(0, 1, 0);

            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            drawTriangle(triangleMesh, point0, point1, point2, normal);

            DefineModel(this.model3Dgroup);
            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = this.model3Dgroup;
            this.mainViewport.Children.Add(modelVisual3D);

        }

        #region 삼각형 그리기 - drawTriangle(triangleMesh, point0, point1, point2, normal)
        void drawTriangle(MeshGeometry3D triangleMesh, Point3D point0, Point3D point1, Point3D point2, Vector3D normal)
        {
            int[,] indexSet = new int[6, 3] {
                { 0, 1, 2 }, { 0, 2, 1 },
                { 1, 0, 2 }, { 1, 2, 0 },
                { 2, 0, 1 }, { 2, 1, 0 }
            };

            for (int i = 0; i < 6; i++)
            {
                triangleMesh.TriangleIndices.Add(indexSet[i, 0]);
                triangleMesh.TriangleIndices.Add(indexSet[i, 1]);
                triangleMesh.TriangleIndices.Add(indexSet[i, 2]);

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Aquamarine));
                GeometryModel3D triangleModel = new GeometryModel3D(triangleMesh, material);
                ModelVisual3D model = new ModelVisual3D(); model.Content = triangleModel;
                this.mainViewport.Children.Add(model);
            }
            
        }
        #endregion

        #region 실린더 추가하기 - AddCylinder(meshGeometry3D, endPoint3D, axisVector3D, radius, sideCount)

        /// <summary>
        /// 실린더 추가하기
        /// </summary>
        /// <param name="meshGeometry3D">3차원 메시 기하</param>
        /// <param name="endPoint3D">종단 3차원 포인트</param>
        /// <param name="axisVector3D">축 3차원 벡터</param>
        /// <param name="radius">반경</param>
        /// <param name="sideCount">면 카운트</param>
        private void AddCylinder(MeshGeometry3D meshGeometry3D, Point3D endPoint3D, Vector3D axisVector3D, double radius, int sideCount)
        {
            // 축에 수진인 2개의 벡터를 설정한다.
            Vector3D v1;

            if ((axisVector3D.Z < -0.01) || (axisVector3D.Z > 0.01))
            {
                v1 = new Vector3D(axisVector3D.Z, axisVector3D.Z, -axisVector3D.X - axisVector3D.Y);
            }
            else
            {
                v1 = new Vector3D(-axisVector3D.Y - axisVector3D.Z, axisVector3D.X, axisVector3D.X);
            }

            Vector3D v2 = Vector3D.CrossProduct(v1, axisVector3D);

            // 반지름 길이를 갖는 벡터로 만든다.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // 상단 끝 캡을 만든다.
            int pt0 = meshGeometry3D.Positions.Count;

            meshGeometry3D.Positions.Add(endPoint3D);

            // 상단 포인트를 만든다.
            double theta = 0;
            double deltaTheta = 2 * Math.PI / sideCount;

            for (int i = 0; i < sideCount; i++)
            {
                meshGeometry3D.Positions.Add(endPoint3D + Math.Cos(theta) * v1 + Math.Sin(theta) * v2);

                theta += deltaTheta;
            }

            // 상단 삼각형을 만든다.
            int pt1 = meshGeometry3D.Positions.Count - 1;
            int pt2 = pt0 + 1;

            for (int i = 0; i < sideCount; i++)
            {
                meshGeometry3D.TriangleIndices.Add(pt0);
                meshGeometry3D.TriangleIndices.Add(pt1);
                meshGeometry3D.TriangleIndices.Add(pt2);

                pt1 = pt2++;
            }

            // 하단 끝 캡을 만든다.
            pt0 = meshGeometry3D.Positions.Count;

            Point3D endPoint2 = endPoint3D + axisVector3D;

            meshGeometry3D.Positions.Add(endPoint2);

            // 하단 포인트를 만든다.
            theta = 0;

            for (int i = 0; i < sideCount; i++)
            {
                meshGeometry3D.Positions.Add(endPoint2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2);

                theta += deltaTheta;
            }

            // 하단 삼각형을 만든다.
            theta = 0;

            pt1 = meshGeometry3D.Positions.Count - 1;
            pt2 = pt0 + 1;

            for (int i = 0; i < sideCount; i++)
            {
                meshGeometry3D.TriangleIndices.Add(sideCount + 1);
                meshGeometry3D.TriangleIndices.Add(pt2);
                meshGeometry3D.TriangleIndices.Add(pt1);

                pt1 = pt2++;
            }

            // 면을 만든다.
            int firstSidePoint = meshGeometry3D.Positions.Count;

            theta = 0;

            for (int i = 0; i < sideCount; i++)
            {
                Point3D p1 = endPoint3D + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;

                meshGeometry3D.Positions.Add(p1);

                Point3D p2 = p1 + axisVector3D;

                meshGeometry3D.Positions.Add(p2);

                theta += deltaTheta;
            }

            // 면 삼각형을 만든다.
            pt1 = meshGeometry3D.Positions.Count - 2;
            pt2 = pt1 + 1;

            int pt3 = firstSidePoint;
            int pt4 = pt3 + 1;

            for (int i = 0; i < sideCount; i++)
            {
                meshGeometry3D.TriangleIndices.Add(pt1);
                meshGeometry3D.TriangleIndices.Add(pt2);
                meshGeometry3D.TriangleIndices.Add(pt4);

                meshGeometry3D.TriangleIndices.Add(pt1);
                meshGeometry3D.TriangleIndices.Add(pt4);
                meshGeometry3D.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }
        }
        #endregion

        #region 모델 정의하기 - DefineModel(model3DGroup)

        /// <summary>
        /// 모델 정의하기
        /// </summary>
        /// <param name="model3DGroup"></param>
        private void DefineModel(Model3DGroup model3DGroup)
        {
            // Z축 실린더를 정의한다.
            MeshGeometry3D mesh3 = new MeshGeometry3D();

            AddCylinder(mesh3, new Point3D(1.44, 0.59, 2.71), new Vector3D(1.32, 1.81, -0.48), 0.1, 20);

            SolidColorBrush brush3 = Brushes.Red;
            DiffuseMaterial material3 = new DiffuseMaterial(brush3);
            GeometryModel3D model3 = new GeometryModel3D(mesh3, material3);

            model3DGroup.Children.Add(model3);
        }
        #endregion
    }
}
