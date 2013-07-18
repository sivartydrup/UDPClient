// Copyright 2013 Travis Purdy. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Imaging;
using System.IO;

namespace UDPClient
{
    public partial class FormUDPClient : Form
    {
        Socket TransmissionSocket;
        IPAddress ServerAdress;
        IPEndPoint ServerEndPoint;

        Graphics g;
        ImageCodecInfo jgpEncoder;
        EncoderParameters myEncoderParameters;
        Bitmap bmpScreenCapture;

        public FormUDPClient()
        {
            InitializeComponent();
        }

        private void FormUDPClient_Load(object sender, EventArgs e)
        {
            TransmissionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ServerAdress = IPAddress.Parse("127.0.0.1");
            ServerEndPoint = new IPEndPoint(ServerAdress, 11000);

            jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID 
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 10L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            timerCaptureAndSend.Enabled = true;
        }

        private void timerCaptureAndSend_Tick(object sender, EventArgs e)
        {
            if (bmpScreenCapture != null)
            {
                bmpScreenCapture.Dispose();
            }

            bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                                Screen.PrimaryScreen.Bounds.Height);

            g = Graphics.FromImage(bmpScreenCapture);

            g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                Screen.PrimaryScreen.Bounds.Y,
                                0, 0,
                                bmpScreenCapture.Size,
                                CopyPixelOperation.SourceCopy);

            MemoryStream ms = new MemoryStream();
            bmpScreenCapture.Save(ms, jgpEncoder, myEncoderParameters);

            TransmissionSocket.SendTo(ms.ToArray(), ServerEndPoint);

            ms.Close();
            ms.Dispose();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
