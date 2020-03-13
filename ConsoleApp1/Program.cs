using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using Tao.FreeType;
using Tao.OpenGl;
using Tao.Platform;

using System.Runtime.InteropServices;

using static System.Math;
using static Tao.FreeGlut.Glut;
using static Tao.OpenGl.Gl;
using static Tao.OpenGl.Glu;

namespace ConsoleApp1
{
    public static class ScreenExtensions
    {
        public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]Point pt, [In]uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX,
            [Out]out uint dpiY);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    class Program
	{
        const int c_sharp = 16;

        static int Angle = 0;

        const byte VK_ESCAPE = 0x1B;


        private void Window_Loaded(object sender)
        {
            var sb = new StringBuilder();
            sb.Append("Angular\n");
            sb.Append(string.Join("\n", Display(DpiType.Angular)));
            sb.Append("\nEffective\n");
            sb.Append(string.Join("\n", Display(DpiType.Effective)));
            sb.Append("\nRaw\n");
            sb.Append(string.Join("\n", Display(DpiType.Raw)));

            glutSetWindowTitle(sb.ToString());
            //this.Content = new TextBox() { Text = sb.ToString() };
        }

        private IEnumerable<string> Display(DpiType type)
        {
            foreach (var screen in Screen.AllScreens)
            {
                uint x, y;
                screen.GetDpi(type, out x, out y);
                yield return screen.DeviceName + " - dpiX=" + x + ", dpiY=" + y;
            }
        }

        static void Main(string[] args)
        {
            // Инициализация GLUT
            glutInit();

            glutInitDisplayMode(GLUT_DOUBLE | GLUT_SINGLE | GLUT_RGB);

            // Координаты и размер окна GLUT
            glutInitWindowPosition(200, 200);
            glutInitWindowSize(800, 600);

            // Создание окна GLUT
            glutCreateWindow("OpenGL C#");

            // Обработчик рендеринга (вызывается при отрисовке окна, аналогично WM_PAINT)
            glutDisplayFunc(GLRenderScene);

            // Обработчик клавитуры (при WM_KEYDOWN)
            glutKeyboardFunc(GLKeyDown);

            // Инициализация OpenGL
            GLInit();

            string hopa = "OpenGL C# Window: ";
            int ww = glutGet(GLUT_WINDOW_WIDTH);
            hopa += ww;
            int wh = glutGet(GLUT_WINDOW_HEIGHT);
            hopa += "x";
            hopa += wh;
            int bw = glutGet(GLUT_WINDOW_BORDER_WIDTH);
            hopa += "; border: ";
            hopa += bw;
            int hh = glutGet(GLUT_WINDOW_HEADER_HEIGHT);
            hopa += "x";
            hopa += hh;
            int sw = glutGet(GLUT_SCREEN_WIDTH);
            hopa += "; screen: ";
            hopa += sw;
            int sh = glutGet(GLUT_SCREEN_HEIGHT);
            hopa += "x";
            hopa += sh;

            //WindowWidth = 800;
            //Console.WindowHeight = 600;

            glutSetWindowTitle(hopa);

            Program lul = new Program();

            lul.Window_Loaded(lul);

            // Главный цикл приложения
            glutMainLoop();
        }

        static void GLInit()
        {
            // Цвет фона - черный
            glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        }

        static void GLRenderScene()
        {
            // Очищаем буфер цвета и глубины
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            glLoadIdentity();

            // Включаем тест глубины,
            // чтобы грани 3D-фигур - рисовались непрозрачными, и не просвечивали
            glEnable(GL_DEPTH_TEST);

            // Поворот
            glRotatef(Angle, 1.0f, 1.0f, 0.0f);

            // -------- Рисование параллелепипеда ----------
            // Длина x высота x глубина:
            //   0.5    1.0      0.25

            // Передняя грань
            glColor3f(0.0f, 1.0f, 0.0f); // зеленый
            glBegin(GL_POLYGON);
            glVertex3f(0.0f, 0.0f, 0.0f); // ЛН
            glVertex3f(0.0f, 1.0f, 0.0f); // ЛВ
            glVertex3f(0.5f, 1.0f, 0.0f); // ПВ
            glVertex3f(0.5f, 0.0f, 0.0f); // ПН
            glEnd();

            // Задняя грань
            // Та же передняя, только, Z = 0.25, а, не 0
            glColor3f(1.0f, 0.0f, 0.0f); // красный
            glBegin(GL_POLYGON);
            glVertex3f(0.0f, 0.0f, 0.25f); // ЛН
            glVertex3f(0.0f, 1.0f, 0.25f); // ЛВ
            glVertex3f(0.5f, 1.0f, 0.25f); // ПВ
            glVertex3f(0.5f, 0.0f, 0.25f); // ПН
            glEnd();

            // Левая грань
            glColor3f(1.0f, 1.0f, 0.0f); // желтый
            glBegin(GL_POLYGON);
            glVertex3f(0.0f, 0.0f, 0.0f); // Перед-низ
            glVertex3f(0.0f, 1.0f, 0.0f); // Перед-верх
            glVertex3f(0.0f, 1.0f, 0.25f); // Зад-верх
            glVertex3f(0.0f, 0.0f, 0.25f); // Зад-низ
            glEnd();

            // Правая грань
            // Та же левая, только, X = 0.5
            glColor3f(0.0f, 1.0f, 1.0f); // Голубой
            glBegin(GL_POLYGON);
            glVertex3f(0.5f, 0.0f, 0.0f); // Перед-низ
            glVertex3f(0.5f, 1.0f, 0.0f); // Перед-верх
            glVertex3f(0.5f, 1.0f, 0.25f); // Зад-верх
            glVertex3f(0.5f, 0.0f, 0.25f); // Зад-низ
            glEnd();

            // Верхняя грань
            glColor3f(1.0f, 0.0f, 1.0f); // Розовый
            glBegin(GL_POLYGON);
            glVertex3f(0.0f, 1.0f, 0.0f); // Перед-лево
            glVertex3f(0.5f, 1.0f, 0.0f); // Перед-право
            glVertex3f(0.5f, 1.0f, 0.25f); // Зад-право
            glVertex3f(0.0f, 1.0f, 0.25f); // Зад-лево
            glEnd();

            // Нижняя грань
            // Та же верхняя, только, Y = 0
            glColor3f(1.0f, 1.0f, 1.0f); // Белый
            glBegin(GL_POLYGON);
            glVertex3f(0.0f, 0.0f, 0.0f); // Перед-лево
            glVertex3f(0.5f, 0.0f, 0.0f); // Перед-право
            glVertex3f(0.5f, 0.0f, 0.25f); // Зад-право
            glVertex3f(0.0f, 0.0f, 0.25f); // Зад-лево
            glEnd();

            glutSwapBuffers();
        }

        static void GLKeyDown(byte key, int x, int y)
        {
            switch(key)
            {
                case VK_ESCAPE:
                    Application.Exit();
                    break;
                case (byte)Keys.Left:
                    Angle -= 5; // Уменьшаем угол поворота на 5 градусов

                    glutPostRedisplay(); // Перерисовываем окно
                    break;
                case (byte)Keys.Right:
                    Angle += 5; // Увеличиваем угол поворота на 5 градусов

                    glutPostRedisplay(); // Перерисовываем окно
                    break;
            }
        }
    }
}
