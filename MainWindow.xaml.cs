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

        #region 생성자
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region 값추출
        void readTrianglePos(double[,] triangle, TextBox value, int idx)
        {
            string text = value.Text;

            string[] list = text.Split(',');
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = list[i].Trim();
                if (double.TryParse(list[i], out double result))
                {
                    triangle[idx, i] = result;
                }
            }
        }
        #endregion

        #region 클릭 이벤트 및 메인 계산
        private void simpleButtonClick(object sender, RoutedEventArgs eassssd)
        {
            model3Dgroup.Children.Clear();
            //값분석
            double[,] trianglePos = new double[3,3]; // => 평면을 구성하는 점 3개 (a, b, c), (d, e, f), (g, h, i)
            double res1, res2, res3;        // 평면의 법선 벡터 = (res1, res2, res3)
            double r;               // r: 평면의 방정식 중 오른쪽 상수항 부분
            double[,] linePos = new double[2, 3];  // => 직선을 구성하는 점 2개 (x0, y0, z0), (x1, y1, z1)
            double res4, res5, res6, t;
            double x, y, z;			// x, y, z: 직선과 평면의 교점




            // 텍스트박스 값읽기 - 삼각형
            readTrianglePos(trianglePos, this.pos1, 0);
            readTrianglePos(trianglePos, this.pos2, 1);
            readTrianglePos(trianglePos, this.pos3, 2);

            // 텍스트박스 값읽기 - 직선
            readTrianglePos(linePos, this.pos4, 0);
            readTrianglePos(linePos, this.pos5, 1);
            double a, b, c, d, e, f, g, h, i;
            a = trianglePos[0, 0];
            b = trianglePos[0, 1];
            c = trianglePos[0, 2];
            d = trianglePos[1, 0];
            e = trianglePos[1, 1];
            f = trianglePos[1, 2];
            g = trianglePos[2, 0];
            h = trianglePos[2, 1];
            i = trianglePos[2, 2];

            // 평면의 방정식 값 도출
            res1 = (e - b) * (i - c) - (f - c) * (h - b);
            res2 = (f - c) * (g - a) - (d - a) * (i - c);
            res3 = (d - a) * (h - b) - (e - b) * (g - a);

            MeshGeometry3D triangleMesh = new MeshGeometry3D();

            // 삼각형의 세 점 추가
            Point3D point0 = new Point3D(trianglePos[0, 0], trianglePos[0, 1], trianglePos[0, 2]);
            Point3D point1 = new Point3D(trianglePos[1, 0], trianglePos[1, 1], trianglePos[1, 2]);
            Point3D point2 = new Point3D(trianglePos[2, 0], trianglePos[2, 1], trianglePos[2, 2]);

            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);

            // 법선벡터 추가
            Vector3D normal = new Vector3D(res1, res2, res3);

            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            drawTriangle(triangleMesh, point0, point1, point2, normal);

            DefineModel(this.model3Dgroup, linePos);
            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = this.model3Dgroup;
            this.mainViewport.Children.Add(modelVisual3D);
            // 14
            r = res1 * trianglePos[0, 0] + res2 * trianglePos[0, 1] + res3 * trianglePos[0, 2];

            /* 두 점을 지나는 직선의 대칭 방정식 이용, (방정식) = t 형태에서
	        계수부분(res4), 상수부분(res6) 각각 계산하여 최종적인 t 값 도출 */
            res4 = res1 * (linePos[1, 0] - linePos[0,0]) + res2 * (linePos[1, 1] - linePos[0, 1]) + res3 * (linePos[1, 2] - linePos[0, 2]);
            res5 = res1 * linePos[0, 0] + res2 * linePos[0, 1] + res3 * linePos[0, 2];
            res6 = r - res5;
            t = res6 / res4;


            /* 찾아낸 t 값을 대칭 방정식에 대입하여 직선과 평면의 교점의 x, y, z 좌표 계산*/
            x = (linePos[1, 0] - linePos[0, 0]) * t + linePos[0, 0];
            y = (linePos[1, 1] - linePos[0, 1]) * t + linePos[0, 1];
            z = (linePos[1, 2] - linePos[0, 2]) * t + linePos[0, 2];

            double[] ABxAP = new double[3], ABxAC = new double[3], BCxBP = new double[3], CAxCP = new double[3];
            double ABC, ABP, PBC, APC;
            double alpha, beta, gamma;
            

            ABxAC[0] = (e - b) * (i - c) - (f - c) * (h - b);
            ABxAC[1] = (f - c) * (g - a) - (d - a) * (i - c);
            ABxAC[2] = (d - a) * (h - b) - (e - b) * (g - a);

            ABxAP[0] = (e - b) * (z - c) - (f - c) * (y - b);
            ABxAP[1] = (f - c) * (x - a) - (d - a) * (z - c);
            ABxAP[2] = (d - a) * (y - b) - (e - b) * (x - a);


            BCxBP[0] = (h - e) * (z - f) - (i - f) * (y - e);
            BCxBP[1] = (i - f) * (x - d) - (g - d) * (z - f);
            BCxBP[2] = (g - d) * (y - e) - (h - e) * (x - d);

            CAxCP[0] = (b - h) * (z - i) - (c - i) * (y - h);
            CAxCP[1] = (c - i) * (x - g) - (a - g) * (z - i);
            CAxCP[2] = (a - g) * (y - h) - (b - h) * (x - g);

            
            ABC = Math.Sqrt(Math.Abs(ABxAC[0] * ABxAC[0] + ABxAC[1] * ABxAC[1] + ABxAC[2] * ABxAC[2]))/2;
            ABP = Math.Sqrt(Math.Abs(ABxAP[0] * ABxAP[0] + ABxAP[1] * ABxAP[1] + ABxAP[2] * ABxAP[2]))/2;
            PBC = Math.Sqrt(Math.Abs(BCxBP[0] * BCxBP[0] + BCxBP[1] * BCxBP[1] + BCxBP[2] * BCxBP[2]))/2;
            APC = Math.Sqrt(Math.Abs(CAxCP[0] * CAxCP[0] + CAxCP[1] * CAxCP[1] + CAxCP[2] * CAxCP[2]))/2;

            string kPos = string.Format("{0:0.000} / {1:0.000} / {2:0.000}", x, y, z);
            this.spot.Content = kPos;

            // 값출력
            this.dimension1.Content = ABC;
            this.dimension2.Content = ABP;
            this.dimension3.Content = PBC;
            this.dimension4.Content = APC;

            alpha = PBC / ABC;
            beta = APC / ABC;
            gamma = ABP / ABC;

            string abg = string.Format("{0:0.000} / {1:0.000} / {2:0.000}", alpha, beta, gamma);

            this.alBeGam.Content = abg;

            if (alpha >= 0 && beta >= 0 && gamma >= 0 && Math.Abs(1 - (alpha + beta + gamma)) <= 0.001)
            {
                this.result.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 80));
                this.result.Content = "관통한다.";
            }
            else
            {
                this.result.Foreground = new SolidColorBrush(Color.FromRgb(255, 0 , 0));
                this.result.Content = "빗나간다.";
            }
        }
        #endregion

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
        private void DefineModel(Model3DGroup model3DGroup, double[,] linePos)
        {
            // Z축 실린더를 정의한다.
            MeshGeometry3D mesh3 = new MeshGeometry3D();

            AddCylinder(mesh3, new Point3D(linePos[0, 0], linePos[0, 1], linePos[0, 2]), new Vector3D(linePos[1, 0], linePos[1, 1], linePos[1, 2]), 0.025, 20);

            SolidColorBrush brush3 = Brushes.Red;
            DiffuseMaterial material3 = new DiffuseMaterial(brush3);
            GeometryModel3D model3 = new GeometryModel3D(mesh3, material3);

            model3DGroup.Children.Add(model3);
        }
        #endregion
    }
}
