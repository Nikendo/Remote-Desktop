using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        /// // Порт
        static int port = 10000;
        // Адрес
        static IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
        // Оправить сообщение
        const byte codeMsg = 1;
        // Повернуть экран
        const byte codeRotate = 2;
        // Выключить компьютер
        const byte codeMousePosDef = 3;
        // Курсор вверх
        const byte codeMouseUp = 4;
        // Курсор вниз    
        const byte codeMouseDown = 5;
        // Курсор влево
        const byte codeMouseLeft = 6;
        // Курсор вправо
        const byte codeMouseRight = 7;
        // Левая кнопка мыши
        const byte codeLeftMouseKayClick = 8;
        // Правая кнопка мыши
        const byte codeRightMouseKayClick = 9;
        // Отправка картинки
        const byte imageSend = 10;

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        //public static event EventHandler Click;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static void MouseEvent(MouseEventFlags value)
        {
            Point position;
           GetCursorPos(out position);

            mouse_event
                ((uint)value,
                 (uint)position.X,
                 (uint)position.Y,
                 0,
                 (UIntPtr)0);
        }

       
        [STAThread]

        static void Main()
        {                                    
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            Point lp;
            Graphics graphics = null;
            Rectangle bouns = Screen.GetBounds(Point.Empty);

            // Создаем локальную конечную точку
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            // Создаем основной сокет
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                // Связываем сокет с конечной точкой
                socket.Bind(ipEndPoint);

                // Переходим в режим "прослушивания"
                socket.Listen(1);
                while (true)
                {
                   
                    // Ждем соединение. При удачном соединение создается новый экземпляр Socket
                    Socket handler = socket.Accept();
                    // Массив, где сохраняем принятые данные.
                    byte[] recBytes = new byte[1024];
                    int nBytes = handler.Receive(recBytes);                    

                    switch (recBytes[0])    // Определяемся с командами клиента
                    {
                        case codeMsg:
                            {
                                //To do
                                break;
                            }
                        case codeRotate:
                            {
                                //To do
                                break;                            
                            }
                        case codeMouseUp:
                            {
                                GetCursorPos(out lp);
                                if (lp.Y > 0)
                                {
                                    lp.Y -= 5;
                                    SetCursorPos(lp.X, lp.Y);
                                }
                                break;
                            }
                        case codeMouseDown:
                            {
                                GetCursorPos(out lp);
                                if (lp.Y > 0)
                                {
                                    lp.Y += 5;
                                    SetCursorPos(lp.X, lp.Y);
                                }
                                break;
                            }
                        case codeMouseLeft:
                            {
                                GetCursorPos(out lp);
                                if (lp.X > 0)
                                {
                                    lp.X -= 5;
                                    SetCursorPos(lp.X, lp.Y);
                                }
                                break;
                            }
                        case codeMouseRight:
                            {
                                GetCursorPos(out lp);
                                if (lp.X > 0)
                                {
                                    lp.X += 5;
                                    SetCursorPos(lp.X, lp.Y);
                                }
                                break;
                            }
                        case codeLeftMouseKayClick:
                            {
                                MouseEvent(MouseEventFlags.LeftDown);
                                MouseEvent(MouseEventFlags.LeftUp);
                                break;
                            }
                        case codeRightMouseKayClick:
                            {                                
                                contextMenuStrip.Show();
                                break;
                            }
                        case codeMousePosDef:
                            {
                                SetCursorPos(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
                                break;
                            }
                        case imageSend:
                            {                                                                
                                Graphics Imageg = null;
                                var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);                                
                                Imageg = Graphics.FromImage(bmp as Image);  //Convert a Bitmap to an Image                      
                                                                
                                Imageg.CopyFromScreen(0, 0, 0, 0, bmp.Size);    //Take screenshot

                                byte[] data = ImageToByte(bmp); //Convert to bytes
                                
                                handler.Send(data); //Send to client                             
                                break;
                            }
                    }
                    //Release socket
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error creating socket. Error: {0}", ex.Message); }
        }

        //Function of converting an image into bytes
        static byte[] ImageToByte(System.Drawing.Image inImage)
        {
            MemoryStream mStream = new MemoryStream();
            inImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return mStream.ToArray();
        }

        private static void RotateScreen()
        {
            throw new NotImplementedException();          
        } 
    }

}

